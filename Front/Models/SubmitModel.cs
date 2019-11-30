using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Front.Models
{
    public class SubmitModel
    {
        
        public string Id { get; set; }
        
        public string Nombre { get; set; }
        
        public string Apellido { get; set; }
       
        public string User { get; set; } //En espanol en la BD
       
        public string Contrasena { get; set; }
        
        public string Edad { get; set; }
        
        public string Token { get; set; }
    }
}
