using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TallerManager.Domain.Entities;

namespace TallerManager.Web.PdfDocuments;

public class ReciboRecepcionDocument : IDocument
{
    private readonly OrdenServicio _orden;
    private readonly Taller _taller;
    private readonly List<FotografiaOrden> _fotos;
    private readonly string _wwwrootPath;

    public ReciboRecepcionDocument(OrdenServicio orden, Taller taller, List<FotografiaOrden> fotos, string wwwrootPath)
    {
        _orden = orden;
        _taller = taller;
        _fotos = fotos;
        _wwwrootPath = wwwrootPath;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        var color = string.IsNullOrWhiteSpace(_taller.ColorPrimario) ? "#003d9b" : _taller.ColorPrimario;

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
                    col.Item().AlignRight().Text("RECIBO DE RECEPCIÓN").FontSize(13).Bold();
                    col.Item().AlignRight().Text(_orden.Folio).FontSize(12).FontColor(color);
                    col.Item().AlignRight().Text(_orden.FechaRecepcion.ToString("dd/MM/yyyy HH:mm")).FontSize(9);
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
                        if (!string.IsNullOrWhiteSpace(_orden.Cliente?.Telefono))
                            c.Item().Text(_orden.Cliente!.Telefono!).FontSize(9);
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("VEHÍCULO").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                        c.Item().Text($"{_orden.Vehiculo?.Marca} {_orden.Vehiculo?.Modelo} {_orden.Vehiculo?.Anio}").FontSize(11).Bold();
                        if (!string.IsNullOrWhiteSpace(_orden.Vehiculo?.Placas))
                            c.Item().Text($"Placas: {_orden.Vehiculo!.Placas}").FontSize(9);
                        if (!string.IsNullOrWhiteSpace(_orden.Vehiculo?.Color))
                            c.Item().Text($"Color: {_orden.Vehiculo!.Color}").FontSize(9);
                    });
                });

                col.Item().PaddingTop(15).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("KILOMETRAJE").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                        c.Item().Text($"{_orden.Kilometraje?.ToString("N0") ?? "—"} km").FontSize(10);
                    });
                });

                if (!string.IsNullOrWhiteSpace(_orden.Observaciones))
                {
                    col.Item().PaddingTop(15).Column(c =>
                    {
                        c.Item().Text("OBSERVACIONES DE RECEPCIÓN").FontSize(8).Bold().FontColor(Colors.Grey.Darken1);
                        c.Item().Text(_orden.Observaciones).FontSize(10);
                    });
                }

                if (_fotos.Any())
                {
                    col.Item().PaddingTop(20).Text("EVIDENCIA FOTOGRÁFICA").FontSize(9).Bold().FontColor(Colors.Grey.Darken1);
                    col.Item().PaddingTop(5).Row(row =>
                    {
                        foreach (var foto in _fotos.Take(6))
                        {
                            var ruta = Path.Combine(_wwwrootPath, foto.RutaArchivo.TrimStart('/'));
                            if (File.Exists(ruta))
                            {
                                row.RelativeItem().Padding(3).Height(100).Image(ruta).FitArea();
                            }
                        }
                    });
                }

                col.Item().PaddingTop(30).Text("Declaro haber entregado el vehículo en las condiciones descritas y aceptadas en este documento.")
                    .FontSize(8).FontColor(Colors.Grey.Darken1);
                col.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        c.Item().PaddingTop(3).Text("Firma del cliente").FontSize(8);
                    });
                    row.ConstantItem(20);
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        c.Item().PaddingTop(3).Text("Firma de recepción").FontSize(8);
                    });
                });
            });

            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Recibo generado el ").FontSize(8).FontColor(Colors.Grey.Medium);
                x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });
    }
}