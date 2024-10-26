using System;
using System.Collections.Generic;

namespace Tasky.Datos.EF;

public partial class Usuario
{
    public Usuario()
    {
        Actividads = new HashSet<Actividad>();
    }

    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int IdPerfil { get; set; }
    public string? NormalizedUserName { get; set; }
    public string? NormalizedEmail { get; set; }
    public int AccessFailedCount { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public string? SecurityStamp { get; set; }
    public bool TwoFactorEnabled { get; set; }

    public virtual Perfil IdPerfilNavigation { get; set; } = null!;
    public virtual ICollection<Actividad> Actividads { get; set; }
}
