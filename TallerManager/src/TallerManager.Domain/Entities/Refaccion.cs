using System.ComponentModel.DataAnnotations;

namespace TallerManager.Domain.Entities;

public class Refaccion
{
    public int Id { get; set; }
    public int TallerId { get; set; }

    public string? Codigo { get; set; }
    public string? Sku { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
    public string? Marca { get; set; }
    public string? Categoria { get; set; }

    public int Existencia { get; set; }
    public int StockMinimo { get; set; }

    public decimal Costo { get; set; }
    public decimal Precio { get; set; }

    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
}