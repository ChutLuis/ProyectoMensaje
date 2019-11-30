using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MensajeApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MensajeApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private IUsuarioService _userService;

        public LogInController(IUsuarioService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AutenticarModel model)
        {
            var user = _userService.Authenticate(model.UserName, model.Password);
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpGet("Autorizado")]
        public string Autorizado()
        {
            int i = 1;
            return "El Usuario Esta Autorizado";
        }


    }
}
