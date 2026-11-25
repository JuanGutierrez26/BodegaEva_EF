using BodegaEva_EF.Controles;
using BodegaEva_EF.Modelos;
using BodegaEva_EF.Servicios;
using BodegaEva_EF.Utilidades;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using VentaModelo = BodegaEva_EF.Modelos.Venta;

namespace BodegaEva_EF.Vistas
{
    internal class UcIncidencias : UserControl
    {
        private enum TipoIcono
        {
            Recibo,
            Efectivo
        }

        private Label lblCodigoVenta;
        private Label lblDetalleVenta;
        private Label lblMotivoVenta;
        private readonly BotonRedondeado btnReversarVenta;
        private Label lblCodigoAbono;
        private Label lblDetalleAbono;
        private Label lblMotivoAbono;
        private readonly BotonRedondeado btnReversarAbono;
        private readonly Label lblOperacionesPila;

        public UcIncidencias()
        {
            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;

            lblCodigoVenta = new Label();
            lblDetalleVenta = new Label();
            lblMotivoVenta = new Label();
            btnReversarVenta = new BotonRedondeado
            {
                Text = "↩ Reversar",
                Estilo = EstiloBoton.Peligro,
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 4, 0, 4)
            };
            lblCodigoAbono = new Label();
            lblDetalleAbono = new Label();
            lblMotivoAbono = new Label();
            btnReversarAbono = new BotonRedondeado
            {
                Text = "↩ Reversar",
                Estilo = EstiloBoton.Peligro,
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 4, 0, 4)
            };
            lblOperacionesPila = new Label
            {
                Text = "0",
                Font = Tema.NumeroGrande,
                ForeColor = Tema.PrimarioOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = Padding.Empty
            };

            btnReversarVenta.Click += (s, e) => ReversarVenta();
            btnReversarAbono.Click += (s, e) => ReversarAbono();

            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
            raiz.Controls.Add(CrearEncabezado(), 0, 0);
            raiz.Controls.Add(CrearTarjetas(), 0, 1);
            raiz.Controls.Add(CrearResumen(), 0, 2);
            Controls.Add(raiz);

            Datos.Cambio += Refrescar;
            Refrescar();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Datos.Cambio -= Refrescar;

            base.Dispose(disposing);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
                Refrescar();
        }

        private Control CrearEncabezado()
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = Padding.Empty,
                Padding = new Padding(18, 14, 18, 12)
            };

            var filas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            filas.Controls.Add(new Label
            {
                Text = "Incidencias y reversiones",
                Font = Tema.Titulo,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);
            filas.Controls.Add(new Label
            {
                Text = "Revierte la operación más reciente en el tope de cada pila cuando ocurra un error de registro",
                Font = Tema.Subtitulo,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 1);
            tarjeta.Controls.Add(filas);
            return tarjeta;
        }

        private Control CrearTarjetas()
        {
            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(14, 0, 14, 0),
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            columnas.Controls.Add(CrearTarjeta(
                "Reversar última venta", TipoIcono.Recibo, "Tope de la pila de ventas",
                out lblCodigoVenta, out lblDetalleVenta, out lblMotivoVenta, btnReversarVenta), 0, 0);
            columnas.Controls.Add(CrearTarjeta(
                "Reversar último abono", TipoIcono.Efectivo, "Tope de la pila de abonos",
                out lblCodigoAbono, out lblDetalleAbono, out lblMotivoAbono, btnReversarAbono), 1, 0);
            return columnas;
        }

        private static Control CrearTarjeta(
            string titulo, TipoIcono icono, string etiquetaPila,
            out Label lblCodigo, out Label lblDetalle, out Label lblMotivo, BotonRedondeado boton)
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = new Padding(0, 0, 7, 0),
                Padding = new Padding(20, 18, 20, 18)
            };

            var filas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

            var cabecera = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            cabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            cabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            cabecera.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            cabecera.Controls.Add(new Label
            {
                Text = titulo,
                Font = Tema.Titulo,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);
            cabecera.Controls.Add(new IconoIncidencia(icono)
            {
                Dock = DockStyle.Right,
                Width = 48,
                Height = 48,
                Margin = Padding.Empty
            }, 1, 0);
            filas.Controls.Add(cabecera, 0, 0);

            var pila = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.EncabezadoTabla,
                MostrarBorde = false,
                Margin = new Padding(0, 0, 0, 8),
                Padding = new Padding(14, 10, 14, 10)
            };
            var contenidoPila = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.EncabezadoTabla
            };
            contenidoPila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            contenidoPila.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            contenidoPila.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contenidoPila.Controls.Add(new Label
            {
                Text = etiquetaPila,
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            lblCodigo = new Label
            {
                Text = "—",
                Font = new Font(Tema.Negrita.FontFamily, 12f, FontStyle.Bold),
                ForeColor = Tema.TextoPrincipal,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            contenidoPila.Controls.Add(lblCodigo, 0, 1);
            pila.Controls.Add(contenidoPila);
            filas.Controls.Add(pila, 0, 1);

            lblDetalle = new Label
            {
                Text = "—",
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            filas.Controls.Add(lblDetalle, 0, 2);

            var acciones = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            acciones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            acciones.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            acciones.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblMotivo = new Label
            {
                Text = "",
                Font = Tema.Pequena,
                ForeColor = Tema.Error,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            acciones.Controls.Add(lblMotivo, 0, 0);
            acciones.Controls.Add(boton, 1, 0);
            filas.Controls.Add(acciones, 0, 3);

            tarjeta.Controls.Add(filas);
            return tarjeta;
        }

        private Control CrearResumen()
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = new Padding(14, 14, 14, 0),
                Padding = new Padding(18, 10, 18, 10)
            };

            var filas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            filas.Controls.Add(new Label
            {
                Text = "Operaciones en pila",
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);
            filas.Controls.Add(lblOperacionesPila, 1, 0);
            tarjeta.Controls.Add(filas);
            return tarjeta;
        }

        private void Refrescar()
        {
            VentaModelo? venta = IncidenciaServicio.TopeVenta();
            lblCodigoVenta.Text = venta == null
                ? "—"
                : $"{venta.Boleta.Codigo} · {venta.Cliente}";
            lblDetalleVenta.Text = venta == null
                ? "No hay ventas pendientes en la pila."
                : $"{venta.Pedido.Resumen} · Total: {Formato.Moneda(venta.Total)} · {Formato.NombreEstado(venta.Boleta.Estado)}";

            bool puedeVenta = IncidenciaServicio.PuedeReversarVenta(out string motivoVenta);
            btnReversarVenta.Enabled = puedeVenta;
            lblMotivoVenta.Text = puedeVenta ? "" : motivoVenta;
            lblMotivoVenta.ForeColor = Tema.Error;

            Abono? abono = IncidenciaServicio.TopeAbono();
            lblCodigoAbono.Text = abono == null
                ? "—"
                : $"{abono.CodigoBoleta} · {abono.Cliente}";
            lblDetalleAbono.Text = abono == null
                ? "No hay abonos pendientes en la pila."
                : $"Abono registrado: {Formato.Moneda(abono.Monto)} · {Antiguedad(abono.Fecha)}";

            bool puedeAbono = IncidenciaServicio.PuedeReversarAbono(out string motivoAbono);
            btnReversarAbono.Enabled = puedeAbono;
            lblMotivoAbono.Text = puedeAbono ? "" : motivoAbono;
            lblMotivoAbono.ForeColor = Tema.Error;

            lblOperacionesPila.Text = IncidenciaServicio.OperacionesEnPila.ToString();
        }

        private void ReversarVenta()
        {
            try
            {
                VentaModelo reversada = IncidenciaServicio.ReversarUltimaVenta();
                Refrescar();
                lblMotivoVenta.Text = $"Venta {reversada.Boleta.Codigo} reversada.";
                lblMotivoVenta.ForeColor = Tema.PrimarioOscuro;
            }
            catch (InvalidOperationException ex)
            {
                Refrescar();
                lblMotivoVenta.Text = ex.Message;
                lblMotivoVenta.ForeColor = Tema.Error;
            }
        }

        private void ReversarAbono()
        {
            try
            {
                Abono reversado = IncidenciaServicio.ReversarUltimoAbono();
                Refrescar();
                lblMotivoAbono.Text = $"Abono de {reversado.CodigoBoleta} reversado.";
                lblMotivoAbono.ForeColor = Tema.PrimarioOscuro;
            }
            catch (InvalidOperationException ex)
            {
                Refrescar();
                lblMotivoAbono.Text = ex.Message;
                lblMotivoAbono.ForeColor = Tema.Error;
            }
        }

        private static string Antiguedad(DateTime fecha)
        {
            string texto = Formato.HaceCuanto(fecha);
            return texto.Length == 0 ? texto : char.ToUpperInvariant(texto[0]) + texto[1..];
        }

        private sealed class IconoIncidencia : Control
        {
            private readonly TipoIcono tipo;

            public IconoIncidencia(TipoIcono tipo)
            {
                this.tipo = tipo;

                SetStyle(ControlStyles.UserPaint |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.SupportsTransparentBackColor, true);
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Width < 8 || Height < 8)
                    return;

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var area = new Rectangle(0, 0, Width - 1, Height - 1);
                using (GraphicsPath ruta = Tema.RutaRedondeada(area, Tema.RadioBoton + 2))
                using (var pincel = new SolidBrush(Tema.ErrorClaro))
                {
                    g.FillPath(pincel, ruta);
                }

                float escala = Math.Min(Width, Height) / 48f;
                g.TranslateTransform((Width - 48 * escala) / 2f, (Height - 48 * escala) / 2f);
                g.ScaleTransform(escala, escala);

                using var lapiz = new Pen(Tema.Error, 2f) { LineJoin = LineJoin.Round };

                if (tipo == TipoIcono.Recibo)
                {
                    g.DrawRectangle(lapiz, new Rectangle(14, 8, 20, 30));
                    g.DrawLine(lapiz, 18, 16, 30, 16);
                    g.DrawLine(lapiz, 18, 22, 30, 22);
                    g.DrawLine(lapiz, 18, 28, 26, 28);
                    g.DrawLines(lapiz, new[]
                    {
                        new PointF(14, 38), new PointF(18, 42), new PointF(22, 38),
                        new PointF(26, 42), new PointF(30, 38), new PointF(34, 42)
                    });
                }
                else
                {
                    g.DrawRectangle(lapiz, new Rectangle(8, 14, 32, 20));
                    g.DrawEllipse(lapiz, new Rectangle(20, 19, 8, 8));
                    TextRenderer.DrawText(g, "S/", Tema.Pequena, new Rectangle(9, 27, 30, 6),
                        Tema.Error, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                }
            }
        }
    }
}
