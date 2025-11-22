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
        public IActionResult Index()
        {
            var lotes = _context.Batches
                .Include(l => l.Componente)
                .Include(l => l.Resultados)
                    .ThenInclude(r => r.Tolerancia)
                .ToList();

            // Select list de componentes para el modal
            var componentes = _context.Componentes
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            // Precios por defecto: ahora y +8 horas, por ejemplo
            var modalVm = new NuevoBatchModalViewModel
            {
                FechaInicio = DateTime.Now,
                FechaExp = DateTime.Now.AddHours(8),
                Componentes = componentes
            };

            ViewBag.ModalVm = modalVm; // opción simple para pasar al partial
            return View(lotes);
        }

        [HttpPost]
        public IActionResult CrearBatch([FromBody] BatchRequest request)
        {
            var ahora = DateTime.Now;
            var fechaLaboral = DiaLaboralHelper.ObtenerFechaLaboral(ahora);

            var folio = FolioHelper.CrearFolio(fechaLaboral, request.Linea, request.ComponenteId, _context, request.Retrabajo);

            var batch = new Lote
            {
                ComponenteId = request.ComponenteId,
                FechaInicio = fechaLaboral,
                FechaExp = fechaLaboral.AddDays(7),
                Folio = folio,
                // Aquí todavía no tienes el Id autoincremental,
                // pero puedes usar un GUID o un placeholder temporal
                RegistroId = $"{fechaLaboral:yyyyMMdd}-TEMP"
            };

            _context.Batches.Add(batch);
            _context.SaveChanges();

            // Ahora sí, ya tienes el Id generado por SQL Server
            batch.RegistroId = $"{fechaLaboral:yyyyMMdd}-{batch.Id}";
            _context.SaveChanges();

            // Generar resultados vacíos
            var tolerancias = _context.Tolerancias
                .Where(t => t.ComponenteId == request.ComponenteId)
                .ToList();

            foreach (var tol in tolerancias)
            {
                _context.ResultadosPrueba.Add(new ResultadoPrueba
                {
                    LoteId = batch.Id,       // ✔ Usa LoteId como FK
                    ToleranciaId = tol.Id,
                    Valor = 0f,
                    EsValido = false         // inicializa como pendiente
                });
            }

            _context.SaveChanges();

            return Ok(new { Folio = batch.Folio, RegistroId = batch.RegistroId });
        }



        // Vista para capturar las 10 pruebas de un lote
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
        public IActionResult GuardarResultados(int loteId, List<ResultadoInput> resultados)
        {
            foreach (var r in resultados)
            {
                var resultado = _context.ResultadosPrueba
                    .Include(x => x.Tolerancia)
                    .FirstOrDefault(x => x.Id == r.ResultadoId);

                if (resultado != null)
                {
                    resultado.Valor = r.Valor;
                    resultado.EsValido = resultado.Valor >= resultado.Tolerancia.Min &&
                                         resultado.Valor <= resultado.Tolerancia.Max;
                }
            }

            _context.SaveChanges();

            var lote = _context.Batches
                .Include(l => l.Resultados)
                .FirstOrDefault(l => l.Id == loteId);

            if (lote.Resultados.All(r => r.Valor.HasValue)) // todas llenadas
            {
                lote.Estado = lote.Resultados.All(r => r.EsValido)
                    ? EstadoBatch.LlenadoAprobado
                    : EstadoBatch.LlenadoRechazado;

                lote.FechaCambioEstado = DateTime.Now; // 🔎 aquí sí se guarda
                _context.SaveChanges();
            }

            return RedirectToAction("Evaluar", new { id = loteId });
        }



    }
    public class ResultadoInput
    {
        public int ResultadoId { get; set; }
        public float Valor { get; set; }
    }


    public class BatchRequest
    {
        public int ComponenteId { get; set; }
        public string Linea { get; set; }
        public bool Retrabajo { get; set; }
    }

}