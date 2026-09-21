namespace TallerManager.Domain.Entities;

public class Taller
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? RazonSocial { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public string? LogoUrl { get; set; }
    public string? ColorPrimario { get; set; } = "#003d9b";
    public string? TerminosLegales { get; set; }
    public string? Configuracion { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}