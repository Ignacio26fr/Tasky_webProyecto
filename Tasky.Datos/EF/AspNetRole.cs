﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Tasky.Datos.EF
{
    public partial class AspNetRole : IdentityRole
    {
        public AspNetRole()
        {
            AspNetRoleClaims = new HashSet<AspNetRoleClaim>();
            Users = new HashSet<AspNetUser>();
        }

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string NormalizedName { get; set; } = null!;
        public string? ConcurrencyStamp { get; set; }

        public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public virtual ICollection<AspNetUser> Users { get; set; }
    }
}
