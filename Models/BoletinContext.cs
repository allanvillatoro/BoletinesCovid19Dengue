using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoletinesCovid19Dengue.Models
{
    public class BoletinContext : DbContext
    {

        public string connectionString = @"Server=tcp:boletinserver.database.windows.net,1433;Initial Catalog=BoletinBD;Persist Security Info=False;User ID=emendez;Password=Gmendez01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public DbSet<Boletin> Boletines { get; set; }

        public DbSet<Admin> Admins { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
          => options.UseSqlServer(connectionString);


        public class Boletin
        {
            [Key]
            public int id { get; set; }
            public string titulo { get; set; }
            public string descripcion { get; set; }

            public DateTime fecha_publicacion { get; set; }
            public string imagen { get; set; }

            public string archivo_pdf { get; set; }

            public bool habilitado { get; set; }
        }


        public class Admin
        {
            [Key]
            public int id { get; set; }

            public string usuario { get; set; }

            public string salt { get; set; }

            public string password { get; set; }

        }

    }
}
