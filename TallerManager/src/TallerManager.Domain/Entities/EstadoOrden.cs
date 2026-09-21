namespace TallerManager.Domain.Entities;

public enum EstadoOrden
{
    Recibido,
    Diagnostico,
    Cotizacion,
    EsperandoAutorizacion,
    Autorizado,
    EnReparacion,
    EsperandoRefaccion,
    Pausado,
    Terminado,
    ListoParaEntrega,
    Entregado,
    Cancelado
}