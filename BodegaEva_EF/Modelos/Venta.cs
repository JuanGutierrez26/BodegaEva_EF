using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Modelos
{
    public class Venta
    {
        public Pedido Pedido { get; }
        public Boleta Boleta { get; }
        public DateTime Fecha { get; }

        public string Cliente => Boleta.Cliente;
        public decimal Total => Boleta.Total;
        public bool FueFiada { get; }

        public Venta(Pedido pedido, Boleta boleta)
        {
            if (pedido == null)
                throw new ArgumentNullException(nameof(pedido));
            if (boleta == null)
                throw new ArgumentNullException(nameof(boleta));
            if (pedido.Total != boleta.Total)
                throw new ArgumentException("El total de la boleta no coincide con el del pedido.");

            Pedido = pedido;
            Boleta = boleta;
            FueFiada = boleta.Estado == EstadoBoleta.Fiada;
            Fecha = DateTime.Now;
        }

        public string Descripcion =>
            $"{Boleta.Codigo} · {Cliente}\n{Pedido.Resumen} · Total: S/ {Total:0.00} · {Boleta.Estado}";

        public override string ToString()
        {
            return $"{Boleta.Codigo} · {Cliente} · S/ {Total:0.00}";
        }
    }
}