using BodegaEva_EF.Estructuras;
using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Servicios
{
    public static class VentaServicio
    {
        public static Pedido PedidoEnCurso { get; private set; }
        public static bool PagoValidado { get; private set; }

        public static int PedidosEnEspera =>
            Datos.ColaPresencial.Contar + Datos.ColaWhatsApp.Contar;

        public static Pedido RegistrarPedido(string cliente, TipoPedido tipo, ListaProductos productos)
        {
            var pedido = new Pedido(cliente, tipo, productos);

            if (tipo == TipoPedido.Presencial)
                Datos.ColaPresencial.Encolar(pedido);
            else
                Datos.ColaWhatsApp.Encolar(pedido);

            Datos.NotificarCambio();
            return pedido;
        }

        public static Pedido DespacharSiguiente()
        {
            if (PedidoEnCurso != null)
                throw new InvalidOperationException("Termina o descarta el pedido en curso antes de despachar otro.");

            if (Datos.ColaPresencial.EstaVacia && Datos.ColaWhatsApp.EstaVacia)
                throw new InvalidOperationException("No hay pedidos en espera.");

            Pedido siguiente = !Datos.ColaPresencial.EstaVacia
                ? Datos.ColaPresencial.Desencolar()
                : Datos.ColaWhatsApp.Desencolar();

            PedidoEnCurso = siguiente;
            PagoValidado = false;

            Datos.NotificarCambio();
            return siguiente;
        }

        public static Pedido DescartarPorFaltaDeStock()
        {
            if (PedidoEnCurso == null)
                throw new InvalidOperationException("No hay un pedido en curso.");

            Pedido descartado = PedidoEnCurso;
            PedidoEnCurso = null;
            PagoValidado = false;

            Datos.NotificarCambio();
            return descartado;
        }

        public static Pago EncolarPago(string codigo, decimal monto)
        {
            if (PedidoEnCurso == null)
                throw new InvalidOperationException("No hay un pedido en curso.");

            var pago = new Pago(codigo, monto);
            Datos.ColaPagos.Encolar(pago);

            Datos.NotificarCambio();
            return pago;
        }

        public static bool ValidarSiguientePago(out string motivo)
        {
            if (PedidoEnCurso == null)
                throw new InvalidOperationException("No hay un pedido en curso.");

            Pago pago = Datos.ColaPagos.Frente();
            if (pago == null)
                throw new InvalidOperationException("No hay pagos en la cola.");

            if (pago.Validar(PedidoEnCurso.Total, out motivo))
            {
                Datos.ColaPagos.Desencolar();
                PagoValidado = true;
                Datos.NotificarCambio();
                return true;
            }

            Datos.ColaPagos.Retirar(pago.Codigo);
            PagoValidado = false;
            Datos.NotificarCambio();
            return false;
        }

        public static Venta CerrarVenta(bool fiado)
        {
            if (PedidoEnCurso == null)
                throw new InvalidOperationException("No hay un pedido en curso.");
            if (!fiado && !PagoValidado)
                throw new InvalidOperationException("El pago aún no ha sido validado.");

            Pedido pedido = PedidoEnCurso;
            var boleta = new Boleta(
                Datos.GenerarCodigoBoleta(),
                pedido.Cliente,
                pedido.Resumen,
                pedido.Total,
                fiado);
            var venta = new Venta(pedido, boleta);

            Datos.Boletas.Insertar(boleta);
            Datos.Ventas.Apilar(venta);
            Datos.Registro.Agregar(new Operacion(
                boleta.Codigo,
                fiado ? TipoOperacion.VentaFiada : TipoOperacion.Venta,
                pedido.Resumen,
                pedido.Total));

            PedidoEnCurso = null;
            PagoValidado = false;

            Datos.NotificarCambio();
            return venta;
        }

        public static void LimpiarEstado()
        {
            PedidoEnCurso = null;
            PagoValidado = false;
        }
    }
}
