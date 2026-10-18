using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using UsuariosApi.Data;
using UsuariosApi.Entities;
using UsuariosApi.Interfaces;

namespace UsuariosApi.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Usuario>> ObtenerTodosAsync()
    {
        return _context.Usuarios
            .AsNoTracking()
            .OrderBy(x => x.Nombre)
            .ToListAsync();
    }

    public Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        return _context.Usuarios
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<Usuario?> ObtenerPorCorreoAsync(string correo)
    {
        return _context.Usuarios
            .FirstOrDefaultAsync(x => x.Correo == correo);
    }

    public async Task<Usuario> CrearAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new InvalidOperationException("Ya existe un usuario con ese correo.", ex);
        }

        return usuario;
    }

    public async Task ActualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new InvalidOperationException("Ya existe un usuario con ese correo.", ex);
        }
    }

    public async Task EliminarAsync(Usuario usuario)
    {
        _context.Usuarios.Remove(usuario);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlException { Number: 1062 })
        {
            throw new InvalidOperationException("Ya existe un usuario con ese correo.", ex);
        }
    }
}