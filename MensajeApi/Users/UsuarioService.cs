using MensajeApi.Auth;
using MensajeApi.Models;
using Metodos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MensajeApi.Users
{
    public class UsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuario;

        private readonly AppSettings _appSettings;
        public UsuarioService(IUsuarioDatabaseSettings settings, IOptions<AppSettings> appSettings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _usuario = database.GetCollection<Usuario>(settings.UsuarioCollectionName);
            _appSettings = appSettings.Value;
        }

        public List<Usuario> Get()
        {
            var usuarios = _usuario.Find(Usuario => true).ToList();

            for (int i = 0; i < usuarios.Count; i++)
            {
                usuarios[i].Token = null;
                usuarios[i].Contrasena = null;
            }

            return usuarios;

        }

        public Usuario Get(string User)
        {
            var user =  _usuario.Find<Usuario>(Usuario => Usuario.User == User).FirstOrDefault();
            user.Token = null;
            return user;
        }

        public SubmitModel Create(SubmitModel Usuario)
        {
            SDESRepository n1 = new SDESRepository();
            n1.ObtenerLlave(_appSettings.Key);
            Usuario Inserted = new Usuario();
            Inserted.Nombre = Usuario.Nombre;
            Inserted.Apellido = Usuario.Apellido;
            Inserted.User = Usuario.User;
            Inserted.Edad = Usuario.Edad;
            Inserted.Contrasena = n1.CifrarPass(Usuario.Contrasena);
            _usuario.InsertOne(Inserted);
            Usuario.Contrasena = null;
            Usuario.Token = null;
            return Usuario;
        }

        public void Update(string NombredeUsuario, Usuario UsuarioIn) =>
            _usuario.ReplaceOne(Usuario => Usuario.User== NombredeUsuario, UsuarioIn);

        public void Remove(Usuario UsuarioIn) =>
            _usuario.DeleteOne(Usuario => Usuario.User == UsuarioIn.User);

        public void Remove(string User) =>
            _usuario.DeleteOne(Usuario => Usuario.User == User);
    }
}
