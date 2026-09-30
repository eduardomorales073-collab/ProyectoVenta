using Aplicacion.Mensajeria.Eventos;
using Aplicacion.Modelos;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Aplicacion.Mensajeria.Consumidores
{
    public class AdjudicacionCreadaConsumer : IConsumer<AdjudicacionCreada>
    {
        private readonly IMongoCollection<AdjudicacionHistorial> _collection;
        private readonly ILogger<AdjudicacionCreadaConsumer> _logger;

        public AdjudicacionCreadaConsumer(
            IMongoCollection<AdjudicacionHistorial> collection,
            ILogger<AdjudicacionCreadaConsumer> logger)
        {
            _collection = collection;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AdjudicacionCreada> context)
        {
            var msg = context.Message;
            var historial = new AdjudicacionHistorial
            {
                AdjudicacionId = msg.AdjudicacionId,
                OrdenCompra = msg.OrdenCompra,
                Fecha_Resolucion = msg.Fecha_Resolucion,
                Estado = msg.Estado,
                Fecha = msg.Fecha,
                Usuario = msg.Usuario
            };

            await _collection.InsertOneAsync(historial);
            _logger.LogInformation("Adjudicación creada registrada en MongoDB: {AdjudicacionId}", msg.AdjudicacionId);
        }
    }
}