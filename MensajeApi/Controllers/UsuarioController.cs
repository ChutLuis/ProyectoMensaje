using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MensajeApi.Models;
using MensajeApi.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MensajeApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]    
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _UsuarioService;

        public UsuarioController(UsuarioService UsuarioService)
        {
            _UsuarioService = UsuarioService;
        }

        [HttpGet]
        public ActionResult<List<Usuario>> Get() =>
            _UsuarioService.Get();

        [HttpGet("GetWithParam")]
        public ActionResult<Usuario> Get(string Id)
        {
            var Usuario = _UsuarioService.Get(Id);
            Usuario.Contrasena = null;
            if (Usuario == null)
            {
                return NotFound();
            }

            return Ok(Usuario);
        }

        [HttpPost]
        public ActionResult<Usuario> Create(SubmitModel Usuario)
        {
            var Inserted = _UsuarioService.Create(Usuario);            
            if (Inserted == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            return Ok(Inserted);
        }

        [HttpPut]
        public IActionResult Update(string User, Usuario UsuarioIn)
        {
            var Usuario = _UsuarioService.Get(User);

            if (Usuario == null)
            {
                return NotFound();
            }

            _UsuarioService.Update(User, UsuarioIn);

            return NoContent();
        }

        [HttpDelete]
        public IActionResult Delete(string User)
        {
            var Usuario = _UsuarioService.Get(User);

            if (Usuario == null)
            {
                return NotFound();
            }

            _UsuarioService.Remove(Usuario.User);

            return NoContent();
        }
    }
}
