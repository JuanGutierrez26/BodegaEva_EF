using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Servicios
{
    public static class FiadoServicio
    {
        public static List<Boleta> ListarFiadas()
        {
            var fiadas = new List<Boleta>();

            foreach (Boleta boleta in Datos.Boletas.Inorden())
            {
                if (boleta.Estado == EstadoBoleta.Fiada)
                    fiadas.Add(boleta);
            }
            return fiadas;
        }

        public static Boleta Buscar(string codigo)
        {
            return Datos.Boletas.Buscar(codigo);
        }

        public static Abono RegistrarAbono(string codigoBoleta, decimal monto)
        {
            Boleta boleta = Datos.Boletas.Buscar(codigoBoleta);

            if (boleta == null)
                throw new InvalidOperationException("No existe una boleta con ese código.");
            if (boleta.Estado != EstadoBoleta.Fiada)
                throw new InvalidOperationException("Esa boleta no tiene deuda pendiente.");

            var abono = new Abono(boleta.Codigo, boleta.Cliente, monto);

            boleta.AplicarAbono(monto);
            Datos.Abonos.Apilar(abono);
            Datos.Registro.Agregar(new Operacion(
                boleta.Codigo,
                TipoOperacion.Abono,
                $"Abono de {boleta.Cliente}",
                monto));

            Datos.NotificarCambio();
            return abono;
        }

        public static Abono UltimoAbono()
        {
            return Datos.Abonos.Tope();
        }

        public static int AbonosEnPila => Datos.Abonos.Contar;
    }
}
