using System.ComponentModel.DataAnnotations;

namespace CanguroDTOs.Shared.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        public string Clave { get; set; }

    }
}
