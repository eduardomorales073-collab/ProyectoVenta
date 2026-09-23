using Aplicacion.DTO;

namespace Aplicacion.Interfaz
{
    public interface IProvArtService
    {
        Task<List<ProveeArtcDTO>> GetAllsync();
        Task<ProveeArtcDTO> GetByIdAsync(int idProveedor, int idArticulo);
        Task AddAsync(CreateProveeArtcDTO dto);
        Task DeleteAsync(int idProveedor, int idArticulo);
        Task UpdateAsync(UpdateProveeArtcDTO dto);
    }
}