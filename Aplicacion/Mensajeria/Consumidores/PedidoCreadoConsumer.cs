using Aplicacion.Mensajeria.Eventos;
using Aplicacion.Modelos;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aplicacion.Mensajeria.Consumidores
{
    public class PedidoCreadoConsumer : IConsumer<PedidoCreado>
    {
        private readonly IMongoCollection<PedidoHistorial> _collection;
        private readonly ILogger<PedidoCreadoConsumer> _logger;

        public PedidoCreadoConsumer(
            IMongoCollection<PedidoHistorial> collection,
            ILogger<PedidoCreadoConsumer> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PedidoCreado> context)
        {
            var msg = context.Message;

            var historial = new PedidoHistorial
            {
                PedidoId = msg.PedidoId,
                Codigo = msg.Codigo,
                IdDepartamento = msg.IdDepartamento,
                Cantidad = msg.Cantidad,
                Fecha = msg.Fecha,
                Usuario = msg.Usuario
            };

            await _collection.InsertOneAsync(historial);

            _logger.LogInformation("Pedido creado registrado en MongoDB: {Codigo}", msg.Codigo);
        }
    }
}