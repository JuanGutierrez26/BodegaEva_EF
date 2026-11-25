using BodegaEva_EF.Controles;
using BodegaEva_EF.Utilidades;
using BodegaEva_EF.Vistas;
using BodegaEva_EF.Vistas.Venta;

namespace BodegaEva_EF
{
    public partial class FormPrincipal : Form
    {
        private readonly Dictionary<int, Control> vistasVenta = new();
        private readonly UcInicio inicio;
        private UcFiado? fiado;
        private UcHistorial? historial;
        private UcIncidencias? incidencias;
        private bool historialActivo;

        public FormPrincipal()
        {
            InitializeComponent();

            inicio = new UcInicio { Dock = DockStyle.Fill };
            inicio.NuevaVentaSolicitada += (s, e) => IrAPaso(1);
            inicio.FiadoSolicitado += (s, e) => MostrarFiado();
            inicio.HistorialSolicitado += (s, e) => MostrarHistorial();
            inicio.IncidenciasSolicitado += (s, e) => MostrarIncidencias();
            PanelContenido.Controls.Add(inicio);
            ActualizarSeleccionMenu(inicio);
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
            ActualizarSeleccionMenu(vista);
        }

        private void MostrarInicio()
        {
            MostrarVista(inicio);
        }

        private UcFiado ObtenerFiado()
        {
            return fiado ??= new UcFiado { Dock = DockStyle.Fill };
        }

        private void MostrarFiado()
        {
            MostrarVista(ObtenerFiado());
        }

        private UcHistorial ObtenerHistorial()
        {
            return historial ??= new UcHistorial { Dock = DockStyle.Fill };
        }

        private void MostrarHistorial()
        {
            MostrarVista(ObtenerHistorial());
        }

        private UcIncidencias ObtenerIncidencias()
        {
            return incidencias ??= new UcIncidencias { Dock = DockStyle.Fill };
        }

        private void MostrarIncidencias()
        {
            MostrarVista(ObtenerIncidencias());
        }

        private void MostrarVenta()
        {
            IrAPaso(1);
        }

        private void ActualizarSeleccionMenu(Control vista)
        {
            BtnInicio.Estilo = ReferenceEquals(vista, inicio)
                ? EstiloBoton.Primario
                : EstiloBoton.Secundario;
            BtnFiado.Estilo = fiado != null && ReferenceEquals(vista, fiado)
                ? EstiloBoton.Primario
                : EstiloBoton.Secundario;

            historialActivo = historial != null && ReferenceEquals(vista, historial);
            BtnHistorial.Estilo = historialActivo
                ? EstiloBoton.PrimarioClaro
                : EstiloBoton.Secundario;
            BtnIncidencias.Estilo = incidencias != null && ReferenceEquals(vista, incidencias)
                ? EstiloBoton.PrimarioClaro
                : EstiloBoton.Secundario;
        }

        private void BtnInicio_Click(object? sender, EventArgs e)
        {
            MostrarInicio();
        }

        private void BtnVenta_Click(object? sender, EventArgs e)
        {
            MostrarVenta();
        }

        private void BtnFiado_Click(object? sender, EventArgs e)
        {
            MostrarFiado();
        }

        private void BtnHistorial_Click(object? sender, EventArgs e)
        {
            MostrarHistorial();
        }

        private void BtnIncidencias_Click(object? sender, EventArgs e)
        {
            MostrarIncidencias();
        }

        private void BtnSalir_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
