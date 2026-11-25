using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Estructuras
{
    public class PilaAbonos
    {
        private Nodo<Abono> tope;

        public int Contar { get; private set; }
        public bool EstaVacia => tope == null;

        public void Apilar(Abono abono)
        {
            if (abono == null)
                throw new ArgumentNullException(nameof(abono));

            var nuevo = new Nodo<Abono>(abono);
            nuevo.Siguiente = tope;
            tope = nuevo;
            Contar++;
        }

        public Abono Tope()
        {
            return tope?.Dato;
        }

        public Abono Desapilar()
        {
            if (tope == null)
                throw new InvalidOperationException("No hay abonos para reversar.");

            Abono abono = tope.Dato;
            tope = tope.Siguiente;
            Contar--;
            return abono;
        }

        public IEnumerable<Abono> Recorrer()
        {
            Nodo<Abono> actual = tope;
            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Siguiente;
            }
        }
    }
}
