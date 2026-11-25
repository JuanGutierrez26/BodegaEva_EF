using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Modelos
{
    public class Abono
    {
        public string CodigoBoleta { get; }
        public string Cliente { get; }
        public decimal Monto { get; }
        public DateTime Fecha { get; }

        public Abono(string codigoBoleta, string cliente, decimal monto)
        {
            if (string.IsNullOrWhiteSpace(codigoBoleta))
                throw new ArgumentException("El código de la boleta es obligatorio.");
            if (string.IsNullOrWhiteSpace(cliente))
                throw new ArgumentException("El cliente es obligatorio.");
            if (monto <= 0)
                throw new ArgumentException("El monto del abono debe ser mayor que cero.");

            CodigoBoleta = codigoBoleta.Trim().ToUpper();
            Cliente = cliente.Trim();
            Monto = monto;
            Fecha = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{CodigoBoleta} · {Cliente} · S/ {Monto:0.00}";
        }
    }
}
