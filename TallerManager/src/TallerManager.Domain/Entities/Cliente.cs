using System.ComponentModel.DataAnnotations;

namespace TallerManager.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public int TallerId { get; set; }
    public Taller? Taller { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string? Notas { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}