using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Estructuras
{
    public class ArbolBoleta
    {
        private NodoArbol<Boleta> raiz;

        public int Contar { get; private set; }
        public bool EstaVacio => raiz == null;

        public void Insertar(Boleta boleta)
        {
            if (boleta == null)
                throw new ArgumentNullException(nameof(boleta));

            if (raiz == null)
            {
                raiz = new NodoArbol<Boleta>(boleta);
                Contar++;
                return;
            }

            NodoArbol<Boleta> actual = raiz;
            while (true)
            {
                int comparacion = boleta.CompareTo(actual.Dato);

                if (comparacion == 0)
                    throw new InvalidOperationException($"Ya existe una boleta con el código {boleta.Codigo}.");

                if (comparacion < 0)
                {
                    if (actual.Izquierdo == null)
                    {
                        actual.Izquierdo = new NodoArbol<Boleta>(boleta);
                        break;
                    }
                    actual = actual.Izquierdo;
                }
                else
                {
                    if (actual.Derecho == null)
                    {
                        actual.Derecho = new NodoArbol<Boleta>(boleta);
                        break;
                    }
                    actual = actual.Derecho;
                }
            }
            Contar++;
        }

        public Boleta Buscar(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            string buscado = codigo.Trim().ToUpper();
            NodoArbol<Boleta> actual = raiz;

            while (actual != null)
            {
                int comparacion = string.Compare(buscado, actual.Dato.Codigo, StringComparison.Ordinal);

                if (comparacion == 0)
                    return actual.Dato;

                actual = comparacion < 0 ? actual.Izquierdo : actual.Derecho;
            }
            return null;
        }

        public List<Boleta> Inorden()
        {
            var resultado = new List<Boleta>();
            RecorrerInorden(raiz, resultado);
            return resultado;
        }

        private void RecorrerInorden(NodoArbol<Boleta> nodo, List<Boleta> resultado)
        {
            if (nodo == null)
                return;

            RecorrerInorden(nodo.Izquierdo, resultado);
            resultado.Add(nodo.Dato);
            RecorrerInorden(nodo.Derecho, resultado);
        }

        public int Altura()
        {
            return CalcularAltura(raiz);
        }

        private int CalcularAltura(NodoArbol<Boleta> nodo)
        {
            if (nodo == null)
                return 0;

            return 1 + Math.Max(CalcularAltura(nodo.Izquierdo), CalcularAltura(nodo.Derecho));
        }
    }
}
