using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasky.Logica
{
    public interface IRepositorio <T>
    {
        List<T> obtenerTodos();
        T? obtenerPorId(int id);
        void agregar(T entidad);
        void actualizar(T entidad);
        void eliminar(int id);




    }
}
