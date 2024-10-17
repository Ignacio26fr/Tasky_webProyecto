using System;
using System.Collections.Generic;

namespace Tasky.Web.Tasky.Datos
{
    public partial class AspNetUserToken
    {
        public int UserId { get; set; }
        public string LoginProvider { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Value { get; set; }

        public virtual Usuario User { get; set; } = null!;
    }
}
