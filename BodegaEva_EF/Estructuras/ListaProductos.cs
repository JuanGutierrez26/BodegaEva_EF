using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Estructuras
{
    public class ListaProductos
    {
        private Nodo<Producto> primero;
        private Nodo<Producto> ultimo;

        public int Contar { get; private set; }
        public bool EstaVacia => primero == null;

        public void Agregar(Producto producto)
        {
            if (producto == null)
                throw new ArgumentNullException(nameof(producto));

            var nuevo = new Nodo<Producto>(producto);

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

        public bool Quitar(string nombre)
        {
            Nodo<Producto> anterior = null;
            Nodo<Producto> actual = primero;

            while (actual != null)
            {
                if (string.Equals(actual.Dato.Nombre, nombre, StringComparison.OrdinalIgnoreCase))
                {
                    if (anterior == null)
                        primero = actual.Siguiente;
                    else
                        anterior.Siguiente = actual.Siguiente;

                    if (actual == ultimo)
                        ultimo = anterior;

                    Contar--;
                    return true;
                }
                anterior = actual;
                actual = actual.Siguiente;
            }
            return false;
        }

        public decimal CalcularTotal()
        {
            decimal total = 0;
            Nodo<Producto> actual = primero;
            while (actual != null)
            {
                total += actual.Dato.Subtotal;
                actual = actual.Siguiente;
            }
            return total;
        }

        public string ObtenerResumen()
        {
            var partes = new List<string>();
            Nodo<Producto> actual = primero;
            while (actual != null)
            {
                partes.Add(actual.Dato.ToString());
                actual = actual.Siguiente;
            }
            return string.Join(", ", partes);
        }

        public IEnumerable<Producto> Recorrer()
        {
            Nodo<Producto> actual = primero;
            while (actual != null)
            {
                yield return actual.Dato;
                actual = actual.Siguiente;
            }
        }
    }
}
