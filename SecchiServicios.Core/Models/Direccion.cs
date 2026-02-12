using SecchiServicios.Core.Enums;

namespace SecchiServicios.Core.Models;

public class Direccion
{
    public string Calle { get; set; } = String.Empty;
    public int Altura { get; set; }
    public Provincias Provincia { get; set; }

    public string DireccionCompleta => $"{Calle} {Altura}";
}