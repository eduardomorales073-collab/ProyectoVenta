using Aplicacion.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplicacion.Interfaz
{
    public interface IRelacionService
    {
        Task<List<RelacionDTO>> GetAllsync();
        Task AddAsync(CreateRelacionDTO dto);
        Task DeleteAsync(int id);
    }
}