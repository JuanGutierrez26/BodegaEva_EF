using BodegaEva_EF.Vistas;
using BodegaEva_EF.Vistas.Venta;

namespace BodegaEva_EF
{
    public partial class FormPrincipal : Form
    {
        private readonly Dictionary<int, Control> vistasVenta = new();

        public FormPrincipal()
        {
            InitializeComponent();

            var inicio = new UcInicio { Dock = DockStyle.Fill };
            inicio.NuevaVentaSolicitada += (s, e) => IrAPaso(1);
            PanelContenido.Controls.Add(inicio);
        }

        private void IrAPaso(int paso)
        {
            switch (paso)
            {
                case 1:
                    MostrarVista(ObtenerPasoProductos());
                    break;

                case 2:
                    MostrarVista(ObtenerPasoPedido());
                    break;

                case 3:
                    MostrarVista(ObtenerPasoPago());
                    break;

                case 4:
                    MostrarVista(ObtenerPasoConfirmacion());
                    break;
            }
        }

        private PasoProductos ObtenerPasoProductos()
        {
            if (!vistasVenta.TryGetValue(1, out Control? vista))
            {
                var pasoProductos = new PasoProductos { Dock = DockStyle.Fill };
                pasoProductos.NavegacionSolicitada += IrAPaso;
                vistasVenta[1] = pasoProductos;
                vista = pasoProductos;
            }

            return (PasoProductos)vista;
        }

        private PasoPedido ObtenerPasoPedido()
        {
            if (!vistasVenta.TryGetValue(2, out Control? vista))
            {
                var pasoPedido = new PasoPedido(ObtenerPasoProductos()) { Dock = DockStyle.Fill };
                pasoPedido.NavegacionSolicitada += IrAPaso;
                vistasVenta[2] = pasoPedido;
                vista = pasoPedido;
            }

            return (PasoPedido)vista;
        }

        private PasoPago ObtenerPasoPago()
        {
            if (!vistasVenta.TryGetValue(3, out Control? vista))
            {
                var pasoPago = new PasoPago(ObtenerPasoProductos()) { Dock = DockStyle.Fill };
                pasoPago.NavegacionSolicitada += IrAPaso;
                vistasVenta[3] = pasoPago;
                vista = pasoPago;
            }

            return (PasoPago)vista;
        }

        private PasoConfirmacion ObtenerPasoConfirmacion()
        {
            if (!vistasVenta.TryGetValue(4, out Control? vista))
            {
                var pasoConfirmacion = new PasoConfirmacion(ObtenerPasoPago()) { Dock = DockStyle.Fill };
                pasoConfirmacion.NavegacionSolicitada += IrAPaso;
                vistasVenta[4] = pasoConfirmacion;
                vista = pasoConfirmacion;
            }

            return (PasoConfirmacion)vista;
        }

        private void MostrarVista(Control vista)
        {
            foreach (Control control in PanelContenido.Controls)
                control.Visible = false;

            if (!PanelContenido.Controls.Contains(vista))
                PanelContenido.Controls.Add(vista);

            vista.Dock = DockStyle.Fill;
            vista.Visible = true;
            vista.BringToFront();
        }

        private void MostrarVenta()
        {
            IrAPaso(1);
        }

        private void BtnVenta_Click(object? sender, EventArgs e)
        {
            MostrarVenta();
        }

        private void BtnSalir_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
