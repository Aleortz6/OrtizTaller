using System.ComponentModel.DataAnnotations;

namespace TallerManager.Domain.Entities;

public enum TipoCita
{
    Cita,
    Recepcion,
    Diagnostico,
    Entrega,
    Servicio
}

public enum EstadoCita
{
    Pendiente,
    Confirmada,
    Completada,
    Cancelada
}

public class Cita
{
    public int Id { get; set; }
    public int TallerId { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    public string Titulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int? VehiculoId { get; set; }
    public Vehiculo? Vehiculo { get; set; }

    public int? OrdenId { get; set; }
    public OrdenServicio? Orden { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public int DuracionMinutos { get; set; } = 30;

    public TipoCita Tipo { get; set; } = TipoCita.Cita;
    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;

    public bool Activo { get; set; } = true;
}