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

            if (lote != null)
            {
                if (lote.Resultados.All(r => r.Valor.HasValue))
                {
                    lote.Estado = lote.Resultados.All(r => r.EsValido)
                        ? EstadoBatch.LlenadoAprobado
                        : EstadoBatch.LlenadoRechazado;

                    lote.FechaCambioEstado = DateTime.Now; // 🔎 fecha de cambio
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Evaluar", new { id = loteId });
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
