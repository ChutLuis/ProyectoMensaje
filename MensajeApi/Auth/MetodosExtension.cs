using MensajeApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MensajeApi.Auth
{
    public static class MetodosExtension
    {
        public static IEnumerable<Usuario> WithoutPasswords(this IEnumerable<Usuario> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static Usuario WithoutPassword(this Usuario user)
        {
            user.Contrasena = null;
            return user;
        }
    }
}
