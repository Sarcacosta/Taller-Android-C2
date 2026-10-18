using UsuariosApi.DTOs;

namespace UsuariosApi.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> ObtenerTodosAsync();

    Task<UsuarioDto?> ObtenerPorIdAsync(int id);

    Task<UsuarioDto> CrearAsync(CrearUsuarioDto dto);

    Task<bool> ActualizarAsync(
        int id,
        ActualizarUsuarioDto dto
    );

    Task<bool> EliminarAsync(int id);

    // LOGIN
    Task<UsuarioDto?> LoginAsync(LoginDto dto);
}