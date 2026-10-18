using System.ComponentModel.DataAnnotations;

namespace UsuariosApi.DTOs;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}