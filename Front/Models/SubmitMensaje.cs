
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Front.Models
{
    public class SubmitMensaje
    {        
        public string Id { get; set; }        
        public string Message { get; set; }// El mensaje        
        public string Emisor { get; set; }//Quien lo envio        
        public DateTime FechaEnviado { get; set; }//Quien lo envio
    }
}
