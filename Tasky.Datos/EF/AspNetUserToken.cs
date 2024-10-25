using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

<<<<<<< HEAD
namespace Tasky.Datos.EF;

public partial class AspNetUserToken
{
    public string UserId { get; set; } = null!;
    public string LoginProvider { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Value { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
=======
namespace Tasky.Datos.EF
{
    public partial class AspNetUserToken : IdentityUserToken<string>
    {
        public override string UserId { get; set; } = null!;
        public override string LoginProvider { get; set; } = null!;
        public override string Name { get; set; } = null!;
        public override string? Value { get; set; }

        public virtual AspNetUsers User { get; set; } = null!;
    }
>>>>>>> Dev
}
