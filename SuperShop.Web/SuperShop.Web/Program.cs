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

// Por omissão, o Identity manda quem não está autenticado para
// "/Account/Login" (LoginPath) e quem está autenticado mas sem permissões
// para uma página do sistema (AccessDeniedPath). Aqui apontamos os dois
// para a mesma action "NotAuthorized" — mesmo quem ainda não fez login vai
// logo parar a uma página a dizer que não tem acesso, em vez do formulário
// de login. Repara que isto também anula o "ReturnUrl" que o Login usava
// para voltar à página original depois de autenticar.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/NotAuthorized";
    options.AccessDeniedPath = "/Account/NotAuthorized";
});

// Regista o Seed como "Transient": só é usado uma vez, ao arrancar a
// aplicação, e depois é descartado — não fica em memória o resto do tempo.
builder.Services.AddTransient<Seed>();

// Regista o repositório dos produtos como "Scoped": um objeto novo por cada
// pedido HTTP (cada vez que alguém carrega numa página), reaproveitado
// durante esse pedido, e depois descartado.
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Aula 22 — Repositório das encomendas (um repositório por ÁREA: trata de
// Order, OrderDetail e OrderDetailTemp). Sem esta linha dá o erro
// "Unable to resolve service for type IOrderRepository".
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// UserHelper: encapsula o UserManager<User>, o SignInManager<User> e o
// RoleManager<IdentityRole>, para não os injetar diretamente em todo o lado
// (controladores, seed, etc.) e centralizar a gestão de utilizadores,
// roles e autenticação num único sítio.
builder.Services.AddScoped<IUserHelper, UserHelper>();

// ImageHelper: centraliza o upload de ficheiros (imagens) para dentro de
// wwwroot, gerando sempre um nome único (Guid) para nunca haver colisões.
// Já não é usado pelos Produtos (ver IBlobHelper), mas fica disponível.
builder.Services.AddScoped<IImageHelper, ImageHelper>();

// BlobHelper: centraliza o upload de imagens para o Azure Blob Storage — é
// o que os Produtos usam agora, em vez do ImageHelper (pasta local).
builder.Services.AddScoped<IBlobHelper, BlobHelper>();

// ConverterHelper: converte entre Product (entidade, o que vai para a base
// de dados) e ProductViewModel (o que a view do Create/Edit recebe, que tem
// também o ficheiro da imagem) — nos dois sentidos.
builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

var app = builder.Build();

// Antes de arrancar a aplicação, corre o Seed: garante que a base de dados
// existe e, se estiver vazia, cria os roles (Admin/Customer), o utilizador
// admin e popula-a com alguns produtos de exemplo associados a esse admin.
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

// Tem de vir logo no início do pipeline, antes do routing/autenticação:
// sempre que uma resposta sair com um status code de erro (sem corpo, ex.:
// 404) e ainda não tiver sido tratada, reexecuta o pedido para o caminho
// indicado, substituindo "{0}" pelo código (ex.: 404 -> "/Error/404"). É
// assim que apanhamos, por exemplo, um controlador ou action que não existe.
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // tem de vir ANTES do UseAuthorization, senão a app rebenta
app.UseAuthorization();

// Rota específica para o 404 genérico: "/Error/404" (gerado pelo
// UseStatusCodePagesWithReExecute acima) é reencaminhado para
// Home/Error404. Tem de vir antes da rota "default", que é mais genérica.
app.MapControllerRoute(
    name: "error404",
    pattern: "Error/404",
    defaults: new { controller = "Home", action = "Error404" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
