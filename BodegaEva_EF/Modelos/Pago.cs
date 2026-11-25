using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Modelos
{
    public class Pago
    {
        public string Codigo { get; }
        public decimal Monto { get; }
        public DateTime Fecha { get; }

        public Pago(string codigo, decimal monto)
        {
            Codigo = codigo?.Trim().ToUpper() ?? "";
            Monto = monto;
            Fecha = DateTime.Now;
        }

        public bool Validar(decimal totalEsperado, out string motivo)
        {
            if (Codigo.Length < 6 || Codigo.Length > 12 || !Codigo.All(char.IsLetterOrDigit))
            {
                motivo = "El código de pago debe tener de 6 a 12 letras o números.";
                return false;
            }

            if (Monto != totalEsperado)
            {
                motivo = $"El monto (S/ {Monto:0.00}) no coincide con el total (S/ {totalEsperado:0.00}).";
                return false;
            }

            motivo = "";
            return true;
        }

        public override string ToString()
        {
            return $"{Codigo} · S/ {Monto:0.00}";
        }
    }
}
