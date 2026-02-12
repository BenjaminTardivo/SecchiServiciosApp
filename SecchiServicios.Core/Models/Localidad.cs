using SecchiServicios.Core.Enums;

namespace SecchiServicios.Core.Models;

public class Localidad
{
    public int Id { get; set; }
    public string Nombre { get; set; } =  string.Empty;
    public string CodigoPostal { get; set; }
    public Provincias Provincia { get; set; }
}