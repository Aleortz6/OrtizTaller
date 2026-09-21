namespace TallerManager.Domain.Entities;

public enum TipoFotografia
{
    Recepcion,
    Dano,
    Diagnostico,
    Reparacion,
    Refaccion,
    Entrega
}

public class FotografiaOrden
{
    public int Id { get; set; }
    public int TallerId { get; set; }

    public int OrdenId { get; set; }
    public OrdenServicio? Orden { get; set; }

    public TipoFotografia Tipo { get; set; } = TipoFotografia.Recepcion;
    public string RutaArchivo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}