using Aplicacion.Mensajeria.Eventos;
using Aplicacion.Modelos;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aplicacion.Mensajeria.Consumidores
{
    public class OfertaActualizadaConsumer : IConsumer<OfertaActualizada>
    {
        private readonly IMongoCollection<OfertaActualizacionHistorial> _collection;
        private readonly ILogger<OfertaActualizadaConsumer> _logger;

        public OfertaActualizadaConsumer(
            IMongoCollection<OfertaActualizacionHistorial> collection,
            ILogger<OfertaActualizadaConsumer> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OfertaActualizada> context)
        {
            var msg = context.Message;
            var historial = new OfertaActualizacionHistorial
            {
                OfertaId = msg.OfertaId,
                IdProveedor = msg.IdProveedor,
                IdPedidoInterno = msg.IdPedidoInterno,
                PrecioAnterior = msg.PrecioAnterior,
                PrecioNuevo = msg.PrecioNuevo,
                Fecha = msg.Fecha,
                Proveedor = msg.Proveedor
            };

            await _collection.InsertOneAsync(historial);
            _logger.LogInformation("Oferta actualizada registrada en MongoDB: {OfertaId}", msg.OfertaId);
        }
    }
}