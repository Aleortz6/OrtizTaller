namespace TallerManager.Domain.Entities;

public enum TipoDetalleOrden
{
    Servicio,
    Refaccion,
    ManoDeObra
}

public class OrdenDetalle
{
    public int Id { get; set; }

    public int OrdenId { get; set; }
    public OrdenServicio? Orden { get; set; }

    public TipoDetalleOrden Tipo { get; set; } = TipoDetalleOrden.Servicio;

    public int? RefaccionId { get; set; }
    public Refaccion? Refaccion { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public decimal Cantidad { get; set; } = 1;
    public decimal CostoUnitario { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public bool Autorizado { get; set; }
}