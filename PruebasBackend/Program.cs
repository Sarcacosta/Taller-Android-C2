using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using UsuariosApi.Controllers;
using UsuariosApi.Data;
using UsuariosApi.DTOs;
using UsuariosApi.Interfaces;
using UsuariosApi.Repositories;
using UsuariosApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseTestServer();
builder.Logging.ClearProviders();
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase("taller-tests"));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddControllers().AddApplicationPart(typeof(UsuariosController).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
await using var app = builder.Build();
app.MapControllers();
app.UseSwagger();
await app.StartAsync();
var client = app.GetTestClient();
var results = new List<string>();
void Check(string name, bool passed) { if (!passed) throw new Exception(name); results.Add("OK " + name); }
var request = new CrearUsuarioDto { Nombre="Alexander", Apellido="Pérez", Correo="alexander.taller@example.com", Password="Taller2026!", FechaNacimiento=new DateOnly(2005,6,15), Universidad="UCompensar", Semestre=3 };
var response = await client.PostAsJsonAsync("/api/Usuarios", request);
Check("Registro HTTP 201", response.StatusCode == HttpStatusCode.Created);
var raw = await response.Content.ReadAsStringAsync();
var user = JsonSerializer.Deserialize<UsuarioDto>(raw, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
Check("Datos nuevos en respuesta", user.FechaNacimiento == request.FechaNacimiento && user.Universidad=="UCompensar" && user.Semestre==3);
Check("Sin contraseña ni hash en respuesta", !raw.Contains("password", StringComparison.OrdinalIgnoreCase));
var evidence = Path.GetFullPath("EvidenciasTecnicas"); Directory.CreateDirectory(evidence);
await File.WriteAllTextAsync(Path.Combine(evidence,"registro_201.json"),raw);
request.Correo=" ALEXANDER.TALLER@EXAMPLE.COM ";
// Las validaciones HTTP exigen un correo bien formado; la normalización se prueba también directamente en servicio.
using (var scope = app.Services.CreateScope()) {
    var service = scope.ServiceProvider.GetRequiredService<IUsuarioService>();
    try { await service.CrearAsync(request); Check("Correo normalizado duplicado",false); }
    catch (InvalidOperationException ex) { Check("Correo normalizado duplicado",ex.Message=="Ya existe un usuario con ese correo."); }
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var entity = await db.Usuarios.SingleAsync();
    Check("BCrypt guardado y verificable",entity.PasswordHash != request.Password && BCrypt.Net.BCrypt.Verify(request.Password,entity.PasswordHash));
}
request.Correo="alexander.taller@example.com";
response=await client.PostAsJsonAsync("/api/Usuarios",request);
raw=await response.Content.ReadAsStringAsync();
Check("Duplicado HTTP 400 con mensaje correcto",response.StatusCode==HttpStatusCode.BadRequest && raw.Contains("Ya existe un usuario con ese correo."));
await File.WriteAllTextAsync(Path.Combine(evidence,"correo_duplicado_400.json"),raw);
response=await client.PostAsJsonAsync("/api/Usuarios/login",new LoginDto { Correo=request.Correo,Password=request.Password });
var logged=await response.Content.ReadFromJsonAsync<UsuarioDto>();
Check("Login HTTP 200 y nombre real",response.StatusCode==HttpStatusCode.OK && logged?.Nombre=="Alexander" && logged.Id==user.Id);
await File.WriteAllTextAsync(Path.Combine(evidence,"login_200.json"),JsonSerializer.Serialize(logged,new JsonSerializerOptions(JsonSerializerDefaults.Web){WriteIndented=true}));
response=await client.PostAsJsonAsync("/api/Usuarios/login",new LoginDto { Correo=request.Correo,Password="incorrecta" });
Check("Login incorrecto HTTP 401",response.StatusCode==HttpStatusCode.Unauthorized);
foreach(var sem in new[]{0,11}) {
    request.Correo=$"sem{sem}@example.com"; request.Semestre=sem;
    Check($"Semestre {sem} HTTP 400",(await client.PostAsJsonAsync("/api/Usuarios",request)).StatusCode==HttpStatusCode.BadRequest);
}
request.Semestre=3; request.Correo="invalid@example.com";
foreach(var date in new DateOnly?[]{null,new DateOnly(2999,1,1),new DateOnly(1800,1,1)}) {
    request.FechaNacimiento=date;
    Check($"Fecha {date?.ToString() ?? "ausente"} HTTP 400",(await client.PostAsJsonAsync("/api/Usuarios",request)).StatusCode==HttpStatusCode.BadRequest);
}
request.FechaNacimiento=new DateOnly(2005,6,15);request.Universidad=" ";
Check("Universidad vacía HTTP 400",(await client.PostAsJsonAsync("/api/Usuarios",request)).StatusCode==HttpStatusCode.BadRequest);
var today = new DateOnly(2026,10,13);
Check("Edad antes del cumpleaños",UsuarioService.CalcularEdad(new DateOnly(2006,10,14),today)==19);
Check("Edad el día del cumpleaños",UsuarioService.CalcularEdad(new DateOnly(2006,10,13),today)==20);
Check("Edad después del cumpleaños",UsuarioService.CalcularEdad(new DateOnly(2006,10,12),today)==20);
Check("Nacimiento bisiesto antes de marzo",UsuarioService.CalcularEdad(new DateOnly(2004,2,29),new DateOnly(2025,2,28))==20);
Check("Nacimiento bisiesto en marzo",UsuarioService.CalcularEdad(new DateOnly(2004,2,29),new DateOnly(2025,3,1))==21);
response=await client.PutAsJsonAsync($"/api/Usuarios/{user.Id}",new ActualizarUsuarioDto { Nombre=user.Nombre,Apellido=user.Apellido,Correo=user.Correo,FechaNacimiento=user.FechaNacimiento,Universidad=user.Universidad,Semestre=4,Activo=true });
Check("PUT HTTP 204",response.StatusCode==HttpStatusCode.NoContent);
Check("PUT persiste semestre",(await client.GetFromJsonAsync<UsuarioDto>($"/api/Usuarios/{user.Id}"))?.Semestre==4);
var swagger=await client.GetStringAsync("/swagger/v1/swagger.json");
await File.WriteAllTextAsync(Path.Combine(evidence,"swagger.json"),swagger);
using(var schema=JsonDocument.Parse(swagger)) {
 var fields=schema.RootElement.GetProperty("components").GetProperty("schemas").GetProperty("CrearUsuarioDto").GetProperty("properties");
 Check("Swagger incluye nuevos campos",fields.TryGetProperty("fechaNacimiento",out _) && fields.TryGetProperty("universidad",out _) && fields.TryGetProperty("semestre",out _));
 Check("Swagger excluye Edad y RolId",!fields.TryGetProperty("edad",out _) && !fields.TryGetProperty("rolId",out _));
}
Check("DELETE HTTP 204",(await client.DeleteAsync($"/api/Usuarios/{user.Id}")).StatusCode==HttpStatusCode.NoContent);
Check("GET eliminado HTTP 404",(await client.GetAsync($"/api/Usuarios/{user.Id}")).StatusCode==HttpStatusCode.NotFound);
await File.WriteAllLinesAsync(Path.Combine(evidence,"pruebas_backend.txt"),new[]{"Pruebas ejecutadas con ASP.NET TestServer y EF Core InMemory. No prueban conexión MySQL ni restricciones SQL."}.Concat(results));
Console.WriteLine($"{results.Count} pruebas correctas.");
