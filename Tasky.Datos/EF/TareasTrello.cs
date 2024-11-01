
namespace Tasky.Datos.EF
{
    public class TareasTrello
    {
        public int Id { get; set; }
        public string TrelloCardId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Estado { get; set; }
        public int? IdLista { get; set; }
        public string? IdObject { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.Now;
        public ListasTrello Lista { get; set; }
        public TaskyObject TaskyObject { get; set; }
    }
}
