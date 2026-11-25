using BodegaEva_EF.Controles;
using BodegaEva_EF.Modelos;
using BodegaEva_EF.Servicios;
using BodegaEva_EF.Utilidades;
using System.Drawing.Drawing2D;

namespace BodegaEva_EF.Vistas.Venta
{
    internal class PasoConfirmacion : UserControl
    {
        private readonly PasoPago pago;

        private IndicadorPasos indicador = null!;
        private Label lblCliente = null!;
        private Insignia insigniaEstado = null!;
        private FlowLayoutPanel listaProductos = null!;
        private Label lblTotal = null!;
        private Label lblMensaje = null!;
        private Panel panelGuardado = null!;
        private BotonRedondeado btnGuardar = null!;

        private Pedido? pedido;
        private Pedido? pedidoListado;
        private Boleta? boleta;
        private int anchoListado = -1;
        private bool guardada;
        private bool guardando;

        public event Action<int>? NavegacionSolicitada;

        public PasoConfirmacion(PasoPago pago)
        {
            this.pago = pago;

            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;

            ConstruirInterfaz();
            Refrescar();
        }

        private void ConstruirInterfaz()
        {
            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = new Padding(24, 20, 24, 24),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            indicador = new IndicadorPasos
            {
                Dock = DockStyle.Top,
                Width = 760,
                Height = 52,
                PasoActual = 4
            };
            indicador.ConfigurarPasos("Productos", "Pedido", "Pago", "Confirmación");
            indicador.PasoSeleccionado += Indicador_PasoSeleccionado;
            raiz.Controls.Add(indicador, 0, 0);
            raiz.Controls.Add(CrearEspacio(), 0, 1);

            var tarjeta = CrearTarjeta();
            tarjeta.Margin = new Padding(80, 0, 80, 0);
            raiz.Controls.Add(tarjeta, 0, 2);

            Controls.Add(raiz);
        }

        private static Panel CrearEspacio()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Fondo,
                Margin = Padding.Empty
            };
        }

        private PanelRedondeado CrearTarjeta()
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                ColorBorde = Tema.Borde,
                MostrarBorde = true,
                Margin = Padding.Empty,
                Padding = new Padding(24, 18, 24, 18)
            };

            var filas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 9,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 82F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 2F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));

            filas.Controls.Add(CrearEncabezado(), 0, 0);
            filas.Controls.Add(CrearEspacioBlanco(), 0, 1);
            filas.Controls.Add(CrearListaProductos(), 0, 2);
            filas.Controls.Add(CrearEspacioBlanco(), 0, 3);
            filas.Controls.Add(new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Borde,
                Margin = Padding.Empty,
                Height = 2
            }, 0, 4);
            filas.Controls.Add(CrearEspacioBlanco(), 0, 5);
            filas.Controls.Add(CrearTotal(), 0, 6);
            filas.Controls.Add(CrearEspacioBlanco(), 0, 7);
            filas.Controls.Add(CrearPie(), 0, 8);

            tarjeta.Controls.Add(filas);
            return tarjeta;
        }

        private static Panel CrearEspacioBlanco()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                Margin = Padding.Empty
            };
        }

        private Control CrearEncabezado()
        {
            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var texto = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            texto.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            texto.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            texto.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            texto.Controls.Add(new Label
            {
                Text = "Resumen de la venta",
                Font = Tema.Titulo,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Margin = Padding.Empty
            }, 0, 0);

            lblCliente = new Label
            {
                Text = "Cliente: —",
                Font = Tema.Subtitulo,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Margin = Padding.Empty
            };
            texto.Controls.Add(lblCliente, 0, 1);

            insigniaEstado = new Insignia
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Margin = new Padding(0, 10, 0, 0)
            };
            insigniaEstado.Personalizar("Pendiente", Tema.EncabezadoTabla, Tema.TextoSecundario);

            columnas.Controls.Add(texto, 0, 0);
            columnas.Controls.Add(insigniaEstado, 1, 0);
            return columnas;
        }

        private Control CrearListaProductos()
        {
            var contenedor = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            contenedor.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            contenedor.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contenedor.Controls.Add(new Label
            {
                Text = "Productos",
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            listaProductos = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = new Padding(0, 0, 6, 0),
                BackColor = Tema.Blanco
            };
            listaProductos.Resize += ListaProductos_Resize;
            contenedor.Controls.Add(listaProductos, 0, 1);
            return contenedor;
        }

        private Control CrearTotal()
        {
            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = new Padding(0, 0, 8, 0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            columnas.Controls.Add(new Label
            {
                Text = "Total",
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            lblTotal = new Label
            {
                Text = "—",
                Font = Tema.NumeroGrande,
                ForeColor = Tema.PrimarioOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = Padding.Empty
            };
            columnas.Controls.Add(lblTotal, 1, 0);
            return columnas;
        }

        private Control CrearPie()
        {
            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var mensajes = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                Margin = new Padding(0, 0, 16, 0)
            };
            lblMensaje = new Label
            {
                Text = "Pendiente de confirmación.",
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            panelGuardado = CrearMensajeGuardado();
            mensajes.Controls.Add(lblMensaje);
            mensajes.Controls.Add(panelGuardado);

            btnGuardar = new BotonRedondeado
            {
                Text = "Guardar venta",
                Estilo = EstiloBoton.Primario,
                Dock = DockStyle.Fill,
                Image = CrearIconoDisquete(),
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(12, 0, 12, 0),
                Margin = Padding.Empty
            };
            btnGuardar.Click += BtnGuardar_Click;

            columnas.Controls.Add(mensajes, 0, 0);
            columnas.Controls.Add(btnGuardar, 1, 0);
            return columnas;
        }

        private Panel CrearMensajeGuardado()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                Visible = false,
                Margin = Padding.Empty
            };

            var fila = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Tema.Blanco,
                Margin = Padding.Empty
            };
            fila.Controls.Add(new IconoVerificacion
            {
                Size = new Size(22, 22),
                Margin = new Padding(0, 2, 8, 2)
            });

            var insignia = new Insignia
            {
                Margin = new Padding(0, 2, 0, 2)
            };
            insignia.Personalizar("Venta apilada, registrada en el diario y boleta guardada",
                Tema.PrimarioClaro, Tema.PrimarioOscuro);
            fila.Controls.Add(insignia);

            panel.Controls.Add(fila);
            return panel;
        }

        private void ListaProductos_Resize(object? sender, EventArgs e)
        {
            if (IsHandleCreated && !IsDisposed)
                ActualizarListaProductos();
        }

        private void Refrescar()
        {
            Pedido? actual = VentaServicio.PedidoEnCurso;

            if (actual != null)
            {
                if (guardada && !ReferenceEquals(pedido, actual))
                {
                    guardada = false;
                    boleta = null;
                }

                if (!guardada || pedido == null)
                    pedido = actual;
            }
            else if (!guardada)
            {
                pedido = null;
                boleta = null;
            }

            bool fiado = boleta != null
                ? boleta.Estado == EstadoBoleta.Fiada
                : pago.Fiado;
            ActualizarEstado(fiado);
            ActualizarResumen();

            btnGuardar.Enabled = !guardada && !guardando && pedido != null;

            if (!guardada)
            {
                panelGuardado.Visible = false;
                lblMensaje.Visible = true;
                lblMensaje.Text = pedido == null
                    ? "No hay un pedido en curso."
                    : "Pendiente de confirmación. Revisa el resumen y pulsa «Guardar venta».";
                lblMensaje.ForeColor = pedido == null ? Tema.Error : Tema.TextoSecundario;
            }
        }

        private void ActualizarEstado(bool fiado)
        {
            insigniaEstado.Personalizar(
                fiado ? "Fiado" : "Pagado",
                fiado ? Tema.AmbarClaro : Tema.PrimarioClaro,
                fiado ? Tema.AmbarOscuro : Tema.PrimarioOscuro);
        }

        private void ActualizarResumen()
        {
            lblCliente.Text = $"Cliente: {pedido?.Cliente ?? "—"}";
            lblTotal.Text = pedido == null ? "—" : Formato.Moneda(pedido.Total);
            ActualizarListaProductos();
        }

        private void ActualizarListaProductos()
        {
            int ancho = Math.Max(280, listaProductos.ClientSize.Width
                - listaProductos.Padding.Left - listaProductos.Padding.Right
                - SystemInformation.VerticalScrollBarWidth - 4);
            int esperados = pedido == null ? 1 : pedido.Productos.Contar;

            if (ReferenceEquals(pedidoListado, pedido) && anchoListado == ancho &&
                listaProductos.Controls.Count == esperados)
                return;

            listaProductos.SuspendLayout();

            while (listaProductos.Controls.Count > 0)
                listaProductos.Controls[0].Dispose();

            if (pedido == null)
            {
                listaProductos.Controls.Add(new Label
                {
                    Text = "No hay productos para mostrar.",
                    Font = Tema.Pequena,
                    ForeColor = Tema.TextoSecundario,
                    AutoSize = false,
                    Width = ancho,
                    Height = 34,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(0, 0, 0, 6)
                });
            }
            else
            {
                foreach (Producto producto in pedido.Productos.Recorrer())
                    listaProductos.Controls.Add(CrearFilaProducto(producto, ancho));
            }

            AjustarFilasProductos(listaProductos, ancho);
            pedidoListado = pedido;
            anchoListado = ancho;
            listaProductos.ResumeLayout();
        }

        private static void AjustarFilasProductos(FlowLayoutPanel lista, int ancho)
        {
            int alto = lista.Padding.Top + lista.Padding.Bottom;
            foreach (Control fila in lista.Controls)
            {
                fila.Width = ancho;
                alto += fila.Margin.Top + fila.Height + fila.Margin.Bottom;
            }

            lista.AutoScrollMinSize = new Size(0, alto);
        }

        private static Control CrearFilaProducto(Producto producto, int ancho)
        {
            int anchoNombre = Math.Max(80, ancho - 56 - 150 - 24);
            Size medido = TextRenderer.MeasureText(producto.Nombre, Tema.Normal,
                new Size(anchoNombre, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
            int alto = Math.Max(44, medido.Height + 12);

            var fila = new PanelRedondeado
            {
                AutoSize = false,
                Height = alto,
                BackColor = Tema.Blanco,
                ColorBorde = Tema.Borde,
                MostrarBorde = true,
                Margin = new Padding(0, 0, 0, 6),
                Padding = Padding.Empty
            };

            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = new Padding(10, 4, 10, 4),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 56F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            columnas.Controls.Add(new Label
            {
                Text = $"{producto.Cantidad} ×",
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            columnas.Controls.Add(new Label
            {
                Text = producto.Nombre,
                Font = Tema.Normal,
                ForeColor = Tema.TextoPrincipal,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 1, 0);

            columnas.Controls.Add(new Label
            {
                Text = Formato.Moneda(producto.Subtotal),
                Font = Tema.Negrita,
                ForeColor = Tema.PrimarioOscuro,
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = Padding.Empty
            }, 2, 0);

            fila.Controls.Add(columnas);
            return fila;
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (guardada || guardando)
                return;

            Pedido? actual = VentaServicio.PedidoEnCurso;
            if (actual == null)
            {
                MostrarError("No hay un pedido en curso.");
                return;
            }

            if (!ReferenceEquals(pedido, actual))
                pedido = actual;

            bool fiado = pago.Fiado;
            ActualizarEstado(fiado);

            if (!fiado && !VentaServicio.PagoValidado)
            {
                MostrarError("Valida un pago antes de guardar o marca la venta como fiada.");
                return;
            }

            guardando = true;
            btnGuardar.Enabled = false;

            try
            {
                BodegaEva_EF.Modelos.Venta venta = VentaServicio.CerrarVenta(fiado);
                pedido = venta.Pedido;
                boleta = venta.Boleta;
                guardada = true;

                ActualizarEstado(boleta.Estado == EstadoBoleta.Fiada);
                ActualizarResumen();

                lblMensaje.Visible = false;
                panelGuardado.Visible = true;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                btnGuardar.Enabled = true;
                MostrarError(ex.Message);
            }
            finally
            {
                guardando = false;
            }
        }

        private void MostrarError(string mensaje)
        {
            lblMensaje.Visible = true;
            panelGuardado.Visible = false;
            lblMensaje.Text = mensaje;
            lblMensaje.ForeColor = Tema.Error;
        }

        private void Indicador_PasoSeleccionado(int paso)
        {
            if (paso == 4)
                return;

            NavegacionSolicitada?.Invoke(paso);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible && !IsDisposed)
                Refrescar();
        }

        private static Image CrearIconoDisquete()
        {
            var imagen = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(imagen))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var lapiz = new Pen(Color.White, 1.4F) { LineJoin = LineJoin.Round };
                using GraphicsPath ruta = Tema.RutaRedondeada(new Rectangle(1, 1, 14, 14), 2);
                g.DrawPath(lapiz, ruta);
                g.DrawRectangle(lapiz, new Rectangle(4, 2, 8, 5));
                g.DrawLine(lapiz, 4, 13, 12, 13);
                g.DrawLine(lapiz, 4, 10, 7, 10);
                g.DrawLine(lapiz, 9, 10, 12, 10);
            }
            return imagen;
        }

        private sealed class IconoVerificacion : Control
        {
            public IconoVerificacion()
            {
                SetStyle(ControlStyles.UserPaint |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer, true);
                BackColor = Tema.Blanco;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var circulo = new Rectangle(1, 1, Width - 2, Height - 2);
                using (var relleno = new SolidBrush(Tema.Primario))
                    g.FillEllipse(relleno, circulo);

                using var lapiz = new Pen(Color.White, 2F) { LineJoin = LineJoin.Round };
                g.DrawLines(lapiz, new[]
                {
                    new PointF(circulo.Left + 5, circulo.Top + circulo.Height / 2F),
                    new PointF(circulo.Left + circulo.Width / 2F - 1, circulo.Bottom - 5),
                    new PointF(circulo.Right - 4, circulo.Top + 5)
                });
            }
        }
    }
}
