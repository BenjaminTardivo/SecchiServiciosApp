using System.Reflection;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SecchiServicios.Pdf.Models;

namespace SecchiServicios.Pdf.Documents;

public class ReciboDocument : IDocument
{
    private readonly ReciboDocumentModel _data;

    public ReciboDocument(ReciboDocumentModel data)
    {
        _data = data;
    }

    public DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Title = $"Recibo {_data.NroRecibo}",
            Author = "SECCHI COMUNICACIONES",
            Subject = "Recibo"
        };
    }

    public void Compose(IDocumentContainer container)
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
                        .Element(ComposeHeader);

                    // ==========================
                    // DATOS DEL CLIENTE
                    // ==========================

                    column.Item()
                        .PaddingTop(8)
                        .Element(ComposeCliente);

                    // Línea separadora
                    column.Item()
                        .PaddingTop(8)
                        .LineHorizontal(1);

                    // ==========================
                    // MONTO EN LETRAS
                    // ==========================

                    column.Item()
                        .PaddingTop(8)
                        .Element(ComposeMontoLetras);

                    // ==========================
                    // CONCEPTO
                    // ==========================

                    column.Item()
                        .PaddingTop(16)
                        .Element(ComposeConcepto);

                    // ==========================
                    // ESPACIO
                    // ==========================

                    column.Item()
                        .Height(25);

                    // ==========================
                    // TOTAL
                    // ==========================

                    column.Item()
                        .Element(ComposeTotal);
                });
        });
    }

    // ============================================================
    // HEADER
    // ============================================================

    private void ComposeHeader(IContainer container)
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
                                .Text(_data.NroRecibo.ToString("D10"))
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
                                .Text(_data.Fecha.ToString("d.M.yyyy"))
                                .Bold();
                        });
                });
        });
    }

    // ============================================================
    // CLIENTE
    // ============================================================

    private void ComposeCliente(IContainer container)
    {
        container.Column(column =>
        {
            ComposeClienteRow(column, "Sr/es:", _data.Cliente);

            ComposeClienteRow(column, "Domicilio:", _data.Domicilio);

            ComposeClienteRow(column, "Localidad:", _data.Localidad);

            ComposeClienteRow(column, "Teléfono:", _data.Telefono);

            column.Item()
                .Row(row =>
                {
                    row.AutoItem()
                        .Text("Condición IVA:")
                        .Bold();

                    row.RelativeItem()
                        .PaddingLeft(8)
                        .Text(_data.CondicionIva);

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

    private void ComposeMontoLetras(IContainer container)
    {
        container.Text(text =>
        {
            text.Span("Recibí el importe de: ");

            text.Span(_data.MontoLetras.ToLower())
                .Bold();

            text.Span(" .-");
        });
    }

    // ============================================================
    // CONCEPTO
    // ============================================================

    private void ComposeConcepto(IContainer container)
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
                        .Text(_data.Servicio);

                    // Precio unitario
                    row.ConstantItem(90)
                        .AlignRight()
                        .Text(FormatearMonto(_data.MontoNumerico));

                    // Total
                    row.ConstantItem(90)
                        .AlignRight()
                        .Text(FormatearMonto(_data.MontoNumerico));
                });
        });
    }

    // ============================================================
    // TOTAL
    // ============================================================

    private void ComposeTotal(IContainer container)
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
                .Text(FormatearMonto(_data.MontoNumerico))
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