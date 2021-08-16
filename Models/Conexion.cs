using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BoletinesCovid19Dengue.Models
{
    public class Conexion
    {
        //Conexion local
        public string connectionString = @"Server=tcp:boletinserver.database.windows.net,1433;Initial Catalog=BoletinBD;Persist Security Info=False;User ID=emendez;Password=Gmendez01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public IEnumerable<Bulletin> getBoletin()
        {
            string sql = "select * from Boletines";
            List<Bulletin> lstBoletin = new List<Bulletin>();
            using (SqlConnection conexionSQL = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(sql, conexionSQL);
                conexionSQL.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    Bulletin boletin = new Bulletin();
                    boletin.id = Convert.ToInt32(sqlDataReader["id"]);
                    boletin.titulo = sqlDataReader["titulo"].ToString();
                    boletin.descripcion = sqlDataReader["descripcion"].ToString();
                    boletin.fecha_publicacion = Convert.ToDateTime(sqlDataReader["fecha_publicacion"].ToString());
                    boletin.imagen = sqlDataReader["imagen"].ToString();
                    boletin.archivo_pdf = sqlDataReader["archivo_pdf"].ToString();
                    boletin.habilitado = Convert.ToBoolean(sqlDataReader["habilitado"].ToString());

                    lstBoletin.Add(boletin);
                }
                conexionSQL.Close();
                return lstBoletin;
            }
        }

    }
}
