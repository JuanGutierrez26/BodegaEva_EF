using BodegaEva_EF.Controles;
using BodegaEva_EF.Modelos;
using BodegaEva_EF.Servicios;
using BodegaEva_EF.Utilidades;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BodegaEva_EF.Vistas
{
    internal class UcHistorial : UserControl
    {
        private const string PlaceholderBuscar = "Ej. BOL-0045";
        private const int AnchoCodigo = 104;
        private const int AnchoTotal = 110;
        private const int AnchoEstado = 96;
        private const int MargenTabla = 8;
        private const int AnchoMinimoTabla = 760;
        private const int AltoEncabezado = 30;
        private const int AltoFila = 48;

        private readonly TextBox txtBuscar;
        private readonly Label lblMensaje;
        private readonly Panel tablaContenedor;
        private readonly TableLayoutPanel tablaBoletas;
        private readonly Label lblTotalBoletas;

        private string? filtroCodigo;
        private bool buscarEsPlaceholder = true;

        public UcHistorial()
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
                RowCount = 1,
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

            lblTotalBoletas = new Label
            {
                Text = "0",
                Font = Tema.NumeroGrande,
                ForeColor = Tema.PrimarioOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
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
            raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
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
                Text = "Todas las boletas",
                Font = Tema.Titulo,
                ForeColor = Tema.TextoPrincipal,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);

            filas.Controls.Add(new Label
            {
                Text = "Registro ordenado por código de todas las ventas pagadas y fiadas",
                Font = Tema.Subtitulo,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 1);

            filas.Controls.Add(new Label
            {
                Text = "Buscar por código",
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
            var tarjeta = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                MostrarBorde = true,
                Margin = new Padding(14, 0, 14, 0),
                Padding = new Padding(12, 12, 12, 12)
            };
            tarjeta.Controls.Add(tablaContenedor);
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
                Text = "Boletas registradas",
                Font = Tema.Etiqueta,
                ForeColor = Tema.TextoSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            }, 0, 0);
            filas.Controls.Add(lblTotalBoletas, 1, 0);
            tarjeta.Controls.Add(filas);
            return tarjeta;
        }

        private void BuscarBoleta()
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text) || txtBuscar.Text == PlaceholderBuscar)
            {
                filtroCodigo = null;
                lblMensaje.Visible = false;
                Refrescar();
                return;
            }

            buscarEsPlaceholder = false;

            string? error = Validaciones.CodigoBoleta(txtBuscar.Text, out string codigo);
            if (!Validaciones.Mostrar(lblMensaje, error!))
                return;

            Boleta? encontrada = Datos.Boletas.Buscar(codigo);
            if (encontrada == null)
            {
                filtroCodigo = null;
                Refrescar();
                Validaciones.Mostrar(lblMensaje, "No existe una boleta con ese código.");
                return;
            }

            filtroCodigo = codigo;
            Refrescar();
            lblMensaje.ForeColor = Tema.PrimarioOscuro;
            lblMensaje.Text = $"Boleta {codigo} localizada.";
            lblMensaje.Visible = true;
        }

        private void Refrescar()
        {
            List<Boleta> boletas = Datos.Boletas.Inorden();

            if (filtroCodigo != null)
            {
                Boleta? encontrada = Datos.Boletas.Buscar(filtroCodigo);
                boletas = encontrada == null
                    ? new List<Boleta>()
                    : new List<Boleta> { encontrada };
            }

            tablaBoletas.SuspendLayout();

            while (tablaBoletas.Controls.Count > 0)
                tablaBoletas.Controls[0].Dispose();

            tablaBoletas.RowStyles.Clear();
            tablaBoletas.RowCount = 1;
            tablaBoletas.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoEncabezado));
            tablaBoletas.Controls.Add(CrearEncabezadoTabla(), 0, 0);

            if (boletas.Count == 0)
            {
                tablaBoletas.RowCount = 2;
                tablaBoletas.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
                tablaBoletas.Controls.Add(new Label
                {
                    Text = filtroCodigo == null
                        ? "No hay boletas registradas."
                        : "No hay boletas con ese código.",
                    Font = Tema.Normal,
                    ForeColor = Tema.TextoSecundario,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = Padding.Empty
                }, 0, 1);
            }
            else
            {
                tablaBoletas.RowCount = boletas.Count + 1;

                for (int i = 0; i < boletas.Count; i++)
                {
                    tablaBoletas.RowStyles.Add(new RowStyle(SizeType.Absolute, AltoFila));
                    tablaBoletas.Controls.Add(CrearFilaBoleta(boletas[i]), 0, i + 1);
                }
            }

            tablaBoletas.ResumeLayout();
            AjustarAnchoTabla();
            lblTotalBoletas.Text = Datos.Boletas.Contar.ToString();
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
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AnchoCodigo));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AnchoTotal));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, AnchoEstado));
        }

        private static Control CrearEncabezadoTabla()
        {
            var fila = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
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
            fila.Controls.Add(CrearCelda("Productos", ContentAlignment.MiddleLeft), 2, 0);
            fila.Controls.Add(CrearCelda("Total", ContentAlignment.MiddleRight), 3, 0);
            fila.Controls.Add(CrearCelda("Estado", ContentAlignment.MiddleCenter), 4, 0);
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

        private static Label CrearCeldaValor(string texto, Font fuente, Color color, ContentAlignment alineacion)
        {
            return new Label
            {
                Text = texto,
                AutoSize = false,
                Font = fuente,
                ForeColor = color,
                Dock = DockStyle.Fill,
                TextAlign = alineacion,
                Margin = Padding.Empty
            };
        }

        private static PanelRedondeado CrearFilaBoleta(Boleta boleta)
        {
            var fila = new PanelRedondeado
            {
                Dock = DockStyle.Fill,
                BackColor = Tema.Blanco,
                ColorBorde = Tema.Borde,
                MostrarBorde = true,
                Margin = new Padding(0, 0, 0, 4),
                Padding = Padding.Empty
            };

            var columnas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = new Padding(MargenTabla, 0, MargenTabla, 0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            ConfigurarColumnasTabla(columnas);
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            columnas.Controls.Add(CrearCeldaValor(boleta.Codigo, Tema.Codigo, Tema.PrimarioOscuro, ContentAlignment.MiddleLeft), 0, 0);
            columnas.Controls.Add(CrearCeldaValor(boleta.Cliente, Tema.Normal, Tema.TextoPrincipal, ContentAlignment.MiddleLeft), 1, 0);
            columnas.Controls.Add(CrearCeldaValor(
                string.IsNullOrWhiteSpace(boleta.Detalle) ? "—" : boleta.Detalle,
                Tema.Pequena, Tema.TextoSecundario, ContentAlignment.MiddleLeft), 2, 0);
            columnas.Controls.Add(CrearCeldaValor(Formato.Moneda(boleta.Total), Tema.Negrita, Tema.TextoPrincipal, ContentAlignment.MiddleRight), 3, 0);

            var insignia = new Insignia
            {
                AjustarTamano = false,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Font = Tema.Etiqueta
            };
            insignia.Estado = boleta.Estado;
            columnas.Controls.Add(insignia, 4, 0);

            fila.Controls.Add(columnas);
            return fila;
        }
    }
}
