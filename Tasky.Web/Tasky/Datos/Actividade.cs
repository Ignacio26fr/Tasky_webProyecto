using System;
using System.Collections.Generic;

namespace Tasky.Web.Tasky.Datos
{
    public partial class Actividade
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public int IdUsuario { get; set; }
        public int IdUsuarioNavigationId { get; set; }

        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    }
}
