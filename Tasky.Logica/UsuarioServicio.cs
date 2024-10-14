using Tasky.Datos.EF;

namespace Tasky.Logica
{
    public interface IUsuarioServicio : IRepositorio<Usuario>
    {
    }

    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly TaskyContext _context;

        public UsuarioServicio(TaskyContext context)
        {
            _context = context;
        }

        public void agregar(Usuario entidad)
        {
            _context.Usuarios.Add(entidad);
            _context.SaveChanges();
        }

        public void actualizar(Usuario entidad)
        {
            _context.Usuarios.Update(entidad);
            _context.SaveChanges();
        }

        public void eliminar(int id)
        {
            var usuario = _context.Usuarios.Find(id) ?? throw new Exception("No se ha encontrado el usuario");
            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();
        }

        public Usuario? obtenerPorId(int id)
        {
            return _context.Usuarios.Find(id);
        }

        public List<Usuario> obtenerTodos()
        {
            return _context.Usuarios.ToList();
        }
    }
}
