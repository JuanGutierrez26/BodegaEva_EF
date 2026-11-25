namespace BodegaEva_EF.Modelos
{
    public class Producto
    {
        public string Nombre { get; }
        public int Cantidad { get; }
        public decimal Precio { get; }

        public decimal Subtotal => Cantidad * Precio;

        public Producto(string nombre, int cantidad, decimal precio)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del producto es obligatorio.");
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero.");
            if (precio <= 0)
                throw new ArgumentException("El precio debe ser mayor que cero.");

            Nombre = nombre.Trim();
            Cantidad = cantidad;
            Precio = precio;
        }

        public override string ToString()
        {
            return Cantidad == 1 ? Nombre : $"{Nombre} ×{Cantidad}";
        }
    }
}