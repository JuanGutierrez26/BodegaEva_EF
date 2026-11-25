using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Text;

namespace BodegaEva_EF.Utilidades
{
    public static class Tema
    {
        public static readonly Color Primario = ColorTranslator.FromHtml("#2E7D5B");
        public static readonly Color PrimarioClaro = ColorTranslator.FromHtml("#E3F2EB");
        public static readonly Color PrimarioOscuro = ColorTranslator.FromHtml("#1F5C42");

        public static readonly Color Ambar = ColorTranslator.FromHtml("#F2A93B");
        public static readonly Color AmbarClaro = ColorTranslator.FromHtml("#FFF3DC");
        public static readonly Color AmbarOscuro = ColorTranslator.FromHtml("#7A4B00");

        public static readonly Color Error = ColorTranslator.FromHtml("#C93B3B");
        public static readonly Color ErrorClaro = ColorTranslator.FromHtml("#FDECEC");

        public static readonly Color Fondo = ColorTranslator.FromHtml("#F6F7F8");
        public static readonly Color Blanco = Color.White;
        public static readonly Color Borde = ColorTranslator.FromHtml("#D5DCE0");
        public static readonly Color EncabezadoTabla = ColorTranslator.FromHtml("#F0F2F4");
        public static readonly Color Deshabilitado = ColorTranslator.FromHtml("#DCE0E4");

        public static readonly Color TextoPrincipal = ColorTranslator.FromHtml("#1F2933");
        public static readonly Color TextoSecundario = ColorTranslator.FromHtml("#737D82");

        public static readonly Font Titulo = new Font("Segoe UI", 20f, FontStyle.Bold);
        public static readonly Font Subtitulo = new Font("Segoe UI", 10f);
        public static readonly Font Normal = new Font("Segoe UI", 10f);
        public static readonly Font Negrita = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font Pequena = new Font("Segoe UI", 9f);
        public static readonly Font Etiqueta = new Font("Segoe UI", 9f, FontStyle.Bold);
        public static readonly Font NumeroGrande = new Font("Segoe UI", 24f, FontStyle.Bold);
        public static readonly Font Codigo = new Font("Consolas", 10f, FontStyle.Bold);

        public const int RadioTarjeta = 12;
        public const int RadioBoton = 8;
        public const int RadioInsignia = 10;

        public static Color FondoEstado(EstadoBoleta estado)
        {
            return estado switch
            {
                EstadoBoleta.Pagada => PrimarioClaro,
                EstadoBoleta.Fiada => AmbarClaro,
                _ => Deshabilitado
            };
        }

        public static Color TextoEstado(EstadoBoleta estado)
        {
            return estado switch
            {
                EstadoBoleta.Pagada => PrimarioOscuro,
                EstadoBoleta.Fiada => AmbarOscuro,
                _ => TextoSecundario
            };
        }

        public static GraphicsPath RutaRedondeada(Rectangle area, int radio)
        {
            int d = radio * 2;
            var ruta = new GraphicsPath();

            ruta.AddArc(area.X, area.Y, d, d, 180, 90);
            ruta.AddArc(area.Right - d, area.Y, d, d, 270, 90);
            ruta.AddArc(area.Right - d, area.Bottom - d, d, d, 0, 90);
            ruta.AddArc(area.X, area.Bottom - d, d, d, 90, 90);
            ruta.CloseFigure();

            return ruta;
        }
    }
}