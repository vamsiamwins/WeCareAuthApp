using Serilog;
using Microsoft.EntityFrameworkCore;
using WeCareAuthApp.IDP;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace WeCareAuthApp.API
{
    internal static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<UserDbContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnString")));

            builder.Services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddIdentityServer(options =>
                {
                    // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddAspNetIdentity<User>()
                .AddDeveloperSigningCredential();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            return builder.Build();
        }

        public static  WebApplication ConfigurePipeline(this WebApplication app)
        {
            //app.UseSerilogRequestLogging();
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    await UserDbContextSeeder.SeedTestData(services);
            //}

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Provider IDP");
                }
                );
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                app.UseStaticFiles();
                app.UseRouting();
                app.UseIdentityServer();
                app.UseAuthentication();

                app.UseAuthorization();

                app.MapControllers();
                app.MapGet("/", () => "Hello");

            return app;
        }
    }
}
