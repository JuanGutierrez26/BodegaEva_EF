using BodegaEva_EF.Estructuras;
using BodegaEva_EF.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Servicios
{
    public static class Datos
    {
        private static int contadorBoletas = 0;

        public static ColaPedido ColaPresencial { get; private set; } = new ColaPedido(TipoPedido.Presencial);
        public static ColaPedido ColaWhatsApp { get; private set; } = new ColaPedido(TipoPedido.WhatsApp);
        public static ColaPago ColaPagos { get; private set; } = new ColaPago();

        public static PilaVentas Ventas { get; private set; } = new PilaVentas();
        public static PilaAbonos Abonos { get; private set; } = new PilaAbonos();

        public static ArbolBoleta Boletas { get; private set; } = new ArbolBoleta();
        public static ListaOperaciones Registro { get; private set; } = new ListaOperaciones();

        public static int OperacionesEnPila => Ventas.Contar + Abonos.Contar;

        public static event Action Cambio;

        public static void NotificarCambio()
        {
            Cambio?.Invoke();
        }

        public static string GenerarCodigoBoleta()
        {
            contadorBoletas++;
            return $"BOL-{contadorBoletas:0000}";
        }

        public static void Reiniciar()
        {
            contadorBoletas = 0;
            ColaPresencial = new ColaPedido(TipoPedido.Presencial);
            ColaWhatsApp = new ColaPedido(TipoPedido.WhatsApp);
            ColaPagos = new ColaPago();
            Ventas = new PilaVentas();
            Abonos = new PilaAbonos();
            Boletas = new ArbolBoleta();
            Registro = new ListaOperaciones();
            NotificarCambio();
        }
    }
}
