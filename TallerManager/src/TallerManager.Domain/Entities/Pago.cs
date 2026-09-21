namespace TallerManager.Domain.Entities;

public enum MetodoPago
{
    Efectivo,
    Tarjeta,
    Transferencia,
    Otro
}

public class Pago
{
    public int Id { get; set; }
    public int TallerId { get; set; }

    public int OrdenId { get; set; }
    public OrdenServicio? Orden { get; set; }

    public decimal Monto { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public string? Referencia { get; set; }
    public string? Notas { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}