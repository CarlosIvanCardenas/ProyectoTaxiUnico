using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxiUnicoWebClient.Controllers.Services;
using TaxiUnicoWebClient.Models;
using TaxiUnicoWebClient.Models.Classes;

namespace TaxiUnicoWebClient.Controllers
{
    public class HomeController : Controller
    {
        AdminsServices service = new AdminsServices();
           
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        //Cerrar sesión
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Home/Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPost(Administrador loginModel)
        {
            //Guardar coincidencia de usuario en variable
            var usuario = await LoginUserAsync(loginModel.Correo, loginModel.Contraseña);
            //Comprobar que el usuario exista
            if (usuario != null)
            {
                //Crear nueva lista de permisos para el usuario
                var claims = new List<Claim>
                {
                    //Otorgar permiso de autenticación
                    new Claim(ClaimTypes.Email, loginModel.Correo),
                    //new Claim(ClaimTypes.NameIdentifier, loginModel.Id.ToString())
                };
                //Comprobar si el usuario es de tipo administrador
                // if (usuario.Administrador)
                // {
                //     //Otorgar permisos de administrador
                //     claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                // }
                var userIdentity = new ClaimsIdentity(claims, "login");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                //Iniciar sesión
                await HttpContext.SignInAsync(principal);

                //Redireccionar a Home/Index en caso de exito 
                return Redirect("/");
            }
            //Volver a intentar en caso de error
            return View();
        }

        //Metodo para buscar coincidencias de usuario
        private async Task<Administrador> LoginUserAsync(string email, string password)
        {
            var usuario = await service.AdminLogin(email, password);
            return usuario;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
