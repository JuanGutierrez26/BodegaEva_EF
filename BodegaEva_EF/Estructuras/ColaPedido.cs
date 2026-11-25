using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Estructuras
{
    public class ColaPedido
    {
        private Nodo<Pedido> frente;
        private Nodo<Pedido> final;

        public TipoPedido TipoAceptado { get; }
        public int Contar { get; private set; }
        public bool EstaVacia => frente == null;

        public ColaPedido(TipoPedido tipoAceptado)
        {
            TipoAceptado = tipoAceptado;
        }

        public void Encolar(Pedido pedido)
        {
            if (pedido == null)
                throw new ArgumentNullException(nameof(pedido));
            if (pedido.Tipo != TipoAceptado)
                throw new InvalidOperationException(
                    $"Esta cola solo acepta pedidos {TipoAceptado}.");

            var nuevo = new Nodo<Pedido>(pedido);

            if (final == null)
            {
                frente = nuevo;
                final = nuevo;
            }
            else
            {
                final.Siguiente = nuevo;
                final = nuevo;
            }
            Contar++;
        }

        public Pedido Frente()
        {
            return frente?.Dato;
        }

        public Pedido Desencolar()
        {
            if (frente == null)
                throw new InvalidOperationException("No hay pedidos en la cola.");

            Pedido pedido = frente.Dato;
            frente = frente.Siguiente;

            if (frente == null)
                final = null;

            Contar--;
            return pedido;
        }

        public IEnumerable<Pedido> Recorrer()
        {
            Nodo<Pedido> actual = frente;
            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Siguiente;
            }
        }
    }
}
