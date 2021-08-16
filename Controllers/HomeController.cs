using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BoletinesCovid19Dengue.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BoletinesCovid19Dengue.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Bulletin> bulletin = new List<Bulletin>();
            using (BoletinContext db = new BoletinContext())
            {
                //LINQ
                //similar a: select * from libros
                var consulta = from s in db.Boletines
                               where s.habilitado
                               orderby s descending
                               select new Bulletin
                               {
                                   id = s.id,
                                   titulo = s.titulo,
                                   descripcion = s.descripcion,
                                   fecha_publicacion = s.fecha_publicacion,
                                   imagen = s.imagen,
                                   archivo_pdf = s.archivo_pdf,
                                   habilitado = s.habilitado
                               };

                bulletin = consulta.ToList();
            }


            //Clean Cache
            if (HttpContext.Request.Cookies.Count > 0)
            {
                var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains("User.Cookie") || c.Key.Contains("Authentication"));
                foreach (var cookie in siteCookies)
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }

            HttpContext.SignOutAsync(
              CookieAuthenticationDefaults.AuthenticationScheme);

            return View(bulletin);
        }

        public IActionResult SitioInteres()
        {
            return View();
        }

        public IActionResult Historia()
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
