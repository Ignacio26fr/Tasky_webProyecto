namespace Tasky.Web.Models
{
    public class RegistroResponseModel
    {

        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Email { get; set; } = null!;
        public int IdPerfil { get; set; }

    }
}
