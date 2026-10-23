using System.Reflection;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SecchiServicios.Pdf.Models;

namespace SecchiServicios.Pdf.Documents;

public class ReciboDocument : IDocument
{
    private readonly IReadOnlyList<ReciboDocumentModel> _recibos;

    public ReciboDocument(ReciboDocumentModel data)
        : this(new List<ReciboDocumentModel> { data })
    {
    }

    public ReciboDocument(IReadOnlyList<ReciboDocumentModel> recibos)
    {
        _recibos = recibos;
    }

    public DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Title = _recibos.Count == 1
                ? $"Recibo {_recibos[0].NroRecibo}"
                : $"Recibos ({_recibos.Count})",
            Author = "SECCHI COMUNICACIONES",
            Subject = "Recibo"
        };
    }

    public void Compose(IDocumentContainer container)
    {
        foreach (var recibo in _recibos)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5.Landscape());

                page.Margin(30);

                page.DefaultTextStyle(text =>
                    text.FontSize(9)
                        .FontFamily("Arial"));

                page.Content()
                    .Border(1)
                    .Padding(12)
                    .Column(column =>
                    {
                        // ==========================
                        // HEADER
                        // ==========================

                        column.Item()
                            .Element(c => ComposeHeader(c, recibo));

                        // ==========================
                        // DATOS DEL CLIENTE
                        // ==========================

                        column.Item()
                            .PaddingTop(8)
                            .Element(c => ComposeCliente(c, recibo));

                        // Línea separadora
                        column.Item()
                            .PaddingTop(8)
                            .LineHorizontal(1);

                        // ==========================
                        // MONTO EN LETRAS
                        // ==========================

                        column.Item()
                            .PaddingTop(8)
                            .Element(c => ComposeMontoLetras(c, recibo));

                        // ==========================
                        // CONCEPTO
                        // ==========================

                        column.Item()
                            .PaddingTop(16)
                            .Element(c => ComposeConcepto(c, recibo));

                        // ==========================
                        // ESPACIO
                        // ==========================

                        column.Item()
                            .Height(25);

                        // ==========================
                        // TOTAL
                        // ==========================

                        column.Item()
                            .Element(c => ComposeTotal(c, recibo));
                    });
            });
        }
    }

    // ============================================================
    // HEADER
    // ============================================================

    private void ComposeHeader(IContainer container, ReciboDocumentModel data)
    {
        container.Row(row =>
        {
            // ----------------------------------------------------
            // LOGO
            // ----------------------------------------------------

            row.ConstantItem(65)
                .Border(1)
                .AlignCenter()
                .AlignMiddle()
                .Image(LoadEmbeddedResource("SecchiServicios.Pdf.Assets.LogoComprobante.jpeg"));

            // ----------------------------------------------------
            // CENTRO
            // ----------------------------------------------------

            row.RelativeItem()
                .PaddingLeft(15)
                .PaddingRight(15)
                .Column(column =>
                {
                    column.Item()
                        .Border(1)
                        .PaddingVertical(6)
                        .Row(headerRow =>
                        {
                            headerRow.RelativeItem()
                                .AlignCenter()
                                .Column(company =>
                                {
                                    company.Item()
                                        .AlignCenter()
                                        .Text("SECCHI COMUNICACIONES")
                                        .Bold()
                                        .FontSize(12);

                                    company.Item()
                                        .PaddingTop(3)
                                        .Row(info =>
                                        {
                                            info.RelativeItem()
                                                .AlignCenter()
                                                .Text("Av. Urquiza 397")
                                                .Bold();

                                            info.RelativeItem()
                                                .AlignCenter()
                                                .Text("Humberto 1º")
                                                .Bold();

                                            info.RelativeItem()
                                                .AlignCenter()
                                                .Text("03493-402133")
                                                .Bold();
                                        });
                                });
                        });
                });

            // ----------------------------------------------------
            // NUMERO / FECHA
            // ----------------------------------------------------

            row.ConstantItem(125)
                .AlignTop()
                .Column(column =>
                {
                    column.Item()
                        .Row(row =>
                        {
                            row.AutoItem()
                                .Text("Nro Recibo:")
                                .Bold();

                            row.RelativeItem()
                                .AlignRight()
                                .Text(data.NroRecibo.ToString("D10"))
                                .Bold();
                        });

                    column.Item()
                        .PaddingTop(3)
                        .Row(row =>
                        {
                            row.AutoItem()
                                .Text("Fecha:")
                                .Bold();

                            row.RelativeItem()
                                .AlignRight()
                                .Text(data.Fecha.ToString("d.M.yyyy"))
                                .Bold();
                        });
                });
        });
    }

    // ============================================================
    // CLIENTE
    // ============================================================

    private void ComposeCliente(IContainer container, ReciboDocumentModel data)
    {
        container.Column(column =>
        {
            ComposeClienteRow(column, "Sr/es:", data.Cliente);

            ComposeClienteRow(column, "Domicilio:", data.Domicilio);

            ComposeClienteRow(column, "Localidad:", data.Localidad);

            ComposeClienteRow(column, "Teléfono:", data.Telefono);

            column.Item()
                .Row(row =>
                {
                    row.AutoItem()
                        .Text("Condición IVA:")
                        .Bold();

                    row.RelativeItem()
                        .PaddingLeft(8)
                        .Text(data.CondicionIva);

                    row.AutoItem()
                        .Text("CUIT/CUIL:")
                        .Bold();

                    // De momento no tenés CUIT en el DocumentModel
                    row.ConstantItem(120);
                });
        });
    }

    private void ComposeClienteRow(
        ColumnDescriptor column,
        string label,
        string value)
    {
        column.Item()
            .PaddingBottom(2)
            .Row(row =>
            {
                row.AutoItem()
                    .Text(label)
                    .Bold();

                row.RelativeItem()
                    .PaddingLeft(8)
                    .Text(value);
            });
    }

    // ============================================================
    // MONTO EN LETRAS
    // ============================================================

    private void ComposeMontoLetras(IContainer container, ReciboDocumentModel data)
    {
        container.Text(text =>
        {
            text.Span("Recibí el importe de: ");

            text.Span(data.MontoLetras.ToLower())
                .Bold();

            text.Span(" .-");
        });
    }

    // ============================================================
    // CONCEPTO
    // ============================================================

    private void ComposeConcepto(IContainer container, ReciboDocumentModel data)
    {
        container.Column(column =>
        {
            column.Item()
                .Text("En concepto de:")
                .Bold();

            column.Item()
                .PaddingTop(8)
                .Row(row =>
                {
                    // Cantidad
                    row.ConstantItem(40)
                        .AlignCenter()
                        .Text("1");

                    // Servicio
                    row.RelativeItem()
                        .Text(data.Servicio);

                    // Precio unitario
                    row.ConstantItem(90)
                        .AlignRight()
                        .Text(FormatearMonto(data.MontoNumerico));

                    // Total
                    row.ConstantItem(90)
                        .AlignRight()
                        .Text(FormatearMonto(data.MontoNumerico));
                });
        });
    }

    // ============================================================
    // TOTAL
    // ============================================================

    private void ComposeTotal(IContainer container, ReciboDocumentModel data)
    {
        container.Row(row =>
        {
            row.RelativeItem();

            row.AutoItem()
                .PaddingRight(25)
                .Text("Total:")
                .Bold()
                .FontSize(10);

            row.ConstantItem(90)
                .AlignRight()
                .Text(FormatearMonto(data.MontoNumerico))
                .Bold()
                .FontSize(10);
        });
    }

    // ============================================================
    // FORMATO DE DINERO
    // ============================================================

    private static string FormatearMonto(decimal monto)
    {
        return $"$ {monto:0.00}";
    }

    private static byte[] LoadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}