using BarbeariaInfrastructure.Data;
using BarbeariaCore.Application.Interfaces;
using BarbeariaInfrastructure.Data.DatabaseErrors.Providers;
using Microsoft.EntityFrameworkCore;


namespace BarbeariaApi.Extensions
{
    public static class DatabaseExtensions
    {
        // AddBarbeariaDatabase:

        // Busca configuration.GetConnectionString("PostgreSql")
        //depois registra services.AddDbContext<AppDbContext>(...)

        // Caminho da conexão: 
        //Program.cs
        //    ↓
        //AddBarbeariaDatabase
        //    ↓
        //ConnectionStrings:PostgreSql
        //   ↓
        // AppDbContext
        //   ↓
        //Npgsql
        //    ↓
        //PostgreSQL

        // transforma o método em um método de extensão. -> this IServiceCollection

        // IConfiguration configuration -> Recebe as configurações carregadas pelo ASP.NET Core.
        // Exemplos: appsettings.json; appsettings.Development.json; User Secrets;
        public static IServiceCollection AddBarbeariaDatabase(
     this IServiceCollection services,
     IConfiguration configuration)
        {
            var provider = configuration["Database:Provider"]
                ?? throw new InvalidOperationException(
                    "Database:Provider não foi configurado.");

            switch (provider.Trim().ToLowerInvariant())
            {
                case "postgresql":
                    {
                        var connectionString =
                            configuration.GetConnectionString("PostgreSql")
                            ?? throw new InvalidOperationException(
                                "ConnectionStrings:PostgreSql não configurada.");

                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseNpgsql(
                                connectionString,
                                npgsqlOptions =>
                                {
                                    npgsqlOptions.MigrationsAssembly(
                                        "Barbearia.Migrations.PostgreSql");

                                    npgsqlOptions.EnableRetryOnFailure(
                                        maxRetryCount: 5,
                                        maxRetryDelay: TimeSpan.FromSeconds(10),
                                        errorCodesToAdd: null);
                                });
                        });

                        services.AddScoped<
                            IDatabaseErrorClassifier,
                            PostgreSqlErrorClassifier>();

                        break;
                    }

                case "sqlserver":
                    {
                        var connectionString =
                            configuration.GetConnectionString("SqlServer")
                            ?? throw new InvalidOperationException(
                                "ConnectionStrings:SqlServer não configurada.");

                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseSqlServer(
                                connectionString,
                                sqlOptions =>
                                {
                                    sqlOptions.MigrationsAssembly(
                                        "Barbearia.Migrations.SqlServer");

                                    sqlOptions.EnableRetryOnFailure(
                                        maxRetryCount: 5,
                                        maxRetryDelay: TimeSpan.FromSeconds(10),
                                        errorNumbersToAdd: null);
                                });
                        });

                        services.AddScoped<
                            IDatabaseErrorClassifier,
                            SqlServerErrorClassifier>();

                        break;
                    }

                case "mysql":
                    {
                        var connectionString =
                            configuration.GetConnectionString("MySql")
                            ?? throw new InvalidOperationException(
                                "ConnectionStrings:MySql não configurada.");

                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseMySql(
                                connectionString,
                                ServerVersion.AutoDetect(connectionString),
                                mysqlOptions =>
                                {
                                    mysqlOptions.MigrationsAssembly(
                                        "Barbearia.Migrations.MySql");
                                });
                        });

                        services.AddScoped<
                            IDatabaseErrorClassifier,
                            MySqlErrorClassifier>();

                        break;
                    }

                default:
                    throw new InvalidOperationException(
                        $"Provider de banco '{provider}' não suportado.");
            }

            return services;
        }
    }
}


//BarbeariaApi
//   ↓
//DatabaseExtensions
//   ↓
//Database:Provider
//   │
//   ├── PostgreSql
//   │      ↓
//   │   UseNpgsql
//   │      ↓
//   │   Barbearia.Migrations.PostgreSql
//   │
//   ├── SqlServer
//   │      ↓
//   │   UseSqlServer
//   │      ↓
//   │   Barbearia.Migrations.SqlServer
//   │
//   └── MySql
//          ↓
//       UseMySql
//          ↓
//       Barbearia.Migrations.MySql

//Todos usam:
//↓
//AppDbContext
//↓
//BarbeariaInfrastructure


//Program.cs
//    ↓
//AddBarbeariaDatabase(configuration)
//    ↓
//Database: Provider
//    ↓
//PostgreSql?
//SqlServer?
//MySql?
//    ↓
//configura o AppDbContext adequado
