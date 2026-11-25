using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Estructuras
{
    public class PilaVentas
    {
        private Nodo<Venta> tope;

        public int Contar { get; private set; }
        public bool EstaVacia => tope == null;

        public void Apilar(Venta venta)
        {
            if (venta == null)
                throw new ArgumentNullException(nameof(venta));

            var nuevo = new Nodo<Venta>(venta);
            nuevo.Siguiente = tope;
            tope = nuevo;
            Contar++;
        }

        public Venta Tope()
        {
            return tope?.Dato;
        }

        public Venta Desapilar()
        {
            if (tope == null)
                throw new InvalidOperationException("No hay ventas para reversar.");

            Venta venta = tope.Dato;
            tope = tope.Siguiente;
            Contar--;
            return venta;
        }

        public IEnumerable<Venta> Recorrer()
        {
            Nodo<Venta> actual = tope;
            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Siguiente;
            }
        }
    }
}
