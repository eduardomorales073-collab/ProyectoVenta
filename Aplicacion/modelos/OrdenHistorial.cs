using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Aplicacion.Modelos
{
    public class OrdenHistorial
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public int OrdenId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha_Creacion { get; set; }
        public DateTime Fecha_Limite { get; set; }
        public int Tipo_Orden { get; set; }
    }
}