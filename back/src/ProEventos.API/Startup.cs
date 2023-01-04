using System;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProEventos.Application;
using ProEventos.Application.Contratos;
using ProEventos.Domain.Identity;
using ProEventos.Persistance.Contextos;
using ProEventos.Persistence;
using ProEventos.Persistence.Contratos;

namespace ProEventos.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) //Esse eh o construtor da classe Startup e ele recebe o iConfiguration (iConfiguration foi injetado), e esse IConfiguration eh referente ao arquivo de configuracao do projeto, o appsettings.json
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; } //E aqui esta a propriedade do IConfiguration

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) //Esse metodo como o nome ja sugere, configura services do projeto.
        {
            services.AddDbContext<ProEventosContext>( //Aqui estamos pegando o contexto que foi criado na pasta Data. (DataContext.cs)
                context => context.UseSqlite(Configuration.GetConnectionString("Default")) //Com o contexto em maos, nos falamos que o contexto ira usar o sqLite (DB que vamos usar) e passamos como parametro a conexao com esse DB. Essa conexao eh feita usando o configuration, que foi injetado nessa classe e eh referente ao appsettings.json, entao la no appsettings nos colocamos a "rota" para o db e o nome dessa rota sera exatamente o "Default" que passamos como parametro para o useSqlite. 
            ); //Para pegar uma string de conexao basta usar a variavel configuration e o metodo dela "GetConnectionString()".
            
            services.AddIdentityCore<User>(options => { //Essa sao opcoes para a senha, num sistema de verdade, deve ser feito de acordo com a seguranca que voce deseja, por exemplo, aqui como eh um curso deixamos tudo falso, mas talvez num sistema de verdade, seria interessante colocar que letras maiusculas sao obrigatorias para a senha, entre outras opcoes.
                 options.Password.RequireDigit = false;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireLowercase = false;
                 options.Password.RequireUppercase = false;
                 options.Password.RequiredLength = 4;
            })
            .AddRoles<Role>()
            .AddRoleManager<RoleManager<Role>>()
            .AddSignInManager<SignInManager<User>>()
            .AddRoleValidator<RoleValidator<Role>>()
            .AddEntityFrameworkStores<ProEventosContext>()
            .AddDefaultTokenProviders(); //Essa config AddDefaultTokenProviders serve para funcionar o uso dos tokens no accountService (resetToken e atualizar a senha).
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //Configuracoes de autenticacao do JWT.
                    .AddJwtBearer(options => {
                        options.TokenValidationParameters = new TokenValidationParameters{
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });

            services.AddControllers()
                    .AddJsonOptions(options => 
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
                    ) //Essa configuracao eh para funcionar os enums. Sem essa config quando for fazer um put ou um post, devera ser enviado o codigo do enum (1,2,3) e nao podera ser enviado o nome composto (naoInformado, Participante). Mas isso depende de projeto a projeto, alguns preferem que o json retorne apenas o id mesmo. (Obs: com essa config voce pode mandar tanto o nome composto, quanto o id, os dois irao funcionar).
                    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    ); //Esse NewtonsoftJson devera ser importado pelo nugget, e com a linha de codigo acima, ira resolver erros de loop.
           
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IEventoService, EventoService>(); 
            services.AddScoped<ILoteService, LoteService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            
            services.AddScoped<IGeralPersist, GeralPersist>();
            services.AddScoped<IEventoPersist, EventoPersist>(); //Esses comandos servem para dizer ao c# quais as classes que a interface devera implentar quando for injetada em algum arquivo (como no controller). (Essa era a minha duvida que eu perguntei na aula da udemy).
            services.AddScoped<ILotePersist, LotePersist>();
            services.AddScoped<IUserPersist, UserPersist>();


            services.AddCors();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProEventos.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProEventos.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(
                cors => cors.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin());

            app.UseStaticFiles(new StaticFileOptions() {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
                RequestPath = new PathString("/Resources")
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
