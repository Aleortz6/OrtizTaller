namespace TallerManager.Domain.Entities;

public enum TipoMovimiento
{
    Entrada,
    Salida,
    Ajuste
}

public class MovimientoInventario
{
    public int Id { get; set; }
    public int TallerId { get; set; }

    public int RefaccionId { get; set; }
    public Refaccion? Refaccion { get; set; }

    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; }
    public decimal? CostoUnitario { get; set; }

    public string? Motivo { get; set; }
    public string? Referencia { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}