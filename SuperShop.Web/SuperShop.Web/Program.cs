using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperShop.Web.Data;
using SuperShop.Web.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity: passa a existir gestão de utilizadores e autenticação, usando a
// nossa classe User (que estende o IdentityUser) e o IdentityRole "normal".
builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    // Configuração simplificada, só para desenvolvimento/testes.
    // Antes de ir para produção, reforçar estas regras!
    cfg.Password.RequireDigit = false;
    cfg.Password.RequireUppercase = false;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// Regista o Seed como "Transient": só é usado uma vez, ao arrancar a
// aplicação, e depois é descartado — não fica em memória o resto do tempo.
builder.Services.AddTransient<Seed>();

// Regista o repositório dos produtos como "Scoped": um objeto novo por cada
// pedido HTTP (cada vez que alguém carrega numa página), reaproveitado
// durante esse pedido, e depois descartado.
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// UserHelper: encapsula o UserManager<User>, para não o injetar diretamente
// em todo o lado (controladores, seed, etc.) e centralizar a gestão de
// utilizadores num único sítio.
builder.Services.AddScoped<IUserHelper, UserHelper>();

var app = builder.Build();

// Antes de arrancar a aplicação, corre o Seed: garante que a base de dados
// existe e, se estiver vazia, cria o utilizador admin e popula-a com alguns
// produtos de exemplo associados a esse admin.
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

app.UseAuthentication();   // tem de vir ANTES do UseAuthorization, senão a app rebenta
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
