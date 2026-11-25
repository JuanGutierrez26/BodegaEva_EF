using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Modelos
{
    public class Operacion
    {
        public string Codigo { get; }
        public TipoOperacion Tipo { get; }
        public string Descripcion { get; }
        public decimal Monto { get; }
        public DateTime Fecha { get; }

        public Operacion(string codigo, TipoOperacion tipo, string descripcion, decimal monto)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código de la operación es obligatorio.");
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción es obligatoria.");
            if (monto <= 0)
                throw new ArgumentException("El monto debe ser mayor que cero.");

            Codigo = codigo.Trim().ToUpper();
            Tipo = tipo;
            Descripcion = descripcion.Trim();
            Monto = monto;
            Fecha = DateTime.Now;
        }

        public string NombreTipo
        {
            get
            {
                return Tipo switch
                {
                    TipoOperacion.Venta => "Venta",
                    TipoOperacion.VentaFiada => "Venta fiada",
                    TipoOperacion.Abono => "Abono",
                    TipoOperacion.ReversionVenta => "Reversión de venta",
                    TipoOperacion.ReversionAbono => "Reversión de abono",
                    _ => Tipo.ToString()
                };
            }
        }

        public override string ToString()
        {
            return $"{Fecha:HH:mm} · {Codigo} · {NombreTipo} · S/ {Monto:0.00}";
        }
    }
}
