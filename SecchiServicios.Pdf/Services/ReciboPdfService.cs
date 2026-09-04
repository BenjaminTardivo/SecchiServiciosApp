using QuestPDF.Fluent;
using SecchiServicios.Core.Models;
using SecchiServicios.Pdf.Documents;
using SecchiServicios.Pdf.Models;
using SecchiServicios.Data.Helpers;

namespace SecchiServicios.Pdf.Services;

public class ReciboPdfService
{
    public void GenerarPdf(Recibo recibo)
    {
        var model = ReciboDocumentDataSource.GetReciboModel(recibo);

        var document = new ReciboDocument(model);

        var path = Path.Combine(AppPaths.GetRecibosPdfDataFolder(), recibo.Id.ToString() + " - " + recibo.Cliente.Nombre);

        document.GeneratePdf(path);
    }
}