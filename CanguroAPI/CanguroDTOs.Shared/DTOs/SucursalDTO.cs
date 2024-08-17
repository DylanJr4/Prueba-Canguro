namespace CanguroDTOs.Shared.DTOs
{
    public class SucursalDTO
    {
        public int? IdSucursal { get; set; }
        public string Descripcion { get; set; } = null!;

        public string Direccion { get; set; } = null!;

        public string Identificacion { get; set; } = null!;

        public DateTime? FechaCreacion { get; set; }

        public int? IdMoneda { get; set; }
        public string? NombreMoneda { get; set; }
    }
}
