using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MensajeApi.Mensaje;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MensajeApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly ConversacionServices _ConversacionService;
        private readonly MensajeServices _mensajeServices;

        public ConversationController(ConversacionServices ConversacionService, MensajeServices mensaje)
        {
            _ConversacionService = ConversacionService;
            _mensajeServices = mensaje;
        }

        [HttpGet]
        public ActionResult<List<Conversacion>> Get() =>
            _ConversacionService.Get();

        [HttpGet("GetWithParam")]
        public ActionResult<Conversacion> Get(string Emisor, string Receptor)
        {
            var Conversacion = _ConversacionService.Get(Emisor, Receptor);

            if (Conversacion == null)
            {
                return NotFound();
            }

            return Conversacion;
        }

        [HttpGet("GetFromId")]
        public ActionResult<Conversacion> Get(string Id)
        {
            var Conversacion = _ConversacionService.GetId(Id);

            if (Conversacion == null)
            {
                return NotFound();
            }

            return Ok(Conversacion);
        }

        [HttpGet("GetUserConversations")]
        public ActionResult<List<Conversacion>> GetD(string User)
        {
            var Conversacion = _ConversacionService.Get(User);

            if (Conversacion == null)
            {
                return NotFound();
            }

            return Conversacion;
        }



        [HttpPost("Create")]
        public ActionResult<Conversacion> Create(Conversacion Conversacion)
        {
            var Inserted = _ConversacionService.Create(Conversacion);
            if (Inserted == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            return Ok(Inserted);
        }

        [HttpPut]
        public IActionResult Update(string Emisor, string Receptor, Conversacion ConversacionIn)
        {
            var Conversacion = _ConversacionService.Get(Emisor, Receptor);

            if (Conversacion == null)
            {
                return NotFound();
            }

            _ConversacionService.Update(Emisor, Receptor, ConversacionIn);

            return NoContent();
        }

        [HttpDelete]
        public IActionResult Delete(Conversacion Conv)
        {
            var Conversacion = _ConversacionService.Get(Conv.Remitente, Conv.Destinatario);

            if (Conversacion == null)
            {
                return NotFound();
            }

            _ConversacionService.Remove(Conv);

            return NoContent();
        }


        [HttpPost("NMensaje")]
        public ActionResult<Conversacion> NuevoMensaje(AddNewMessageModel nuevoMensaje)
        {
            List<string> msg = new List<string>();
            Conversacion conversacion = _ConversacionService.GetId(nuevoMensaje.Id);
            SubmitMensaje MensajeInsert = new SubmitMensaje();
            MensajeInsert.Message = nuevoMensaje.Message;
            MensajeInsert.FechaEnviado = nuevoMensaje.FechaEnviado;
            if (conversacion.Remitente.Equals(nuevoMensaje.EmisorMensaje) || conversacion.Destinatario.Equals(nuevoMensaje.EmisorMensaje))
            {
                if (conversacion.Mensajes!=null)
                {
                    MensajeInsert.Emisor = nuevoMensaje.EmisorMensaje;
                    var ParaLista = _mensajeServices.Create(MensajeInsert);
                    conversacion.Mensajes.Add(ParaLista.Id);
                    _ConversacionService.Update(conversacion.Remitente, conversacion.Destinatario, conversacion);
                }
                else
                {
                    MensajeInsert.Emisor = nuevoMensaje.EmisorMensaje;
                    var ParaLista = _mensajeServices.Create(MensajeInsert);
                    msg.Add(ParaLista.Id);
                    conversacion.Mensajes = msg;
                    _ConversacionService.Update(conversacion.Remitente, conversacion.Destinatario, conversacion);
                }
            }
            else
            {
                return Unauthorized("No existe el remitente del mensaje en esta conversacion");
            }
            return Ok(conversacion);
        }
        
        [HttpPost("ObtMensaje")]
        public ActionResult<List<SubmitMensaje>> ObtenerMensajesEnConversacion(SubmitConversacion Conv)
        {
            var ConversacionAObtener = _ConversacionService.GetId(Conv.Id);
            List<SubmitMensaje> mensajesDescifrados = new List<SubmitMensaje>();
            if (ConversacionAObtener.Remitente.Equals(Conv.Remitente)&&ConversacionAObtener.Destinatario.Equals(Conv.Destinatario))
            {
                if (ConversacionAObtener.Mensajes!=null)
                {
                    foreach (var item in ConversacionAObtener.Mensajes)
                    {
                        var inserted = _mensajeServices.Get(item);
                        mensajesDescifrados.Add(inserted);
                    }
                }
                else
                {
                    return Ok(mensajesDescifrados);
                }
            }
            else
            {
                return NotFound();
            }


            return Accepted(mensajesDescifrados);
        }
    }
}
