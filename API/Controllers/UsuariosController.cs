using Microsoft.AspNetCore.Mvc;
using UsuariosApi.DTOs;
using UsuariosApi.Interfaces;

namespace UsuariosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service)
    {
        _service = service;
    }

    // ==========================================
    // GET - OBTENER TODOS LOS USUARIOS
    // ==========================================
    [HttpGet]
    public async Task<ActionResult<List<UsuarioDto>>> ObtenerTodos()
    {
        var usuarios = await _service.ObtenerTodosAsync();

        return Ok(usuarios);
    }

    // ==========================================
    // GET - OBTENER USUARIO POR ID
    // ==========================================
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorId(int id)
    {
        var usuario = await _service.ObtenerPorIdAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        return Ok(usuario);
    }

    // ==========================================
    // POST - CREAR USUARIO
    // ==========================================
    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> Crear(
        CrearUsuarioDto dto)
    {
        try
        {
            var usuario =
                await _service.CrearAsync(dto);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = usuario.Id },
                usuario
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                new
                {
                    mensaje = ex.Message
                }
            );
        }
    }

    // ==========================================
    // POST - LOGIN
    // ==========================================
    [HttpPost("login")]
    public async Task<ActionResult<UsuarioDto>> Login(
        LoginDto dto)
    {
        var usuario =
            await _service.LoginAsync(dto);

        if (usuario is null)
        {
            return Unauthorized(
                new
                {
                    mensaje = "Correo o contraseña incorrectos."
                }
            );
        }

        return Ok(usuario);
    }

    // ==========================================
    // PUT - ACTUALIZAR USUARIO
    // ==========================================
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        ActualizarUsuarioDto dto)
    {
        try
        {
            var actualizado =
                await _service.ActualizarAsync(id, dto);

            if (!actualizado)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                new
                {
                    mensaje = ex.Message
                }
            );
        }
    }

    // ==========================================
    // DELETE - ELIMINAR USUARIO
    // ==========================================
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var eliminado =
            await _service.EliminarAsync(id);

        if (!eliminado)
        {
            return NotFound();
        }

        return NoContent();
    }
}