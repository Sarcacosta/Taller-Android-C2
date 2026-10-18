using Microsoft.EntityFrameworkCore;
using UsuariosApi.Data;
using UsuariosApi.Interfaces;
using UsuariosApi.Repositories;
using UsuariosApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
        {
            mensaje = "Revise los datos enviados.",
            errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(x => x.Key, x => x.Value!.Errors.Select(e =>
                    string.IsNullOrEmpty(e.ErrorMessage) ? "Valor inválido." : e.ErrorMessage).ToArray())
        });
});

var connectionString =
    builder.Configuration.GetConnectionString("MySql");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0))
    ));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Para la práctica local con el emulador Android
// evitamos redireccionar HTTP hacia HTTPS.
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();