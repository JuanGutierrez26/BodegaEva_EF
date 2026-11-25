using BodegaEva_EF.Estructuras;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Modelos
{
    public class Pedido
    {
        public string Cliente { get; }
        public TipoPedido Tipo { get; }
        public ListaProductos Productos { get; }
        public DateTime Fecha { get; }

        public decimal Total => Productos.CalcularTotal();
        public string Resumen => Productos.ObtenerResumen();

        public Pedido(string cliente, TipoPedido tipo, ListaProductos productos)
        {
            if (string.IsNullOrWhiteSpace(cliente))
                throw new ArgumentException("El cliente es obligatorio.");
            if (productos == null || productos.EstaVacia)
                throw new ArgumentException("El pedido debe tener al menos un producto.");

            Cliente = cliente.Trim();
            Tipo = tipo;
            Productos = productos;
            Fecha = DateTime.Now;
        }

        public override string ToString()
        {
            string origen = Tipo == TipoPedido.Presencial ? "Presencial" : "WhatsApp";
            return $"{Cliente} · {origen} · {Productos.Contar} prod. · S/ {Total:0.00}";
        }
    }
}
