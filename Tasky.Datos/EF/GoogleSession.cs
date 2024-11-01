
namespace Tasky.Datos.EF;

public class GoogleSession
{
    public int Id { get; set; }
    public string AccessToken { get; set; }
    public string UserId { get; set; }
    public virtual AspNetUsers User { get; set; } = null!;
}
