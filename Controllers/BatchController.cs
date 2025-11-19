using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Batch.Data;
using Batch.Models;
using Batch.Helpers;

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
            return View(lista);
        }

        // POST: CrearBatch (desde modal)
        [HttpPost]
        public IActionResult CrearBatch([FromBody] BatchRequest request)
        {
            var ahora = DateTime.Now;
            var fechaLaboral = DiaLaboralHelper.ObtenerFechaLaboral(ahora);

            var consecutivo = _context.Batches
                .Count(b => b.FechaInicio == fechaLaboral && b.ComponenteId == request.ComponenteId) + 1;

            var folio = $"{consecutivo:00}{request.Linea}";

            var batch = new Lote
            {
                ComponenteId = request.ComponenteId,
                FechaInicio = fechaLaboral,
                FechaExp = fechaLaboral.AddDays(7),
                Folio = folio
            };

            _context.Batches.Add(batch);
            _context.SaveChanges();

            var tolerancias = _context.Tolerancias
                .Where(t => t.ComponenteId == request.ComponenteId)
                .ToList();

            foreach (var tol in tolerancias)
            {
                _context.ResultadosPrueba.Add(new ResultadoPrueba
                {
                    BatchId = batch.Id,
                    ToleranciaId = tol.Id,
                    Valor = 0f
                });
            }

            _context.SaveChanges();

            return Ok(new { Folio = batch.Folio });
        }
    }

    // DTO para recibir datos del modal
    public class BatchRequest
    {
        public int ComponenteId { get; set; }
        public string Linea { get; set; }
    }
}