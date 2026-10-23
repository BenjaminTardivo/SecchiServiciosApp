using SecchiServicios.Core.Models;
using SecchiServicios.Pdf.Services;

namespace SecchiServicios.Pdf.Models;
using QuestPDF.Helpers;

public class ReciboDocumentDataSource
{
    public static ReciboDocumentModel GetReciboModel(Recibo recibo)
    {
        return new ReciboDocumentModel
        {
            NroRecibo = recibo.Id,
            Fecha = recibo.Fecha,
            Cliente = recibo.Cliente.Nombre,
            Servicio = recibo.ProductoServicio.Nombre,
            Domicilio = recibo.Cliente.Direccion.DireccionCompleta,
            Localidad = recibo.Cliente.Localidad.Nombre,
            CondicionIva = recibo.Cliente.CondicionIva.ToString(),
            MontoNumerico = recibo.Total,
            Telefono = recibo.Cliente.Telefono,
            
            MontoLetras = NumeroALetras.Convertir(recibo.Total)
        };
    }
}