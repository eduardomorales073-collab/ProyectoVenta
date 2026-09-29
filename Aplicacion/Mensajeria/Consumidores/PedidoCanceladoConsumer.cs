using Aplicacion.Mensajeria.Eventos;
using Aplicacion.Modelos;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aplicacion.Mensajeria.Consumidores
{
    public class PedidoCanceladoConsumer : IConsumer<PedidoCancelado>
    {
        private readonly IMongoCollection<CancelacionHistorial> _collection;
        private readonly ILogger<PedidoCanceladoConsumer> _logger;

        public PedidoCanceladoConsumer(
            IMongoCollection<CancelacionHistorial> collection,
            ILogger<PedidoCanceladoConsumer> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PedidoCancelado> context)
        {
            var msg = context.Message;

            var historial = new CancelacionHistorial
            {
                PedidoId = msg.PedidoId,
                Codigo = msg.Codigo,
                Motivo = msg.Motivo,
                Fecha = msg.Fecha,
                Usuario = msg.Usuario
            };

            await _collection.InsertOneAsync(historial);

            _logger.LogInformation("Cancelación registrada en MongoDB: {PedidoId}", msg.PedidoId);
        }
    }
}