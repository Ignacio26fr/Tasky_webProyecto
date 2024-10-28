using System;
using System.Collections.Generic;

namespace Tasky.Datos.EF
{
    public partial class TaskyPriority
    {
        public TaskyPriority()
        {
            TaskyObjects = new HashSet<TaskyObject>();
        }

        public int IdPriority { get; set; }
        public string Descripcion { get; set; } = null!;

        public virtual ICollection<TaskyObject> TaskyObjects { get; set; }
    }
}
