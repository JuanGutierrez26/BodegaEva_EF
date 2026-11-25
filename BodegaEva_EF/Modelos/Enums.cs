using System;
using System.Collections.Generic;
using System.Text;

namespace BodegaEva_EF.Modelos
{
    public enum TipoPedido
    {
        Presencial,
        WhatsApp
    }
    public enum EstadoBoleta
    {
        Pagada,
        Fiada,
        Anulada
    }
    public enum TipoOperacion
    {
        Venta,
        VentaFiada,
        Abono,
        ReversionVenta,
        ReversionAbono
    }
}
