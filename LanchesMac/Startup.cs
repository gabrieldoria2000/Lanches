using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        //Sobreescreve as políticas do Identity ( lembrar de retirar isso em tempo de PRD)
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredLength = 3;
            options.Password.RequireUppercase = false;
            options.Password.RequiredUniqueChars = 1;
            options.Password.RequireNonAlphanumeric = false;
        });


        //COM ESSE REGISTRO, TODA VEZ QUE EU SOLICITAR UMA INSTANCIA REFERENCIANDO A INTERFACE, O CONTAINER NATIVO
        //DA INJEÇÃO DE DEPENDENCIA VAI CRIAR UMA INSTANCIA DA CLASSE E VAI INJETAR NO CONSTRUTOR AONDE EU ESTIVER SOLICITANDO
        services.AddTransient<ILancheRepository, LancheRepository>();
        services.AddTransient<ICategoriaRepository, CategoriaRepository>();
        services.AddTransient<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IseedUserRoleInitial, seedUserRoleInitial>();

        //usa singleton para utilizar durante todo o tempo de vida da aplicação
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        //adddscoped significa o tempo de vida "a cada request" isso significa que se
        //dois clientes solicitarem um carrinho ao mesmo tempo, eles obterão instancias do carrinho diferentes,
        //porque são requests diferentes. AddScoped trabalha a nível de requisição
        services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

        services.AddControllersWithViews();

        //REGISTRA OS MIDLEWARE PARA O CARRINHO DE COMPRAS
        //habilita o uso do cache
        services.AddMemoryCache();
        //habilita o uso da session
        services.AddSession();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IseedUserRoleInitial seedUserRoleInitial)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseRouting();

        //cria os perfis
        seedUserRoleInitial.SeedRoles();
        //cria os usuarios e atribui o perfil
        seedUserRoleInitial.SeedUsers();
        

        app.UseSession();

        // Autenticação SEMPRE antes da Autorização
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            //endpoints.MapControllerRoute(
            //    name: "teste",
            //    pattern: "testeme",
            //    defaults: new { controller ="teste", Action="index"}
            //    );


            endpoints.MapControllerRoute(
              name: "areas",
              pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
            );

            endpoints.MapControllerRoute(
               name: "categoriaFiltro",
               pattern: "Lanche/{action}/{categoria?}",
               defaults: new { controller = "Lanche", Action = "List" }
               );

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}