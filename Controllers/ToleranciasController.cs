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

        // POST: Tolerancias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Tolerancia tolerancia)
        {
            if (id != tolerancia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tolerancia);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tolerancias.Any(e => e.Id == tolerancia.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Si falla la validación, recargar el SelectList
            ViewData["ComponenteId"] = new SelectList(_context.Componentes, "Id", "Name", tolerancia.ComponenteId);
            return View(tolerancia);
        }

    }
}
