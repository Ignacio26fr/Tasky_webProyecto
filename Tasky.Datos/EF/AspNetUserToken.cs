using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Tasky.Datos.EF
{
    public partial class AspNetUserToken : IdentityUserToken<string>
    {
        public override string UserId { get; set; } = null!;
        public override string LoginProvider { get; set; } = null!;
        public override string Name { get; set; } = null!;
        public override string? Value { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
    }
}
