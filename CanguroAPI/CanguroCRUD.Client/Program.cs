var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuración de sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(3); // Duración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Marca la cookie como esencial para la funcionalidad del sitio
});

// Registrar HttpClient personalizado para el servicio
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:16382/") // Ajusta la base según tu API
    };
    // Puedes ajustar la autenticación aquí si es necesario
    client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
    return client;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Agregado para mayor seguridad en producción
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Añadir el middleware de sesiones antes de UseAuthorization
app.UseSession();

app.UseAuthentication(); // Asegúrate de tener configurada la autenticación si la estás utilizando
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
