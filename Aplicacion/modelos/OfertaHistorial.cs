using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Aplicacion.Modelos
{
    public class OfertaHistorial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int OfertaId { get; set; }
        public int IdProveedor { get; set; }
        public int IdPedidoInterno { get; set; }
        public decimal Precio { get; set; }
        public DateTime Fecha { get; set; }
        public string Proveedor { get; set; } = string.Empty;
    }
}