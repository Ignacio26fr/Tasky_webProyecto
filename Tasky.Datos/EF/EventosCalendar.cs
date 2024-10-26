using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasky.Datos.EF
{
    public class EventosCalendar
    {
        public int IdEventoCalendar { get; set; }
        public string GoogleEventId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string ZonaHoraria { get; set; }
        public string Localizacion { get; set; }
        public string Estado { get; set; }
        public bool Sincronizado { get; set; } = false;
        public int? IdObject { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.Now;
        public TaskyObject TaskyObject { get; set; }
    }
}
