using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TallerManager.Domain.Entities;

namespace TallerManager.Web.PdfDocuments;

public class ComprobanteEntregaDocument : IDocument
{
    private readonly OrdenServicio _orden;
    private readonly Taller _taller;
    private readonly List<FotografiaOrden> _fotos;
    private readonly List<Pago> _pagos;
    private readonly string _wwwrootPath;

    public ComprobanteEntregaDocument(OrdenServicio orden, Taller taller, List<FotografiaOrden> fotos, List<Pago> pagos, string wwwrootPath)
    {
        _orden = orden;
        _taller = taller;
        _fotos = fotos;
        _pagos = pagos;
        _wwwrootPath = wwwrootPath;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        var color = string.IsNullOrWhiteSpace(_taller.ColorPrimario) ? "#003d9b" : _taller.ColorPrimario;
        var totalPagado = _pagos.Sum(p => p.Monto);
        var saldoPendiente = _orden.Total - totalPagado;

        container.Page(page =>
        {
            page.Size(PageSizes.Letter);
            page.Margin(40);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Calibri));

            page.Header().Row(row =>
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
                });

                row.ConstantItem(150).Column(col =>
                {
                    col.Item().AlignRight().Text("COMPROBANTE DE ENTREGA").FontSize(12).Bold();
                    col.Item().AlignRight().Text(_orden.Folio).FontSize(12).FontColor(color);
                    col.Item().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(9);
                });
            });

            page.Content().PaddingTop(20).Column(col =>
            {
                col.Item().Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("CLIENTE").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                        c.Item().Text(_orden.Cliente?.Nombre ?? "—").FontSize(11).Bold();
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("VEHÍCULO").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                        c.Item().Text($"{_orden.Vehiculo?.Marca} {_orden.Vehiculo?.Modelo} {_orden.Vehiculo?.Anio}").FontSize(11).Bold();
                        if (!string.IsNullOrWhiteSpace(_orden.Vehiculo?.Placas))
                            c.Item().Text($"Placas: {_orden.Vehiculo!.Placas}").FontSize(9);
                    });
                });

                col.Item().PaddingTop(20).Text("TRABAJOS REALIZADOS").FontSize(9).Bold().FontColor(Colors.Grey.Darken1);
col.Item().PaddingTop(5).Table(table =>
{
    table.ColumnsDefinition(columns =>
    {
        columns.RelativeColumn(1.5f);
        columns.RelativeColumn(3.5f);
        columns.RelativeColumn(1);
        columns.RelativeColumn(1.5f);
        columns.RelativeColumn(1.5f);
    });

    table.Header(header =>
    {
        header.Cell().Element(CellStyleHeader).Text("TIPO");
        header.Cell().Element(CellStyleHeader).Text("DESCRIPCIÓN");
        header.Cell().Element(CellStyleHeader).AlignRight().Text("CANT.");
        header.Cell().Element(CellStyleHeader).AlignRight().Text("PRECIO UNIT.");
        header.Cell().Element(CellStyleHeader).AlignRight().Text("SUBTOTAL");
    });

    foreach (var detalle in _orden.Detalles.OrderBy(d => d.Id))
    {
        table.Cell().Element(CellStyle).Text(EtiquetaTipo(detalle.Tipo));
        table.Cell().Element(CellStyle).Text(detalle.Descripcion);
        table.Cell().Element(CellStyle).AlignRight().Text(detalle.Cantidad.ToString("0.##"));
        table.Cell().Element(CellStyle).AlignRight().Text(detalle.PrecioUnitario.ToString("C2"));
        table.Cell().Element(CellStyle).AlignRight().Text(detalle.Subtotal.ToString("C2"));
    }

    IContainer CellStyleHeader(IContainer c) =>
        c.Background(color).Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).FontSize(8).Bold());

    static IContainer CellStyle(IContainer c) =>
        c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6);
});

                col.Item().PaddingTop(15).AlignRight().Column(c =>
{
    c.Item().Row(row =>
    {
        row.ConstantItem(140).Text("Subtotal:").FontSize(10);
        row.ConstantItem(80).AlignRight().Text(_orden.Subtotal.ToString("C2")).FontSize(10);
    });

    if (_orden.Descuento > 0)
    {
        c.Item().Row(row =>
        {
            var etiquetaDescuento = string.IsNullOrWhiteSpace(_orden.MotivoDescuento)
                ? "Descuento:"
                : $"Descuento ({_orden.MotivoDescuento}):";
            row.ConstantItem(140).Text(etiquetaDescuento).FontSize(10);
            row.ConstantItem(80).AlignRight().Text($"-{_orden.Descuento:C2}").FontSize(10);
        });
    }

    c.Item().Row(row =>
    {
        row.ConstantItem(140).Text("Total del servicio:").FontSize(10);
        row.ConstantItem(80).AlignRight().Text(_orden.Total.ToString("C2")).FontSize(10);
    });
    c.Item().Row(row =>
    {
        row.ConstantItem(140).Text("Total pagado:").FontSize(10);
        row.ConstantItem(80).AlignRight().Text(totalPagado.ToString("C2")).FontSize(10);
    });
    c.Item().PaddingTop(3).BorderTop(1).BorderColor(Colors.Grey.Darken1).Row(row =>
    {
        row.ConstantItem(140).Text("Saldo pendiente:").FontSize(11).Bold();
        row.ConstantItem(80).AlignRight().Text(saldoPendiente.ToString("C2")).FontSize(11).Bold()
            .FontColor(saldoPendiente > 0 ? Colors.Red.Medium : color);
    });
});

                var fotosEntrega = _fotos.Where(f => f.Tipo == TipoFotografia.Entrega).ToList();
                if (fotosEntrega.Any())
                {
                    col.Item().PaddingTop(20).Text("ESTADO DEL VEHÍCULO AL MOMENTO DE LA ENTREGA").FontSize(9).Bold().FontColor(Colors.Grey.Darken1);
                    col.Item().PaddingTop(5).Row(row =>
                    {
                        foreach (var foto in fotosEntrega.Take(6))
                        {
                            var ruta = Path.Combine(_wwwrootPath, foto.RutaArchivo.TrimStart('/'));
                            if (File.Exists(ruta))
                            {
                                row.RelativeItem().Padding(3).Height(100).Image(ruta).FitArea();
                            }
                        }
                    });
                }

                col.Item().PaddingTop(30).Text("El cliente recibe el vehículo a su entera satisfacción, confirmando que los trabajos descritos fueron realizados correctamente.")
                    .FontSize(8).FontColor(Colors.Grey.Darken1);
                col.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        c.Item().PaddingTop(3).Text("Firma de conformidad del cliente").FontSize(8);
                    });
                });
            });

            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Comprobante generado el ").FontSize(8).FontColor(Colors.Grey.Medium);
                x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });
    }
    private static string EtiquetaTipo(TipoDetalleOrden tipo) => tipo switch
{
    TipoDetalleOrden.Servicio => "Servicio",
    TipoDetalleOrden.Refaccion => "Refacción",
    TipoDetalleOrden.ManoDeObra => "Mano de Obra",
    _ => tipo.ToString()
};
}