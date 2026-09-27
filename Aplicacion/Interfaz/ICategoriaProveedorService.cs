using Aplicacion.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplicacion.Interfaz
{
    public interface ICategoriaProveedorService
    {
        Task<List<CategoriaProveedorDTO>> GetAllsync();
        Task<CategoriaProveedorDTO> GetByIdAsync(int id);
        Task AddAsync(CreateCategoriaProveedorDTO categoria);
        Task UpdateAsync(UpdateCategoriaProveedorDTO categoria);
        Task DeleteAsync(int id);
    }
}