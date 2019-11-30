using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MensajeApi.Mensaje
{
    public class ConversacionDatabaseSettings : IConversacionDatabaseSettings
    {
        public string ConversacionCollectionName { get ; set ; }
        public string ConnectionString { get ; set; }
        public string DatabaseName { get ; set; }
    }
    public interface IConversacionDatabaseSettings
    {
        string ConversacionCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
