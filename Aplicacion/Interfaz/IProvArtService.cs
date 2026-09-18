using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IProvArtService
    {
        Task<List<ProveeArtcDTO>> GetAllsync();
        Task<ProveeArtcDTO> GetByIdAsync(int id);
        Task AddAsync(CreateProveeArtcDTO proveeArtc);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateProveeArtcDTO proveeArtc);
    }
}
