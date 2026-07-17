namespace controlCalidad.Models.ViewModels;

public class DashboardCalidadViewModel
{
    public int TotalGenerales { get; set; }
    public int Aprobados { get; set; }
    public int Rechazados { get; set; }
    public double PromedioDefectos { get; set; }
    public List<InspeccionViewModel> UltimasInspecciones { get; set; }
    public string MejorInspector { get; set; }
    public string FocoAlerta { get; set; }
}