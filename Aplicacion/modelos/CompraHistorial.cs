using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aplicacion.Modelos
{
    public class CompraHistorial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public Guid OrdenId { get; set; }
        public string Detalle { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Proveedor { get; set; } = string.Empty;
    }
}