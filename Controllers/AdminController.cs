using BoletinesCovid19Dengue.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static BoletinesCovid19Dengue.Models.BoletinContext;

namespace BoletinesCovid19Dengue.Controllers
{
    public class AdminController : Controller
    {

        
        public IActionResult Index()
        {

            string usuarioTemporal = "Temporal";
            List<Login> loging = new List<Login>();
            using (BoletinContext db = new BoletinContext())
            {
                var validarUsuariosDB = from s in db.Admins
                                        select new Login
                                        {
                                            usuario = s.usuario,
                                        };



                loging = validarUsuariosDB.ToList();

            }

            if (loging.Count() <= 0)
            {


                var UserClaim = new List<Claim>()
                        {
                        new Claim(ClaimTypes.Name,usuarioTemporal)
                        };
                var userIdentity = new ClaimsIdentity(UserClaim, "CorrectUser");

                var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });


                HttpContext.SignInAsync(userPrincipal);

                TempData["mensaje"] = "Bienvenido Administrador Cree su Usuario y Password.";
                return RedirectToAction("Create");
            }

            return View();
        }


        [HttpPost]
        public IActionResult Index(LoginCreate login)
        {
            string saltValidar = "", passwordValidar, passwordConHash = "", nameUser = "";

            using (BoletinContext db = new BoletinContext())
            {
                var validar = from s in db.Admins
                              where s.usuario.Equals(login.usuario)
                              select new Login
                              {
                                  usuario = s.usuario,
                                  salt = s.salt,
                                  password = s.password,


                              };

                if (validar == null)
                {
                    ViewBag.Error = "Usuario o passwor no validos";


                    TempData["mensaje"] = "Usuario o password no validos.";
                    return RedirectToAction("Index");
                }

                foreach (var dato in validar)
                {
                    nameUser = dato.usuario;
                    saltValidar = dato.salt;
                    passwordConHash = dato.password;

                }


                Boolean ValidaUsuarios = String.Equals(login.usuario, nameUser, StringComparison.Ordinal);

                if (ValidaUsuarios == true)
                {
                    passwordValidar = GenerateHash(login.password, saltValidar);


                    if (passwordValidar == passwordConHash)
                    {


                        var UserClaim = new List<Claim>()
                        {
                        new Claim(ClaimTypes.Name,nameUser)
                        };
                        var userIdentity = new ClaimsIdentity(UserClaim, "CorrectUser");

                        var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });


                        HttpContext.SignInAsync(userPrincipal);

                        return RedirectToAction("Index", "Panel");
                    }

                }


            }

            TempData["mensaje"] = "Usuario o password no validos.";

            return RedirectToAction("Index");

        }





        
        public string CreateSalt(int zize)
        {
            var salt = new System.Security.Cryptography.RNGCryptoServiceProvider();
            var buff = new byte[zize];

            salt.GetBytes(buff);
            return Convert.ToBase64String(buff);

        }

        public string GenerateHash(string imput, string salt)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(imput + salt);
            System.Security.Cryptography.SHA256Managed sha256HashString =
              new System.Security.Cryptography.SHA256Managed();

            byte[] hash = sha256HashString.ComputeHash(bytes);
            return BitConverter.ToString(hash);

        }


        [Authorize]
        public IActionResult Create()
        {

            return View();
        }



        [HttpPost]
        public IActionResult Create(Login login)
        {

            List<Login> loging = new List<Login>();
            using (BoletinContext db = new BoletinContext())
            {
                var validarUsuariosDB = from s in db.Admins
                                        where s.usuario.Equals(login.usuario)
                                        select new Login
                                        {
                                            usuario = s.usuario,
                                        };

                loging = validarUsuariosDB.ToList();

            }

            if (loging.Count() <= 0)
            {


                using (BoletinContext db = new BoletinContext())
                {
                    Admin admin = new Admin();
                    string salt = CreateSalt(10);
                    string hashPassword = GenerateHash(login.password, salt);


                    admin.id = login.id;
                    admin.usuario = login.usuario;
                    admin.salt = salt;
                    admin.password = hashPassword;


                    db.Admins.Add(admin);

                    int filasAfectadas = db.SaveChanges();
                    if (filasAfectadas > 0)
                    {
                        Console.WriteLine("Usuario guardado");
                        TempData["mensaje2"] = "Ingrese con el usuario creado";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Console.WriteLine("Hubo un error");
                        ViewBag.resCreate = 1;
                        return View();
                    }
                }

            }

            TempData["mensaje"] = "Nombre de usuario no disponible.";
            return RedirectToAction("Create");
        }




    }
}
