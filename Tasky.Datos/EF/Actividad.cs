using System;
using System.Collections.Generic;

namespace Tasky.Datos.EF;

public partial class Actividad
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = null!;
    public DateTime Fecha { get; set; }
    public int IdUsuario { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
