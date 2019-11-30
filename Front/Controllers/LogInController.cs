using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Front.Models;
using Front.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Net;

namespace Front.Controllers
{
    public class LogInController : Controller
    {
        private static string Token;
        private static SubmitModel CurrentUser = new SubmitModel();
        private static HostSettings hostSettings;
        private static string IDConversacion;

        public LogInController(IOptions<HostSettings> host)
        {
            hostSettings = host.Value;
        }
        // GET: LogIn
        public ActionResult Index()
        {
            return View();
        }

        // GET: LogIn/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: LogIn/Create
        public ActionResult Create(LogInViewModel serialize)
        {
            if (ModelState.IsValid)
            {
                SubmitModel model = new SubmitModel();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(hostSettings.DireccionHost);
                    //HTTP GET                
                    var JSON = JsonConvert.SerializeObject(serialize);
                    var Content = new StringContent(JSON, Encoding.UTF8, "application/json");
                    var responseTask = client.PostAsync("api/LogIn/Authenticate", Content);
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        model = JsonConvert.DeserializeObject<SubmitModel>(readTask.Result);
                        client.DefaultRequestHeaders.Authorization
                           = new AuthenticationHeaderValue("Bearer", model.Token);
                        //CurrentUser = model;
                        HttpContext.Session.SetString("User", readTask.Result);
                        var resultAutorize = client.GetAsync("api/LogIn/Autorizado");
                        Token = model.Token;
                        var result2 = resultAutorize.Result;
                        if (result2.IsSuccessStatusCode)
                        {
                            ModelState.Clear();
                            return View();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No se pudo autenticar");
                        return View("Index");
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "No se ingreso el Usuario y Contraseña correctamente");
            return View("Index");
            
        }
        
        // GET: LogIn/Edit/5
        public ActionResult VistaCrearUsuario()
        {
            return View();
        }

        // POST: LogIn/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearUsuario(SubmitModel Usuario)
        {
            try
            {                
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(hostSettings.DireccionHost);
                    //HTTP POSt                    
                    var JSON = JsonConvert.SerializeObject(Usuario);
                    var Content = new StringContent(JSON, Encoding.UTF8, "application/json");
                    var responseTask = client.PostAsync("api/Usuario/", Content);
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();                        
                        var model = JsonConvert.DeserializeObject<SubmitModel>(readTask.Result);                        
                        var resultAutorize = client.GetAsync("api/LogIn/Autorizado");

                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Index");
            }
        }

        // GET: LogIn/Delete/5
        public ActionResult VistaLlenarLista()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(hostSettings.DireccionHost);
                //HTTP POSt           
                client.DefaultRequestHeaders.Authorization
                           = new AuthenticationHeaderValue("Bearer",Token);
                var responseTask = client.GetAsync("api/Usuario/");
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    List<SubmitModel> model = JsonConvert.DeserializeObject<List<SubmitModel>>(readTask.Result);
                    var Actual =  JsonConvert.DeserializeObject<SubmitModel>(HttpContext.Session.GetString("User"));
                    int aux = model.FindIndex(x=>x.User==Actual.User);
                    model.RemoveAt(aux);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View();
        }

        // POST: LogIn/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CrearChat(string User)
        {
            using (var client = new HttpClient())
            {
                Conversacion newConv = new Conversacion();
                var Actual = JsonConvert.DeserializeObject<SubmitModel>(HttpContext.Session.GetString("User"));
                newConv.Remitente = Actual.User;                
                client.BaseAddress = new Uri(hostSettings.DireccionHost);
                //HTTP GET User destino
                client.DefaultRequestHeaders.Authorization
                           = new AuthenticationHeaderValue("Bearer", Token);
                var Content = new StringContent(User);
                var ResponseGet = client.GetAsync("/api/Usuario/GetWithParam?Id="+User);        
                var result = ResponseGet.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    var model = JsonConvert.DeserializeObject<SubmitModel>(readTask.Result);
                    //Sabemos que existe el User Destinatario se agrega a la conversacion
                    newConv.Destinatario = model.User;



                    var responseGet2 = client.GetAsync("api/Conversation/GetWithParam?Emisor="+Actual.User+"&Receptor="+model.User);
                    var resultGet1 = responseGet2.Result;
                    if (resultGet1.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError(string.Empty, "La conversacion ya existe");
                        return RedirectToAction("VistaLlenarLista");
                    }
                    else if (!resultGet1.IsSuccessStatusCode)
                    {
                        var responseGet3 = client.GetAsync("api/Conversation/GetWithParam?Emisor=" + model.User + "&Receptor=" + Actual.User);
                        var resultGet2 = responseGet2.Result;
                        if (resultGet2.IsSuccessStatusCode)
                        {
                            ModelState.AddModelError(string.Empty, "La conversacion ya existe");
                            return RedirectToAction("VistaLlenarLista");
                        }
                        else
                        {
                            var JSON = JsonConvert.SerializeObject(newConv);
                            var contenido = new StringContent(JSON, Encoding.UTF8, "application/json");
                            var responseTask = client.PostAsync("api/Conversation/Create/", contenido);
                            var resultPost = responseTask.Result;
                        }
                    }
                    
                    



                    return View("Create");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View();
        }


        public ActionResult ObtenerChats()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(hostSettings.DireccionHost);
                //HTTP POSt           
                client.DefaultRequestHeaders.Authorization
                           = new AuthenticationHeaderValue("Bearer", Token);
                var Actual = JsonConvert.DeserializeObject<SubmitModel>(HttpContext.Session.GetString("User"));
                var ResponseGet = client.GetAsync("/api/Conversation/GetUserConversations?User=" + Actual.User);
                var result = ResponseGet.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    List<Conversacion> model = JsonConvert.DeserializeObject<List<Conversacion>>(readTask.Result);                    
                    
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View();
        }

        public ActionResult VisualizarChat(string Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(hostSettings.DireccionHost);
                //HTTP POSt           
                client.DefaultRequestHeaders.Authorization
                           = new AuthenticationHeaderValue("Bearer", Token);
                var ResponseGet = client.GetAsync("/api/Conversation/GetFromId?Id=" + Id);
                var result = ResponseGet.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    Conversacion model = JsonConvert.DeserializeObject<Conversacion>(readTask.Result);
                    SubmitConversacion nuevaConv = new SubmitConversacion();
                    nuevaConv.Id = model.Id;
                    nuevaConv.Remitente = model.Remitente;
                    nuevaConv.Destinatario = model.Destinatario;

                    var JSON = JsonConvert.SerializeObject(nuevaConv);
                    var contenido = new StringContent(JSON, Encoding.UTF8, "application/json");
                    var ResponseGet2 = client.PostAsync("/api/Conversation/ObtMensaje",contenido);
                    var result2 = ResponseGet2.Result;

                    if (result2.IsSuccessStatusCode)
                    {
                        var readTask2 = result2.Content.ReadAsStringAsync();
                        readTask2.Wait();
                        List<SubmitMensaje> n1 = new List<SubmitMensaje>();
                        IDConversacion = model.Id;
                        if (result2.StatusCode!=HttpStatusCode.OK)
                        {
                            n1 = JsonConvert.DeserializeObject<List<SubmitMensaje>>(readTask2.Result);
                        }
                        

                        return View(n1);

                    }                    
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View();            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NuevoMensaje(string Mensaje)
        {
            using (var client = new HttpClient())
            {
                AddNewMessageModel n1 = new AddNewMessageModel();
                var Actual = JsonConvert.DeserializeObject<SubmitModel>(HttpContext.Session.GetString("User"));
                n1.Id = IDConversacion;
                n1.EmisorMensaje = Actual.User;
                n1.FechaEnviado = DateTime.Now;
                n1.Message = Mensaje;
                client.BaseAddress = new Uri(hostSettings.DireccionHost);
                //HTTP POSt           
                client.DefaultRequestHeaders.Authorization
                           = new AuthenticationHeaderValue("Bearer", Token);
                var JSON = JsonConvert.SerializeObject(n1);
                var contenido = new StringContent(JSON, Encoding.UTF8, "application/json");
                var ResponseGet2 = client.PostAsync("/api/Conversation/NMensaje", contenido);
                var result2 = ResponseGet2.Result;
                if (result2.IsSuccessStatusCode)
                {
                    var readTask = result2.Content.ReadAsStringAsync();
                    readTask.Wait();
                    Conversacion model = JsonConvert.DeserializeObject<Conversacion>(readTask.Result);

                    return RedirectToAction("VisualizarChat",new {Id=model.Id });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View();
        }
    }
}