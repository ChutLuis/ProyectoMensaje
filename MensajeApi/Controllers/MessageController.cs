using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MensajeApi.Mensaje;
using MensajeApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MensajeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MensajeServices _MensajeService;

        public MessageController(MensajeServices MensajeService)
        {
            _MensajeService = MensajeService;
        }


        [HttpGet("GetWithParam")]
        public ActionResult<SubmitModel> Get(string Id)
        {
            var Mensaje = _MensajeService.Get(Id);
            if (Mensaje == null)
            {
                return NotFound();
            }

            return Ok(Mensaje);
        }

        [HttpPost]
        public ActionResult<Mensaje.Mensaje> Create(SubmitMensaje Mensaje)
        {
            var Inserted = _MensajeService.Create(Mensaje);
            if (Inserted == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            return Ok(Inserted);
        }

        //[HttpPut]
        //public IActionResult Update(string Id, Mensaje.SubmitMensaje MensajeIn)
        //{
        //    var Mensaje = _MensajeService.Get(Id);

        //    if (Mensaje == null)
        //    {
        //        return NotFound();
        //    }

        //    _MensajeService.Update(User, MensajeIn);

        //    return NoContent();
        //}

        [HttpDelete]
        public IActionResult Delete(Mensaje.Mensaje mensaje)
        {
            var Mensaje = _MensajeService.Get(mensaje.Id);

            if (Mensaje == null)
            {
                return NotFound();
            }

            _MensajeService.Remove(mensaje);

            return NoContent();
        }
    }
}
