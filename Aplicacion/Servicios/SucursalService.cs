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
            var nuevaSucursal = _mapper.Map<Sucursal>(sucursal);
            var todos = await _sucursalRepositorio.GetAllasync();
            nuevaSucursal.id = todos.Any() ? todos.Max(s => s.id) + 1 : 1;

            await _sucursalRepositorio.AddAsync(nuevaSucursal);

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
            await _detalleTelefonoRepositorio.DeleteBySucursalAsync(id);
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
            await _sucursalRepositorio.UpdateAsync(_mapper.Map<Sucursal>(sucursal));
            await _detalleTelefonoRepositorio.DeleteBySucursalAsync(sucursal.id);

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

        // ===== HELPER: Obtener los teléfonos con fecha =====
        private async Task<List<TelefonoDTO>> GetTelefonosBySucursalAsync(int idSucursal)
        {
            var detalles = await _detalleTelefonoRepositorio.GetBySucursalAsync(idSucursal);
            var telefonos = new List<TelefonoDTO>();

            foreach (var d in detalles)
            {
                var tel = await _telefonoRepositorio.GetAsync(d.id_Telefono);
                if (tel != null)
                {
                    telefonos.Add(new TelefonoDTO(tel.id, tel.Tel, tel.Fecha));
                }
            }

            return telefonos.OrderByDescending(t => t.fecha).ToList();
        }
    }
}