using System.ComponentModel.DataAnnotations;

namespace Tasky.Web.Models
{
    public class RegistroViewModel
    {



        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = null!;


        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\+?[0-9\s\-]{7,15}$", ErrorMessage = "El formato del teléfono no es válido.")]
        public string Telefono { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Se debe confirmar la contraseña.")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]

        public string ConfirmarPassword { get; set; } = null!;



    }
}
