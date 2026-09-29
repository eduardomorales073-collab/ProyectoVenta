using Aplicacion.Mensajeria.Eventos;
using Aplicacion.Modelos;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aplicacion.Mensajeria.Consumidores
{
    public class OrdenCreadaConsumer : IConsumer<OrdenCreada>
    {
        private readonly IMongoCollection<OrdenHistorial> _collection;
        private readonly ILogger<OrdenCreadaConsumer> _logger;

        public OrdenCreadaConsumer(
            IMongoCollection<OrdenHistorial> collection,
            ILogger<OrdenCreadaConsumer> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrdenCreada> context)
        {
            var msg = context.Message;

            var historial = new OrdenHistorial
            {
                OrdenId = msg.OrdenId,
                Descripcion = msg.Descripcion,
                Fecha_Creacion = msg.Fecha_Creacion,
                Fecha_Limite = msg.Fecha_Limite,
                Tipo_Orden = msg.Tipo_Orden
            };

            await _collection.InsertOneAsync(historial);

            _logger.LogInformation("Orden creada registrada en MongoDB: {OrdenId}", msg.OrdenId);
        }
    }
}