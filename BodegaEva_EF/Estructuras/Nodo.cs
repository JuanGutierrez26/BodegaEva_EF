using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Estructuras
{
    public class Nodo<T>
    {
        public T Dato { get; set; }
        public Nodo<T> Siguiente { get; set; }

        public Nodo(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}
