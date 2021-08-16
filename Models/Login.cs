namespace BoletinesCovid19Dengue.Models
{
    public class Login
    {

        public int id { get; set; }

        public string usuario { get; set; }

        public string salt { get; set; }

        public string password { get; set; }
    }
}
