using System.ComponentModel.DataAnnotations;

namespace TallerManager.Domain.Entities;

public class Vehiculo
{
    public int Id { get; set; }
    public int TallerId { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    [Required(ErrorMessage = "La marca es obligatoria")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "El modelo es obligatorio")]
    public string Modelo { get; set; } = string.Empty;

    public int? Anio { get; set; }
    public string? Version { get; set; }
    public string? Vin { get; set; }
    public string? Placas { get; set; }
    public string? Color { get; set; }
    public int? Kilometraje { get; set; }
    public string? Combustible { get; set; }
    public string? Transmision { get; set; }
    public string? Motor { get; set; }
    public string? Notas { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;
}