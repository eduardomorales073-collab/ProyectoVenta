using Aplicacion.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplicacion.Interfaz
{
    public interface IProvRubService
    {
        Task<List<ProveeRubroDTO>> GetAllsync();
        Task<ProveeRubroDTO> GetByIdAsync(int idProveedor, int idRubro);
        Task AddAsync(CreateProveeRubroDTO proveeRubro);
        Task DeleteAsync(int idProveedor, int idRubro);
        Task UpdateAsync(UpdateProveeRubroDTO proveeRubro);

        // ✅ NUEVO
        Task EliminarPorProveedorAsync(int idProveedor);
    }
}