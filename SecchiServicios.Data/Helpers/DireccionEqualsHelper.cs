using SecchiServicios.Core.Models;

namespace SecchiServicios.Data.Helpers;

public class DireccionEqualsHelper
{
    public static bool DireccionEquals(Direccion a, Direccion b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Calle == b.Calle &&
               a.Altura == b.Altura &&
               a.Provincia == b.Provincia;
    }
}