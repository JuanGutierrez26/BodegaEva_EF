using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BodegaEva_EF.Utilidades
{
    public static class Validaciones
    {
        public const int MaxCantidad = 999;
        public const decimal MaxPrecio = 9999.99m;

        private static readonly Regex PatronNombre = new Regex(@"^[\p{L}][\p{L} .'\-]*$");
        private static readonly Regex PatronCodigoBoleta = new Regex(@"^BOL-\d{4}$");

        public static string Cliente(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe el nombre del cliente.";

            string limpio = texto.Trim();

            if (limpio.Length < 2)
                return "El nombre es demasiado corto.";
            if (limpio.Length > 60)
                return "El nombre no puede superar los 60 caracteres.";
            if (!PatronNombre.IsMatch(limpio))
                return "El nombre solo puede tener letras, espacios, punto y guion.";

            return null;
        }

        public static string NombreProducto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe el nombre del producto.";
            if (texto.Trim().Length > 60)
                return "El producto no puede superar los 60 caracteres.";

            return null;
        }

        public static string Cantidad(string texto, out int cantidad)
        {
            cantidad = 0;

            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe la cantidad.";
            if (!Formato.TryLeerEntero(texto, out cantidad))
                return "La cantidad debe ser un número entero.";
            if (cantidad <= 0)
                return "La cantidad debe ser mayor que cero.";
            if (cantidad > MaxCantidad)
                return $"La cantidad no puede superar {MaxCantidad}.";

            return null;
        }

        public static string Precio(string texto, out decimal precio)
        {
            precio = 0;

            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe el precio.";
            if (!Formato.TryLeerDecimal(texto, out precio))
                return "Escribe un precio válido, por ejemplo 10.50.";
            if (precio <= 0)
                return "El precio debe ser mayor que cero.";
            if (precio > MaxPrecio)
                return $"El precio no puede superar {Formato.Moneda(MaxPrecio)}.";

            return null;
        }

        public static string Abono(string texto, decimal deudaPendiente, out decimal monto)
        {
            monto = 0;

            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe el monto del abono.";
            if (!Formato.TryLeerDecimal(texto, out monto))
                return "Escribe un monto válido, por ejemplo 20.00.";
            if (monto <= 0)
                return "El abono debe ser mayor que cero.";
            if (monto > deudaPendiente)
                return $"El abono no puede superar la deuda ({Formato.Moneda(deudaPendiente)}).";

            return null;
        }

        public static string CodigoBoleta(string texto, out string codigo)
        {
            codigo = Formato.NormalizarCodigo(texto);

            if (codigo.Length == 0)
                return "Escribe el código de la boleta.";
            if (!PatronCodigoBoleta.IsMatch(codigo))
                return "Usa el formato BOL-0039.";

            return null;
        }

        public static string CodigoPago(string texto, out string codigo)
        {
            codigo = Formato.NormalizarCodigo(texto);

            if (codigo.Length == 0)
                return "Escribe el código de pago.";
            if (codigo.Length > 20)
                return "El código de pago es demasiado largo.";

            return null;
        }

        public static string MontoPago(string texto, out decimal monto)
        {
            monto = 0;

            if (string.IsNullOrWhiteSpace(texto))
                return "Escribe el monto del pago.";
            if (!Formato.TryLeerDecimal(texto, out monto))
                return "Escribe un monto válido, por ejemplo 37.50.";
            if (monto <= 0)
                return "El monto debe ser mayor que cero.";

            return null;
        }

        public static bool Mostrar(Label etiqueta, string error)
        {
            etiqueta.Text = error ?? "";
            etiqueta.ForeColor = Tema.Error;
            etiqueta.Visible = error != null;
            return error == null;
        }
    }
}
