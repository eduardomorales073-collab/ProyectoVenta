using Aplicacion.Mensajeria.Eventos;
using Aplicacion.Modelos;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aplicacion.Mensajeria.Consumidores
{
    public class OfertaRegistradaConsumer : IConsumer<OfertaRegistrada>
    {
        private readonly IMongoCollection<OfertaHistorial> _collection;
        private readonly ILogger<OfertaRegistradaConsumer> _logger;

        public OfertaRegistradaConsumer(
            IMongoCollection<OfertaHistorial> collection,
            ILogger<OfertaRegistradaConsumer> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OfertaRegistrada> context)
        {
            var msg = context.Message;

            var historial = new OfertaHistorial
            {
                OfertaId = msg.OfertaId,
                IdProveedor = msg.IdProveedor,
                IdPedidoInterno = msg.IdPedidoInterno,
                Precio = msg.Precio,
                Fecha = msg.Fecha,
                Proveedor = msg.Proveedor
            };

            await _collection.InsertOneAsync(historial);

            _logger.LogInformation("Oferta registrada en MongoDB: {OfertaId}", msg.OfertaId);
        }
    }
}