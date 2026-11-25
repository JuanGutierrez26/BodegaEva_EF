using BodegaEva_EF.Controles;
using BodegaEva_EF.Modelos;
using BodegaEva_EF.Servicios;
using BodegaEva_EF.Utilidades;
using System.Drawing.Drawing2D;

namespace BodegaEva_EF.Vistas.Venta
{
    internal class PasoPago : UserControl
    {
        private const float AltoBannerError = 100F;
        private const float AltoBannerFiado = 88F;
        private const int FilaAlerta = 3;
        private const int FilaEspacioAlerta = 4;
        private const int FilaPanelPagos = 5;
        private const int FilaEspacioPanel = 6;

        private readonly PasoProductos origen;

        private bool fiado;
        private bool bannerErrorActivo;
        private Pago? pagoRechazado;

        private TableLayoutPanel raiz = null!;
        private IndicadorPasos indicador = null!;
        private PanelRedondeado opcionNo = null!;
        private PanelRedondeado opcionSi = null!;
        private Label etiquetaNo = null!;
        private Label etiquetaSi = null!;
        private Label lblResumen = null!;
        private Insignia insigniaPagos = null!;
        private TextBox txtCodigo = null!;
        private TextBox txtMonto = null!;
        private Label lblErrorCampos = null!;
        private Label lblEstadoPago = null!;
        private FlowLayoutPanel listaPagos = null!;
        private Control panelPagos = null!;
        private PanelRedondeado bannerError = null!;
        private PanelRedondeado bannerFiado = null!;
        private Label lblTituloError = null!;
        private Label lblDetalleError = null!;
        private BotonRedondeado btnRetirar = null!;
        private PanelRedondeado btnVolver = null!;

        public event Action<int>? NavegacionSolicitada;

        public bool Fiado => fiado;

        public PasoPago(PasoProductos origen)
        {
            this.origen = origen;

            Dock = DockStyle.Fill;
            BackColor = Tema.Fondo;

            ConstruirInterfaz();
            ActualizarResumen();
            AplicarModalidad();
            ActualizarContador();
            ActualizarListaPagos();
            ActualizarBotonRetirar();

            Datos.Cambio += Datos_Cambio;
        }

        private void ConstruirInterfaz()
        {
            raiz = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 8,
                Margin = Padding.Empty,
                Padding = new Padding(24, 20, 24, 24),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 0F));
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 0F));
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
            indicador.PasoActual = 3;
            indicador.PasoSeleccionado += Indicador_PasoSeleccionado;
            raiz.Controls.Add(indicador, 0, 0);

            raiz.Controls.Add(CrearEspacio(), 0, 1);
            raiz.Controls.Add(CrearTarjetaFiado(), 0, 2);
            raiz.Controls.Add(CrearContenedorAlertas(), 0, FilaAlerta);
            raiz.Controls.Add(CrearEspacio(), 0, FilaEspacioAlerta);
            panelPagos = CrearPanelPagos();
            raiz.Controls.Add(panelPagos, 0, FilaPanelPagos);
            raiz.Controls.Add(CrearEspacio(), 0, FilaEspacioPanel);
            raiz.Controls.Add(CrearAcciones(), 0, 7);

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

        private Control CrearTarjetaFiado()
        {
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = Padding.Empty,
                Padding = new Padding(16, 12, 16, 12)
            };

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
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
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
            texto.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            texto.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            texto.Controls.Add(new Label
            {
                Text = "¿Se va a fiar?",
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Margin = Padding.Empty
            }, 0, 0);
            texto.Controls.Add(new Label
            {
                Text = "Si el cliente paga después, marca Sí para registrar la boleta como fiado.",
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Margin = Padding.Empty
            }, 0, 1);

            var selector = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            selector.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            selector.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            selector.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            opcionNo = CrearOpcion("No", out etiquetaNo);
            opcionNo.Margin = new Padding(0, 8, 5, 8);
            opcionNo.Click += (s, e) => SeleccionarFiado(false);

            opcionSi = CrearOpcion("Sí", out etiquetaSi);
            opcionSi.Margin = new Padding(5, 8, 0, 8);
            opcionSi.Click += (s, e) => SeleccionarFiado(true);

            selector.Controls.Add(opcionNo, 0, 0);
            selector.Controls.Add(opcionSi, 1, 0);

            columnas.Controls.Add(texto, 0, 0);
            columnas.Controls.Add(selector, 1, 0);
            tarjeta.Controls.Add(columnas);
            return tarjeta;
        }

        private static PanelRedondeado CrearOpcion(string texto, out Label etiqueta)
        {
            var opcion = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.EncabezadoTabla,
                ColorBorde = Tema.Borde,
                MostrarBorde = true,
                Interactivo = true,
                Padding = Padding.Empty
            };

            etiqueta = new Label
            {
                Text = texto,
                Font = Tema.Negrita,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = Padding.Empty
            };

            opcion.Controls.Add(etiqueta);
            opcion.ConectarHijos();
            return opcion;
        }

        private void SeleccionarFiado(bool valor)
        {
            if (valor && !fiado)
            {
                bannerErrorActivo = false;
                pagoRechazado = null;
            }

            fiado = valor;
            AplicarModalidad();
            ActualizarBotonRetirar();
        }

        private void AplicarModalidad()
        {
            ActualizarSeleccion();

            bannerFiado.Visible = fiado;
            bannerError.Visible = !fiado && bannerErrorActivo;
            panelPagos.Visible = !fiado;

            raiz.RowStyles[FilaAlerta].Height = fiado
                ? AltoBannerFiado
                : bannerErrorActivo ? AltoBannerError : 0F;
            raiz.RowStyles[FilaEspacioAlerta].Height = fiado || bannerErrorActivo ? 10F : 0F;
            raiz.RowStyles[FilaPanelPagos].SizeType = fiado ? SizeType.Absolute : SizeType.Percent;
            raiz.RowStyles[FilaPanelPagos].Height = fiado ? 0F : 100F;
            raiz.RowStyles[FilaEspacioPanel].Height = fiado ? 0F : 10F;
        }

        private void ActualizarSeleccion()
        {
            PintarOpcion(opcionNo, etiquetaNo, !fiado, Tema.PrimarioOscuro);
            PintarOpcion(opcionSi, etiquetaSi, fiado, Tema.Ambar);
        }

        private static void PintarOpcion(PanelRedondeado opcion, Label etiqueta, bool seleccionada, Color colorSeleccion)
        {
            opcion.BackColor = seleccionada ? colorSeleccion : Tema.EncabezadoTabla;
            opcion.ColorBorde = seleccionada ? colorSeleccion : Tema.Borde;
            etiqueta.ForeColor = seleccionada ? Tema.Blanco : Tema.TextoSecundario;
            opcion.Invalidate();
        }

        private Control CrearPanelPagos()
        {
            var panel = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = Padding.Empty,
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
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            filas.Controls.Add(CrearEncabezadoPagos(), 0, 0);
            filas.Controls.Add(CrearContenidoPagos(), 0, 1);
            panel.Controls.Add(filas);
            return panel;
        }

        private Control CrearEncabezadoPagos()
        {
            var fila = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            fila.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            fila.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            fila.Controls.Add(new Label
            {
                Text = "Cola de pagos",
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            insigniaPagos = new Insignia
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Margin = new Padding(0, 6, 4, 0)
            };
            insigniaPagos.Personalizar("0 pagos en cola", Tema.EncabezadoTabla, Tema.TextoSecundario);
            fila.Controls.Add(insigniaPagos, 1, 0);
            return fila;
        }

        private Control CrearContenidoPagos()
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
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            columnas.Controls.Add(CrearFormularioPago(), 0, 0);
            columnas.Controls.Add(CrearListaPagos(), 1, 0);
            return columnas;
        }

        private Control CrearFormularioPago()
        {
            var filas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                Margin = new Padding(0, 0, 14, 0),
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108F));
            filas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblResumen = new Label
            {
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            filas.Controls.Add(lblResumen, 0, 0);
            filas.SetColumnSpan(lblResumen, 2);

            txtCodigo = new TextBox
            {
                Text = "",
                Font = Tema.Normal,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 4, 0, 4)
            };
            txtMonto = new TextBox
            {
                Text = "0.00",
                Font = Tema.Normal,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 4, 0, 4)
            };

            AgregarCampo(filas, 1, "Código de pago", txtCodigo);
            AgregarCampo(filas, 2, "Monto (S/)", txtMonto);

            lblErrorCampos = new Label
            {
                Font = Tema.Pequena,
                ForeColor = Tema.Error,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty,
                Visible = false
            };
            filas.Controls.Add(lblErrorCampos, 0, 3);
            filas.SetColumnSpan(lblErrorCampos, 2);

            var btnEncolar = new BotonRedondeado
            {
                Text = "Encolar pago",
                Estilo = EstiloBoton.Secundario,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty
            };
            btnEncolar.Click += BtnEncolar_Click;
            filas.Controls.Add(btnEncolar, 0, 4);
            filas.SetColumnSpan(btnEncolar, 2);

            var btnValidar = new BotonRedondeado
            {
                Text = "Validar siguiente pago",
                Estilo = EstiloBoton.Primario,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 6, 0, 6)
            };
            btnValidar.Click += BtnValidar_Click;
            filas.Controls.Add(btnValidar, 0, 5);
            filas.SetColumnSpan(btnValidar, 2);

            lblEstadoPago = new Label
            {
                Text = "",
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Margin = Padding.Empty
            };
            filas.Controls.Add(lblEstadoPago, 0, 6);
            filas.SetColumnSpan(lblEstadoPago, 2);

            return filas;
        }

        private static void AgregarCampo(TableLayoutPanel contenedor, int fila, string etiqueta, Control campo)
        {
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

        private Control CrearListaPagos()
        {
            var panel = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                ColorBorde = Tema.Borde,
                MostrarBorde = true,
                Margin = Padding.Empty,
                Padding = new Padding(10, 8, 10, 8)
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
            filas.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            filas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            filas.Controls.Add(new Label
            {
                Text = "Pagos en espera",
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            listaPagos = new FlowLayoutPanel
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
            listaPagos.Resize += (s, e) => AjustarFilasPagos(listaPagos);
            filas.Controls.Add(listaPagos, 0, 1);

            panel.Controls.Add(filas);
            return panel;
        }

        private Control CrearContenedorAlertas()
        {
            var contenedor = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Fondo,
                Margin = Padding.Empty
            };
            contenedor.Controls.Add(CrearBannerError());
            contenedor.Controls.Add(CrearBannerFiado());
            return contenedor;
        }

        private Control CrearBannerFiado()
        {
            bannerFiado = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.AmbarClaro,
                ColorBorde = Tema.Ambar,
                MostrarBorde = true,
                Visible = false,
                Margin = Padding.Empty,
                Padding = new Padding(12, 8, 12, 8)
            };

            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.AmbarClaro
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            columnas.Controls.Add(new IconoBoleta
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 0, 8)
            }, 0, 0);

            columnas.Controls.Add(new Label
            {
                Text = "Se omitirá el cobro inmediato y se registrará en Fiado.",
                Font = Tema.Normal,
                ForeColor = Tema.AmbarOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 1, 0);

            var insignia = new Insignia
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Margin = new Padding(0, 6, 4, 0)
            };
            insignia.Personalizar("Boleta marcada como fiado", Tema.Ambar, Tema.Blanco);
            columnas.Controls.Add(insignia, 2, 0);

            bannerFiado.Controls.Add(columnas);
            return bannerFiado;
        }

        private Control CrearBannerError()
        {
            bannerError = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.ErrorClaro,
                ColorBorde = Tema.Error,
                MostrarBorde = true,
                Visible = false,
                Margin = Padding.Empty,
                Padding = new Padding(12, 8, 12, 8)
            };

            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.ErrorClaro
            };
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 310F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            columnas.Controls.Add(new IconoExclamacion
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 8, 0, 8)
            }, 0, 0);

            var texto = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.ErrorClaro
            };
            texto.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            texto.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            texto.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblTituloError = new Label
            {
                Text = "Error al validar el pago",
                Font = Tema.Negrita,
                ForeColor = Tema.Error,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Margin = Padding.Empty
            };
            lblDetalleError = new Label
            {
                Text = "",
                Font = Tema.Pequena,
                ForeColor = Tema.Error,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                Margin = Padding.Empty
            };
            texto.Controls.Add(lblTituloError, 0, 0);
            texto.Controls.Add(lblDetalleError, 0, 1);
            columnas.Controls.Add(texto, 1, 0);

            var botones = new TableLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(8, 12, 0, 12),
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.ErrorClaro
            };
            botones.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            botones.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            botones.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            btnRetirar = new BotonRedondeado
            {
                Text = "Retirar pago",
                Estilo = EstiloBoton.Peligro,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty
            };
            btnRetirar.Click += BtnRetirar_Click;

            btnVolver = CrearBotonVolver();

            botones.Controls.Add(btnRetirar, 0, 0);
            botones.Controls.Add(btnVolver, 1, 0);
            columnas.Controls.Add(botones, 2, 0);

            bannerError.Controls.Add(columnas);
            return bannerError;
        }

        private PanelRedondeado CrearBotonVolver()
        {
            var boton = new PanelRedondeado
            {
                AutoSize = false,
                Size = new Size(150, 40),
                BackColor = Tema.Blanco,
                ColorBorde = Tema.Error,
                MostrarBorde = true,
                Interactivo = true,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            boton.Controls.Add(new Label
            {
                Text = "Volver a pagar",
                Font = Tema.Negrita,
                ForeColor = Tema.Error,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = Padding.Empty
            });
            boton.ConectarHijos();
            boton.Click += (s, e) => VolverAPagar();
            return boton;
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

            var btnContinuar = new BotonRedondeado
            {
                Text = "Continuar →",
                Estilo = EstiloBoton.Primario,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty
            };
            btnContinuar.Click += BtnContinuar_Click;

            columnas.Controls.Add(CrearEspacio(), 0, 0);
            columnas.Controls.Add(btnContinuar, 1, 0);
            return columnas;
        }

        private void BtnEncolar_Click(object? sender, EventArgs e)
        {
            if (fiado)
                return;

            Validaciones.Mostrar(lblErrorCampos, null);
            lblEstadoPago.Text = "";

            decimal monto = 0;
            string error = Validaciones.CodigoPago(txtCodigo.Text, out string codigo)
                           ?? Validaciones.MontoPago(txtMonto.Text, out monto);
            if (error != null)
            {
                Validaciones.Mostrar(lblErrorCampos, error);
                return;
            }

            try
            {
                VentaServicio.EncolarPago(codigo, monto);
                txtCodigo.Clear();
                txtMonto.Text = "0.00";
                OcultarError();
                lblEstadoPago.Text = $"Pago {codigo} encolado por {Formato.Moneda(monto)}.";
                lblEstadoPago.ForeColor = Tema.TextoSecundario;
                ActualizarContador();
                ActualizarListaPagos();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                Validaciones.Mostrar(lblErrorCampos, ex.Message);
            }
        }

        private void BtnValidar_Click(object? sender, EventArgs e)
        {
            if (fiado)
                return;

            Validaciones.Mostrar(lblErrorCampos, null);
            lblEstadoPago.Text = "";
            Pago? actual = Datos.ColaPagos.Frente();

            try
            {
                bool valido = VentaServicio.ValidarSiguientePago(false, out string motivo);
                if (valido)
                {
                    OcultarError();
                    lblEstadoPago.Text = "Pago validado. Ya puedes continuar con la venta.";
                    lblEstadoPago.ForeColor = Tema.PrimarioOscuro;
                    return;
                }

                MostrarError(actual, motivo);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                Validaciones.Mostrar(lblErrorCampos, ex.Message);
            }
        }

        private void BtnRetirar_Click(object? sender, EventArgs e)
        {
            if (pagoRechazado == null || !Datos.ColaPagos.Contiene(pagoRechazado.Codigo))
            {
                OcultarError();
                return;
            }

            string codigo = pagoRechazado.Codigo;
            Datos.ColaPagos.Retirar(codigo);
            Datos.NotificarCambio();
            OcultarError();
            lblEstadoPago.Text = $"Pago {codigo} retirado de la cola.";
            lblEstadoPago.ForeColor = Tema.TextoSecundario;
        }

        private void VolverAPagar()
        {
            OcultarError();
            txtCodigo.Clear();
            txtMonto.Text = "0.00";
            lblEstadoPago.Text = "Ingresa un nuevo código y monto.";
            lblEstadoPago.ForeColor = Tema.TextoSecundario;
            txtCodigo.Focus();
        }

        private void BtnContinuar_Click(object? sender, EventArgs e)
        {
            if (!fiado && !VentaServicio.PagoValidado)
            {
                lblEstadoPago.Text = "Valida un pago antes de continuar o marca la venta como fiada.";
                lblEstadoPago.ForeColor = Tema.Error;
                return;
            }

            NavegacionSolicitada?.Invoke(4);
        }

        private void Indicador_PasoSeleccionado(int paso)
        {
            if (paso == 3)
                return;

            NavegacionSolicitada?.Invoke(paso);
        }

        private void MostrarError(Pago? pago, string motivo)
        {
            pagoRechazado = pago;
            lblTituloError.Text = pago == null
                ? "Error al validar el pago"
                : $"Error al validar el pago {pago.Codigo}";
            lblDetalleError.Text = motivo;

            bannerErrorActivo = true;
            AplicarModalidad();
            ActualizarBotonRetirar();
        }

        private void OcultarError()
        {
            bannerErrorActivo = false;
            AplicarModalidad();
            pagoRechazado = null;
            ActualizarBotonRetirar();
        }

        private void ActualizarBotonRetirar()
        {
            btnRetirar.Enabled = pagoRechazado != null && Datos.ColaPagos.Contiene(pagoRechazado.Codigo);
        }

        private void ActualizarResumen()
        {
            Pedido? pedido = VentaServicio.PedidoEnCurso;
            lblResumen.Text = pedido == null
                ? $"{origen.Cliente} · sin pedido en curso"
                : $"{pedido.Cliente} · {Formato.Moneda(pedido.Total)}";
        }

        private void ActualizarContador()
        {
            int cantidad = Datos.ColaPagos.Contar;
            string texto = $"{cantidad} {(cantidad == 1 ? "pago" : "pagos")} en cola";
            insigniaPagos.Personalizar(texto, Tema.EncabezadoTabla, Tema.TextoSecundario);
        }

        private void ActualizarListaPagos()
        {
            listaPagos.SuspendLayout();

            while (listaPagos.Controls.Count > 0)
                listaPagos.Controls[0].Dispose();

            foreach (Pago pago in Datos.ColaPagos.Recorrer())
                listaPagos.Controls.Add(CrearFilaPago(pago));

            if (Datos.ColaPagos.EstaVacia)
                listaPagos.Controls.Add(CrearFilaVacia());

            AjustarFilasPagos(listaPagos);
            listaPagos.ResumeLayout();
        }

        private static void AjustarFilasPagos(FlowLayoutPanel lista)
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
                Text = "Sin pagos en espera.",
                Font = Tema.Pequena,
                ForeColor = Tema.TextoSecundario,
                AutoSize = false,
                Height = 34,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 6)
            };
        }

        private static Control CrearFilaPago(Pago pago)
        {
            var fila = new PanelRedondeado
            {
                AutoSize = false,
                Height = 80,
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
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            columnas.Controls.Add(new Label
            {
                Text = Formato.NormalizarCodigo(pago.Codigo),
                Font = Tema.Negrita,
                ForeColor = Tema.TextoPrincipal,
                AutoEllipsis = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);
            columnas.Controls.Add(new Label
            {
                Text = Formato.Moneda(pago.Monto),
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

        private void Datos_Cambio()
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke((Action)RefrescarDesdeEstado);
                return;
            }

            RefrescarDesdeEstado();
        }

        private void RefrescarDesdeEstado()
        {
            ActualizarResumen();
            AplicarModalidad();
            ActualizarContador();
            ActualizarListaPagos();
            ActualizarBotonRetirar();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible && !IsDisposed)
                RefrescarDesdeEstado();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Datos.Cambio -= Datos_Cambio;

            base.Dispose(disposing);
        }

        private sealed class IconoBoleta : Control
        {
            public IconoBoleta()
            {
                SetStyle(ControlStyles.UserPaint |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer, true);
                BackColor = Tema.AmbarClaro;
                Size = new Size(24, 28);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                int ancho = Math.Min(24, Math.Max(12, Width - 8));
                int alto = Math.Min(32, Math.Max(16, Height - 8));
                var area = new Rectangle((Width - ancho) / 2, (Height - alto) / 2, ancho, alto);

                using (var relleno = new SolidBrush(Tema.Blanco))
                using (var lapiz = new Pen(Tema.AmbarOscuro, 1.5F) { LineJoin = LineJoin.Round })
                {
                    using (GraphicsPath ruta = Tema.RutaRedondeada(area, 2))
                    {
                        g.FillPath(relleno, ruta);
                        g.DrawPath(lapiz, ruta);
                    }

                    g.DrawLine(lapiz, area.Left + 5, area.Top + 7, area.Right - 5, area.Top + 7);
                    g.DrawLine(lapiz, area.Left + 5, area.Top + 12, area.Right - 7, area.Top + 12);
                    g.DrawLine(lapiz, area.Left + 5, area.Top + 17, area.Right - 5, area.Top + 17);
                }
            }
        }

        private sealed class IconoExclamacion : Control
        {
            public IconoExclamacion()
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

                var circulo = new Rectangle(1, 1, Width - 2, Height - 2);
                using (var relleno = new SolidBrush(Tema.Error))
                    g.FillEllipse(relleno, circulo);

                TextRenderer.DrawText(g, "!", Tema.Etiqueta, circulo, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            }
        }
    }
}
