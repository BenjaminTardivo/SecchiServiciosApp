using SecchiServicios.Core.Enums;

namespace SecchiServicios.Core.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int DNI { get; set; }
    public Direccion Direccion { get; set; }
    public string Vendedor { get; set; } = string.Empty;
    public Localidad? Localidad { get; set; }
    public int? LocalidadId { get; set; }
    public string Telefono { get; set; } = string.Empty;
    public CondicionesIVA CondicionIva { get; set; }
}