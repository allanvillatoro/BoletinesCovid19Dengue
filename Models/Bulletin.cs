using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BoletinesCovid19Dengue.Models
{
    public class Bulletin
    {
        [DisplayName("#")]
        [Required(ErrorMessage = "Campo requerido para continuar")]
        public int id { get; set; }
        [DisplayName("Titulo")]
        [Required(ErrorMessage = "Campo requerido para continuar")]
        public string titulo { get; set; }
        [DisplayName("Descripcion")]
        [Required(ErrorMessage = "Campo requerido para continuar")]
        public string descripcion { get; set; }
        [DisplayName("Fecha de Publicacion")]
        [Required(ErrorMessage = "Campo requerido para continuar")]
        public DateTime fecha_publicacion { get; set; }
        [NotMapped]
        [DisplayName("Imagen")]
        [Required(ErrorMessage = "Campo requerido para continuar")]
        public IFormFile image { get; set; }
        [DisplayName("Imagen")]
        [Required(ErrorMessage = "Campo requerido para continuar")]
        public string imagen { get; set; }
        [NotMapped]
        [DisplayName("Archivo PDF")]
        [Required(ErrorMessage = "Campo requerido para continuar")]
        public IFormFile files { get; set; }
        [DisplayName("Archivo PDF")]
        [Required(ErrorMessage = "Campo requerido para continuar")]
        public string archivo_pdf { get; set; }
        [DisplayName("Habilitado")]
        public bool habilitado { get; set; }

    }
}