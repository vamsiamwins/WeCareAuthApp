using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WeCareAuthApp.API;
using WeCareAuthApp.IDP;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);
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
    builder.Services.AddHttpClient<TokenClient>();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();
    //var app = builder
    //    .ConfigureServices()
    //    .ConfigurePipeline();
    var app = builder.Build();
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
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}