using System;
using System.Collections.Generic;

namespace Tasky.Datos.EF
{
    public partial class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }
}
