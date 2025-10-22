using consensoria_autos.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 1️⃣ REGISTRO DE SERVICIOS (antes de builder.Build())
// ======================================================

// MVC y Razor
builder.Services.AddControllersWithViews();

// Sesión y acceso al contexto HTTP
builder.Services.AddSession();                 // Para HttpContext.Session
builder.Services.AddHttpContextAccessor();     // Para @inject IHttpContextAccessor en Razor

// Cliente HTTP para consumir la API externa
builder.Services.AddHttpClient<ApiClient>();   // Inyección del servicio ApiClient


// ======================================================
// 2️⃣ CONSTRUIR LA APLICACIÓN
// ======================================================
var app = builder.Build();

// ======================================================
// 3️⃣ CONFIGURAR EL PIPELINE HTTP
// ======================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar sesiones (antes de los controladores)
app.UseSession();

app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
