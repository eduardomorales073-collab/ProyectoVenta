using Aplicacion.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplicacion.Interfaz
{
    public interface IUnidadMedidaService
    {
        Task<List<UnidadMedidaDTO>> GetAllsync();
        Task<UnidadMedidaDTO> GetByIdAsync(int id);
        Task AddAsync(CreateUnidadMedidaDTO unidad);
        Task UpdateAsync(UpdateUnidadMedidaDTO unidad);
        Task DeleteAsync(int id);
    }
}