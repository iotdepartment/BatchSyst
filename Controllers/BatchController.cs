using Batch.Data;
using Batch.Helper;
using Batch.Helpers;
using Batch.Models;
using Microsoft.AspNetCore.Mvc;
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
            var lista = _context.Batches
                                .Include(b => b.Componente)
                                .OrderByDescending(b => b.FechaInicio)
                                .ToList();

            // Traer solo componentes con Id del 1 al 8
            var componentes = _context.Componentes
                                      .Where(c => c.Id >= 1 && c.Id <= 8)
                                      .OrderBy(c => c.Id)
                                      .ToList();

            ViewBag.Componentes = componentes;

            return View(lista);
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