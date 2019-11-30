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
    public class ConversacionServices
    {
        private readonly IMongoCollection<Conversacion> _conversacion;
        public ConversacionServices(IConversacionDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _conversacion = database.GetCollection<Conversacion>(settings.ConversacionCollectionName);
            
        }

        public List<Conversacion> Get() =>
            _conversacion.Find(Conversacion => true).ToList();


        public Conversacion Get(string Emisor, string Receptor) =>
           _conversacion.Find<Conversacion>(Conversacion => Conversacion.Remitente == Emisor && Conversacion.Destinatario == Receptor).FirstOrDefault();

        public List<Conversacion> Get(string User) =>
            _conversacion.Find(Conversacion => Conversacion.Remitente == User||Conversacion.Destinatario==User).ToList();

        public Conversacion GetId(string Id) =>
            _conversacion.Find(Conversacion => Conversacion.Id == Id).FirstOrDefault();

        public Conversacion Create(Conversacion Nconversacion)
        {
            var existe = Get(Nconversacion.Remitente,Nconversacion.Destinatario);
            if (existe == null)
            {
                _conversacion.InsertOne(Nconversacion);
            }
            else
            {
                return null;
            }
            
            return Nconversacion;
        }

        public void Update(string Remitente, string Destinatario, Conversacion NConv) =>
           _conversacion.ReplaceOne(Conv => Conv.Remitente == Remitente&&Conv.Destinatario==Destinatario, NConv);



        public void Remove(Conversacion msgd) =>
            _conversacion.DeleteOne(msg => msg.Id == msgd.Id);

    }
}
