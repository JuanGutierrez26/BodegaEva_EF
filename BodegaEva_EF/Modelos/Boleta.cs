using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Modelos
{
    public class Boleta : IComparable<Boleta>
    {
        public string Codigo { get; }
        public string Cliente { get; }
        public string Detalle { get; }
        public decimal Total { get; }
        public DateTime Fecha { get; }

        public decimal DeudaPendiente { get; private set; }
        public EstadoBoleta Estado { get; private set; }

        public Boleta(string codigo, string cliente, string detalle, decimal total, bool fiado)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código de la boleta es obligatorio.");
            if (string.IsNullOrWhiteSpace(cliente))
                throw new ArgumentException("El cliente es obligatorio.");
            if (total <= 0)
                throw new ArgumentException("El total debe ser mayor que cero.");

            Codigo = codigo.Trim().ToUpper();
            Cliente = cliente.Trim();
            Detalle = detalle?.Trim() ?? "";
            Total = total;
            Fecha = DateTime.Now;

            Estado = fiado ? EstadoBoleta.Fiada : EstadoBoleta.Pagada;
            DeudaPendiente = fiado ? total : 0;
        }

        public void AplicarAbono(decimal monto)
        {
            if (Estado != EstadoBoleta.Fiada)
                throw new InvalidOperationException("Solo se puede abonar a una boleta fiada.");
            if (monto <= 0)
                throw new ArgumentException("El abono debe ser mayor que cero.");
            if (monto > DeudaPendiente)
                throw new ArgumentException($"El abono no puede superar la deuda (S/ {DeudaPendiente:0.00}).");

            DeudaPendiente -= monto;
            if (DeudaPendiente == 0)
                Estado = EstadoBoleta.Pagada;
        }

        public void RevertirAbono(decimal monto)
        {
            if (Estado == EstadoBoleta.Anulada)
                throw new InvalidOperationException("La boleta está anulada.");
            if (monto <= 0 || DeudaPendiente + monto > Total)
                throw new ArgumentException("El monto a revertir no es válido.");

            DeudaPendiente += monto;
            Estado = EstadoBoleta.Fiada;
        }

        public void Anular()
        {
            Estado = EstadoBoleta.Anulada;
            DeudaPendiente = 0;
        }

        public int CompareTo(Boleta otra)
        {
            return string.Compare(Codigo, otra.Codigo, StringComparison.Ordinal);
        }

        public override string ToString()
        {
            return $"{Codigo} · {Cliente} · S/ {Total:0.00}";
        }
    }
}
