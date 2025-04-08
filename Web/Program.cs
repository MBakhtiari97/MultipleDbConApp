using DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Service.UserServices;

namespace Web;

public class Program
{
    private static string GetSecret()
    {
        var userSecretId = typeof(Web.Program).Assembly
                        .GetCustomAttributes(typeof(UserSecretsIdAttribute), false)
                        .OfType<UserSecretsIdAttribute>()
                        .FirstOrDefault()?.UserSecretsId;
        if (userSecretId == null)
            throw new Exception("Init Application Configuration File Has Not Set Yet!");
        return userSecretId!;
    }

    private static void AddConnectionStrings(WebApplicationBuilder builder)
    {
        var userSecretId = GetSecret();
        builder.Configuration.AddUserSecrets(userSecretId);

        var masterConnectionString = builder.Configuration.GetConnectionString("MasterConnection");
        var logConnectionString = builder.Configuration.GetConnectionString("LogConnection");

        builder.Services.AddDbContext<MasterDbContext>(options =>
                                                         options.UseSqlServer(masterConnectionString)
                                                         .EnableSensitiveDataLogging());
        builder.Services.AddDbContext<LogDbContext>(options =>
                                                 options.UseSqlServer(logConnectionString)
                                                 .EnableSensitiveDataLogging());
    }

    private static void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserServices, UserServices>();
        builder.Services.AddScoped<IUserReader, UserReader>();
        builder.Services.AddScoped<IUserSaver, UserSaver>();
    }
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        AddConnectionStrings(builder);

        RegisterServices(builder);

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
