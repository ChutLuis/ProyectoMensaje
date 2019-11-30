using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Front.Models
{
    public class Conversacion
    {        
        public string Id { get; set; } // Id de la Conversacion        
        public string Remitente { get; set; }//Quien lo envio        
        public string Destinatario { get; set; }//Quien lo recibio        
        public List<string> Mensajes { get; set; }//Lista de Ids de los mensajes

    }
}
