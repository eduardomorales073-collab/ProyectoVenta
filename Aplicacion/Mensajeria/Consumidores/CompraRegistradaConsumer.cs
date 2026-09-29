using Aplicacion.Mensajeria.Eventos;
using Aplicacion.Modelos;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aplicacion.Mensajeria.Consumidores
{
    public class CompraRegistradaConsumer : IConsumer<CompraRegistrada>
    {
        private readonly IMongoCollection<CompraHistorial> _collection;
        private readonly ILogger<CompraRegistradaConsumer> _logger;

        public CompraRegistradaConsumer(
            IMongoCollection<CompraHistorial> collection,
            ILogger<CompraRegistradaConsumer> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CompraRegistrada> context)
        {
            var msg = context.Message;

            var historial = new CompraHistorial
            {
                OrdenId = msg.OrdenId,
                Detalle = msg.Detalle,
                Monto = msg.Monto,
                Fecha = msg.Fecha,
                Proveedor = msg.Proveedor
            };

            await _collection.InsertOneAsync(historial);

            _logger.LogInformation("Historial de compra escrito en MongoDB: {OrdenId}", msg.OrdenId);
        }
    }
}