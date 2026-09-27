using Aplicacion.modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplicacion.Repositorio
{
    public abstract class CategoriaProveedorRepositorio
    {
        public abstract Task<List<Categoria_Proveedor>> GetAllasync();
        public abstract Task<Categoria_Proveedor> GetAsync(int id);
        public abstract Task AddAsync(Categoria_Proveedor categoria);
        public abstract Task UpdateAsync(Categoria_Proveedor categoria);
        public abstract Task DeletAsync(int id);
    }
}