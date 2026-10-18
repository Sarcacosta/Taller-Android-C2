using UsuariosApi.DTOs;
using UsuariosApi.Entities;
using UsuariosApi.Interfaces;

namespace UsuariosApi.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;

    public UsuarioService(IUsuarioRepository repo)
    {
        _repo = repo;
    }

    // ==========================================
    // OBTENER TODOS
    // ==========================================
    public async Task<List<UsuarioDto>> ObtenerTodosAsync()
    {
        var usuarios = await _repo.ObtenerTodosAsync();

        return usuarios
            .Select(Map)
            .ToList();
    }

    // ==========================================
    // OBTENER POR ID
    // ==========================================
    public async Task<UsuarioDto?> ObtenerPorIdAsync(int id)
    {
        var usuario = await _repo.ObtenerPorIdAsync(id);

        return usuario is null
            ? null
            : Map(usuario);
    }

    // ==========================================
    // CREAR USUARIO
    // ==========================================
    public async Task<UsuarioDto> CrearAsync(CrearUsuarioDto d)
    {
        var correo = d.Correo
            .Trim()
            .ToLowerInvariant();

        // Validar que el correo no esté registrado
        if (await _repo.ObtenerPorCorreoAsync(correo) is not null)
        {
            throw new InvalidOperationException(
                "Ya existe un usuario con ese correo."
            );
        }

        ValidarDatos(d.FechaNacimiento, d.Nombre, d.Apellido, d.Universidad);
        if (System.Text.Encoding.UTF8.GetByteCount(d.Password) > 72)
            throw new InvalidOperationException("La contraseña no debe superar 72 bytes UTF-8.");

        // Convertir contraseña en HASH
        var passwordHash =
            BCrypt.Net.BCrypt.HashPassword(d.Password);

        var usuario = new Usuario
        {
            Nombre = d.Nombre.Trim(),
            Apellido = d.Apellido.Trim(),
            Correo = correo,
            PasswordHash = passwordHash,
            FechaNacimiento = d.FechaNacimiento!.Value,
            Universidad = d.Universidad.Trim(),
            Semestre = d.Semestre
        };

        await _repo.CrearAsync(usuario);

        return Map(usuario);
    }

    // ==========================================
    // LOGIN
    // ==========================================
    public async Task<UsuarioDto?> LoginAsync(LoginDto dto)
    {
        var correo = dto.Correo
            .Trim()
            .ToLowerInvariant();

        // Buscar usuario por correo
        var usuario =
            await _repo.ObtenerPorCorreoAsync(correo);

        // Usuario no encontrado
        if (usuario is null)
        {
            return null;
        }

        // Validar que esté activo
        if (!usuario.Activo)
        {
            return null;
        }

        // Validar que tenga contraseña registrada
        if (string.IsNullOrWhiteSpace(usuario.PasswordHash))
        {
            return null;
        }

        // Comparar la contraseña ingresada
        // con el HASH almacenado en la BD
        var passwordCorrecto =
            BCrypt.Net.BCrypt.Verify(
                dto.Password,
                usuario.PasswordHash
            );

        // Contraseña incorrecta
        if (!passwordCorrecto)
        {
            return null;
        }

        // LOGIN CORRECTO
        return Map(usuario);
    }

    // ==========================================
    // ACTUALIZAR
    // ==========================================
    public async Task<bool> ActualizarAsync(
        int id,
        ActualizarUsuarioDto d)
    {
        var usuario =
            await _repo.ObtenerPorIdAsync(id);

        if (usuario is null)
        {
            return false;
        }

        var correo = d.Correo
            .Trim()
            .ToLowerInvariant();

        var otro =
            await _repo.ObtenerPorCorreoAsync(correo);

        if (otro is not null &&
            otro.Id != id)
        {
            throw new InvalidOperationException(
                "El correo pertenece a otro usuario."
            );
        }

        usuario.Nombre = d.Nombre.Trim();
        usuario.Apellido = d.Apellido.Trim();
        usuario.Correo = correo;
        ValidarDatos(d.FechaNacimiento, d.Nombre, d.Apellido, d.Universidad);
        usuario.FechaNacimiento = d.FechaNacimiento!.Value;
        usuario.Universidad = d.Universidad.Trim();
        usuario.Semestre = d.Semestre;
        usuario.Activo = d.Activo;

        await _repo.ActualizarAsync(usuario);

        return true;
    }

    // ==========================================
    // ELIMINAR
    // ==========================================
    public async Task<bool> EliminarAsync(int id)
    {
        var usuario =
            await _repo.ObtenerPorIdAsync(id);

        if (usuario is null)
        {
            return false;
        }

        await _repo.EliminarAsync(usuario);

        return true;
    }

    // ==========================================
    // MAPEO ENTITY -> DTO
    // ==========================================
    public static int CalcularEdad(DateOnly nacimiento, DateOnly hoy)
    {
        var edad = hoy.Year - nacimiento.Year;
        if (hoy.Month < nacimiento.Month ||
            (hoy.Month == nacimiento.Month && hoy.Day < nacimiento.Day)) edad--;
        return edad;
    }

    private static void ValidarDatos(DateOnly? fecha, string nombre, string apellido, string universidad)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        if (fecha is null || fecha > hoy || CalcularEdad(fecha.Value, hoy) > 120)
            throw new InvalidOperationException("Seleccione una fecha de nacimiento válida, no futura y con edad hasta 120 años.");
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(universidad))
            throw new InvalidOperationException("Nombre, apellido y universidad son obligatorios.");
    }

    private static UsuarioDto Map(Usuario u)
    {
        return new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Correo = u.Correo,
            FechaNacimiento = u.FechaNacimiento,
            Universidad = u.Universidad,
            Semestre = u.Semestre,
            Edad = CalcularEdad(u.FechaNacimiento, DateOnly.FromDateTime(DateTime.Today)),
            Activo = u.Activo,
            FechaCreacion = u.FechaCreacion
        };
    }
}