using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Servicios
{
    public static class IncidenciaServicio
    {
        public static Venta TopeVenta()
        {
            return Datos.Ventas.Tope();
        }

        public static Abono TopeAbono()
        {
            return Datos.Abonos.Tope();
        }

        public static int OperacionesEnPila => Datos.OperacionesEnPila;

        public static bool PuedeReversarVenta(out string motivo)
        {
            Venta venta = Datos.Ventas.Tope();

            if (venta == null)
            {
                motivo = "No hay ventas para reversar.";
                return false;
            }

            if (venta.FueFiada && venta.Boleta.DeudaPendiente != venta.Boleta.Total)
            {
                motivo = "La boleta tiene abonos registrados. Reversa primero los abonos.";
                return false;
            }

            motivo = "";
            return true;
        }

        public static bool PuedeReversarAbono(out string motivo)
        {
            if (Datos.Abonos.EstaVacia)
            {
                motivo = "No hay abonos para reversar.";
                return false;
            }

            motivo = "";
            return true;
        }

        public static Venta ReversarUltimaVenta()
        {
            if (!PuedeReversarVenta(out string motivo))
                throw new InvalidOperationException(motivo);

            Venta venta = Datos.Ventas.Desapilar();
            venta.Boleta.Anular();

            Datos.Registro.Agregar(new Operacion(
                venta.Boleta.Codigo,
                TipoOperacion.ReversionVenta,
                $"Reversión de venta de {venta.Cliente}",
                venta.Total));

            Datos.NotificarCambio();
            return venta;
        }

        public static Abono ReversarUltimoAbono()
        {
            if (!PuedeReversarAbono(out string motivo))
                throw new InvalidOperationException(motivo);

            Abono abono = Datos.Abonos.Tope();
            Boleta boleta = Datos.Boletas.Buscar(abono.CodigoBoleta);

            if (boleta == null)
                throw new InvalidOperationException("No se encontró la boleta del abono.");

            boleta.RevertirAbono(abono.Monto);
            Datos.Abonos.Desapilar();

            Datos.Registro.Agregar(new Operacion(
                boleta.Codigo,
                TipoOperacion.ReversionAbono,
                $"Reversión de abono de {abono.Cliente}",
                abono.Monto));

            Datos.NotificarCambio();
            return abono;
        }
    }
}
