using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Aplicacion.Servicios
{
    public class ReportePdfService
    {
        private readonly string _connectionString;

        public ReportePdfService(IConfiguration config)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            _connectionString = config.GetConnectionString("LocalSqlServer")
                ?? "Server=localhost;Database=SystemVentas;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
        }

        // ============================================================
        // HELPERS
        // ============================================================
        private DataTable EjecutarQuery(string sql)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        // ✅ CORREGIDO: Encabezado sin .Column en el container padre
        private void Encabezado(IContainer container, string titulo, string subtitulo)
        {
            container
                .Background("#1e293b")
                .Padding(15)
                .Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(titulo).FontSize(20).Bold().FontColor(Colors.White);
                        c.Item().Text(subtitulo).FontSize(10).FontColor("#cbd5e1");
                    });

                    row.ConstantItem(150).AlignRight().Column(c =>
                    {
                        c.Item().Text("SistemaVentas").FontSize(14).Bold().FontColor(Colors.White);
                        c.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor("#cbd5e1");
                    });
                });
        }

        private void PiePagina(IContainer container)
        {
            container.AlignCenter().Text(txt =>
            {
                txt.DefaultTextStyle(t => t.FontSize(8).FontColor(Colors.Grey.Medium));
                txt.Span("SistemaVentas - Reporte generado automaticamente - Pagina ");
                txt.CurrentPageNumber();
                txt.Span(" de ");
                txt.TotalPages();
            });
        }

        private void TablaDatos(IContainer container, DataTable dt, Dictionary<string, string> titulos)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    foreach (var col in dt.Columns.Cast<DataColumn>())
                    {
                        cols.RelativeColumn();
                    }
                });

                // Encabezados
                table.Header(header =>
                {
                    foreach (var col in dt.Columns.Cast<DataColumn>())
                    {
                        var titulo = titulos.ContainsKey(col.ColumnName) ? titulos[col.ColumnName] : col.ColumnName;
                        header.Cell().Background("#334155").Padding(6)
                            .Text(titulo).FontColor(Colors.White).Bold().FontSize(9);
                    }
                });

                // Filas
                int i = 0;
                foreach (DataRow fila in dt.Rows)
                {
                    // ✅ CORREGIDO: usar string hex en lugar de Colors.White
                    var bgColor = i % 2 == 0 ? "#ffffff" : "#f1f5f9";
                    foreach (var col in dt.Columns.Cast<DataColumn>())
                    {
                        var valor = fila[col] == DBNull.Value ? "-" : fila[col].ToString();
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(valor).FontSize(8);
                    }
                    i++;
                }
            });
        }

        private byte[] DocumentoBase(string titulo, string subtitulo, DataTable dt, Dictionary<string, string> titulos)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(t => t.FontSize(9));

                    page.Header().Element(c => Encabezado(c, titulo, subtitulo));
                    page.Content().PaddingVertical(10).Element(c => TablaDatos(c, dt, titulos));
                    page.Footer().Element(PiePagina);
                });
            }).GeneratePdf();
        }

        // ============================================================
        // 1. RANKING DE PROVEEDORES
        // ============================================================
        public byte[] GenerarRankingProveedores()
        {
            var sql = @"SELECT 
                            p.Nombre AS Proveedor,
                            p.NIT,
                            p.Categoria,
                            COUNT(DISTINCT da.id_Pedido) AS TotalPedidos,
                            ISNULL(SUM(da.Precio * da.Cantidad), 0) AS MontoTotal,
                            ISNULL(AVG(da.Precio), 0) AS PrecioPromedio
                        FROM Proveedor p
                        LEFT JOIN Detalle_Adjudicacion da ON da.id_Proveedor = p.id
                        GROUP BY p.Nombre, p.NIT, p.Categoria
                        ORDER BY MontoTotal DESC";

            var dt = EjecutarQuery(sql);
            return DocumentoBase("RANKING DE PROVEEDORES", "Top proveedores con mayores montos adjudicados", dt, new Dictionary<string, string>
            {
                ["Proveedor"] = "Proveedor",
                ["NIT"] = "NIT",
                ["Categoria"] = "Categoria",
                ["TotalPedidos"] = "Pedidos",
                ["MontoTotal"] = "Monto Total (Q)",
                ["PrecioPromedio"] = "Precio Prom. (Q)"
            });
        }

        // ============================================================
        // 2. GASTO DEPARTAMENTAL
        // ============================================================
        public byte[] GenerarGastoDepartamental()
        {
            var sql = @"SELECT 
                            d.Nombre AS Departamento,
                            s.Nombre AS Sucursal,
                            COUNT(DISTINCT p.id) AS TotalPedidos,
                            SUM(da.Precio * da.Cantidad) AS TotalGastado
                        FROM Detalle_Adjudicacion da
                        INNER JOIN Pedido_Interno p ON da.id_Pedido = p.id
                        INNER JOIN Departamento d ON p.id_Departamento = d.id
                        INNER JOIN Sucursal s ON d.id_Sucursal = s.id
                        GROUP BY d.Nombre, s.Nombre
                        ORDER BY TotalGastado DESC";

            var dt = EjecutarQuery(sql);
            return DocumentoBase("GASTO DEPARTAMENTAL", "Monto total gastado por departamento y sucursal", dt, new Dictionary<string, string>
            {
                ["Departamento"] = "Departamento",
                ["Sucursal"] = "Sucursal",
                ["TotalPedidos"] = "Pedidos",
                ["TotalGastado"] = "Total Gastado (Q)"
            });
        }

        // ============================================================
        // 3. ORDENES ACTIVAS
        // ============================================================
        public byte[] GenerarOrdenesActivas()
        {
            var sql = @"SELECT 
                            o.id AS IdOrden,
                            o.Descripcion,
                            CONVERT(VARCHAR(10), o.Fecha_Creacion, 103) AS FechaCreacion,
                            CONVERT(VARCHAR(10), o.Fecha_Limite, 103) AS FechaLimite,
                            DATEDIFF(DAY, GETDATE(), o.Fecha_Limite) AS DiasRestantes,
                            COUNT(DISTINCT p.id) AS TotalPedidos,
                            COUNT(DISTINCT ofp.id) AS TotalOfertas
                        FROM Orden_Compra o
                        LEFT JOIN Pedido_Interno p ON p.id_OrdenCompra = o.id
                        LEFT JOIN Oferta_Proveedor ofp ON ofp.id_Pedido_Interno = p.id
                        WHERE o.Fecha_Limite >= GETDATE()
                        GROUP BY o.id, o.Descripcion, o.Fecha_Creacion, o.Fecha_Limite
                        ORDER BY o.Fecha_Limite ASC";

            var dt = EjecutarQuery(sql);
            return DocumentoBase("ORDENES ACTIVAS", "Ordenes de compra abiertas a recibir ofertas", dt, new Dictionary<string, string>
            {
                ["IdOrden"] = "ID",
                ["Descripcion"] = "Descripcion",
                ["FechaCreacion"] = "Creada",
                ["FechaLimite"] = "Limite",
                ["DiasRestantes"] = "Dias",
                ["TotalPedidos"] = "Pedidos",
                ["TotalOfertas"] = "Ofertas"
            });
        }

        // ============================================================
        // 4. PEDIDOS PENDIENTES
        // ============================================================
        public byte[] GenerarPedidosPendientes()
        {
            var sql = @"SELECT 
                            p.codigo AS Codigo,
                            p.cantidad AS Cantidad,
                            d.Nombre AS Departamento,
                            s.Nombre AS Sucursal,
                            CONVERT(VARCHAR(10), p.Fecha_Solicitada, 103) AS FechaSolicitada,
                            DATEDIFF(DAY, p.Fecha_Solicitada, GETDATE()) AS DiasEsperando
                        FROM Pedido_Interno p
                        INNER JOIN Departamento d ON p.id_Departamento = d.id
                        INNER JOIN Sucursal s ON d.id_Sucursal = s.id
                        WHERE p.id_OrdenCompra IS NULL
                        ORDER BY p.Fecha_Solicitada ASC";

            var dt = EjecutarQuery(sql);
            return DocumentoBase("PEDIDOS PENDIENTES", "Pedidos internos sin asignar a una orden", dt, new Dictionary<string, string>
            {
                ["Codigo"] = "Codigo",
                ["Cantidad"] = "Cantidad",
                ["Departamento"] = "Departamento",
                ["Sucursal"] = "Sucursal",
                ["FechaSolicitada"] = "Fecha Solicitud",
                ["DiasEsperando"] = "Dias Esperando"
            });
        }

        // ============================================================
        // 5. HISTORIAL POR ARTICULO
        // ============================================================
        public byte[] GenerarHistorialArticulo()
        {
            var sql = @"SELECT 
                            a.codigo AS Codigo,
                            a.Nombre AS Articulo,
                            a.unidad_medida AS Unidad,
                            p.Nombre AS Proveedor,
                            da.Precio AS PrecioAdjudicado,
                            da.Cantidad AS Cantidad,
                            (da.Precio * da.Cantidad) AS Subtotal,
                            CONVERT(VARCHAR(10), o.Fecha_Creacion, 103) AS FechaOrden
                        FROM Detalle_Adjudicacion da
                        INNER JOIN Proveedor p ON da.id_Proveedor = p.id
                        INNER JOIN Detalle_Pedido dp ON da.id_Pedido = dp.id_Pedido
                        INNER JOIN Articulo a ON dp.id_Articulo = a.id
                        INNER JOIN Pedido_Interno pi ON da.id_Pedido = pi.id
                        INNER JOIN Orden_Compra o ON pi.id_OrdenCompra = o.id
                        ORDER BY a.Nombre ASC, o.Fecha_Creacion DESC";

            var dt = EjecutarQuery(sql);
            return DocumentoBase("HISTORIAL POR ARTICULO", "Compras de articulos con proveedor y precio", dt, new Dictionary<string, string>
            {
                ["Codigo"] = "Codigo",
                ["Articulo"] = "Articulo",
                ["Unidad"] = "Unidad",
                ["Proveedor"] = "Proveedor",
                ["PrecioAdjudicado"] = "Precio (Q)",
                ["Cantidad"] = "Cantidad",
                ["Subtotal"] = "Subtotal (Q)",
                ["FechaOrden"] = "Fecha"
            });
        }

        // ============================================================
        // 6. OFERTAS POR ORDEN
        // ============================================================
        public byte[] GenerarOfertasOrden()
        {
            var sql = @"SELECT 
                            o.id AS IdOrden,
                            o.Descripcion AS Orden,
                            p.codigo AS PedidoCodigo,
                            pr.Nombre AS Proveedor,
                            ofp.Precio AS PrecioOfertado,
                            CONVERT(VARCHAR(10), ofp.Fecha_Oferta, 103) AS FechaOferta,
                            CASE 
                                WHEN ofp.Precio = (SELECT MIN(ofp2.Precio) FROM Oferta_Proveedor ofp2 WHERE ofp2.id_Pedido_Interno = ofp.id_Pedido_Interno)
                                THEN 'GANADOR'
                                ELSE ''
                            END AS Estado
                        FROM Oferta_Proveedor ofp
                        INNER JOIN Pedido_Interno p ON ofp.id_Pedido_Interno = p.id
                        INNER JOIN Orden_Compra o ON p.id_OrdenCompra = o.id
                        INNER JOIN Proveedor pr ON ofp.id_Proveedor = pr.id
                        ORDER BY o.id ASC, ofp.Precio ASC";

            var dt = EjecutarQuery(sql);
            return DocumentoBase("OFERTAS POR ORDEN", "Comparativa de ofertas recibidas por orden", dt, new Dictionary<string, string>
            {
                ["IdOrden"] = "ID Orden",
                ["Orden"] = "Descripcion Orden",
                ["PedidoCodigo"] = "Pedido",
                ["Proveedor"] = "Proveedor",
                ["PrecioOfertado"] = "Precio (Q)",
                ["FechaOferta"] = "Fecha Oferta",
                ["Estado"] = "Estado"
            });
        }

        // ============================================================
        // 7. EFICIENCIA DEL PROCESO
        // ============================================================
        public byte[] GenerarEficienciaProceso()
        {
            var sql = @"SELECT 
                            o.id AS IdOrden,
                            o.Descripcion AS Orden,
                            CONVERT(VARCHAR(10), o.Fecha_Creacion, 103) AS FechaOrden,
                            CONVERT(VARCHAR(10), MIN(a.Fecha_Resolucion), 103) AS FechaAdjudicacion,
                            DATEDIFF(DAY, o.Fecha_Creacion, MIN(a.Fecha_Resolucion)) AS DiasProceso,
                            COUNT(DISTINCT p.id) AS PedidosAdjudicados,
                            COUNT(DISTINCT da.id_Proveedor) AS ProveedoresParticipantes,
                            SUM(da.Precio * da.Cantidad) AS MontoTotal
                        FROM Orden_Compra o
                        INNER JOIN Pedido_Interno p ON p.id_OrdenCompra = o.id
                        INNER JOIN Detalle_Adjudicacion da ON da.id_Pedido = p.id
                        INNER JOIN Adjudicacion a ON da.id_Adjudicacion = a.id
                        GROUP BY o.id, o.Descripcion, o.Fecha_Creacion
                        ORDER BY DiasProceso ASC";

            var dt = EjecutarQuery(sql);
            return DocumentoBase("EFICIENCIA DEL PROCESO", "Tiempo promedio desde orden hasta adjudicacion", dt, new Dictionary<string, string>
            {
                ["IdOrden"] = "ID",
                ["Orden"] = "Orden",
                ["FechaOrden"] = "Creada",
                ["FechaAdjudicacion"] = "Adjudicada",
                ["DiasProceso"] = "Dias",
                ["PedidosAdjudicados"] = "Pedidos",
                ["ProveedoresParticipantes"] = "Proveedores",
                ["MontoTotal"] = "Monto (Q)"
            });
        }

        // ============================================================
        // 8. VARIACION DE PRECIOS
        // ============================================================
        public byte[] GenerarVariacionPrecios()
        {
            var sql = @"SELECT 
                            a.codigo AS CodigoArticulo,
                            a.Nombre AS Articulo,
                            p.Nombre AS Proveedor,
                            CONVERT(VARCHAR(10), ofp.Fecha_Oferta, 103) AS Fecha,
                            ofp.Precio AS Precio,
                            LAG(ofp.Precio) OVER (PARTITION BY ofp.id_Proveedor, dp.id_Articulo ORDER BY ofp.Fecha_Oferta) AS PrecioAnterior,
                            ofp.Precio - LAG(ofp.Precio) OVER (PARTITION BY ofp.id_Proveedor, dp.id_Articulo ORDER BY ofp.Fecha_Oferta) AS Diferencia,
                            CASE 
                                WHEN LAG(ofp.Precio) OVER (PARTITION BY ofp.id_Proveedor, dp.id_Articulo ORDER BY ofp.Fecha_Oferta) IS NULL THEN 'PRECIO INICIAL'
                                WHEN ofp.Precio > LAG(ofp.Precio) OVER (PARTITION BY ofp.id_Proveedor, dp.id_Articulo ORDER BY ofp.Fecha_Oferta) THEN 'SUBE'
                                WHEN ofp.Precio < LAG(ofp.Precio) OVER (PARTITION BY ofp.id_Proveedor, dp.id_Articulo ORDER BY ofp.Fecha_Oferta) THEN 'BAJA'
                                ELSE 'IGUAL'
                            END AS Tendencia
                        FROM Oferta_Proveedor ofp
                        INNER JOIN Proveedor p ON ofp.id_Proveedor = p.id
                        INNER JOIN Pedido_Interno pi ON ofp.id_Pedido_Interno = pi.id
                        INNER JOIN Detalle_Pedido dp ON dp.id_Pedido = pi.id
                        INNER JOIN Articulo a ON dp.id_Articulo = a.id
                        ORDER BY a.Nombre ASC, p.Nombre ASC, ofp.Fecha_Oferta ASC";

            var dt = EjecutarQuery(sql);
            return DocumentoBase("VARIACION DE PRECIOS", "Evolucion de precios por articulo y proveedor", dt, new Dictionary<string, string>
            {
                ["CodigoArticulo"] = "Codigo",
                ["Articulo"] = "Articulo",
                ["Proveedor"] = "Proveedor",
                ["Fecha"] = "Fecha",
                ["Precio"] = "Precio (Q)",
                ["PrecioAnterior"] = "Precio Ant.",
                ["Diferencia"] = "Diferencia",
                ["Tendencia"] = "Tendencia"
            });
        }
    }
}