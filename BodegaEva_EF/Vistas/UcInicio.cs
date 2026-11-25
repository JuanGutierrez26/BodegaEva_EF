using BodegaEva_EF.Controles;
using BodegaEva_EF.Servicios;
using BodegaEva_EF.Utilidades;
using System.Drawing.Drawing2D;

namespace BodegaEva_EF.Vistas
{
    internal class UcInicio : UserControl
    {
        private readonly Label lblPresenciales;
        private readonly Label lblWhatsApp;
        private readonly Label lblVentas;

        public UcInicio()
        {
            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;

            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Margin = Padding.Empty,
                Padding = new Padding(28, 24, 28, 22),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));

            raiz.Controls.Add(CrearEncabezado(), 0, 0);
            raiz.Controls.Add(CrearAccesos(), 0, 1);

            var tituloResumen = new Label
            {
                Text = "Resumen de hoy",
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            raiz.Controls.Add(tituloResumen, 0, 2);
            raiz.Controls.Add(CrearResumen(out lblPresenciales, out lblWhatsApp, out lblVentas), 0, 3);

            Controls.Add(raiz);

            ActualizarResumen();
            Datos.Cambio += ActualizarResumen;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Datos.Cambio -= ActualizarResumen;

            base.Dispose(disposing);
        }

        private void ActualizarResumen()
        {
            lblPresenciales.Text = Datos.ColaPresencial.Contar.ToString();
            lblWhatsApp.Text = Datos.ColaWhatsApp.Contar.ToString();
            lblVentas.Text = Datos.Ventas.Contar.ToString();
        }

        private static Control CrearEncabezado()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = Tema.Fondo
            };

            var subtitulo = new Label
            {
                Text = "Elige una opción para comenzar a atender en mostrador",
                Font = Tema.Subtitulo,
                ForeColor = Tema.TextoSecundario,
                AutoSize = true,
                Location = new Point(0, 46)
            };

            var titulo = new Label
            {
                Text = "¡Buen día, Eva!",
                Font = Tema.Titulo,
                ForeColor = Tema.TextoPrincipal,
                AutoSize = true,
                Location = new Point(0, 2)
            };

            panel.Controls.Add(subtitulo);
            panel.Controls.Add(titulo);
            return panel;
        }

        private static Control CrearAccesos()
        {
            var cuadricula = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            cuadricula.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            cuadricula.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            cuadricula.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            cuadricula.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            cuadricula.Controls.Add(CrearTarjetaAcceso(
                TipoIcono.Venta,
                "Nueva venta",
                "Registrar productos, gestionar pedido y cobrar o fiar",
                Tema.PrimarioClaro, Tema.Primario), 0, 0);

            cuadricula.Controls.Add(CrearTarjetaAcceso(
                TipoIcono.Fiado,
                "Fiado",
                "Consultar deudas pendientes y registrar abonos",
                Tema.AmbarClaro, Tema.AmbarOscuro), 1, 0);

            cuadricula.Controls.Add(CrearTarjetaAcceso(
                TipoIcono.Historial,
                "Historial",
                "Buscar boletas emitidas y revisar su estado",
                Tema.EncabezadoTabla, Tema.TextoPrincipal), 0, 1);

            cuadricula.Controls.Add(CrearTarjetaAcceso(
                TipoIcono.Incidencias,
                "Incidencias",
                "Revertir la última venta o el último abono aplicado",
                Tema.ErrorClaro, Tema.Error), 1, 1);

            return cuadricula;
        }

        private static PanelRedondeado CrearTarjetaAcceso(
            TipoIcono icono, string titulo, string descripcion, Color fondoIcono, Color trazoIcono)
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                Interactivo = true,
                MostrarBorde = true,
                Margin = new Padding(7)
            };

            var contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = new Padding(18, 16, 18, 16),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

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
            cabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            cabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            cabecera.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            cabecera.Controls.Add(new IconoTarjeta(icono, fondoIcono, trazoIcono)
            {
                Dock = DockStyle.Left,
                Width = 46,
                Height = 46,
                Margin = Padding.Empty
            }, 0, 0);

            cabecera.Controls.Add(new Label
            {
                Text = "→",
                Font = Tema.Titulo,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopRight,
                Margin = Padding.Empty
            }, 1, 0);

            contenido.Controls.Add(cabecera, 0, 0);
            contenido.Controls.Add(new Label
            {
                Text = titulo,
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 1);
            contenido.Controls.Add(new Label
            {
                Text = descripcion,
                Font = Tema.Normal,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Margin = Padding.Empty
            }, 0, 2);

            tarjeta.Controls.Add(contenido);
            tarjeta.ConectarHijos();
            return tarjeta;
        }

        private Control CrearResumen(out Label presenciales, out Label whatsapp, out Label ventas)
        {
            var fila = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
            fila.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            presenciales = CrearTarjetaResumen(fila, 0, "Pedidos presenciales", "EN COLA",
                Tema.Primario, Tema.PrimarioClaro);
            whatsapp = CrearTarjetaResumen(fila, 1, "Pedidos de WhatsApp", "EN COLA",
                Tema.AmbarOscuro, Tema.AmbarClaro);
            ventas = CrearTarjetaResumen(fila, 2, "Ventas apiladas", "EN PILA",
                Tema.TextoPrincipal, Tema.EncabezadoTabla);

            return fila;
        }

        private void InitializeComponent()
        {

        }

        private static Label CrearTarjetaResumen(
            TableLayoutPanel contenedor, int columna, string titulo, string etiqueta,
            Color acento, Color fondoInsignia)
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = columna == 2 ? Padding.Empty : new Padding(0, 0, 14, 0)
            };

            var contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = new Padding(16, 12, 16, 12),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));

            contenido.Controls.Add(new Label
            {
                Text = titulo,
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            var numero = new Label
            {
                Text = "0",
                Font = Tema.NumeroGrande,
                ForeColor = acento,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            contenido.Controls.Add(numero, 0, 1);

            var insignia = new Insignia { Margin = Padding.Empty, Anchor = AnchorStyles.Left };
            insignia.Personalizar(etiqueta, fondoInsignia, acento);
            contenido.Controls.Add(insignia, 0, 2);

            tarjeta.Controls.Add(contenido);
            contenedor.Controls.Add(tarjeta, columna, 0);
            return numero;
        }

        private enum TipoIcono
        {
            Venta,
            Fiado,
            Historial,
            Incidencias
        }

        private sealed class IconoTarjeta : Control
        {
            private readonly TipoIcono tipo;
            private readonly Color relleno;
            private readonly Color trazo;

            public IconoTarjeta(TipoIcono tipo, Color relleno, Color trazo)
            {
                this.tipo = tipo;
                this.relleno = relleno;
                this.trazo = trazo;

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
                using (var pincel = new SolidBrush(relleno))
                {
                    g.FillPath(pincel, ruta);
                }

                float escala = Math.Min(Width, Height) / 48f;
                g.TranslateTransform((Width - 48 * escala) / 2f, (Height - 48 * escala) / 2f);
                g.ScaleTransform(escala, escala);

                using var lapiz = new Pen(trazo, 2f) { LineJoin = LineJoin.Round };
                Dibujar(g, lapiz);
            }

            private void Dibujar(Graphics g, Pen lapiz)
            {
                switch (tipo)
                {
                    case TipoIcono.Venta:
                        g.DrawRectangle(lapiz, new Rectangle(13, 16, 22, 20));
                        g.DrawArc(lapiz, new Rectangle(18, 9, 12, 14), 180, 180);
                        g.DrawLine(lapiz, 13, 22, 35, 22);
                        break;

                    case TipoIcono.Fiado:
                        g.DrawEllipse(lapiz, new Rectangle(11, 11, 26, 26));
                        TextRenderer.DrawText(g, "S/", Tema.Etiqueta, new Rectangle(11, 11, 26, 26),
                            trazo, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                                   TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                        break;

                    case TipoIcono.Historial:
                        g.DrawEllipse(lapiz, new Rectangle(11, 11, 26, 26));
                        g.DrawLine(lapiz, 24, 24, 24, 16);
                        g.DrawLine(lapiz, 24, 24, 31, 28);
                        break;

                    default:
                        PointF[] triangulo =
                        {
                            new PointF(24, 11),
                            new PointF(40, 38),
                            new PointF(8, 38),
                            new PointF(24, 11)
                        };
                        g.DrawLines(lapiz, triangulo);
                        g.DrawLine(lapiz, 24, 21, 24, 29);
                        using (var pincel = new SolidBrush(trazo))
                            g.FillEllipse(pincel, new RectangleF(22.5f, 33f, 3f, 3f));
                        break;
                }
            }
        }
    }
}
