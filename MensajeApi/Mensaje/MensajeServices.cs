using MensajeApi.Auth;
using Metodos;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MensajeApi.Mensaje
{
    public class MensajeServices
    {
        private readonly IMongoCollection<Mensaje> _mensaje;

        private readonly AppSettings _appSettings;
        public MensajeServices(IMensajeDatabaseSettings settings, IOptions<AppSettings> appSettings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _mensaje = database.GetCollection<Mensaje>(settings.MensajeCollectionName);
            _appSettings = appSettings.Value;
        }

        //public Mensaje Get(string Id) =>
        //    _mensaje.Find(Mensaje => Mensaje.Id == Id).FirstOrDefault();


        public SubmitMensaje Get(string Id)
        {
            SDESRepository sDES = new SDESRepository();
            sDES.ObtenerLlave(_appSettings.Key);
            //Obtener Mensaje Cifrado
            var Message = _mensaje.Find<Mensaje>(msg => msg.Id == Id).FirstOrDefault();
           //Descifrar Mensaje
            SubmitMensaje mensajeDescifrado = new SubmitMensaje();
            mensajeDescifrado.Emisor = Message.Emisor;
            mensajeDescifrado.FechaEnviado = Message.FechaEnviado;
            mensajeDescifrado.Id = Message.Id;
            mensajeDescifrado.Message = sDES.DescifrarPass(Message.Message);
            return mensajeDescifrado;

        }

        public Mensaje Create(SubmitMensaje Nmensaje)
        {
            SDESRepository n1 = new SDESRepository();
            n1.ObtenerLlave(_appSettings.Key);
            Mensaje Inserted = new Mensaje();
            Inserted.Emisor = Nmensaje.Emisor;
            Inserted.FechaEnviado = Nmensaje.FechaEnviado;            
            Inserted.Message = n1.CifrarPass(Nmensaje.Message);
            _mensaje.InsertOne(Inserted);            
            return Inserted;       
        }

       

        public void Remove(Mensaje msgd) =>
            _mensaje.DeleteOne(msg => msg.Id == msgd.Id);
        
    }
}
