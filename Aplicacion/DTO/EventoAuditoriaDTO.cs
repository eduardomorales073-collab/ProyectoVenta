using System;

namespace Aplicacion.DTO
{
    /// <summary>
    /// DTO unificado para todos los eventos de auditoría del ciclo de compras.
    /// </summary>
    public record EventoAuditoriaDTO(
        string Id,               // _id de MongoDB
        string Tipo,             // "Compra", "Pedido", "Oferta", "Orden", "Cancelacion"
        DateTime Fecha,          // Fecha del evento (unificada)
        string Titulo,           // Resumen corto (ej. "PED-2026-010")
        string Detalle,          // Detalle completo
        string? Usuario,         // Usuario que realizó el evento (si aplica)
        string? Referencia       // Id de referencia (ej. PedidoId, OrdenId)
    );
}