using System;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ProEventos.Application;
using ProEventos.Application.Contratos;
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
            services.AddControllers()
                        .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling =
                            Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        ); //Esse NewtonsoftJson devera ser importado pelo nugget, e com a linha de codigo acima, ira resolver erros de loop.
           
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IEventoService, EventoService>(); 
            services.AddScoped<ILoteService, LoteService>();
            
            services.AddScoped<IGeralPersist, GeralPersist>();
            services.AddScoped<IEventoPersist, EventoPersist>(); //Esses comandos servem para dizer ao c# quais as classes que a interface devera implentar quando for injetada em algum arquivo (como no controller). (Essa era a minha duvida que eu perguntei na aula da udemy).
            services.AddScoped<ILotePersist, LotePersist>();

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
