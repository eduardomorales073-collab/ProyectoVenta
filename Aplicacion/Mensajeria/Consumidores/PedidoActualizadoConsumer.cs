using Aplicacion.Mensajeria.Eventos;
using Aplicacion.Modelos;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aplicacion.Mensajeria.Consumidores
{
    public class PedidoActualizadoConsumer : IConsumer<PedidoActualizado>
    {
        private readonly IMongoCollection<PedidoActualizacionHistorial> _collection;
        private readonly ILogger<PedidoActualizadoConsumer> _logger;

        public PedidoActualizadoConsumer(
            IMongoCollection<PedidoActualizacionHistorial> collection,
            ILogger<PedidoActualizadoConsumer> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PedidoActualizado> context)
        {
            var msg = context.Message;
            var historial = new PedidoActualizacionHistorial
            {
                PedidoId = msg.PedidoId,
                Codigo = msg.Codigo,
                Cantidad = msg.Cantidad,
                IdDepartamento = msg.IdDepartamento,
                Fecha = msg.Fecha,
                Usuario = msg.Usuario
            };

            await _collection.InsertOneAsync(historial);
            _logger.LogInformation("Pedido actualizado registrado en MongoDB: {Codigo}", msg.Codigo);
        }
    }
}