using Microsoft.AspNetCore.Identity;

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
}
