
namespace Tasky.Datos.EF;

public class TablerosTrello
{
    public int IdTablero { get; set; }
    public string TrelloBoardId { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.Now;
}
