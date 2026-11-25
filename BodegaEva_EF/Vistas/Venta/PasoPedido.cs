using BodegaEva_EF.Controles;
using BodegaEva_EF.Estructuras;
using BodegaEva_EF.Modelos;
using BodegaEva_EF.Servicios;
using BodegaEva_EF.Utilidades;
using System.Drawing.Drawing2D;

namespace BodegaEva_EF.Vistas.Venta
{
    internal class PasoPedido : UserControl
    {
        private readonly PasoProductos origen;

        private string Cliente => origen.Cliente;
        private ListaProductos Productos => origen.Productos;

        private TipoPedido tipoSeleccionado = TipoPedido.Presencial;
        private IndicadorPasos indicador = null!;
        private PanelRedondeado tarjetaPresencial = null!;
        private PanelRedondeado tarjetaWhatsApp = null!;
        private Label checkPresencial = null!;
        private Label checkWhatsApp = null!;
        private Label lblColaPresencial = null!;
        private Label lblColaWhatsApp = null!;
        private FlowLayoutPanel listaPresencial = null!;
        private FlowLayoutPanel listaWhatsApp = null!;
        private Label lblMensaje = null!;
        private BotonRedondeado btnAgregarCola = null!;

        public event Action<int>? NavegacionSolicitada;

        public PasoPedido(PasoProductos origen)
        {
            this.origen = origen;

            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;

            ConstruirInterfaz();
            ActualizarSeleccion();
            ActualizarColas();

            Datos.Cambio += Datos_Cambio;
        }

        private void ConstruirInterfaz()
        {
            var raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 11,
                Margin = Padding.Empty,
                Padding = new Padding(24, 20, 24, 24),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 76F));

            indicador = new IndicadorPasos
            {
                Dock = DockStyle.Top,
                Width = 760,
                Height = 52
            };
            indicador.ConfigurarPasos("Productos", "Pedido", "Pago", "Confirmación");
            indicador.PasoActual = 2;
            indicador.PasoSeleccionado += Indicador_PasoSeleccionado;
            raiz.Controls.Add(indicador, 0, 0);

            raiz.Controls.Add(CrearEspacio(), 0, 1);
            raiz.Controls.Add(CrearSelectorCanal(), 0, 2);
            raiz.Controls.Add(CrearEspacio(), 0, 3);
            raiz.Controls.Add(CrearBotonAgregarCola(), 0, 4);
            raiz.Controls.Add(CrearEspacio(), 0, 5);
            raiz.Controls.Add(CrearEspacio(), 0, 7);
            raiz.Controls.Add(CrearColas(), 0, 8);
            raiz.Controls.Add(CrearEspacio(), 0, 9);
            raiz.Controls.Add(CrearAcciones(), 0, 10);

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

        private Control CrearBotonAgregarCola()
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
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            fila.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            btnAgregarCola = new BotonRedondeado
            {
                Estilo = EstiloBoton.Secundario,
                Dock = DockStyle.Fill
            };
            btnAgregarCola.Click += BtnAgregarCola_Click;

            fila.Controls.Add(btnAgregarCola, 1, 0);
            return fila;
        }

        private Control CrearSelectorCanal()
        {
            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            tarjetaPresencial = CrearTarjetaCanal(
                "Presencial",
                "Prioridad alta",
                Tema.PrimarioClaro,
                Tema.PrimarioOscuro,
                "Orden de llegada",
                Tema.AmbarClaro,
                Tema.AmbarOscuro,
                "Se atiende en mostrador antes que los pedidos por WhatsApp.",
                out checkPresencial);
            tarjetaPresencial.Margin = new Padding(0, 0, 7, 0);
            tarjetaPresencial.Click += (s, e) => Seleccionar(TipoPedido.Presencial);

            tarjetaWhatsApp = CrearTarjetaCanal(
                "WhatsApp",
                "Orden de llegada",
                Tema.AmbarClaro,
                Tema.AmbarOscuro,
                "Seguimiento por mensaje",
                Tema.PrimarioClaro,
                Tema.PrimarioOscuro,
                "Se atiende después de la cola presencial.",
                out checkWhatsApp);
            tarjetaWhatsApp.Margin = new Padding(7, 0, 0, 0);
            tarjetaWhatsApp.Click += (s, e) => Seleccionar(TipoPedido.WhatsApp);

            columnas.Controls.Add(tarjetaPresencial, 0, 0);
            columnas.Controls.Add(tarjetaWhatsApp, 1, 0);
            return columnas;
        }

        private static PanelRedondeado CrearTarjetaCanal(
            string titulo,
            string textoInsignia1,
            Color relleno1,
            Color letra1,
            string textoInsignia2,
            Color relleno2,
            Color letra2,
            string descripcion,
            out Label check)
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Interactivo = true,
                Padding = new Padding(16, 14, 16, 12)
            };

            var contenido = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Tema.Blanco,
                Margin = Padding.Empty
            };

            contenido.Controls.Add(new Label
            {
                Text = titulo,
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6)
            });

            var insignias = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                BackColor = Tema.Blanco,
                Margin = new Padding(0, 0, 0, 6)
            };
            insignias.Controls.Add(CrearInsignia(textoInsignia1, relleno1, letra1));
            insignias.Controls.Add(CrearInsignia(textoInsignia2, relleno2, letra2));
            contenido.Controls.Add(insignias);

            contenido.Controls.Add(new Label
            {
                Text = descripcion,
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                AutoSize = true,
                MaximumSize = new Size(360, 48),
                Margin = new Padding(0, 0, 0, 6)
            });

            check = new Label
            {
                Text = "✓ Seleccionado",
                Font = Tema.Etiqueta,
                ForeColor = Tema.PrimarioOscuro,
                AutoSize = true,
                Visible = false
            };
            contenido.Controls.Add(check);

            tarjeta.Controls.Add(contenido);
            tarjeta.ConectarHijos();
            return tarjeta;
        }

        private static Insignia CrearInsignia(string texto, Color relleno, Color letra)
        {
            var insignia = new Insignia();
            insignia.Personalizar(texto, relleno, letra);
            insignia.Margin = new Padding(0, 0, 6, 0);
            return insignia;
        }

        private void Seleccionar(TipoPedido tipo)
        {
            tipoSeleccionado = tipo;
            btnAgregarCola.Enabled = true;
            ActualizarSeleccion();
        }

        private void ActualizarSeleccion()
        {
            tarjetaPresencial.ColorBorde = tipoSeleccionado == TipoPedido.Presencial
                ? Tema.PrimarioOscuro
                : Tema.Borde;
            tarjetaWhatsApp.ColorBorde = tipoSeleccionado == TipoPedido.WhatsApp
                ? Tema.PrimarioOscuro
                : Tema.Borde;
            checkPresencial.Visible = tipoSeleccionado == TipoPedido.Presencial;
            checkWhatsApp.Visible = tipoSeleccionado == TipoPedido.WhatsApp;

            btnAgregarCola.Text = tipoSeleccionado == TipoPedido.Presencial
                ? "Agregar a cola presencial"
                : "Agregar a cola de WhatsApp";
        }

        
        private string ObtenerDetalleAlerta()
        {
            string? producto = null;

            foreach (Producto item in Productos.Recorrer())
            {
                if (item.Nombre.Contains("Arroz", StringComparison.OrdinalIgnoreCase))
                {
                    producto = item.Nombre;
                    break;
                }
            }

            producto ??= Productos.EstaVacia
                ? "el producto seleccionado"
                : ObtenerPrimerProducto();

            return $"Falta confirmar reemplazo de {producto} con el cliente. (pendiente de integración)";
        }

        private string ObtenerPrimerProducto()
        {
            foreach (Producto item in Productos.Recorrer())
                return item.Nombre;

            return "el producto seleccionado";
        }

        private Control CrearColas()
        {
            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            CrearPanelCola(columnas, 0, "Cola presencial", out lblColaPresencial, out listaPresencial);
            CrearPanelCola(columnas, 1, "Cola WhatsApp", out lblColaWhatsApp, out listaWhatsApp);
            return columnas;
        }

        private static void CrearPanelCola(
            TableLayoutPanel contenedor,
            int columna,
            string titulo,
            out Label etiqueta,
            out FlowLayoutPanel lista)
        {
            var panel = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = columna == 0 ? new Padding(0, 0, 7, 0) : new Padding(7, 0, 0, 0),
                Padding = new Padding(14, 12, 14, 12)
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
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            etiqueta = new Label
            {
                Text = titulo,
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };

            var area = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = new Padding(0, 0, 4, 0),
                BackColor = Tema.Blanco
            };
            area.Resize += (s, e) => AjustarAnchoFilas(area);
            lista = area;

            filas.Controls.Add(etiqueta, 0, 0);
            filas.Controls.Add(lista, 0, 1);
            panel.Controls.Add(filas);
            contenedor.Controls.Add(panel, columna, 0);
        }

        private void ActualizarColas()
        {
            lblColaPresencial.Text = $"Cola presencial ({Datos.ColaPresencial.Contar})";
            lblColaWhatsApp.Text = $"Cola WhatsApp ({Datos.ColaWhatsApp.Contar})";

            LlenarCola(listaPresencial, Datos.ColaPresencial, "PED");
            LlenarCola(listaWhatsApp, Datos.ColaWhatsApp, "WSP");
        }

        private static void LlenarCola(FlowLayoutPanel lista, ColaPedido cola, string prefijo)
        {
            lista.SuspendLayout();

            while (lista.Controls.Count > 0)
                lista.Controls[0].Dispose();

            int indice = 0;
            foreach (Pedido pedido in cola.Recorrer())
            {
                indice++;
                lista.Controls.Add(CrearFilaPedido($"{prefijo}-{indice:000}", pedido));
            }

            if (indice == 0)
                lista.Controls.Add(CrearFilaVacia());

            AjustarAnchoFilas(lista);
            lista.ResumeLayout();
        }

        private static void AjustarAnchoFilas(FlowLayoutPanel lista)
        {
            int ancho = Math.Max(240, lista.ClientSize.Width - lista.Padding.Left - lista.Padding.Right - 4);

            foreach (Control fila in lista.Controls)
                fila.Width = ancho;

            int alto = lista.Padding.Top + lista.Padding.Bottom;
            foreach (Control fila in lista.Controls)
                alto += fila.Margin.Top + fila.Height + fila.Margin.Bottom;

            lista.AutoScrollMinSize = new Size(0, alto);
        }

        private static Control CrearFilaVacia()
        {
            return new Label
            {
                Text = "Sin pedidos en espera.",
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                AutoSize = false,
                Height = 34,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 6)
            };
        }

        private static Control CrearFilaPedido(string codigo, Pedido pedido)
        {
            var fila = new PanelRedondeado
            {
                AutoSize = false,
                Height = 60,
                BackColor = Tema.Blanco,
                ColorBorde = Tema.Borde,
                MostrarBorde = true,
                Margin = new Padding(0, 0, 0, 6)
            };

            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = new Padding(10, 4, 10, 4),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var cliente = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            cliente.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            cliente.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            cliente.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            cliente.Controls.Add(new Label
            {
                Text = codigo,
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);
            cliente.Controls.Add(new Label
            {
                Text = Formato.Recortar(pedido.Cliente, 40),
                Font = Tema.Normal,
                ForeColor = Tema.TextoPrincipal,
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 1);

            columnas.Controls.Add(cliente, 0, 0);
            columnas.Controls.Add(new Label
            {
                Text = Formato.Moneda(pedido.Total),
                Font = Tema.Etiqueta,
                ForeColor = Tema.PrimarioOscuro,
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = Padding.Empty
            }, 1, 0);

            fila.Controls.Add(columnas);
            return fila;
        }

        private Control CrearAcciones()
        {
            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var izquierda = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            izquierda.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            izquierda.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            izquierda.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            izquierda.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var btnDespachar = new BotonRedondeado
            {
                Text = "Despachar siguiente",
                Estilo = EstiloBoton.Secundario,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 14, 0)
            };
            btnDespachar.Click += BtnDespachar_Click;
            izquierda.Controls.Add(btnDespachar, 0, 0);

            izquierda.Controls.Add(new Label
            {
                Text = "Se atiende primero la cola presencial",
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 14, 0)
            }, 0, 1);

            lblMensaje = new Label
            {
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Margin = new Padding(0, 0, 14, 0)
            };
            izquierda.Controls.Add(lblMensaje, 0, 2);

            var btnContinuar = new BotonRedondeado
            {
                Text = "Continuar →",
                Estilo = EstiloBoton.Primario,
                Dock = DockStyle.Fill,
                Margin = new Padding(14, 0, 0, 0)
            };
            btnContinuar.Click += BtnContinuar_Click;

            columnas.Controls.Add(izquierda, 0, 0);
            columnas.Controls.Add(btnContinuar, 1, 0);
            return columnas;
        }

        private void BtnAgregarCola_Click(object? sender, EventArgs e)
        {
            string error = Validaciones.Cliente(Cliente)
                           ?? (Productos.EstaVacia ? "Agrega al menos un producto." : null);
            if (error != null)
            {
                MostrarMensaje(error, Tema.Error);
                return;
            }

            ListaProductos copia = CopiarProductos(Productos);
            string cola = tipoSeleccionado == TipoPedido.Presencial ? "presencial" : "de WhatsApp";

            try
            {
                VentaServicio.RegistrarPedido(Cliente, tipoSeleccionado, copia);
                MostrarMensaje($"Pedido agregado a la cola {cola}: {Cliente} · {Formato.Moneda(copia.CalcularTotal())}",
                    Tema.PrimarioOscuro);
                btnAgregarCola.Enabled = false;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MostrarMensaje(ex.Message, Tema.Error);
            }
        }

        private static ListaProductos CopiarProductos(ListaProductos origen)
        {
            var copia = new ListaProductos();

            foreach (Producto producto in origen.Recorrer())
                copia.Agregar(new Producto(producto.Nombre, producto.Cantidad, producto.Precio));

            return copia;
        }

        private void BtnDespachar_Click(object? sender, EventArgs e)
        {
            try
            {
                Pedido despachado = VentaServicio.DespacharSiguiente();
                string cola = despachado.Tipo == TipoPedido.Presencial ? "presencial" : "de WhatsApp";
                MostrarMensaje($"Pedido despachado de la cola {cola}: {despachado.Cliente} · {Formato.Moneda(despachado.Total)}",
                    Tema.PrimarioOscuro);
            }
            catch (InvalidOperationException ex)
            {
                MostrarMensaje(ex.Message, Tema.Error);
            }
        }

        private void Indicador_PasoSeleccionado(int paso)
        {
            if (paso == 2)
                return;

            NavegacionSolicitada?.Invoke(paso);
        }

        private void BtnContinuar_Click(object? sender, EventArgs e)
        {
            NavegacionSolicitada?.Invoke(3);
        }

        private void MostrarMensaje(string texto, Color color)
        {
            lblMensaje.Text = texto;
            lblMensaje.ForeColor = color;
        }

        private void Datos_Cambio()
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke((Action)ActualizarColas);
                return;
            }

            ActualizarColas();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible && btnAgregarCola != null)
                btnAgregarCola.Enabled = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Datos.Cambio -= Datos_Cambio;

            base.Dispose(disposing);
        }

        private sealed class IconoAdvertencia : Control
        {
            public IconoAdvertencia()
            {
                SetStyle(ControlStyles.UserPaint |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer, true);
                Size = new Size(24, 24);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                PointF[] puntos =
                {
                    new PointF(Width / 2f, 3f),
                    new PointF(Width - 3f, Height - 4f),
                    new PointF(3f, Height - 4f)
                };

                using var relleno = new SolidBrush(Tema.Ambar);
                g.FillPolygon(relleno, puntos);

                using var lapiz = new Pen(Tema.AmbarOscuro, 1.5f) { LineJoin = LineJoin.Round };
                g.DrawPolygon(lapiz, puntos);
                g.DrawLine(lapiz, Width / 2f, Height * 0.42f, Width / 2f, Height * 0.70f);

                using var brocha = new SolidBrush(Tema.AmbarOscuro);
                g.FillEllipse(brocha, Width / 2f - 1.5f, Height * 0.78f, 3f, 3f);
            }
        }
    }
}
