using Batch.Data;
using Batch.Helper;
using Batch.Helpers;
using Batch.Models;
using Batch.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Batch.Controllers
{
    public class BatchController : Controller
    {
        private readonly AppDbContext _context;

        public BatchController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Batches
        [HttpGet]
        public IActionResult Crear()
        {
            var lotes = _context.Batches.Include
                (l => l.Componente).Include
                (l => l.Resultados).ThenInclude
                (r => r.Tolerancia).Where
                (l => l.RFID == null &&
                (l.Estado == EstadoBatch.PendienteDeLlenado || l.Estado == EstadoBatch.LlenadoAprobado)).ToList();

            var componentes = _context.Componentes
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();

            var modalVm = new NuevoBatchModalViewModel
            {
                FechaInicio = DateTime.Now,
                FechaExp = DateTime.Now.AddHours(8),
                Componentes = componentes
            };

            ViewBag.ModalVm = modalVm;
            return View(lotes);
        }

        [HttpPost]
        public IActionResult CrearBatch([FromBody] BatchRequest request)
        {
            var ahora = DateTime.Now;
            var fechaLaboral = DiaLaboralHelper.ObtenerFechaLaboral(ahora);

            var tolerancias = _context.Tolerancias
                .Where(t => t.ComponenteId == request.ComponenteId)
                .ToList();

            if (!tolerancias.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    mensaje = "El componente seleccionado no tiene tolerancias registradas. Agrega tolerancias antes de crear un batch."
                });
            }

            var folio = FolioHelper.CrearFolio(fechaLaboral, request.Linea, request.ComponenteId, _context, request.Retrabajo);

            var batch = new Lote
            {
                ComponenteId = request.ComponenteId,
                FechaInicio = fechaLaboral,
                FechaExp = fechaLaboral.AddDays(7),
                Folio = folio,
                RegistroId = $"{fechaLaboral:yyyyMMdd}-TEMP",
                RFID = null
            };

            _context.Batches.Add(batch);
            _context.SaveChanges();

            batch.RegistroId = $"{fechaLaboral:yyyyMMdd}-{batch.Id}";
            _context.SaveChanges();

            foreach (var tol in tolerancias)
            {
                _context.ResultadosPrueba.Add(new ResultadoPrueba
                {
                    LoteId = batch.Id,
                    ToleranciaId = tol.Id,
                    Valor = null,
                    EsValido = false
                });
            }

            _context.SaveChanges();

            return Ok(new
            {
                success = true,
                mensaje = "Batch creado correctamente",
                folio = batch.Folio,
                registroId = batch.RegistroId
            });
        }

        // Vista para capturar las pruebas de un lote
        public IActionResult Evaluar(int id)
        {
            var lote = _context.Batches
                .Include(l => l.Resultados)
                .ThenInclude(r => r.Tolerancia)
                .FirstOrDefault(l => l.Id == id);

            if (lote == null)
                return NotFound();

            return View(lote);
        }

        [HttpPost]
        public IActionResult GuardarResultados([FromBody] GuardarResultadosRequest request)
        {
            // Guardar valores capturados
            foreach (var r in request.Resultados)
            {
                var resultado = _context.ResultadosPrueba
                    .Include(x => x.Tolerancia)
                    .FirstOrDefault(x => x.Id == r.ResultadoId);

                if (resultado != null)
                {
                    resultado.Valor = r.Valor;
                    resultado.EsValido = resultado.Valor.HasValue &&
                                         resultado.Valor >= resultado.Tolerancia.Min &&
                                         resultado.Valor <= resultado.Tolerancia.Max;
                }
            }

            _context.SaveChanges();

            bool enviado = false;

            // Si se presionó "Enviar"
            if (request.Enviar)
            {
                enviado = true;

                var lote = _context.Batches
                    .Include(l => l.Resultados)
                    .FirstOrDefault(l => l.Id == request.LoteId);

                if (lote != null)
                {
                    foreach (var res in lote.Resultados)
                    {
                        if (!res.Valor.HasValue)
                        {
                            res.Valor = 0;
                            res.EsValido = false;
                        }
                    }

                    lote.Estado = lote.Resultados.All(r => r.EsValido)
                        ? EstadoBatch.LlenadoAprobado
                        : EstadoBatch.LlenadoRechazado;

                    lote.FechaCambioEstado = DateTime.Now;
                    _context.SaveChanges();
                }
            }

            return Ok(new
            {
                success = true,
                enviado = enviado,
                mensaje = enviado
                    ? "Resultados enviados correctamente"
                    : "Datos guardados correctamente"
            });
        }

        [HttpPost]
        public IActionResult GuardarRFID([FromBody] GuardarRFIDRequest request)
        {
            var lote = _context.Batches.FirstOrDefault(l => l.Id == request.LoteId);

            if (lote == null)
                return NotFound();

            // Solo permitir asignar RFID si el lote está aprobado para llenado
            if (lote.Estado == EstadoBatch.LlenadoAprobado)
            {
                lote.RFID = request.RFID;
                lote.Estado = EstadoBatch.RFIDAsignado; // ⚡ cambia el estado automáticamente
                _context.SaveChanges();

                return Ok(new { success = true, nuevoEstado = lote.Estado, rfid = lote.RFID });
            }

            return BadRequest("El lote no está aprobado, no se puede asignar RFID.");
        }

        [HttpGet]
        public IActionResult Lista()
        {
            var lotes = _context.Batches
                .Include(l => l.Componente)
                .Include(l => l.Resultados)
                    .ThenInclude(r => r.Tolerancia)
                .OrderByDescending(l => l.FechaCreacion)
                .ToList();

            return View(lotes);
        }

    }




    public class GuardarRFIDRequest
    {
        public int LoteId { get; set; }
        public string RFID { get; set; }
    }

    public class ResultadoInput
    {
        public int ResultadoId { get; set; }
        public float? Valor { get; set; } // ⚡ ahora nullable
    }

    public class BatchRequest
    {
        public int ComponenteId { get; set; }
        public string Linea { get; set; }
        public bool Retrabajo { get; set; }
    }

    public class GuardarResultadosRequest
    {
        public int LoteId { get; set; }
        public bool Enviar { get; set; }
        public List<ResultadoInput> Resultados { get; set; }
    }
}