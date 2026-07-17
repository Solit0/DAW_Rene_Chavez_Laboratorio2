using System.ComponentModel.DataAnnotations;
namespace controlCalidad.Models;

public class InspeccionCalidad
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [Display(Name = "Número de Inspección")]
    public string NumeroInspeccion { get; set; }

    [Required(ErrorMessage = "El lote es obligatorio")]
    public string Lote { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un producto")]
    public string Producto { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un inspector")]
    public string Inspector { get; set; }

    [Required(ErrorMessage = "Ingrese la cantidad inspeccionada")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe inspeccionar al menos 1 unidad")]
    [Display(Name = "Cantidades Inspeccionadas")]
    public int CantidadesInspeccionadas { get; set; }

    [Required(ErrorMessage = "Ingrese la cantidad defectuosa")]
    [Range(0, int.MaxValue, ErrorMessage = "No puede ser un valor negativo")]
    [Display(Name = "Cantidades Defectuosas")]
    public int CantidadesDefectuosas { get; set; }

    public string Resultado { get; set; } 

    public string Defecto { get; set; } 
}