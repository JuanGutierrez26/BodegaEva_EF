using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BodegaEva_EF.Utilidades
{
    public static class Formato
    {
        private static readonly CultureInfo Cultura = CultureInfo.InvariantCulture;

        public static string Moneda(decimal monto)
        {
            return "S/ " + monto.ToString("N2", Cultura);
        }

        public static string HaceCuanto(DateTime fecha)
        {
            TimeSpan diferencia = DateTime.Now - fecha;

            if (diferencia.TotalSeconds < 60)
                return "hace un momento";
            if (diferencia.TotalMinutes < 60)
                return $"hace {(int)diferencia.TotalMinutes} min";
            if (diferencia.TotalHours < 24)
                return $"hace {(int)diferencia.TotalHours} h";

            int dias = (int)diferencia.TotalDays;
            return dias == 1 ? "hace 1 día" : $"hace {dias} días";
        }

        public static string NombreEstado(EstadoBoleta estado)
        {
            return estado switch
            {
                EstadoBoleta.Pagada => "Pagada",
                EstadoBoleta.Fiada => "Fiada",
                EstadoBoleta.Anulada => "Anulada",
                _ => estado.ToString()
            };
        }

        public static string Recortar(string texto, int maximo)
        {
            if (string.IsNullOrEmpty(texto) || texto.Length <= maximo)
                return texto;

            return texto.Substring(0, maximo - 1).TrimEnd() + "…";
        }

        public static string NormalizarCodigo(string texto)
        {
            return texto?.Trim().ToUpper() ?? "";
        }

        public static bool TryLeerDecimal(string texto, out decimal valor)
        {
            valor = 0;
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            string limpio = texto.Trim()
                                 .Replace("S/", "", StringComparison.OrdinalIgnoreCase)
                                 .Replace(',', '.')
                                 .Trim();

            const NumberStyles estilo = NumberStyles.AllowDecimalPoint
                                      | NumberStyles.AllowLeadingWhite
                                      | NumberStyles.AllowTrailingWhite;

            if (!decimal.TryParse(limpio, estilo, Cultura, out decimal leido))
                return false;

            valor = Math.Round(leido, 2);
            return true;
        }

        public static bool TryLeerEntero(string texto, out int valor)
        {
            const NumberStyles estilo = NumberStyles.AllowLeadingWhite
                                      | NumberStyles.AllowTrailingWhite;

            return int.TryParse(texto, estilo, Cultura, out valor);
        }
    }
}
