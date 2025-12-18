using Batch.Data;
using Batch.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Batch.Controllers
{
    public class ToleranciasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ToleranciasController> _logger;

        public ToleranciasController(AppDbContext context, ILogger<ToleranciasController> logger)
        {
            _context = context;
            _logger = logger;

        }

        public IActionResult Index()
        {
            var lista = _context.Tolerancias
                                .Include(t => t.Componente) // Cargar el nombre del componente
                                .ToList();
            return View(lista);
        }

        public IActionResult Create()
        {
            ViewData["ComponenteId"] = new SelectList(_context.Componentes, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tolerancia tolerancia)
        {
            if (ModelState.IsValid)
            {
                _context.Tolerancias.Add(tolerancia);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Recargar el SelectList con el valor seleccionado
            ViewData["ComponenteId"] = new SelectList(_context.Componentes, "Id", "Name", tolerancia.ComponenteId);
            return View(tolerancia);
        }

        // GET: Tolerancias/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tolerancia = _context.Tolerancias
                                     .Include(t => t.Componente)
                                     .FirstOrDefault(t => t.Id == id);

            if (tolerancia == null)
            {
                return NotFound();
            }

            // Cargar lista de componentes para el dropdown
            ViewData["ComponenteId"] = new SelectList(_context.Componentes, "Id", "Name", tolerancia.ComponenteId);
            return View(tolerancia);
        }

        [HttpPost]
        public IActionResult EditarAjax([FromBody] Tolerancia tolerancia)
        {
            var db = _context.Tolerancias.FirstOrDefault(t => t.Id == tolerancia.Id);

            if (db == null)
                return NotFound();

            // ✅ Convertir valores vacíos, null o "" a 0
            decimal max = 0;
            decimal min = 0;

            if (tolerancia.Max.HasValue)
                max = tolerancia.Max.Value;

            if (tolerancia.Min.HasValue)
                min = tolerancia.Min.Value;

            // ✅ Asignar valores seguros
            db.ComponenteId = tolerancia.ComponenteId;
            db.Prueba = tolerancia.Prueba;
            db.Max = max;
            db.Min = min;

            _context.SaveChanges();

            // ✅ Obtener nombre del componente sin que EF evalúe expresiones inválidas
            var componente = _context.Componentes
                .Where(c => c.Id == db.ComponenteId)
                .Select(c => c.Name)
                .FirstOrDefault();

            return Json(new
            {
                success = true,
                id = db.Id,
                componente = componente,
                prueba = db.Prueba,
                max = db.Max,
                min = db.Min
            });
        }

    }
}
