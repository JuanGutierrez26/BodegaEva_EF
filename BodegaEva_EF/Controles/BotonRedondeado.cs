using BodegaEva_EF.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text;

namespace BodegaEva_EF.Controles
{
    public enum EstiloBoton
    {
        Primario,
        Peligro,
        Secundario,
        PrimarioClaro
    }

    public class BotonRedondeado : Button
    {
        private EstiloBoton estilo = EstiloBoton.Primario;
        private bool encima;
        private bool presionado;

        [DefaultValue(EstiloBoton.Primario)]
        public EstiloBoton Estilo
        {
            get => estilo;
            set { estilo = value; Invalidate(); }
        }

        [DefaultValue(Tema.RadioBoton)]
        public int Radio { get; set; } = Tema.RadioBoton;

        public BotonRedondeado()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Font = Tema.Negrita;
            Cursor = Cursors.Hand;
            Size = new Size(140, 40);
            Text = "Botón";
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            encima = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            encima = false;
            presionado = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                presionado = true;
                Invalidate();
            }
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            presionado = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Cursor = Enabled ? Cursors.Hand : Cursors.Default;
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            ButtonRenderer.DrawParentBackground(g, ClientRectangle, this);

            if (Width < 4 || Height < 4)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            ObtenerColores(out Color fondo, out Color texto, out Color borde);

            var area = new Rectangle(0, 0, Width - 1, Height - 1);
            int radio = Math.Min(Radio, Math.Min(Width, Height) / 2 - 1);

            using (GraphicsPath ruta = Tema.RutaRedondeada(area, radio))
            using (var relleno = new SolidBrush(fondo))
            {
                g.FillPath(relleno, ruta);

                if (borde != Color.Empty)
                {
                    using var lapiz = new Pen(borde);
                    g.DrawPath(lapiz, ruta);
                }
            }

            Rectangle areaTexto = ClientRectangle;
            if (Image != null)
            {
                Rectangle areaImagen = ObtenerAreaImagen(Image);
                g.DrawImage(Image, areaImagen);

                if (EsIzquierda(ImageAlign))
                {
                    areaTexto.X = areaImagen.Right + 6;
                    areaTexto.Width -= areaImagen.Width + 6;
                }
                else if (EsDerecha(ImageAlign))
                {
                    areaTexto.Width = areaImagen.Left - 6;
                }
            }

            TextRenderer.DrawText(g, Text, Font, areaTexto, texto,
                ObtenerFlagsTexto(TextAlign) |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPrefix);
        }

        private Rectangle ObtenerAreaImagen(Image imagen)
        {
            int x = EsIzquierda(ImageAlign)
                ? Padding.Left
                : EsDerecha(ImageAlign)
                    ? Width - Padding.Right - imagen.Width
                    : (Width - imagen.Width) / 2;

            int y = EsArriba(ImageAlign)
                ? Padding.Top
                : EsAbajo(ImageAlign)
                    ? Height - Padding.Bottom - imagen.Height
                    : (Height - imagen.Height) / 2;

            return new Rectangle(x, y, imagen.Width, imagen.Height);
        }

        private static bool EsIzquierda(ContentAlignment alineacion)
        {
            return alineacion is ContentAlignment.TopLeft
                or ContentAlignment.MiddleLeft
                or ContentAlignment.BottomLeft;
        }

        private static bool EsDerecha(ContentAlignment alineacion)
        {
            return alineacion is ContentAlignment.TopRight
                or ContentAlignment.MiddleRight
                or ContentAlignment.BottomRight;
        }

        private static bool EsArriba(ContentAlignment alineacion)
        {
            return alineacion is ContentAlignment.TopLeft
                or ContentAlignment.TopCenter
                or ContentAlignment.TopRight;
        }

        private static bool EsAbajo(ContentAlignment alineacion)
        {
            return alineacion is ContentAlignment.BottomLeft
                or ContentAlignment.BottomCenter
                or ContentAlignment.BottomRight;
        }

        private static TextFormatFlags ObtenerFlagsTexto(ContentAlignment alineacion)
        {
            TextFormatFlags horizontal = EsIzquierda(alineacion)
                ? TextFormatFlags.Left
                : EsDerecha(alineacion)
                    ? TextFormatFlags.Right
                    : TextFormatFlags.HorizontalCenter;

            TextFormatFlags vertical = EsArriba(alineacion)
                ? TextFormatFlags.Top
                : EsAbajo(alineacion)
                    ? TextFormatFlags.Bottom
                    : TextFormatFlags.VerticalCenter;

            return horizontal | vertical;
        }

        private void ObtenerColores(out Color fondo, out Color texto, out Color borde)
        {
            borde = Color.Empty;

            if (!Enabled)
            {
                fondo = Tema.Deshabilitado;
                texto = Tema.TextoSecundario;
                return;
            }

            switch (estilo)
            {
                case EstiloBoton.Peligro:
                    fondo = Tema.Error;
                    texto = Color.White;
                    break;

                case EstiloBoton.Secundario:
                    fondo = Tema.Blanco;
                    texto = Tema.TextoPrincipal;
                    borde = Tema.Borde;
                    break;

                case EstiloBoton.PrimarioClaro:
                    fondo = Tema.PrimarioClaro;
                    texto = Tema.PrimarioOscuro;
                    borde = Tema.Primario;
                    break;

                default:
                    fondo = Tema.Primario;
                    texto = Color.White;
                    break;
            }

            if (presionado)
                fondo = estilo == EstiloBoton.Secundario ? Tema.Deshabilitado : ControlPaint.Dark(fondo, 0.2f);
            else if (encima)
                fondo = estilo == EstiloBoton.Secundario ? Tema.EncabezadoTabla : ControlPaint.Dark(fondo, 0.1f);
        }
    }
}
