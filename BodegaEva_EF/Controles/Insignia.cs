using BodegaEva_EF.Modelos;
using BodegaEva_EF.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text;

namespace BodegaEva_EF.Controles
{
    public class Insignia : Control
    {
        private const int PaddingHorizontal = 12;
        private const int AltoFijo = 22;

        private EstadoBoleta estado = EstadoBoleta.Pagada;
        private Color colorRelleno = Tema.PrimarioClaro;
        private Color colorLetra = Tema.PrimarioOscuro;
        private bool ajustarTamano = true;

        [DefaultValue(EstadoBoleta.Pagada)]
        public EstadoBoleta Estado
        {
            get => estado;
            set
            {
                estado = value;
                colorRelleno = Tema.FondoEstado(value);
                colorLetra = Tema.TextoEstado(value);
                Text = Formato.NombreEstado(value);
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Recalcular();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ColorRelleno
        {
            get => colorRelleno;
            set { colorRelleno = value; Invalidate(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ColorLetra
        {
            get => colorLetra;
            set { colorLetra = value; Invalidate(); }
        }

        [DefaultValue(true)]
        public bool AjustarTamano
        {
            get => ajustarTamano;
            set { ajustarTamano = value; Recalcular(); }
        }

        public Insignia()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            Font = Tema.Etiqueta;
            Estado = EstadoBoleta.Pagada;
        }

        public void Personalizar(string texto, Color relleno, Color letra)
        {
            colorRelleno = relleno;
            colorLetra = letra;
            Text = texto;
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Recalcular();
        }

        private void Recalcular()
        {
            if (ajustarTamano)
                Size = Medir(Text, Font);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ButtonRenderer.DrawParentBackground(e.Graphics, ClientRectangle, this);
            Dibujar(e.Graphics, ClientRectangle, Text, Font, colorRelleno, colorLetra);
        }

        public static Size Medir(string texto, Font fuente)
        {
            Size texto2 = TextRenderer.MeasureText(texto ?? "", fuente,
                new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);

            return new Size(texto2.Width + PaddingHorizontal * 2, AltoFijo);
        }

        public static void Dibujar(Graphics g, Rectangle area, string texto, Font fuente,
                                   Color relleno, Color letra)
        {
            Size tamano = Medir(texto, fuente);
            int ancho = Math.Min(tamano.Width, area.Width);
            int alto = Math.Min(tamano.Height, area.Height);

            if (ancho < 8 || alto < 8)
                return;

            var pildora = new Rectangle(
                area.X + (area.Width - ancho) / 2,
                area.Y + (area.Height - alto) / 2,
                ancho - 1,
                alto - 1);

            SmoothingMode anterior = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int radio = Math.Min(Tema.RadioInsignia, pildora.Height / 2);
            using (GraphicsPath ruta = Tema.RutaRedondeada(pildora, radio))
            using (var pincel = new SolidBrush(relleno))
            {
                g.FillPath(pincel, ruta);
            }

            g.SmoothingMode = anterior;

            TextRenderer.DrawText(g, texto ?? "", fuente, pildora, letra,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPadding |
                TextFormatFlags.NoPrefix);
        }
    }
}
