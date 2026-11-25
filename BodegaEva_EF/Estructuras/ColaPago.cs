using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Estructuras
{
    public class ColaPago
    {
        private Nodo<Pago> frente;
        private Nodo<Pago> final;

        public int Contar { get; private set; }
        public bool EstaVacia => frente == null;

        public void Encolar(Pago pago)
        {
            if (pago == null)
                throw new ArgumentNullException(nameof(pago));
            if (Contiene(pago.Codigo))
                throw new InvalidOperationException($"El pago {pago.Codigo} ya está en la cola.");

            var nuevo = new Nodo<Pago>(pago);

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

        public Pago Frente()
        {
            return frente?.Dato;
        }

        public Pago Desencolar()
        {
            if (frente == null)
                throw new InvalidOperationException("No hay pagos en la cola.");

            Pago pago = frente.Dato;
            frente = frente.Siguiente;

            if (frente == null)
                final = null;

            Contar--;
            return pago;
        }

        public bool Retirar(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            string buscado = codigo.Trim().ToUpper();
            Nodo<Pago> anterior = null;
            Nodo<Pago> actual = frente;

            while (actual != null)
            {
                if (actual.Dato.Codigo == buscado)
                {
                    if (anterior == null)
                        frente = actual.Siguiente;
                    else
                        anterior.Siguiente = actual.Siguiente;

                    if (actual == final)
                        final = anterior;

                    Contar--;
                    return true;
                }
                anterior = actual;
                actual = actual.Siguiente;
            }
            return false;
        }

        public bool Contiene(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            string buscado = codigo.Trim().ToUpper();
            Nodo<Pago> actual = frente;

            while (actual != null)
            {
                if (actual.Dato.Codigo == buscado)
                    return true;
                actual = actual.Siguiente;
            }
            return false;
        }

        public IEnumerable<Pago> Recorrer()
        {
            Nodo<Pago> actual = frente;
            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Siguiente;
            }
        }
    }
}
