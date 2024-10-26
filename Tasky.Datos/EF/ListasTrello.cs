using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasky.Datos.EF
{
    public class ListasTrello
    {
        public int IdLista { get; set; }
        public string TrelloListId { get; set; }
        public string Nombre { get; set; }
        public int? TableroId { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.Now;
        public TablerosTrello Tablero { get; set; }
    }
}
