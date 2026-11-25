using BodegaEva_EF.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text;

namespace BodegaEva_EF.Controles
{
    public class IndicadorPasos : Control
    {
        private const int Margen = 6;

        private string[] pasos = { "Productos", "Pedido", "Pago", "Confirmación" };
        private int pasoActual = 1;

        public event Action<int>? PasoSeleccionado;

        [DefaultValue(1)]
        public int PasoActual
        {
            get => pasoActual;
            set
            {
                pasoActual = Math.Clamp(value, 1, pasos.Length);
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TotalPasos => pasos.Length;

        public IndicadorPasos()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            Size = new Size(760, 52);
        }

        public void ConfigurarPasos(params string[] nombres)
        {
            if (nombres == null || nombres.Length < 2)
                throw new ArgumentException("Se necesitan al menos dos pasos.");

            pasos = nombres;
            pasoActual = Math.Clamp(pasoActual, 1, pasos.Length);
            Invalidate();
        }

        public bool Siguiente()
        {
            if (pasoActual >= pasos.Length)
                return false;

            PasoActual = pasoActual + 1;
            return true;
        }

        public bool Anterior()
        {
            if (pasoActual <= 1)
                return false;

            PasoActual = pasoActual - 1;
            return true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            for (int i = 0; i < pasos.Length; i++)
            {
                if (Celda(i).Contains(e.Location))
                {
                    PasoSeleccionado?.Invoke(i + 1);
                    return;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bool sobrePaso = false;
            for (int i = 0; i < pasos.Length; i++)
            {
                if (Celda(i).Contains(e.Location))
                {
                    sobrePaso = true;
                    break;
                }
            }

            Cursor = sobrePaso ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            ButtonRenderer.DrawParentBackground(g, ClientRectangle, this);

            if (Width < 20 || Height < 20)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var tarjeta = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath ruta = Tema.RutaRedondeada(tarjeta, Tema.RadioTarjeta))
            using (var relleno = new SolidBrush(Tema.Blanco))
            using (var borde = new Pen(Tema.Borde))
            {
                g.FillPath(relleno, ruta);
                g.DrawPath(borde, ruta);
            }

            for (int i = 0; i < pasos.Length; i++)
                DibujarPaso(g, Celda(i), i);
        }

        private Rectangle Celda(int indice)
        {
            int anchoCelda = (Width - Margen * 2) / pasos.Length;
            int altoCelda = Height - Margen * 2 - 1;
            return new Rectangle(Margen + indice * anchoCelda, Margen, anchoCelda - 4, altoCelda);
        }

        private void DibujarPaso(Graphics g, Rectangle celda, int indice)
        {
            int numero = indice + 1;
            bool activo = numero == pasoActual;

            if (activo)
            {
                int radio = Math.Min(Tema.RadioBoton + 4, celda.Height / 2 - 1);
                using GraphicsPath pildora = Tema.RutaRedondeada(celda, radio);
                using var relleno = new SolidBrush(Tema.PrimarioOscuro);
                g.FillPath(relleno, pildora);
            }

            Color fondoCirculo = activo ? Tema.Blanco : Tema.EncabezadoTabla;
            Color textoCirculo = activo ? Tema.PrimarioOscuro : Tema.TextoSecundario;
            Color textoPaso = activo ? Tema.Blanco : Tema.TextoSecundario;
            Font fuente = activo ? Tema.Negrita : Tema.Normal;
            string textoNumero = numero.ToString();

            const int diametro = 22;
            const int separacion = 8;
            const int holgura = 16;

            Size medida = TextRenderer.MeasureText(g, pasos[indice], fuente,
                new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);

            int disponible = Math.Max(0, celda.Width - diametro - separacion - holgura);
            int anchoTexto = Math.Min(medida.Width, disponible);
            int anchoGrupo = diametro + separacion + anchoTexto;
            int x0 = celda.X + (celda.Width - anchoGrupo) / 2;

            var circulo = new Rectangle(x0, celda.Y + (celda.Height - diametro) / 2, diametro, diametro);
            using (var pincel = new SolidBrush(fondoCirculo))
                g.FillEllipse(pincel, circulo);

            TextRenderer.DrawText(g, textoNumero, Tema.Etiqueta, circulo, textoCirculo,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);

            var areaTexto = new Rectangle(x0 + diametro + separacion, celda.Y, anchoTexto, celda.Height);
            TextRenderer.DrawText(g, pasos[indice], fuente, areaTexto, textoPaso,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
        }
    }
}
