using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Tasky.Datos.EF
{
    public partial class AspNetUsers : IdentityUser
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetUserTokens = new HashSet<AspNetUserToken>();
            Roles = new HashSet<AspNetRole>();
        }

        public override string Id { get; set; } = null!;
        public override string? UserName { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? imagenDePerfil { get; set; }
        public override string? NormalizedUserName { get; set; }
        public override string? Email { get; set; }
        public override string? NormalizedEmail { get; set; }
        public override bool EmailConfirmed { get; set; }
        public override string? PasswordHash { get; set; }
        public override string? SecurityStamp { get; set; }
        public override string? ConcurrencyStamp { get; set; }
        public override string? PhoneNumber { get; set; }
        public override bool PhoneNumberConfirmed { get; set; }
        public override bool TwoFactorEnabled { get; set; }
        public override DateTimeOffset? LockoutEnd { get; set; }
        public override bool LockoutEnabled { get; set; }
        public override int AccessFailedCount { get; set; }

        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; }

        public virtual ICollection<AspNetRole> Roles { get; set; }
    }
}
