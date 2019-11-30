using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MensajeApi.Mensaje
{
    public class SubmitMensaje
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("Mensaje")]
        public string Message { get; set; }// El mensaje
        [BsonElement("Emisor")]
        public string Emisor { get; set; }//Quien lo envio
        [BsonElement("Fecha")]
        public DateTime FechaEnviado { get; set; }//Quien lo envio
    }
}
