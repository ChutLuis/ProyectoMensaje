using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MensajeApi.Users
{
    public class UsuarioDatabaseSettings : IUsuarioDatabaseSettings
    {
        public string UsuarioCollectionName { get ; set; }
        public string ConnectionString { get; set ; }
        public string DatabaseName { get; set; }
    }
    public interface IUsuarioDatabaseSettings
    {
        string UsuarioCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
