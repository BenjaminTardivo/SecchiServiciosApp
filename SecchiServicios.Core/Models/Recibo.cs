using System.Runtime.InteropServices.JavaScript;

namespace SecchiServicios.Core.Models;

public class Recibo
{
    public int Id { get; set; }
    
    public DateTime Fecha { get; set; }
    
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; }

    public int ProductoServicioId { get; set; }
    public ProductoServicio ProductoServicio { get; set; }

    public decimal Total { get; set; }
}