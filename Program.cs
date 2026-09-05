using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Regista o Seed como "Transient": só é usado uma vez, ao arrancar a
// aplicação, e depois é descartado — não fica em memória o resto do tempo.
builder.Services.AddTransient<Seed>();

// Regista o repositório dos produtos como "Scoped": um objeto novo por cada
// pedido HTTP (cada vez que alguém carrega numa página), reaproveitado
// durante esse pedido, e depois descartado.
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

// Antes de arrancar a aplicação, corre o Seed: garante que a base de dados
// existe e, se estiver vazia, popula-a com alguns produtos de exemplo.
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<Seed>();
    await seeder.SeedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
