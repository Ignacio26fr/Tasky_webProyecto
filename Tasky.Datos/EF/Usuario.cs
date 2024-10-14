using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tasky.Datos.EF
{
    public partial class Usuario : IdentityUser<int>
    {
        public Usuario()
        {
            
        }

        public override int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int IdPerfil { get; set; }

        public virtual Perfil IdPerfilNavigation { get; set; } = null!;
        
    }
}
