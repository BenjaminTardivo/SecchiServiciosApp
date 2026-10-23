using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using SecchiServicios.Pdf.Models;

namespace SecchiServicios.Pdf.Printing;

public class ReciboPrintRenderer
{
    private const float Margin = 30f;

    public void Render(
        Graphics graphics,
        ReciboDocumentModel data,
        RectangleF bounds)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.TextRenderingHint =
            System.Drawing.Text.TextRenderingHint.AntiAlias;

        // ============================================================
        // ÁREA ÚTIL
        // ============================================================

        float x = bounds.Left + Margin;
        float y = bounds.Top + Margin;

        float width = bounds.Width - Margin * 2;

        // ============================================================
        // FUENTES
        // ============================================================

        using var fontNormal = new Font(
            "Arial",
            9f,
            FontStyle.Regular);

        using var fontSmall = new Font(
            "Arial",
            8f,
            FontStyle.Regular);

        using var fontBold = new Font(
            "Arial",
            9f,
            FontStyle.Bold);

        using var fontHeader = new Font(
            "Arial",
            13f,
            FontStyle.Bold);

        using var brush = new SolidBrush(Color.Black);

        // ============================================================
        // HEADER
        // ============================================================

        DrawHeader(
            graphics,
            data,
            x,
            ref y,
            width,
            fontHeader,
            fontSmall,
            fontNormal,
            brush);

        y += 15;

        // ============================================================
        // CLIENTE
        // ============================================================

        DrawCliente(
            graphics,
            data,
            x,
            ref y,
            width,
            fontNormal,
            brush);

        y += 18;

        // ============================================================
        // CONCEPTO
        // ============================================================

        DrawConcepto(
            graphics,
            data,
            x,
            ref y,
            width,
            fontNormal,
            brush);

        y += 15;

        // ============================================================
        // TOTAL
        // ============================================================

        DrawTotal(
            graphics,
            data,
            x,
            ref y,
            width,
            fontBold,
            brush);

        y += 15;

        // ============================================================
        // MONTO EN LETRAS
        // ============================================================

        DrawMontoLetras(
            graphics,
            data,
            x,
            ref y,
            width,
            fontNormal,
            brush);
    }

    // ================================================================
    // HEADER
    // ================================================================

    private void DrawHeader(
        Graphics graphics,
        ReciboDocumentModel data,
        float x,
        ref float y,
        float width,
        Font headerFont,
        Font smallFont,
        Font normalFont,
        Brush brush)
    {
        float leftWidth = width * 0.125f;
        float centerWidth = width * 0.625f;
        float rightWidth = width * 0.25f;

        float centerX = x + leftWidth;

        // ------------------------------------------------------------
        // Título
        // ------------------------------------------------------------

        using var centerFormat = new StringFormat
        {
            Alignment = StringAlignment.Center
        };

        graphics.DrawString(
            "SECCHI COMUNICACIONES",
            headerFont,
            brush,
            new RectangleF(
                centerX,
                y,
                centerWidth,
                25),
            centerFormat);

        // ------------------------------------------------------------
        // Datos de la empresa
        // ------------------------------------------------------------

        graphics.DrawString(
            "Av. Urquiza 397          Humberto 1º          03493-480209",
            smallFont,
            brush,
            new RectangleF(
                centerX,
                y + 20,
                centerWidth,
                20),
            centerFormat);

        // ------------------------------------------------------------
        // Número de recibo
        // ------------------------------------------------------------

        float rightX = centerX + centerWidth;

        graphics.DrawString(
            "Nro Recibo:",
            normalFont,
            brush,
            rightX,
            y);

        using var rightFormat = new StringFormat
        {
            Alignment = StringAlignment.Far
        };

        graphics.DrawString(
            data.NroRecibo.ToString("D10"),
            normalFont,
            brush,
            new RectangleF(
                rightX,
                y,
                rightWidth,
                20),
            rightFormat);

        // ------------------------------------------------------------
        // Fecha
        // ------------------------------------------------------------

        graphics.DrawString(
            "Fecha:",
            normalFont,
            brush,
            rightX,
            y + 25);

        graphics.DrawString(
            data.Fecha.ToString("d.M.yyyy"),
            normalFont,
            brush,
            new RectangleF(
                rightX,
                y + 25,
                rightWidth,
                20),
            rightFormat);

        y += 45;
    }

    // ================================================================
    // CLIENTE
    // ================================================================

    private void DrawCliente(
        Graphics graphics,
        ReciboDocumentModel data,
        float x,
        ref float y,
        float width,
        Font font,
        Brush brush)
    {
        DrawLabelValue(
            graphics,
            "Sr/es:",
            data.Cliente,
            x,
            ref y,
            width,
            font,
            brush);

        DrawLabelValue(
            graphics,
            "Domicilio:",
            data.Domicilio,
            x,
            ref y,
            width,
            font,
            brush);

        DrawLabelValue(
            graphics,
            "Localidad:",
            data.Localidad,
            x,
            ref y,
            width,
            font,
            brush);

        DrawLabelValue(
            graphics,
            "Teléfono:",
            data.Telefono,
            x,
            ref y,
            width,
            font,
            brush);

        // Condición IVA
        graphics.DrawString(
            "Condición IVA:",
            font,
            brush,
            x,
            y);

        float labelWidth = 75;

        graphics.DrawString(
            data.CondicionIva,
            font,
            brush,
            x + labelWidth + 10,
            y);

        graphics.DrawString(
            "CUIT/CUIL:",
            font,
            brush,
            x + width - 80,
            y);

        y += 16;
    }

    private void DrawLabelValue(
        Graphics graphics,
        string label,
        string value,
        float x,
        ref float y,
        float width,
        Font font,
        Brush brush)
    {
        graphics.DrawString(
            label,
            font,
            brush,
            x,
            y);

        graphics.DrawString(
            value,
            font,
            brush,
            x + 75,
            y);

        y += 16;
    }

    // ================================================================
    // CONCEPTO
    // ================================================================

    private void DrawConcepto(
        Graphics graphics,
        ReciboDocumentModel data,
        float x,
        ref float y,
        float width,
        Font font,
        Brush brush)
    {
        graphics.DrawString(
            "En concepto de:",
            font,
            brush,
            x,
            y);

        y += 20;

        // Cantidad
        graphics.DrawString(
            "1",
            font,
            brush,
            x + 5,
            y);

        // Servicio
        graphics.DrawString(
            data.Servicio,
            font,
            brush,
            x + 30,
            y);

        using var rightFormat = new StringFormat
        {
            Alignment = StringAlignment.Far
        };

        // Importe
        graphics.DrawString(
            data.MontoNumerico.ToString("$ 0.00"),
            font,
            brush,
            new RectangleF(
                x + width - 160,
                y,
                80,
                20),
            rightFormat);

        // Importe total línea
        graphics.DrawString(
            data.MontoNumerico.ToString("$ 0.00"),
            font,
            brush,
            new RectangleF(
                x + width - 80,
                y,
                80,
                20),
            rightFormat);

        y += 20;
    }

    // ================================================================
    // TOTAL
    // ================================================================

    private void DrawTotal(
        Graphics graphics,
        ReciboDocumentModel data,
        float x,
        ref float y,
        float width,
        Font font,
        Brush brush)
    {
        using var rightFormat = new StringFormat
        {
            Alignment = StringAlignment.Far
        };

        graphics.DrawString(
            "Total:",
            font,
            brush,
            new RectangleF(
                x + width - 160,
                y,
                80,
                20),
            rightFormat);

        graphics.DrawString(
            data.MontoNumerico.ToString("$ 0.00"),
            font,
            brush,
            new RectangleF(
                x + width - 80,
                y,
                80,
                20),
            rightFormat);

        y += 20;
    }

    // ================================================================
    // MONTO EN LETRAS
    // ================================================================

    private void DrawMontoLetras(
        Graphics graphics,
        ReciboDocumentModel data,
        float x,
        ref float y,
        float width,
        Font font,
        Brush brush)
    {
        graphics.DrawString(
            $"Recibí el importe de: {data.MontoLetras.ToLower()} .-",
            font,
            brush,
            new RectangleF(
                x,
                y,
                width,
                40));
    }
}