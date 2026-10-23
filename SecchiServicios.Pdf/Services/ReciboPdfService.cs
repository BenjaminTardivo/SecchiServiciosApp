using System.Drawing.Printing;
using QuestPDF.Fluent;
using SecchiServicios.Core.Models;
using SecchiServicios.Pdf.Documents;
using SecchiServicios.Pdf.Models;
using SecchiServicios.Data.Helpers;
using SecchiServicios.Pdf.Printing;

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

    public string GenerarPdfMultipage(List<Recibo> recibos)
    {
        if (recibos == null || recibos.Count == 0)
            throw new ArgumentException("La lista de recibos no puede estar vacía.", nameof(recibos));

        var models = recibos
            .Select(ReciboDocumentDataSource.GetReciboModel)
            .ToList();

        var document = new ReciboDocument(models);

        var fileName = recibos.Count == 1
            ? $"{recibos[0].Id} - {recibos[0].Cliente.Nombre}"
            : $"Recibos ({recibos.Count}) - {DateTime.Now:yyyy-MM-dd HH-mm-ss}";

        var path = Path.Combine(AppPaths.GetRecibosPdfDataFolder(), fileName);

        document.GeneratePdf(path);

        return path;
    }

    public void ImprimirRecibos(List<Recibo> recibos, string? printerName = null)
    {
        if (recibos == null || recibos.Count == 0)
            return;

        var models = recibos
            .Select(ReciboDocumentDataSource.GetReciboModel)
            .ToList();

        var renderer = new ReciboPrintRenderer();
        int currentIndex = 0;

        using var printDoc = new PrintDocument();

        // Configure A5 landscape paper
        printDoc.DefaultPageSettings.PaperSize = new PaperSize("A5", 5811, 4134);
        printDoc.DefaultPageSettings.Landscape = true;
        printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

        if (!string.IsNullOrEmpty(printerName))
            printDoc.PrinterSettings.PrinterName = printerName;

        printDoc.PrintPage += (sender, e) =>
        {
            var model = models[currentIndex];
            renderer.Render(e.Graphics!, model, e.MarginBounds);
            currentIndex++;
            e.HasMorePages = currentIndex < models.Count;
        };

        printDoc.Print();
    }
}