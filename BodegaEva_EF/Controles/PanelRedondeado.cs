using BodegaEva_EF.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text;

namespace BodegaEva_EF.Controles
{
    public class PanelRedondeado : Panel
    {
        private int radio = Tema.RadioTarjeta;
        private Color colorBorde = Tema.Borde;
        private bool mostrarBorde = true;
        private bool interactivo;
        private bool encima;

        [Category("Appearance")]
        [DefaultValue(Tema.RadioTarjeta)]
        public int Radio
        {
            get => radio;
            set { radio = Math.Max(0, value); Invalidate(); }
        }

        [Category("Appearance")]
        public Color ColorBorde
        {
            get => colorBorde;
            set { colorBorde = value; Invalidate(); }
        }

        private bool ShouldSerializeColorBorde() => colorBorde != Tema.Borde;
        private void ResetColorBorde() => ColorBorde = Tema.Borde;

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool MostrarBorde
        {
            get => mostrarBorde;
            set { mostrarBorde = value; Invalidate(); }
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool Interactivo
        {
            get => interactivo;
            set
            {
                interactivo = value;
                Cursor = value ? Cursors.Hand : Cursors.Default;
                if (!value) encima = false;
                Invalidate();
            }
        }

        protected override Padding DefaultPadding => new Padding(16);

        public PanelRedondeado()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Tema.Blanco;
        }

        public void ConectarHijos()
        {
            foreach (Control hijo in Controls)
                Conectar(hijo);
        }

        private void Conectar(Control control)
        {
            if (control is ButtonBase || control is TextBoxBase)
                return;

            control.Click += (s, e) => OnClick(e);
            control.MouseEnter += (s, e) => ActualizarHover();
            control.MouseLeave += (s, e) => ActualizarHover();

            if (interactivo)
                control.Cursor = Cursors.Hand;

            foreach (Control hijo in control.Controls)
                Conectar(hijo);
        }

        private void ActualizarHover()
        {
            bool dentro = interactivo && IsHandleCreated &&
                          ClientRectangle.Contains(PointToClient(Cursor.Position));

            if (dentro != encima)
            {
                encima = dentro;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            ActualizarHover();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ActualizarHover();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            ButtonRenderer.DrawParentBackground(e.Graphics, ClientRectangle, this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width > 4 && Height > 4)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var area = new Rectangle(0, 0, Width - 1, Height - 1);
                Color borde = encima ? Tema.Primario : colorBorde;

                using var relleno = new SolidBrush(BackColor);
                using var lapiz = new Pen(borde);

                int r = Math.Min(radio, Math.Min(Width, Height) / 2 - 1);

                if (r < 1)
                {
                    g.FillRectangle(relleno, area);
                    if (mostrarBorde || encima)
                        g.DrawRectangle(lapiz, area);
                }
                else
                {
                    using GraphicsPath ruta = Tema.RutaRedondeada(area, r);
                    g.FillPath(relleno, ruta);
                    if (mostrarBorde || encima)
                        g.DrawPath(lapiz, ruta);
                }
            }

            base.OnPaint(e);
        }
    }
}
