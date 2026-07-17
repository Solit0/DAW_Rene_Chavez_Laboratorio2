namespace controlCalidad.Models.ViewModels;

public class InspeccionViewModel
{
    public InspeccionCalidad Inspeccion { get; set; }

    public double PorcentajeDefectos => 
        Inspeccion.CantidadesInspeccionadas == 0 ? 0 :
            Math.Round((double)Inspeccion.CantidadesDefectuosas / Inspeccion.CantidadesInspeccionadas * 100, 2);

    public string NivelCalidad
    {
        get
        {
            if (PorcentajeDefectos <= 1) return "Excelente";
            if (PorcentajeDefectos <= 3) return "Bueno";
            if (PorcentajeDefectos <= 5) return "Regular";
            return "Crítico";
        }
    }
}