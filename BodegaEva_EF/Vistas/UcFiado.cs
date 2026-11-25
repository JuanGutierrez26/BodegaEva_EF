using BodegaEva_EF.Controles;
using BodegaEva_EF.Modelos;
using BodegaEva_EF.Servicios;
using BodegaEva_EF.Utilidades;
using System;
using System.Collections.Generic;

namespace BodegaEva_EF.Vistas
{
    internal class UcFiado : UserControl
    {
        private const string PlaceholderBuscar = "Ej. BOL-0039";
        private const int AnchoCodigoTabla = 104;
        private const int AnchoTotalTabla = 108;
        private const int AnchoDeudaTabla = 136;
        private const int MargenTabla = 8;
        private const int AnchoMinimoTabla = 570;

        private readonly TextBox txtBuscar;
        private readonly Label lblMensaje;
        private readonly Panel tablaContenedor;
        private readonly TableLayoutPanel tablaBoletas;
        private readonly Dictionary<string, PanelRedondeado> filasPorCodigo = new();
        private readonly Label lblDetalleCodigo;
        private readonly Label lblDetalleCliente;
        private readonly Insignia insigniaEstado;
        private readonly Label lblTotal;
        private readonly Label lblDeuda;
        private readonly NumericUpDown nudAbono;
        private readonly BotonRedondeado btnAbono;
        private readonly Label lblUltimoAbono;
        private readonly Label lblAbonosPila;

        private Boleta? seleccionada;
        private bool buscarEsPlaceholder = true;

        public UcFiado()
        {
            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;

            txtBuscar = new TextBox
            {
                Text = PlaceholderBuscar,
                ForeColor = Tema.TextoSecundario,
                Font = Tema.Normal,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 4, 10, 4)
            };
            txtBuscar.Enter += (s, e) =>
            {
                if (!buscarEsPlaceholder)
                    return;

                txtBuscar.Text = "";
                txtBuscar.ForeColor = Tema.TextoPrincipal;
                buscarEsPlaceholder = false;
            };
            txtBuscar.Leave += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                    return;

                txtBuscar.Text = PlaceholderBuscar;
                txtBuscar.ForeColor = Tema.TextoSecundario;
                buscarEsPlaceholder = true;
            };
            txtBuscar.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Enter)
                    return;

                BuscarBoleta();
                e.SuppressKeyPress = true;
            };

            lblMensaje = new Label
            {
                Font = Tema.Pequena,
                ForeColor = Tema.Error,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty,
                Visible = false
            };

            tablaBoletas = new TableLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            tablaBoletas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            tablaContenedor = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Tema.Blanco,
                Margin = Padding.Empty,
                Padding = new Padding(0, 0, 4, 0)
            };
            tablaContenedor.Controls.Add(tablaBoletas);
            tablaContenedor.Resize += (s, e) => AjustarAnchoTabla();

            lblDetalleCodigo = new Label
            {
                Text = "—",
                Font = new Font(Tema.Codigo.FontFamily, 14f, FontStyle.Bold),
                ForeColor = Tema.PrimarioOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };

            lblDetalleCliente = new Label
            {
                Text = "Selecciona una boleta para ver el detalle.",
                Font = Tema.Normal,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };

            insigniaEstado = new Insignia { Anchor = AnchorStyles.Top | AnchorStyles.Right, Margin = Padding.Empty };
            insigniaEstado.Personalizar("—", Tema.Deshabilitado, Tema.TextoSecundario);

            lblTotal = new Label
            {
                Text = "—",
                Font = new Font(Tema.Negrita.FontFamily, 16f, FontStyle.Bold),
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = Padding.Empty
            };

            lblDeuda = new Label
            {
                Text = "—",
                Font = new Font(Tema.Negrita.FontFamily, 16f, FontStyle.Bold),
                ForeColor = Tema.AmbarOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = Padding.Empty
            };

            nudAbono = new NumericUpDown
            {
                DecimalPlaces = 2,
                ThousandsSeparator = true,
                Minimum = 0,
                Maximum = 0,
                Increment = 1,
                Value = 0,
                Font = Tema.Normal,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty
            };

            btnAbono = new BotonRedondeado
            {
                Text = "Registrar abono",
                Estilo = EstiloBoton.Primario,
                Dock = DockStyle.Fill,
                Enabled = false,
                Margin = Padding.Empty
            };
            btnAbono.Click += BtnAbono_Click;
            nudAbono.ValueChanged += (s, e) => ActualizarBotonAbono();

            lblUltimoAbono = new Label
            {
                Text = "Sin abonos registrados.",
                Font = Tema.Normal,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };

            lblAbonosPila = new Label
            {
                Text = "0",
                Font = Tema.NumeroGrande,
                ForeColor = Tema.PrimarioOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = Padding.Empty
            };

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
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
            raiz.Controls.Add(CrearBuscador(), 0, 0);
            raiz.Controls.Add(CrearContenido(), 0, 1);
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

        private Control CrearBuscador()
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
                RowCount = 5,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));

            filas.Controls.Add(new Label
            {
                Text = "Boletas fiadas",
                Font = Tema.Titulo,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            filas.Controls.Add(new Label
            {
                Text = "Selecciona una boleta para revisar su saldo y registrar un abono.",
                Font = Tema.Subtitulo,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 1);

            filas.Controls.Add(new Label
            {
                Text = "Buscar boleta por código",
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 2);

            var buscar = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            buscar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            buscar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            buscar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var btnBuscar = new BotonRedondeado
            {
                Text = "Buscar",
                Estilo = EstiloBoton.Primario,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 4, 0, 4)
            };
            btnBuscar.Click += (s, e) => BuscarBoleta();

            buscar.Controls.Add(txtBuscar, 0, 0);
            buscar.Controls.Add(btnBuscar, 1, 0);
            filas.Controls.Add(buscar, 0, 3);
            filas.Controls.Add(lblMensaje, 0, 4);
            tarjeta.Controls.Add(filas);
            return tarjeta;
        }

        private Control CrearContenido()
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
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var lista = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = new Padding(0, 0, 14, 0),
                Padding = new Padding(12, 12, 12, 12)
            };
            lista.Controls.Add(tablaContenedor);

            columnas.Controls.Add(lista, 0, 0);
            columnas.Controls.Add(CrearDetalle(), 1, 0);
            return columnas;
        }

        private Control CrearDetalle()
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = Padding.Empty,
                Padding = new Padding(18, 16, 18, 16)
            };

            var filas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            filas.Controls.Add(lblDetalleCodigo, 0, 0);

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
            cabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            cabecera.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            cabecera.Controls.Add(lblDetalleCliente, 0, 0);
            cabecera.Controls.Add(insigniaEstado, 1, 0);
            filas.Controls.Add(cabecera, 0, 1);
            filas.Controls.Add(CrearMontos(), 0, 2);

            filas.Controls.Add(new Label
            {
                Text = "Monto del abono (S/)",
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 3);
            filas.Controls.Add(nudAbono, 0, 4);
            filas.Controls.Add(btnAbono, 0, 5);
            filas.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Tema.Blanco, Margin = Padding.Empty }, 0, 6);

            tarjeta.Controls.Add(filas);
            return tarjeta;
        }

        private Control CrearMontos()
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.EncabezadoTabla,
                MostrarBorde = false,
                Margin = Padding.Empty,
                Padding = new Padding(14, 10, 14, 10)
            };

            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.EncabezadoTabla
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            columnas.Controls.Add(CrearBloqueMonto("Total original", lblTotal), 0, 0);
            columnas.Controls.Add(CrearBloqueMonto("Deuda pendiente", lblDeuda), 1, 0);
            tarjeta.Controls.Add(columnas);
            return tarjeta;
        }

        private static Control CrearBloqueMonto(string titulo, Label valor)
        {
            var bloque = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = new Padding(0, 6, 0, 6),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.EncabezadoTabla
            };
            bloque.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            bloque.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            bloque.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            bloque.Controls.Add(new Label
            {
                Text = titulo,
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);
            bloque.Controls.Add(valor, 0, 1);
            return bloque;
        }

        private Control CrearResumen()
        {
            var filas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            filas.Controls.Add(CrearTarjetaResumen("Último abono apilado", lblUltimoAbono, ContentAlignment.MiddleLeft), 0, 0);
            filas.Controls.Add(CrearTarjetaResumen("Abonos en pila", lblAbonosPila, ContentAlignment.MiddleCenter), 1, 0);
            return filas;
        }

        private static Control CrearTarjetaResumen(string titulo, Label valor, ContentAlignment alineacion)
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = new Padding(0, 0, 14, 0),
                Padding = new Padding(16, 12, 16, 12)
            };

            var contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            contenido.Controls.Add(new Label
            {
                Text = titulo,
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            valor.TextAlign = alineacion;
            contenido.Controls.Add(valor, 0, 1);
            tarjeta.Controls.Add(contenido);
            return tarjeta;
        }

        private void Refrescar()
        {
            List<Boleta> fiadas = FiadoServicio.ListarFiadas();
            bool conservar = false;

            foreach (Boleta boleta in fiadas)
            {
                if (ReferenceEquals(boleta, seleccionada))
                {
                    conservar = true;
                    break;
                }
            }

            if (!conservar)
                seleccionada = null;

            filasPorCodigo.Clear();
            tablaBoletas.SuspendLayout();

            while (tablaBoletas.Controls.Count > 0)
                tablaBoletas.Controls[0].Dispose();

            tablaBoletas.RowStyles.Clear();
            tablaBoletas.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tablaBoletas.Controls.Add(CrearEncabezadoTabla(), 0, 0);

            if (fiadas.Count == 0)
            {
                tablaBoletas.RowCount = 2;
                tablaBoletas.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
                tablaBoletas.Controls.Add(new Label
                {
                    Text = "No hay boletas con deuda pendiente.",
                    Font = Tema.Normal,
                    ForeColor = Tema.TextoSecundario,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = Padding.Empty
                }, 0, 1);
            }
            else
            {
                tablaBoletas.RowCount = fiadas.Count + 1;

                for (int i = 0; i < fiadas.Count; i++)
                {
                    tablaBoletas.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
                    tablaBoletas.Controls.Add(CrearFilaBoleta(fiadas[i]), 0, i + 1);
                }
            }

            tablaBoletas.ResumeLayout();
            AjustarAnchoTabla();
            PintarSeleccion();
            ActualizarDetalle();
            ActualizarResumen();
        }

        private void AjustarAnchoTabla()
        {
            int disponible = tablaContenedor.ClientSize.Width
                - tablaContenedor.Padding.Left
                - tablaContenedor.Padding.Right
                - SystemInformation.VerticalScrollBarWidth - 2;
            int ancho = Math.Max(AnchoMinimoTabla, disponible);

            tablaBoletas.SuspendLayout();
            tablaBoletas.MinimumSize = new Size(ancho, 0);
            tablaBoletas.MaximumSize = new Size(ancho, 0);
            tablaBoletas.Width = ancho;
            tablaBoletas.ResumeLayout();
            tablaContenedor.AutoScrollMinSize = new Size(ancho, 0);
        }

        private static void ConfigurarColumnasTabla(TableLayoutPanel columnas)
        {
            columnas.ColumnStyles.Clear();
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AnchoCodigoTabla));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AnchoTotalTabla));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AnchoDeudaTabla));
        }

        private static Control CrearEncabezadoTabla()
        {
            var fila = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = new Padding(MargenTabla, 0, MargenTabla, 0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.EncabezadoTabla
            };
            ConfigurarColumnasTabla(fila);
            fila.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            fila.Controls.Add(CrearCelda("Código", ContentAlignment.MiddleLeft), 0, 0);
            fila.Controls.Add(CrearCelda("Cliente", ContentAlignment.MiddleLeft), 1, 0);
            fila.Controls.Add(CrearCelda("Total", ContentAlignment.MiddleRight), 2, 0);
            fila.Controls.Add(CrearCelda("Deuda pendiente", ContentAlignment.MiddleRight), 3, 0);
            return fila;
        }

        private static Label CrearCelda(string texto, ContentAlignment alineacion)
        {
            return new Label
            {
                Text = texto,
                AutoSize = false,
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = alineacion,
                Margin = Padding.Empty
            };
        }

        private PanelRedondeado CrearFilaBoleta(Boleta boleta)
        {
            var fila = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                ColorBorde = Tema.Borde,
                MostrarBorde = true,
                Interactivo = true,
                Margin = new Padding(0, 0, 0, 4),
                Padding = Padding.Empty
            };

            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = new Padding(MargenTabla, 0, MargenTabla, 0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            ConfigurarColumnasTabla(columnas);
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            columnas.Controls.Add(new Label
            {
                Text = boleta.Codigo,
                AutoSize = false,
                Font = Tema.Codigo,
                ForeColor = Tema.PrimarioOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);
            columnas.Controls.Add(new Label
            {
                Text = boleta.Cliente,
                AutoSize = false,
                Font = Tema.Normal,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 1, 0);
            columnas.Controls.Add(new Label
            {
                Text = Formato.Moneda(boleta.Total),
                AutoSize = false,
                Font = Tema.Normal,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = Padding.Empty
            }, 2, 0);
            columnas.Controls.Add(new Label
            {
                Text = Formato.Moneda(boleta.DeudaPendiente),
                AutoSize = false,
                Font = Tema.Negrita,
                ForeColor = Tema.AmbarOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = Padding.Empty
            }, 3, 0);

            fila.Controls.Add(columnas);
            fila.ConectarHijos();
            fila.Click += (s, e) => Seleccionar(boleta);
            filasPorCodigo[boleta.Codigo] = fila;
            return fila;
        }

        private void PintarSeleccion()
        {
            foreach (KeyValuePair<string, PanelRedondeado> par in filasPorCodigo)
            {
                bool activa = seleccionada != null && par.Key == seleccionada.Codigo;
                par.Value.BackColor = activa ? Tema.PrimarioClaro : Tema.Blanco;
                par.Value.ColorBorde = activa ? Tema.Primario : Tema.Borde;
            }
        }

        private void Seleccionar(Boleta boleta)
        {
            seleccionada = boleta;
            PintarSeleccion();
            ActualizarDetalle();
            lblMensaje.Visible = false;
        }

        private void ActualizarDetalle()
        {
            if (seleccionada == null)
            {
                lblDetalleCodigo.Text = "—";
                lblDetalleCliente.Text = "Selecciona una boleta para ver el detalle.";
                insigniaEstado.Personalizar("—", Tema.Deshabilitado, Tema.TextoSecundario);
                lblTotal.Text = "—";
                lblDeuda.Text = "—";
                nudAbono.Maximum = 0;
                nudAbono.Value = 0;
                ActualizarBotonAbono();
                return;
            }

            lblDetalleCodigo.Text = seleccionada.Codigo;
            lblDetalleCliente.Text = seleccionada.Cliente;
            insigniaEstado.Personalizar("Fiada", Tema.AmbarClaro, Tema.AmbarOscuro);
            lblTotal.Text = Formato.Moneda(seleccionada.Total);
            lblDeuda.Text = Formato.Moneda(seleccionada.DeudaPendiente);
            nudAbono.Maximum = seleccionada.DeudaPendiente;
            nudAbono.Value = Math.Min(nudAbono.Value, seleccionada.DeudaPendiente);
            ActualizarBotonAbono();
        }

        private void ActualizarResumen()
        {
            Abono? ultimo = FiadoServicio.UltimoAbono();
            lblUltimoAbono.Text = ultimo == null
                ? "Sin abonos registrados."
                : $"{ultimo.CodigoBoleta} · {ultimo.Cliente} · {Formato.Moneda(ultimo.Monto)} ({Formato.HaceCuanto(ultimo.Fecha)})";
            lblUltimoAbono.ForeColor = ultimo == null ? Tema.TextoSecundario : Tema.PrimarioOscuro;
            lblAbonosPila.Text = FiadoServicio.AbonosEnPila.ToString();
        }

        private void BuscarBoleta()
        {
            string texto = txtBuscar.Text.Trim() == PlaceholderBuscar ? "" : txtBuscar.Text;
            string error = Validaciones.CodigoBoleta(texto, out string codigo);

            if (error != null)
            {
                MostrarMensaje(error, false);
                return;
            }

            Boleta boleta = FiadoServicio.Buscar(codigo);

            if (boleta == null)
            {
                MostrarMensaje("No existe una boleta con ese código.", false);
                return;
            }

            if (boleta.Estado != EstadoBoleta.Fiada)
            {
                MostrarMensaje("Esa boleta no tiene deuda pendiente.", false);
                return;
            }

            Seleccionar(boleta);
            MostrarMensaje($"Boleta {boleta.Codigo} seleccionada.", true);
        }

        private void BtnAbono_Click(object? sender, EventArgs e)
        {
            if (seleccionada == null || !btnAbono.Enabled)
                return;

            string error = Validaciones.Abono(Formato.Moneda(nudAbono.Value), seleccionada.DeudaPendiente, out decimal monto);

            if (error != null)
            {
                MostrarMensaje(error, false);
                return;
            }

            string codigo = seleccionada.Codigo;

            try
            {
                FiadoServicio.RegistrarAbono(codigo, monto);
                MostrarMensaje($"Abono de {Formato.Moneda(monto)} registrado en {codigo}.", true);
                Refrescar();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                MostrarMensaje(ex.Message, false);
            }
        }

        private void ActualizarBotonAbono()
        {
            btnAbono.Enabled = seleccionada != null
                               && seleccionada.Estado == EstadoBoleta.Fiada
                               && nudAbono.Value > 0
                               && nudAbono.Value <= seleccionada.DeudaPendiente;
        }

        private void MostrarMensaje(string texto, bool exito)
        {
            lblMensaje.Text = texto;
            lblMensaje.ForeColor = exito ? Tema.PrimarioOscuro : Tema.Error;
            lblMensaje.Visible = !string.IsNullOrWhiteSpace(texto);
        }
    }
}
