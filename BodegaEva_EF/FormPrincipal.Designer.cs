using BodegaEva_EF.Controles;
using BodegaEva_EF.Utilidades;

namespace BodegaEva_EF
{
    partial class FormPrincipal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private TableLayoutPanel ContenedorPrincipal;
        private Panel Sidebar;
        private TableLayoutPanel SidebarFilas;
        private Panel PanelContenido;
        private BotonRedondeado BtnInicio;
        private BotonRedondeado BtnVenta;
        private BotonRedondeado BtnFiado;
        private BotonRedondeado BtnHistorial;
        private BotonRedondeado BtnIncidencias;
        private BotonRedondeado BtnSalir;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1120, 720);
            MinimumSize = new Size(920, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Tema.Fondo;
            Text = "Bodega Eva";

            ContenedorPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Fondo
            };
            ContenedorPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132F));
            ContenedorPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ContenedorPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Sidebar = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = Tema.Blanco
            };

            SidebarFilas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            SidebarFilas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            SidebarFilas.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            SidebarFilas.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            SidebarFilas.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));

            SidebarFilas.Controls.Add(new Label
            {
                Text = "Bodega Eva",
                Font = new Font(Tema.Negrita.FontFamily, 13f, FontStyle.Bold),
                ForeColor = Tema.PrimarioOscuro,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = Padding.Empty
            }, 0, 0);

            var menu = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Margin = Padding.Empty,
                Padding = new Padding(10, 8, 10, 0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                BackColor = Tema.Blanco
            };
            menu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            string[] opciones = { "Inicio", "Venta", "Fiado", "Historial", "Incidencias" };
            for (int i = 0; i < opciones.Length; i++)
            {
                menu.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
                var boton = new BotonRedondeado
                {
                    Text = opciones[i],
                    Estilo = EstiloBoton.Secundario,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0, 0, 0, 8)
                };
                menu.Controls.Add(boton, 0, i);

                if (i == 0)
                {
                    BtnInicio = boton;
                    BtnInicio.Click += BtnInicio_Click;
                }
                else if (i == 1)
                {
                    BtnVenta = boton;
                    BtnVenta.Click += BtnVenta_Click;
                }
                else if (i == 2)
                {
                    BtnFiado = boton;
                    BtnFiado.Click += BtnFiado_Click;
                }
                else if (i == 3)
                {
                    BtnHistorial = boton;
                    BtnHistorial.Click += BtnHistorial_Click;
                }
                else if (i == 4)
                {
                    BtnIncidencias = boton;
                    BtnIncidencias.Click += BtnIncidencias_Click;
                }
            }

            menu.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            menu.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Tema.Blanco }, 0, 5);

            SidebarFilas.Controls.Add(menu, 0, 1);

            BtnSalir = new BotonRedondeado
            {
                Text = "Salir",
                Estilo = EstiloBoton.Peligro,
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 0, 10, 16)
            };
            BtnSalir.Click += BtnSalir_Click;
            SidebarFilas.Controls.Add(BtnSalir, 0, 2);

            Sidebar.Controls.Add(SidebarFilas);
            ContenedorPrincipal.Controls.Add(Sidebar, 0, 0);

            PanelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = Tema.Fondo
            };
            ContenedorPrincipal.Controls.Add(PanelContenido, 1, 0);

            Controls.Add(ContenedorPrincipal);
        }

        #endregion
    }
}
