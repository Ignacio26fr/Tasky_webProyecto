using Microsoft.AspNetCore.Identity;

namespace Tasky.Datos.EF;

public partial class AspNetUserClaim : IdentityUserClaim<string>
{
    public override int Id { get; set; }
    public override string UserId { get; set; } = null!;
    public override string? ClaimType { get; set; }
    public override string? ClaimValue { get; set; }

    public virtual AspNetUsers User { get; set; } = null!;
}