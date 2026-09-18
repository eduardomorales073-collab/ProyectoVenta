using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class DetPedRepositorio : DetaPedRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public DetPedRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task AddAsync(Detalle_Pedido detallePedido)
        {
            await _context.Detalle_Pedido.AddAsync(detallePedido);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int idPedido, int idArticulo)
        {
            var detallePedido = await _context.Detalle_Pedido
                .FindAsync(idPedido, idArticulo);
            if (detallePedido != null)
            {
                _context.Detalle_Pedido.Remove(detallePedido);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Detalle_Pedido>> GetAllasync()
        {
            return await _context.Detalle_Pedido.ToListAsync();
        }

        public async Task<Detalle_Pedido> GetAsync(int idPedido, int idArticulo)
        {
            var detallePedido = await _context.Detalle_Pedido
                .FindAsync(idPedido, idArticulo);
            return detallePedido!;
        }

        public async Task UpdateAsync(Detalle_Pedido detallePedido)
        {
            _context.Detalle_Pedido.Update(detallePedido);
            await _context.SaveChangesAsync();
        }
    }
}