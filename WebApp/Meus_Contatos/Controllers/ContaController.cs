using System;
using Meus_Contatos.Models;
using Microsoft.AspNetCore.Mvc; // Referência ao modelo de usuários (você precisará criar)

namespace Meus_Contatos.Controllers
{
    public class ContaController : Controller
    {
        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Lógica de autenticação aqui
                return RedirectToAction("Index", "Home");
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
                // Lógica para registrar o usuário aqui
                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
}
