using System.ComponentModel.DataAnnotations;

namespace TallerManager.Domain.Entities;

public class Personal
{
    public int Id { get; set; }
    public int TallerId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    public string? Telefono { get; set; }
    public string? Puesto { get; set; }
    public string? Especialidad { get; set; }

    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    public string? UsuarioId { get; set; }
    public ApplicationUser? Usuario { get; set; }
}