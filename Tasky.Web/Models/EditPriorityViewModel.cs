using System.ComponentModel.DataAnnotations;
using Tasky.Datos.EF;

namespace Tasky.Web.Models
{
    public class EditPriorityViewModel
    {

        public string IdObject { get; set; } = null!;

        [Required(ErrorMessage = "El campo Prioridad es requerido")]
        public TaskyPriority Priority { get; set; }
    }
}
