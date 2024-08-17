var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuraci�n de sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(3); // Duraci�n de la sesi�n
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Marca la cookie como esencial para la funcionalidad del sitio
});

// Registrar HttpClient personalizado para el servicio
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:16382/") // Ajusta la base seg�n tu API
    };
    // Puedes ajustar la autenticaci�n aqu� si es necesario
    client.DefaultRequestHeaders.Add("Authorization", "Bearer your-token");
    return client;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Agregado para mayor seguridad en producci�n
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// A�adir el middleware de sesiones antes de UseAuthorization
app.UseSession();

app.UseAuthentication(); // Aseg�rate de tener configurada la autenticaci�n si la est�s utilizando
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
