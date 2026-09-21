using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TallerManager.Domain.Entities;

namespace TallerManager.Web.PdfDocuments;

public class CotizacionDocument : IDocument
{
    private readonly OrdenServicio _orden;
private readonly Taller _taller;
private readonly string _wwwrootPath;

public CotizacionDocument(OrdenServicio orden, Taller taller, string wwwrootPath)
{
    _orden = orden;
    _taller = taller;
    _wwwrootPath = wwwrootPath;
}

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.Letter);
            page.Margin(40);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Calibri));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Cotización generada el ").FontSize(8).FontColor(Colors.Grey.Medium);
                x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });
    }

   private void ComposeHeader(IContainer container)
{
    var color = string.IsNullOrWhiteSpace(_taller.ColorPrimario) ? "#003d9b" : _taller.ColorPrimario;

    container.Row(row =>
    {
        if (!string.IsNullOrWhiteSpace(_taller.LogoUrl))
        {
            var rutaFisica = Path.Combine(_wwwrootPath, _taller.LogoUrl.TrimStart('/').Split('?')[0]);
            if (File.Exists(rutaFisica))
            {
                row.ConstantItem(60).Height(60).Image(rutaFisica).FitArea();
            }
        }

        row.RelativeItem().PaddingLeft(10).Column(col =>
        {
            col.Item().Text(_taller.Nombre).FontSize(18).Bold().FontColor(color);
            if (!string.IsNullOrWhiteSpace(_taller.Direccion))
                col.Item().Text(_taller.Direccion).FontSize(9).FontColor(Colors.Grey.Darken1);
            if (!string.IsNullOrWhiteSpace(_taller.Telefono))
                col.Item().Text($"Tel: {_taller.Telefono}").FontSize(9).FontColor(Colors.Grey.Darken1);
            if (!string.IsNullOrWhiteSpace(_taller.Email))
                col.Item().Text(_taller.Email).FontSize(9).FontColor(Colors.Grey.Darken1);
        });

        row.ConstantItem(150).Column(col =>
        {
            col.Item().AlignRight().Text("COTIZACIÓN").FontSize(14).Bold();
            col.Item().AlignRight().Text(_orden.Folio).FontSize(12).FontColor(color);
            col.Item().AlignRight().Text(_orden.FechaRecepcion.ToString("dd/MM/yyyy")).FontSize(9);
        });
    });
}

    private void ComposeContent(IContainer container)
    {
        container.PaddingTop(20).Column(col =>
        {
            col.Item().Element(ComposeClienteVehiculo);
            col.Item().PaddingTop(20).Element(ComposeTablaDetalles);
            col.Item().PaddingTop(10).Element(ComposeTotales);
            col.Item().PaddingTop(30).Element(ComposeTerminos);
        });
    }

    private void ComposeClienteVehiculo(IContainer container)
    {
        container.Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("CLIENTE").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                col.Item().Text(_orden.Cliente?.Nombre ?? "—").FontSize(11).Bold();
                if (!string.IsNullOrWhiteSpace(_orden.Cliente?.Telefono))
                    col.Item().Text(_orden.Cliente!.Telefono!).FontSize(9);
            });

            row.RelativeItem().Column(col =>
            {
                col.Item().Text("VEHÍCULO").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                col.Item().Text($"{_orden.Vehiculo?.Marca} {_orden.Vehiculo?.Modelo} {_orden.Vehiculo?.Anio}").FontSize(11).Bold();
                if (!string.IsNullOrWhiteSpace(_orden.Vehiculo?.Placas))
                    col.Item().Text($"Placas: {_orden.Vehiculo!.Placas}").FontSize(9);
            });
        });
    }

    private void ComposeTablaDetalles(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(4);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1.5f);
                columns.RelativeColumn(1.5f);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyleHeader).Text("DESCRIPCIÓN");
                header.Cell().Element(CellStyleHeader).AlignRight().Text("CANT.");
                header.Cell().Element(CellStyleHeader).AlignRight().Text("PRECIO UNIT.");
                header.Cell().Element(CellStyleHeader).AlignRight().Text("SUBTOTAL");
            });

            foreach (var detalle in _orden.Detalles.OrderBy(d => d.Id))
            {
                table.Cell().Element(CellStyle).Text(detalle.Descripcion);
                table.Cell().Element(CellStyle).AlignRight().Text(detalle.Cantidad.ToString("0.##"));
                table.Cell().Element(CellStyle).AlignRight().Text(detalle.PrecioUnitario.ToString("C2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detalle.Subtotal.ToString("C2"));
            }

            IContainer CellStyleHeader(IContainer c) =>
    c.Background(string.IsNullOrWhiteSpace(_taller.ColorPrimario) ? "#003d9b" : _taller.ColorPrimario)
     .Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).FontSize(8).Bold());

            IContainer CellStyle(IContainer c) =>
                c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6);
        });
    }

    private void ComposeTotales(IContainer container)
    {
        container.AlignRight().Column(col =>
        {
            col.Item().Row(row =>
            {
                row.ConstantItem(100).Text("Subtotal:").FontSize(9);
                row.ConstantItem(80).AlignRight().Text(_orden.Subtotal.ToString("C2")).FontSize(9);
            });
            col.Item().Row(row =>
            {
                row.ConstantItem(100).Text("Descuento:").FontSize(9);
                row.ConstantItem(80).AlignRight().Text($"-{_orden.Descuento:C2}").FontSize(9);
            });
            col.Item().Row(row =>
            {
                row.ConstantItem(100).Text("Impuestos:").FontSize(9);
                row.ConstantItem(80).AlignRight().Text(_orden.Impuestos.ToString("C2")).FontSize(9);
            });
            col.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Grey.Darken1).Row(row =>
            {
                row.ConstantItem(100).Text("TOTAL:").FontSize(12).Bold();
                row.ConstantItem(80).AlignRight().Text(_orden.Total.ToString("C2")).FontSize(12).Bold()
   .FontColor(string.IsNullOrWhiteSpace(_taller.ColorPrimario) ? "#003d9b" : _taller.ColorPrimario);
            });
        });
    }

    private void ComposeTerminos(IContainer container)
{
    var terminos = string.IsNullOrWhiteSpace(_taller.TerminosLegales)
        ? "Esta cotización tiene una vigencia de 3 días naturales a partir de su fecha de emisión."
        : _taller.TerminosLegales;

    container.Column(col =>
    {
        col.Item().Text(terminos).FontSize(8).FontColor(Colors.Grey.Darken1);
            col.Item().PaddingTop(15).Text("Firma de autorización del cliente:").FontSize(9);
            col.Item().PaddingTop(30).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
        });
    }
}

