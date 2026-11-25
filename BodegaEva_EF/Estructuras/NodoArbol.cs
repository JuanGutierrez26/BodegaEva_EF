using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Estructuras
{
    public class NodoArbol<T>
    {
        public T Dato { get; set; }
        public NodoArbol<T> Izquierdo { get; set; }
        public NodoArbol<T> Derecho { get; set; }

        public NodoArbol(T dato)
        {
            Dato = dato;
            Izquierdo = null;
            Derecho = null;
        }
    }
}
