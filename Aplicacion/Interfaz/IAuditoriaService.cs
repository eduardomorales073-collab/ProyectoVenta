using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplicacion.Interfaz
{
    public interface IAuditoriaService
    {
        /// <summary>
        /// Obtiene el historial de auditoría unificado con filtros opcionales.
        /// </summary>
        /// <param name="tipo">Tipo de evento: Compra, Pedido, Oferta, Orden, Cancelacion (null = todos)</param>
        /// <param name="fechaDesde">Fecha inicial (null = sin límite)</param>
        /// <param name="fechaHasta">Fecha final (null = sin límite)</param>
        /// <returns>Lista de eventos ordenados por fecha descendente</returns>
        Task<List<EventoAuditoriaDTO>> GetHistorialAsync(
            string? tipo = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null);
    }
}