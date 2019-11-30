using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MensajeApi.Mensaje
{
    public class Conversacion
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // Id de la Conversacion
        [BsonElement("Remitente")]
        public string Remitente { get; set; }//Quien lo envio
        [BsonElement("Destinatario")]
        public string Destinatario { get; set; }//Quien lo recibio
        [BsonElement("Mensajes")]
        public List<string> Mensajes { get; set; }//Lista de Ids de los mensajes

    }
}
