using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Tasky.Datos.EF
{
    public partial class AspNetUserLogin : IdentityUserLogin<string>
    {
        public override string LoginProvider { get; set; } = null!;
        public override string ProviderKey { get; set; } = null!;
        public override string UserId { get; set; } = null!;

        public virtual AspNetUsers User { get; set; } = null!;
    }
}
