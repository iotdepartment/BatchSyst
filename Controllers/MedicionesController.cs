using Batch.Data;
using Batch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Batch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedicionesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("ResumenLote/{loteId}")]
        public IActionResult ResumenLote(int loteId)
        {
            var lote = _context.Batches
                .Include(l => l.Resultados)
                .ThenInclude(r => r.Tolerancia)
                .FirstOrDefault(l => l.Id == loteId);

            if (lote == null)
                return NotFound("Lote no encontrado");

            var total = lote.Resultados.Count;
            var aprobadas = lote.Resultados.Count(r => r.EsValido);
            var fallidas = total - aprobadas;

            var estadoGlobal = fallidas == 0 ? "APROBADO" : "RECHAZADO";

            return Ok(new
            {
                LoteId = lote.Id,
                Folio = lote.Folio,
                RegistroId = lote.RegistroId,
                TotalPruebas = total,
                Aprobadas = aprobadas,
                Fallidas = fallidas,
                EstadoGlobal = estadoGlobal,
                Detalle = lote.Resultados.Select(r => new {
                    r.Id,
                    Prueba = r.Tolerancia.Prueba,
                    Min = r.Tolerancia.Min,
                    Max = r.Tolerancia.Max,
                    Valor = r.Valor,
                    Estado = r.EsValido ? "OK" : "FAIL"
                })
            });
        }


    }
    public class EvaluarPruebaRequest
    {
        public int ResultadoId { get; set; }
        public float Valor { get; set; }
    }


}
