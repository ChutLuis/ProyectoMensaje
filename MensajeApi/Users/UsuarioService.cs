using MensajeApi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MensajeApi.Users
{
    public class UsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuario;

        public UsuarioService(IUsuarioDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _usuario = database.GetCollection<Usuario>(settings.UsuarioCollectionName);
        }

        public List<Usuario> Get() =>
            _usuario.Find(Usuario => true).ToList();

        public Usuario Get(string id) =>
            _usuario.Find<Usuario>(Usuario => Usuario.Id == id).FirstOrDefault();

        public Usuario Create(Usuario Usuario)
        {
            _usuario.InsertOne(Usuario);
            return Usuario;
        }

        public void Update(string id, Usuario UsuarioIn) =>
            _usuario.ReplaceOne(Usuario => Usuario.Id == id, UsuarioIn);

        public void Remove(Usuario UsuarioIn) =>
            _usuario.DeleteOne(Usuario => Usuario.Id == UsuarioIn.Id);

        public void Remove(string id) =>
            _usuario.DeleteOne(Usuario => Usuario.Id == id);
    }
}
