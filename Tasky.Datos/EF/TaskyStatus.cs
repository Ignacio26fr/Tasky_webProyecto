using System;
using System.Collections.Generic;

namespace Tasky.Datos.EF
{
    public partial class TaskyStatus
    {
        public TaskyStatus()
        {
            TaskyObjects = new HashSet<TaskyObject>();
        }

        public int IdStatus { get; set; }
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<TaskyObject> TaskyObjects { get; set; }
    }
}
