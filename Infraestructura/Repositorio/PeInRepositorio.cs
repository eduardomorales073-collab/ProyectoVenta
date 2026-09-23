using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class PeInRepositorio : PedIntRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public PeInRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Pedido_Interno pedidoInterno)
        {
            await _context.Pedido_Interno.AddAsync(pedidoInterno);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var pedidoInterno = await _context.Pedido_Interno.FindAsync(id);
            if (pedidoInterno != null)
            {
                _context.Pedido_Interno.Remove(pedidoInterno);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Pedido_Interno>> GetAllasync()
        {
            return await _context.Pedido_Interno.ToListAsync();
        }

        public async Task<Pedido_Interno> GetAsync(int id)
        {
            var pedidoInterno = await _context.Pedido_Interno.FindAsync(id);
            return pedidoInterno!;
        }

        public async Task UpdateAsync(Pedido_Interno pedidoInterno)
        {
            _context.Pedido_Interno.Update(pedidoInterno);
            await _context.SaveChangesAsync();
        }
    }
}
