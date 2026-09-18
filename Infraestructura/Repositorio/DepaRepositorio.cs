using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;    
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class DepaRepositorio : DepartaRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public DepaRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Departamento departamento)
        {
            await _context.Departamento.AddAsync(departamento);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var departamento = await _context.Departamento.FindAsync(id);
            if (departamento != null)
            {
                _context.Departamento.Remove(departamento);
                await _context.SaveChangesAsync();
            }   
        }

        public async Task<List<Departamento>> GetAllasync()
        {
            return await _context.Departamento.ToListAsync();
        }

        public async Task<Departamento> GetAsync(int id)
        {
            var departamento = await _context.Departamento.FindAsync(id);
            return departamento!;
        }

        public async   Task UpdateAsync(Departamento departamento)
        {
            _context.Departamento.Update(departamento);
            await _context.SaveChangesAsync();
        }
    }
}
