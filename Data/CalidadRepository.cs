using controlCalidad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace controlCalidad.Data;

public class CalidadRepository
{
    private readonly List<InspeccionCalidad> _bd;

        public CalidadRepository()
        {
            _bd = new List<InspeccionCalidad>();
            GenerarSemillero();
        }

        private void GenerarSemillero()
        {
            string[] inspectores = { "Juan Perez", "Maria Lopez", "Carlos Ruiz" };
            string[] productos = { "Válvulas", "Tuberías", "Filtros" };
            Random rnd = new Random();

            for (int i = 1; i <= 35; i++)
            {
                int cantInspeccionadas = rnd.Next(100, 500);
                int cantDefectuosas = rnd.Next(0, 15); // La mayoría pasa
                double porcentaje = (double)cantDefectuosas / cantInspeccionadas * 100;
                
                _bd.Add(new InspeccionCalidad
                {
                    Id = i,
                    NumeroInspeccion = $"INSP-{1000 + i}",
                    Lote = $"LOTE-2026-{rnd.Next(1, 12)}",
                    Producto = productos[rnd.Next(productos.Length)],
                    Inspector = inspectores[rnd.Next(inspectores.Length)],
                    CantidadesInspeccionadas = cantInspeccionadas,
                    CantidadesDefectuosas = cantDefectuosas,
                    Resultado = porcentaje > 5 ? "Rechazado" : "Aprobado",
                    Defecto = porcentaje > 5 ? "Falla Estructural" : "N/A"
                });
            }
        }

        public List<InspeccionCalidad> ObtenerTodas() => _bd.OrderByDescending(x => x.Id).ToList();

        public List<InspeccionCalidad> Buscar(string query) =>
            _bd.Where(x => x.NumeroInspeccion.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                           x.Lote.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

        public List<InspeccionCalidad> ObtenerAprobadas() => _bd.Where(x => x.Resultado == "Aprobado").ToList();
        
        public List<InspeccionCalidad> ObtenerRechazadas() => _bd.Where(x => x.Resultado == "Rechazado").ToList();

        public List<InspeccionCalidad> Filtrar(string resultado) =>
            string.IsNullOrEmpty(resultado) ? ObtenerTodas() : _bd.Where(x => x.Resultado == resultado).ToList();

        public void Agregar(InspeccionCalidad inspeccion)
        {
            inspeccion.Id = _bd.Any() ? _bd.Max(x => x.Id) + 1 : 1;
            _bd.Add(inspeccion);
        }

        public void Actualizar(InspeccionCalidad inspeccion)
        {
            var existente = _bd.FirstOrDefault(x => x.Id == inspeccion.Id);
            if (existente != null)
            {
                existente.Lote = inspeccion.Lote;
                existente.Producto = inspeccion.Producto;
                existente.Inspector = inspeccion.Inspector;
                existente.CantidadesInspeccionadas = inspeccion.CantidadesInspeccionadas;
                existente.CantidadesDefectuosas = inspeccion.CantidadesDefectuosas;
                existente.Resultado = inspeccion.Resultado;
                existente.Defecto = inspeccion.Defecto;
            }
        }

        public void Eliminar(int id)
        {
            var registro = _bd.FirstOrDefault(x => x.Id == id);
            if (registro != null) _bd.Remove(registro);
        }

        public double CalcularPorcentajeDefectos()
        {
            if (!_bd.Any()) return 0;
            int totalInspeccionadas = _bd.Sum(x => x.CantidadesInspeccionadas);
            int totalDefectuosas = _bd.Sum(x => x.CantidadesDefectuosas);
            return totalInspeccionadas == 0 ? 0 : Math.Round((double)totalDefectuosas / totalInspeccionadas * 100, 2);
        }

        public InspeccionCalidad ObtenerPorId(int id) => _bd.FirstOrDefault(x => x.Id == id);
        
        public bool ExisteNumeroInspeccion(string numero, int idExclude = 0) =>
            _bd.Any(x => x.NumeroInspeccion == numero && x.Id != idExclude);
}
