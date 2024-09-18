using System;
using Infrastructure.Services;
using Meus_Contatos.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; 

namespace Meus_Contatos.Controllers
{
    public class ContaController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                using (var rabbitMQClient = new RabbitMQClient("auth_queue"))
                {
                    var loginRequest = JsonConvert.SerializeObject(model);
                    var token = rabbitMQClient.Call(loginRequest);

                    HttpContext.Response.Cookies.Append("JWTToken", token);

                    return RedirectToAction("Index", "Home");
                }

            
            }

            return View(model);
        }

        // GET: Register
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(RegistrarViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var rabbitMQClient = new RabbitMQClient("addUser_queue"))
                {
                    var registerRequest = JsonConvert.SerializeObject(model);
                    var response = rabbitMQClient.Call(registerRequest);

                    return RedirectToAction("Login", "Conta");
                }
            }

            return View(model);
        }
    }
}
