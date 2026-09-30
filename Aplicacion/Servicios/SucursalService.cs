using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class SucursalService : ISucursalService
    {
        private readonly SucursalRepositorio _sucursalRepositorio;
        private readonly TelefonoRepositorio _telefonoRepositorio;
        private readonly DetaTelRepositorio _detalleTelefonoRepositorio;
        private readonly IMapper _mapper;

        public SucursalService(
            SucursalRepositorio sucursalRepository,
            TelefonoRepositorio telefonoRepositorio,
            DetaTelRepositorio detalleTelefonoRepositorio,
            IMapper mapper)
        {
            _mapper = mapper;
            _sucursalRepositorio = sucursalRepository;
            _telefonoRepositorio = telefonoRepositorio;
            _detalleTelefonoRepositorio = detalleTelefonoRepositorio;
        }

        public async Task AddAsync(CreateSucursalDTO sucursal)
        {
            // 1. Mapear el DTO a la entidad
            var nuevaSucursal = _mapper.Map<Sucursal>(sucursal);

            // 2. Calcular el siguiente id
            var todos = await _sucursalRepositorio.GetAllasync();
            nuevaSucursal.id = todos.Any() ? todos.Max(s => s.id) + 1 : 1;

            // 3. Guardar la sucursal
            await _sucursalRepositorio.AddAsync(nuevaSucursal);

            // 4. Guardar los teléfonos asociados
            if (sucursal.Telefonos != null && sucursal.Telefonos.Any())
            {
                foreach (var numero in sucursal.Telefonos)
                {
                    if (string.IsNullOrWhiteSpace(numero)) continue;

                    var todosTelefonos = await _telefonoRepositorio.GetAllasync();
                    var nuevoIdTelefono = todosTelefonos.Any() ? todosTelefonos.Max(t => t.id) + 1 : 1;

                    var nuevoTelefono = new Telefono
                    {
                        id = nuevoIdTelefono,
                        Tel = numero,
                        Fecha = DateTime.Now
                    };

                    await _telefonoRepositorio.AddAsync(nuevoTelefono);

                    var detalle = new Detalle_Telefono
                    {
                        id_Sucursal = nuevaSucursal.id,
                        id_Telefono = nuevoTelefono.id
                    };

                    await _detalleTelefonoRepositorio.AddAsync(detalle);
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            // 1. Eliminar las relaciones de teléfonos
            await _detalleTelefonoRepositorio.DeleteBySucursalAsync(id);

            // 2. Eliminar la sucursal
            await _sucursalRepositorio.DeletAsync(id);
        }

        public async Task<List<SucursalDTO>> GetAllsync()
        {
            var sucursales = await _sucursalRepositorio.GetAllasync();
            var result = new List<SucursalDTO>();

            foreach (var s in sucursales)
            {
                var telefonos = await GetTelefonosBySucursalAsync(s.id);
                result.Add(new SucursalDTO(s.id, s.Nombre, telefonos));
            }

            return result;
        }

        public async Task<SucursalDTO> GetByIdAsync(int id)
        {
            var sucursal = await _sucursalRepositorio.GetAsync(id);
            if (sucursal == null) return null;

            var telefonos = await GetTelefonosBySucursalAsync(id);
            return new SucursalDTO(sucursal.id, sucursal.Nombre, telefonos);
        }

        public async Task UpdateAsync(UpdateSucursalDTO sucursal)
        {
            // 1. Actualizar la sucursal
            await _sucursalRepositorio.UpdateAsync(_mapper.Map<Sucursal>(sucursal));

            // 2. Eliminar las relaciones anteriores
            await _detalleTelefonoRepositorio.DeleteBySucursalAsync(sucursal.id);

            // 3. Crear las nuevas relaciones
            if (sucursal.Telefonos != null && sucursal.Telefonos.Any())
            {
                foreach (var numero in sucursal.Telefonos)
                {
                    if (string.IsNullOrWhiteSpace(numero)) continue;

                    var todosTelefonos = await _telefonoRepositorio.GetAllasync();
                    var nuevoIdTelefono = todosTelefonos.Any() ? todosTelefonos.Max(t => t.id) + 1 : 1;

                    var nuevoTelefono = new Telefono
                    {
                        id = nuevoIdTelefono,
                        Tel = numero,
                        Fecha = DateTime.Now
                    };

                    await _telefonoRepositorio.AddAsync(nuevoTelefono);

                    var detalle = new Detalle_Telefono
                    {
                        id_Sucursal = sucursal.id,
                        id_Telefono = nuevoTelefono.id
                    };

                    await _detalleTelefonoRepositorio.AddAsync(detalle);
                }
            }
        }

        // ===== HELPER: Obtener los teléfonos de una sucursal =====
        private async Task<List<string>> GetTelefonosBySucursalAsync(int idSucursal)
        {
            var detalles = await _detalleTelefonoRepositorio.GetBySucursalAsync(idSucursal);
            var telefonos = new List<string>();

            foreach (var d in detalles)
            {
                var tel = await _telefonoRepositorio.GetAsync(d.id_Telefono);
                if (tel != null)
                {
                    telefonos.Add(tel.Tel);
                }
            }

            return telefonos;
        }
    }
}