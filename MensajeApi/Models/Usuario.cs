using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MensajeApi.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("Nombre")]
        public string Nombre { get; set; }
        [BsonElement("Apellido")]
        public string Apellido { get; set; }
        [BsonElement("Usuario")]
        public string User { get; set; } //En espanol en la BD
        [BsonElement("Contrasena")]
        public List<byte> Contrasena { get; set; }
        [BsonElement("Edad")]
        public string Edad { get; set; }
        [BsonElement("Token")]
        public string Token { get; set; }

    }
}
