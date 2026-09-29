using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Aplicacion.Modelos
{
    public class CancelacionHistorial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int PedidoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }
}