using BodegaEva_EF.Controles;
using BodegaEva_EF.Estructuras;
using BodegaEva_EF.Modelos;
using BodegaEva_EF.Servicios;
using BodegaEva_EF.Utilidades;
using System.Drawing.Drawing2D;

namespace BodegaEva_EF.Vistas.Venta
{
    internal class PasoProductos : UserControl
    {
        private const string PlaceholderProducto = "Ej. Leche Gloria tarro 400 g";

        private ListaProductos productos = null!;
        private TextBox txtCliente = null!;
        private TextBox txtProducto = null!;
        private NumericUpDown nudCantidad = null!;
        private TextBox txtPrecio = null!;
        private BotonRedondeado btnAgregar = null!;
        private Label lblError = null!;
        private DataGridView dgv = null!;
        private Label lblTotal = null!;
        private IndicadorPasos indicador = null!;
        private Image iconoPapelera = null!;
        private bool productoEsPlaceholder = true;

        public event Action<int>? NavegacionSolicitada;

        public string Cliente => txtCliente.Text.Trim();

        public ListaProductos Productos => productos;

        public PasoProductos()
        {
            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;

            productos = VentaServicio.PedidoEnCurso?.Productos ?? CrearProductosReferencia();
            ConstruirInterfaz();
            Refrescar();
        }

        private static ListaProductos CrearProductosReferencia()
        {
            var lista = new ListaProductos();
            
            return lista;
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
                PasoActual = 1
            };
            indicador.ConfigurarPasos("Productos", "Pedido", "Pago", "Confirmación");
            indicador.PasoSeleccionado += Indicador_PasoSeleccionado;
            raiz.Controls.Add(indicador, 0, 0);
            raiz.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Tema.Fondo, Margin = Padding.Empty }, 0, 1);

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
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            columnas.Controls.Add(CrearPanelFormulario(), 0, 0);
            columnas.Controls.Add(CrearPanelDetalle(), 1, 0);

            raiz.Controls.Add(columnas, 0, 2);
            Controls.Add(raiz);
        }

        private PanelRedondeado CrearPanelFormulario()
        {
            var panel = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = new Padding(0, 0, 14, 0),
                Padding = new Padding(20, 18, 20, 18)
            };

            var filas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 224F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            filas.Controls.Add(new Label
            {
                Text = "Agregar producto",
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            filas.Controls.Add(CrearFormulario(), 0, 1);

            lblError = new Label
            {
                Font = Tema.Pequena,
                ForeColor = Tema.Error,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty,
                Visible = false
            };
            filas.Controls.Add(lblError, 0, 2);

            btnAgregar = new BotonRedondeado
            {
                Text = "+ Agregar producto",
                Estilo = EstiloBoton.Secundario,
                Dock = DockStyle.Fill,
                Enabled = false,
                Margin = Padding.Empty
            };
            btnAgregar.Click += BtnAgregar_Click;
            filas.Controls.Add(btnAgregar, 0, 3);

            panel.Controls.Add(filas);
            return panel;
        }

        private Control CrearFormulario()
        {
            var formulario = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108F));
            formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            txtCliente = new TextBox
            {
                Text = "",
                Font = Tema.Normal,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 6, 0, 6)
            };
            txtCliente.TextChanged += (s, e) => ActualizarBotonAgregar();

            txtProducto = new TextBox
            {
                Text = PlaceholderProducto,
                ForeColor = Tema.TextoSecundario,
                Font = Tema.Normal,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 6, 0, 6)
            };
            txtProducto.Enter += (s, e) =>
            {
                if (!productoEsPlaceholder)
                    return;

                txtProducto.Text = "";
                txtProducto.ForeColor = Tema.TextoPrincipal;
                productoEsPlaceholder = false;
            };
            txtProducto.Leave += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtProducto.Text))
                    return;

                productoEsPlaceholder = true;
                txtProducto.Text = PlaceholderProducto;
                txtProducto.ForeColor = Tema.TextoSecundario;
            };
            txtProducto.TextChanged += (s, e) => ActualizarBotonAgregar();

            nudCantidad = new NumericUpDown
            {
                Minimum = 1,
                Maximum = Validaciones.MaxCantidad,
                Value = 1,
                Font = Tema.Normal,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 6, 0, 6)
            };
            nudCantidad.ValueChanged += (s, e) => ActualizarBotonAgregar();

            txtPrecio = new TextBox
            {
                Text = "0.00",
                Font = Tema.Normal,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 6, 0, 6)
            };
            txtPrecio.TextChanged += (s, e) => ActualizarBotonAgregar();

            AgregarCampo(formulario, 0, "Cliente", txtCliente);
            AgregarCampo(formulario, 1, "Producto", txtProducto);
            AgregarCampo(formulario, 2, "Cantidad", nudCantidad);
            AgregarCampo(formulario, 3, "Precio (S/)", txtPrecio);

            return formulario;
        }

        private static void AgregarCampo(TableLayoutPanel contenedor, int fila, string etiqueta, Control campo)
        {
            contenedor.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            contenedor.Controls.Add(new Label
            {
                Text = etiqueta,
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, fila);
            contenedor.Controls.Add(campo, 1, fila);
        }

        private void ActualizarBotonAgregar()
        {
            bool clienteValido = Validaciones.Cliente(txtCliente.Text) == null;
            bool productoValido = !productoEsPlaceholder &&
                                  Validaciones.NombreProducto(txtProducto.Text) == null;
            bool cantidadValida = nudCantidad.Value >= 1;
            bool precioValido = Validaciones.Precio(txtPrecio.Text, out _) == null;

            btnAgregar.Enabled = clienteValido && productoValido && cantidadValida && precioValido;
        }

        private void BtnAgregar_Click(object? sender, EventArgs e)
        {
            decimal precio = 0;
            string error = Validaciones.Cliente(txtCliente.Text)
                           ?? Validaciones.NombreProducto(productoEsPlaceholder ? "" : txtProducto.Text)
                           ?? Validaciones.Precio(txtPrecio.Text, out precio);

            if (error != null)
            {
                Validaciones.Mostrar(lblError, error);
                return;
            }

            try
            {
                productos.Agregar(new Producto(txtProducto.Text, (int)nudCantidad.Value, precio));
            }
            catch (ArgumentException ex)
            {
                Validaciones.Mostrar(lblError, ex.Message);
                return;
            }

            Validaciones.Mostrar(lblError, null);

            productoEsPlaceholder = true;
            txtProducto.Text = PlaceholderProducto;
            txtProducto.ForeColor = Tema.TextoSecundario;
            nudCantidad.Value = 1;
            txtPrecio.Text = "0.00";

            Refrescar();
            ActualizarBotonAgregar();
        }

        private void Indicador_PasoSeleccionado(int paso)
        {
            if (paso == 1)
                return;

            NavegacionSolicitada?.Invoke(paso);
        }

        private void BtnContinuar_Click(object? sender, EventArgs e)
        {
            NavegacionSolicitada?.Invoke(2);
        }

        private PanelRedondeado CrearPanelDetalle()
        {
            var panel = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = Padding.Empty,
                Padding = new Padding(14, 14, 14, 14)
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
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));

            var tabla = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                Margin = Padding.Empty
            };
            tabla.Controls.Add(CrearTabla());
            filas.Controls.Add(tabla, 0, 0);
            filas.Controls.Add(CrearTarjetaTotal(), 0, 1);

            panel.Controls.Add(filas);
            return panel;
        }

        private DataGridView CrearTabla()
        {
            iconoPapelera = CrearIconoPapelera();

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Tema.Blanco,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = Tema.Normal,
                Margin = Padding.Empty
            };
            dgv.DefaultCellStyle.Font = Tema.Normal;
            dgv.DefaultCellStyle.ForeColor = Tema.TextoPrincipal;
            dgv.DefaultCellStyle.BackColor = Tema.Blanco;
            dgv.DefaultCellStyle.SelectionBackColor = Tema.PrimarioClaro;
            dgv.DefaultCellStyle.SelectionForeColor = Tema.TextoPrincipal;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Font = Tema.Etiqueta;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Tema.TextoSecundario;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Tema.EncabezadoTabla;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Tema.EncabezadoTabla;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 32;
            dgv.RowTemplate.Height = 38;

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Producto",
                HeaderText = "Producto",
                FillWeight = 42,
                ReadOnly = true
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                HeaderText = "Cant.",
                FillWeight = 10,
                ReadOnly = true
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Precio",
                HeaderText = "Precio",
                FillWeight = 18,
                ReadOnly = true
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Subtotal",
                HeaderText = "Subtotal",
                FillWeight = 20,
                ReadOnly = true
            });
            dgv.Columns.Add(new DataGridViewImageColumn
            {
                Name = "Quitar",
                HeaderText = "",
                Image = iconoPapelera,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 48,
                Resizable = DataGridViewTriState.False,
                ReadOnly = true
            });

            dgv.CellClick += Dgv_CellClick;
            return dgv;
        }

        private void Dgv_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || dgv.Columns[e.ColumnIndex].Name != "Quitar")
                return;

            string nombre = dgv.Rows[e.RowIndex].Cells["Producto"].Value?.ToString() ?? "";
            if (productos.Quitar(nombre))
                Refrescar();
        }

        private PanelRedondeado CrearTarjetaTotal()
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.PrimarioClaro,
                MostrarBorde = false,
                Margin = new Padding(0, 14, 0, 0),
                Padding = new Padding(18, 12, 18, 12)
            };

            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.PrimarioClaro
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var importe = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.PrimarioClaro
            };
            importe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            importe.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            importe.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            importe.Controls.Add(new Label
            {
                Text = "Total de la venta",
                Font = Tema.Pequena,
                ForeColor = Tema.PrimarioOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Margin = Padding.Empty
            }, 0, 0);

            lblTotal = new Label
            {
                Text = Formato.Moneda(productos.CalcularTotal()),
                Font = Tema.NumeroGrande,
                ForeColor = Tema.PrimarioOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            importe.Controls.Add(lblTotal, 0, 1);

            var btnContinuar = new BotonRedondeado
            {
                Text = "Continuar →",
                Estilo = EstiloBoton.Primario,
                Dock = DockStyle.Fill,
                Margin = new Padding(14, 10, 0, 10)
            };
            btnContinuar.Click += BtnContinuar_Click;

            columnas.Controls.Add(importe, 0, 0);
            columnas.Controls.Add(btnContinuar, 1, 0);

            tarjeta.Controls.Add(columnas);
            return tarjeta;
        }

        private void Refrescar()
        {
            dgv.Rows.Clear();
            foreach (Producto producto in productos.Recorrer())
            {
                dgv.Rows.Add(
                    producto.Nombre,
                    producto.Cantidad,
                    Formato.Moneda(producto.Precio),
                    Formato.Moneda(producto.Subtotal),
                    iconoPapelera);
            }

            lblTotal.Text = Formato.Moneda(productos.CalcularTotal());
        }

        private static Image CrearIconoPapelera()
        {
            var imagen = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(imagen))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var lapiz = new Pen(Tema.Error, 1.5f) { LineJoin = LineJoin.Round };
                g.DrawLine(lapiz, 3, 5, 13, 5);
                g.DrawLine(lapiz, 6, 3, 10, 3);
                g.DrawRectangle(lapiz, 4, 5, 8, 9);
                g.DrawLine(lapiz, 7, 8, 7, 12);
                g.DrawLine(lapiz, 9, 8, 9, 12);
            }
            return imagen;
        }
    }
}
