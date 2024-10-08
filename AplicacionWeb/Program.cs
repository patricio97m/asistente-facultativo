using EntityFramework;
using EntityFramework.Clases;
using Microsoft.EntityFrameworkCore;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Contexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conexion"))
);


builder.Services.AddScoped<IIntraconsultaService, IntraconsultaService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var Contexto = scope.ServiceProvider.GetRequiredService<Contexto>();
    Contexto.Database.Migrate();
}

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
