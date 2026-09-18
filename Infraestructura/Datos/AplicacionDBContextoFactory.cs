using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Infraestructura.Datos
{
    public class AplicacionDBContextoFactory : IDesignTimeDbContextFactory<AplicacionDBContexto>
    {
        public AplicacionDBContexto CreateDbContext(string[] args)
        {
            string basePath = @"C:\Users\eduar\source\repos\ProyectoVentas\Venta";

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<AplicacionDBContexto>();
            var connectionString = configuration.GetConnectionString("connectionString");

            builder.UseSqlServer(connectionString);

            return new AplicacionDBContexto(builder.Options);
        }
    }
}