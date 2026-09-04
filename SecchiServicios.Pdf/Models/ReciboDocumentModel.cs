namespace SecchiServicios.Pdf.Models;

public class ReciboDocumentModel
{
    public int NroRecibo { get; set; }
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; }
    public string Servicio { get; set; }
    public string Domicilio { get; set; }
    public string Localidad { get; set; }
    public string CondicionIva { get; set; }
    public decimal MontoNumerico { get; set; }
    public string MontoLetras { get; set; }
    public string Telefono { get; set; }
}