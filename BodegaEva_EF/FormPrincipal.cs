using BodegaEva_EF.Vistas;

namespace BodegaEva_EF
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
            PanelContenido.Controls.Add(new UcInicio());
        }

        private void BtnSalir_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
