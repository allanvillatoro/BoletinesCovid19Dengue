using System.IO;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BoletinesCovid19Dengue.Models;
using System.Collections.Generic;
using System;
using static BoletinesCovid19Dengue.Models.BoletinContext;
using Microsoft.AspNetCore.Authorization;
using App.NETFramework.Core.Application.FilesStorages.Models;
using App.NETFramework.Core.Application.FilesStorages;

namespace BoletinesCovid19Dengue.Controllers
{
    public class PanelController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        public PanelController(IWebHostEnvironment hostEnvironment)
        {
            this._hostEnvironment = hostEnvironment;
        }
        public Bulletin buscarBoletin(int id)
        {
            Bulletin bulletin = new Bulletin();
            using (BoletinContext db = new BoletinContext())
            {
                Boletin boletin = db.Boletines.Find(id);
                bulletin.id = boletin.id;
                bulletin.titulo = boletin.titulo;
                bulletin.descripcion = boletin.descripcion;
                bulletin.imagen = boletin.imagen;
                bulletin.archivo_pdf = boletin.archivo_pdf;
                bulletin.fecha_publicacion = boletin.fecha_publicacion;
                bulletin.habilitado = boletin.habilitado;
            }
            return bulletin;
        }


        public void SaveFile(Bulletin bulletin, IFormFile files, IFormFile image, ref string fileNameImage, ref string fileNameFile)
        {
            fileNameImage = string.Empty;
            fileNameFile = string.Empty;

                    string AzureStorage_Cdn = "https://boletincovid.blob.core.windows.net";
                    string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=boletincovid;AccountKey=8MzTKaPF3GMnZJCv3HqnTXI5bm7S7EdqDpFo/1tlX913YxZ1X8Var5RZhra5swXtWHZbqtq9/0I7EoRYLOF4Lg==;EndpointSuffix=core.windows.net";
                    FileStorageConfiguration fileStorageConfiguration = new FileStorageConfiguration { AzureStorage_Cdn = AzureStorage_Cdn, StorageConnectionString = StorageConnectionString };

            if (image != null)
            {
                string uploadValidationMessage = string.Empty;
                string uploadedUri = string.Empty;

                using (var fileStream = image.OpenReadStream())
                {
                    var fileArray = image.FileName.Split('.');
                    string extFile = fileArray[1];
                    string fileName = $"{DateTime.Now.Ticks}.{extFile}";
                    string CONTAINER_NAME_AZURE_STORAGE = "boletines";
                    
                        (uploadValidationMessage, uploadedUri) = FileStorageFactory.Create().UploadToBlob(CONTAINER_NAME_AZURE_STORAGE, fileName, fileStorageConfiguration, null, fileStream).GetAwaiter().GetResult();
                }
                fileNameImage = uploadedUri;
               
            }
            if (files != null)
            {
                string uploadValidationMessage = string.Empty;
                string uploadedUri = string.Empty;

                using (var fileStream = files.OpenReadStream())
                {
                    var fileArray = files.FileName.Split('.');
                    string extFile = fileArray[1];
                    string fileName = $"{DateTime.Now.Ticks}.{extFile}";
                    string CONTAINER_NAME_AZURE_STORAGE = "boletines";

                    (uploadValidationMessage, uploadedUri) = FileStorageFactory.Create().UploadToBlob(CONTAINER_NAME_AZURE_STORAGE, fileName, fileStorageConfiguration, null, fileStream).GetAwaiter().GetResult();
                }
                fileNameFile = uploadedUri;
               
            }
        }

        public void SaveOnlyFile(Bulletin bulletin, IFormFile files)
        {
            var fileName = Path.GetFileNameWithoutExtension(files.FileName);
            var fileExtension = Path.GetExtension(files.FileName);
            bulletin.archivo_pdf = fileName = fileName + fileExtension;
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath + "/file/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                files.CopyTo(fileStream);
            }
        }
        public void SaveOnlyImage(Bulletin bulletin, IFormFile image)
        {
            var fileNameimage = Path.GetFileNameWithoutExtension(image.FileName);
            var fileExtensionimage = Path.GetExtension(image.FileName);
            bulletin.imagen = fileNameimage = fileNameimage + fileExtensionimage;
            string wwwRootPathimage = _hostEnvironment.WebRootPath;
            string pathimage = Path.Combine(wwwRootPathimage + "/imagenes/", fileNameimage);

            using (var fileStream = new FileStream(pathimage, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }
        }


        [Authorize]
        public IActionResult Index()
        {
            List<Bulletin> bulletins = new List<Bulletin>();
            using (BoletinContext db = new BoletinContext())
            {

                var consulta = from s in db.Boletines
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
                bulletins = consulta.ToList();
            }
            return View(bulletins);
        }


        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Bulletin bulletin, IFormFile files, IFormFile image)
        {
            try
            {
                using (BoletinContext db = new BoletinContext())
                {
                    string fileNameImage = string.Empty;
                    string fileNameFile = string.Empty;
                    SaveFile(bulletin, files, image, ref fileNameImage, ref fileNameFile);

                    Boletin boletin = new Boletin();

                    boletin.id = bulletin.id;
                    boletin.titulo = bulletin.titulo;
                    boletin.descripcion = bulletin.descripcion;
                    boletin.imagen = fileNameImage;
                    boletin.fecha_publicacion = bulletin.fecha_publicacion;
                    boletin.archivo_pdf = fileNameFile;
                    boletin.habilitado = bulletin.habilitado;

                    db.Boletines.Add(boletin);

                    int filas = db.SaveChanges();
                    if (filas > 0)
                        Console.WriteLine("Guardado exitosamente.");
                    else
                        return View(bulletin);

                }
            }
            catch (Exception ex)
            {
                ViewBag.resCreate = 1;
                return View(bulletin);
            }

            TempData["mensaje"] = "Se ha creado exitosamente el nuevo registro.";
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            Bulletin encontrado = this.buscarBoletin(id);
            return View(encontrado);
        }
        [HttpPost]
        public IActionResult Edit(Bulletin bulletin, IFormFile files, IFormFile image, Boletin boletin)
        {
            using (BoletinContext db = new BoletinContext())
            {

                if (files != null && image != null)
                {
                    string fileNameImage = string.Empty;
                    string fileNameFile = string.Empty;
                    SaveFile(bulletin, files, image, ref fileNameImage, ref fileNameFile);

                    boletin = db.Boletines.Find(bulletin.id);
                    DeleteData(boletin);
                    boletin.titulo = bulletin.titulo;
                    boletin.descripcion = bulletin.descripcion;
                    boletin.fecha_publicacion = bulletin.fecha_publicacion;
                    boletin.imagen = fileNameImage;
                    boletin.archivo_pdf = fileNameFile;
                    boletin.habilitado = bulletin.habilitado;

                    int filas = db.SaveChanges();
                    if (filas > 0) { Console.WriteLine("Modificado"); }

                    else
                    {
                        Console.WriteLine("Hubo un error");
                        return View(bulletin);
                    }
                    return RedirectToAction("Index");
                }
                else if (files == null && image != null)
                {
                    boletin = db.Boletines.Find(bulletin.id);
                    DeleteImageData(boletin);
                    SaveOnlyImage(bulletin, image);
                    boletin.titulo = bulletin.titulo;
                    boletin.descripcion = bulletin.descripcion;
                    boletin.fecha_publicacion = bulletin.fecha_publicacion;
                    boletin.imagen = bulletin.imagen;
                    bulletin.archivo_pdf = bulletin.archivo_pdf;
                    boletin.habilitado = bulletin.habilitado;

                    int filas = db.SaveChanges();
                    if (filas > 0) { Console.WriteLine("Modificado"); }

                    else
                    {
                        Console.WriteLine("Hubo un error");
                        return View(bulletin);
                    }
                    return RedirectToAction("Index");
                }
                else if (files != null && image == null)
                {
                    boletin = db.Boletines.Find(bulletin.id);
                    DeleteFileData(boletin);
                    SaveOnlyFile(bulletin, files);
                    boletin.titulo = bulletin.titulo;
                    boletin.descripcion = bulletin.descripcion;
                    boletin.fecha_publicacion = bulletin.fecha_publicacion;
                    bulletin.imagen = bulletin.imagen;
                    boletin.archivo_pdf = bulletin.archivo_pdf;
                    boletin.habilitado = bulletin.habilitado;

                    int filas = db.SaveChanges();
                    if (filas > 0)
                    {
                        Console.WriteLine("Modificado");
                    }
                    else
                    {
                        Console.WriteLine("Hubo un error");
                        return View(bulletin);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    boletin = db.Boletines.Find(bulletin.id);
                    boletin.titulo = bulletin.titulo;
                    boletin.descripcion = bulletin.descripcion;
                    boletin.fecha_publicacion = bulletin.fecha_publicacion;
                    bulletin.imagen = bulletin.imagen;
                    bulletin.archivo_pdf = bulletin.archivo_pdf;
                    boletin.habilitado = bulletin.habilitado;

                    int filas = db.SaveChanges();
                    if (filas > 0) { Console.WriteLine("Modificado"); }

                    else
                    {
                        Console.WriteLine("Hubo un error");
                        return View(bulletin);
                    }
                    return RedirectToAction("Index");
                }
            }
        }
        public void DeleteData(Boletin boletin)
        {
            string AzureStorage_Cdn = "https://boletincovid.blob.core.windows.net";
            string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=boletincovid;AccountKey=8MzTKaPF3GMnZJCv3HqnTXI5bm7S7EdqDpFo/1tlX913YxZ1X8Var5RZhra5swXtWHZbqtq9/0I7EoRYLOF4Lg==;EndpointSuffix=core.windows.net";
            FileStorageConfiguration fileStorageConfiguration = new FileStorageConfiguration { AzureStorage_Cdn = AzureStorage_Cdn, StorageConnectionString = StorageConnectionString };


            if (boletin.archivo_pdf != null)
            {
                //https://boletincovid.blob.core.windows.net/boletines/637359833456127696.png
                var fileArray = boletin.archivo_pdf.Split('/');
                string fileName = fileArray[fileArray.Length - 1];
                var response = FileStorageFactory.Create().DeleteFileInBlob("boletines", fileName, fileStorageConfiguration).GetAwaiter().GetResult();
            }
            if (boletin.imagen != null)
            {
                var fileArray = boletin.imagen.Split('/');
                string fileName = fileArray[fileArray.Length - 1];
                var response = FileStorageFactory.Create().DeleteFileInBlob("boletines", fileName, fileStorageConfiguration).GetAwaiter().GetResult();
            }
        }
        public void DeleteFileData(Boletin boletin)
        {
            if (boletin.archivo_pdf != null)
            {
                boletin.archivo_pdf = Path.Combine(_hostEnvironment.WebRootPath, "file", boletin.archivo_pdf);
                FileInfo fi = new FileInfo(boletin.archivo_pdf);
                if (fi != null)
                {
                    System.IO.File.Delete(boletin.archivo_pdf);
                    fi.Delete();
                }
            }

        }
    public void DeleteImageData(Boletin boletin)
    {
        if (boletin.imagen != null)
        {
            boletin.imagen = Path.Combine(_hostEnvironment.WebRootPath, "imagenes", boletin.imagen);
            FileInfo im = new FileInfo(boletin.imagen);
            if (im != null)
            {
                System.IO.File.Delete(boletin.imagen);
                im.Delete();
            }
        }

    }

    [Authorize]
        public IActionResult Delete(int id)
        {
            Bulletin encontrado = this.buscarBoletin(id);
            return View(encontrado);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id, Boletin boletin)
        {
            using (BoletinContext db = new BoletinContext())
            {
                boletin = db.Boletines.Find(id);
                DeleteData(boletin);
                db.Boletines.Remove(boletin);
                int filas = db.SaveChanges();
                if (filas > 0)
                    Console.WriteLine("Eliminado");
                else
                {
                    Console.WriteLine("Hubo un error");
                    Bulletin encontrado = this.buscarBoletin(id);
                    return View(encontrado);
                }
            }
            return RedirectToAction("Index");
        }


        [Authorize]
        public IActionResult Details(int id)
        {
            Bulletin encontrado = this.buscarBoletin(id);
            return View(encontrado);
        }
    }
}
