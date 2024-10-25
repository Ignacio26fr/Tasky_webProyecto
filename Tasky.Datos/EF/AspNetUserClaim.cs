using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

<<<<<<< HEAD
namespace Tasky.Datos.EF;

public partial class AspNetUserClaim
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
=======
namespace Tasky.Datos.EF
{
    public partial class AspNetUserClaim : IdentityUserClaim<string>
    {
        public override int Id { get; set; }
        public override string UserId { get; set; } = null!;
        public override string? ClaimType { get; set; }
        public override string? ClaimValue { get; set; }

        public virtual AspNetUsers User { get; set; } = null!;
    }
>>>>>>> Dev
}
