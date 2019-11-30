using MensajeApi.Models;
using MensajeApi.Users;
using Metodos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MensajeApi.Auth
{
    
        public interface IUsuarioService
        {
            Usuario Authenticate(string username, string password);
            //IEnumerable<Usuario> GetAll();
        }

        public class UserService : IUsuarioService
        {
            private readonly UsuarioService _UsuarioService;
           
            private readonly AppSettings _appSettings;

            public UserService(IOptions<AppSettings> appSettings, UsuarioService UsuarioService)
            {
                _appSettings = appSettings.Value;
            _UsuarioService = UsuarioService;
            }

            public Usuario Authenticate(string username, string password)
            {                
                var user = _UsuarioService.Get(username);
                SDESRepository n1 = new SDESRepository();
                n1.ObtenerLlave(_appSettings.Key);
                // return null if user not found
                if (user == null)
                    return null;
                var ContrasenaDescifrada = n1.DescifrarPass(user.Contrasena);
                if (!password.Equals(ContrasenaDescifrada))
                {
                    return null;
                }
                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.User.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);

                return user.WithoutPassword();
            }

           
        }
    
}
