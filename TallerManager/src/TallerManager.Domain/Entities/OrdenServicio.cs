using System.ComponentModel.DataAnnotations;

namespace TallerManager.Domain.Entities;

public class OrdenServicio
{
    public int Id { get; set; }
    public int TallerId { get; set; }

    public string Folio { get; set; } = string.Empty;

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int VehiculoId { get; set; }
    public Vehiculo? Vehiculo { get; set; }

    public EstadoOrden Estado { get; set; } = EstadoOrden.Recibido;

    public DateTime FechaRecepcion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEstimadaEntrega { get; set; }
    public DateTime? FechaEntrega { get; set; }

    public int? Kilometraje { get; set; }

    [MaxLength(4000)]
    public string? Diagnostico { get; set; }

    [MaxLength(2000)]
    public string? Observaciones { get; set; }

public decimal Subtotal { get; set; }
public decimal Descuento { get; set; }
public string? MotivoDescuento { get; set; }
public decimal Impuestos { get; set; }
public decimal Total { get; set; }

    public bool Autorizado { get; set; }
    public DateTime? FechaAutorizacion { get; set; }

    public ICollection<OrdenDetalle> Detalles { get; set; } = new List<OrdenDetalle>();

    public bool Activo { get; set; } = true;
}