using BotPeliculas.Interfaces;

using BotPeliculas.Models.EF;
using BotPeliculas.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<BotPeliculasContext>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();
builder.Services.AddHttpClient<IPeliculasService, PeliculasService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiraci�n de la sesi�n
    options.Cookie.HttpOnly = true; // Cookie de sesi�n accesible solo por el servidor
    options.Cookie.IsEssential = true; // Asegurarse de que la cookie es esencial para la aplicaci�n
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Registro}/{id?}");

app.Run();
