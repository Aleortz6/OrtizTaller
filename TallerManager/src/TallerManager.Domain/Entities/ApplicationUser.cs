using Microsoft.AspNetCore.Identity;

namespace TallerManager.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public int TallerId { get; set; }
    public Taller? Taller { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}