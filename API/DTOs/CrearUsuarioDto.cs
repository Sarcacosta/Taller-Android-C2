using System.ComponentModel.DataAnnotations;

namespace UsuariosApi.DTOs;

public class CrearUsuarioDto
{
    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(72)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public DateOnly? FechaNacimiento { get; set; }

    [Required, MaxLength(150)]
    public string Universidad { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Semestre { get; set; }


}