using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Estructuras
{
    public class ListaOperaciones
    {
        private Nodo<Operacion> primero;
        private Nodo<Operacion> ultimo;

        public int Contar { get; private set; }
        public bool EstaVacia => primero == null;

        public void Agregar(Operacion operacion)
        {
            if (operacion == null)
                throw new ArgumentNullException(nameof(operacion));

            var nuevo = new Nodo<Operacion>(operacion);

            if (primero == null)
            {
                primero = nuevo;
                ultimo = nuevo;
            }
            else
            {
                ultimo.Siguiente = nuevo;
                ultimo = nuevo;
            }
            Contar++;
        }

        public Operacion Ultima()
        {
            return ultimo?.Dato;
        }

        public List<Operacion> BuscarPorCodigo(string codigo)
        {
            var encontradas = new List<Operacion>();
            if (string.IsNullOrWhiteSpace(codigo))
                return encontradas;

            string buscado = codigo.Trim().ToUpper();
            Nodo<Operacion> actual = primero;

            while (actual != null)
            {
                if (actual.Dato.Codigo == buscado)
                    encontradas.Add(actual.Dato);
                actual = actual.Siguiente;
            }
            return encontradas;
        }

        public IEnumerable<Operacion> Recorrer()
        {
            Nodo<Operacion> actual = primero;
            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Siguiente;
            }
        }
    }
}
