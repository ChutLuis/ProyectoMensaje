using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MensajeApi.Mensaje
{
    public class AddNewMessageModel
    {
        public string Id { get; set; } // Id de la Conversacion
        public string Message { get; set; }// El mensaje
        public string EmisorMensaje { get; set; }
        public DateTime FechaEnviado { get; set; }//A que hora lo envio
    }
}
