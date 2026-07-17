using controlCalidad.Models.ViewModels;

namespace controlCalidad.Controllers;
using controlCalidad.Data;
using controlCalidad.Models;
using controlCalidad.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
public class CalidadController : Controller
{
    private readonly CalidadRepository _repo;

        public CalidadController(CalidadRepository repo)
        {
            _repo = repo;
        }

        private void CargarViewBags()
        {
            ViewBag.Inspectores = new SelectList(new[] { "Juan Perez", "Maria Lopez", "Carlos Ruiz" });
            ViewBag.Productos = new SelectList(new[] { "Válvulas", "Tuberías", "Filtros", "Motores" });
            ViewBag.TiposDefecto = new SelectList(new[] { "N/A", "Falla Estructural", "Desgaste", "Medida Incorrecta" });
            ViewBag.Resultados = new SelectList(new[] { "Aprobado", "Rechazado" });
        }

        public IActionResult Index(string searchString, string filtroResultado)
        {
            var inspecciones = _repo.ObtenerTodas();

            if (!string.IsNullOrEmpty(searchString))
                inspecciones = _repo.Buscar(searchString);
            else if (!string.IsNullOrEmpty(filtroResultado))
                inspecciones = _repo.Filtrar(filtroResultado);

            var viewModels = inspecciones.Select(i => new InspeccionViewModel { Inspeccion = i }).ToList();

            ViewData["Aprobadas"] = _repo.ObtenerAprobadas().Count;
            ViewData["Rechazadas"] = _repo.ObtenerRechazadas().Count;
            ViewData["TotalInspecciones"] = inspecciones.Count;
            ViewData["PromedioDefectos"] = _repo.CalcularPorcentajeDefectos();
            ViewData["FechaServidor"] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            return View(viewModels);
        }

        public IActionResult Dashboard()
        {
            var todas = _repo.ObtenerTodas();
            
            var vm = new DashboardCalidadViewModel
            {
                TotalGenerales = todas.Count,
                Aprobados = _repo.ObtenerAprobadas().Count,
                Rechazados = _repo.ObtenerRechazadas().Count,
                PromedioDefectos = _repo.CalcularPorcentajeDefectos(),
                UltimasInspecciones = todas.Take(5).Select(i => new InspeccionViewModel { Inspeccion = i }).ToList()
            };

            if (todas.Any())
            {
                vm.MejorInspector = todas.Where(x => x.Resultado == "Aprobado")
                                         .GroupBy(x => x.Inspector)
                                         .OrderByDescending(g => g.Count())
                                         .FirstOrDefault()?.Key ?? "N/A";

                vm.FocoAlerta = todas.Where(x => x.Resultado == "Rechazado")
                                     .GroupBy(x => x.Producto)
                                     .OrderByDescending(g => g.Count())
                                     .FirstOrDefault()?.Key ?? "N/A";
            }

            return View(vm);
        }

        public IActionResult Create()
        {
            CargarViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InspeccionCalidad inspeccion)
        {
            AplicarReglasNegocio(inspeccion);

            if (ModelState.IsValid)
            {
                _repo.Agregar(inspeccion);
                return RedirectToAction(nameof(Index));
            }
            CargarViewBags();
            return View(inspeccion);
        }

        public IActionResult Edit(int id)
        {
            var insp = _repo.ObtenerPorId(id);
            if (insp == null) return NotFound();
            
            CargarViewBags();
            return View(insp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(InspeccionCalidad inspeccion)
        {
            AplicarReglasNegocio(inspeccion, inspeccion.Id);

            if (ModelState.IsValid)
            {
                _repo.Actualizar(inspeccion);
                return RedirectToAction(nameof(Index));
            }
            CargarViewBags();
            return View(inspeccion);
        }

        public IActionResult Details(int id)
        {
            var insp = _repo.ObtenerPorId(id);
            if (insp == null) return NotFound();
            return View(new InspeccionViewModel { Inspeccion = insp });
        }

        public IActionResult Delete(int id)
        {
            var insp = _repo.ObtenerPorId(id);
            if (insp == null) return NotFound();
            
            // Regla: No eliminar Aprobados
            if (insp.Resultado == "Aprobado")
            {
                TempData["Error"] = "No se permite eliminar inspecciones con resultado 'Aprobado'.";
                return RedirectToAction(nameof(Index));
            }
            return View(insp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repo.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }

        private void AplicarReglasNegocio(InspeccionCalidad inspeccion, int idExclude = 0)
        {
            if (_repo.ExisteNumeroInspeccion(inspeccion.NumeroInspeccion, idExclude))
                ModelState.AddModelError("NumeroInspeccion", "Este código de inspección ya existe.");

            if (inspeccion.CantidadesDefectuosas > inspeccion.CantidadesInspeccionadas)
                ModelState.AddModelError("CantidadesDefectuosas", "La cantidad defectuosa no puede ser superior a la inspeccionada.");

            if (inspeccion.CantidadesInspeccionadas > 0)
            {
                double porcentaje = (double)inspeccion.CantidadesDefectuosas / inspeccion.CantidadesInspeccionadas * 100;
                
                // Regla 5%: Bloqueo a Rechazado
                if (porcentaje > 5)
                {
                    inspeccion.Resultado = "Rechazado";
                    if (string.IsNullOrEmpty(inspeccion.Defecto) || inspeccion.Defecto == "N/A")
                        ModelState.AddModelError("Defecto", "Debe especificar un defecto cuando se supera el 5% de errores.");
                }
            }
        }
}