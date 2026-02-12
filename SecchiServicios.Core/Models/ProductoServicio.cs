using SecchiServicios.Core.Enums;

namespace SecchiServicios.Core.Models;

public class ProductoServicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Importe { get; set; }
    public TipoProductoServicio Tipo { get; set; }
}