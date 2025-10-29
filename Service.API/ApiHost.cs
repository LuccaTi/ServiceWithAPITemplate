using Service.Business.Configuration;
using Service.Business.Logging;
using Serilog;
using System.Reflection;
using Service.API.Logging;

namespace Service.API
{
    // Classe responsável por inicializar a API
    public static class ApiHost
    {
        #region Atributes
        private const string _className = "ApiHost";
        #endregion

        #region Methods
        public static async Task RunAsync(string[]? args = null, CancellationToken? token = null)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args ?? Array.Empty<string>());
                string apiBaseDirectory = Path.GetDirectoryName(AppContext.BaseDirectory)!;

                builder.Configuration
                    .SetBasePath(apiBaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                string logDirectory = Path.Combine(apiBaseDirectory, builder.Configuration["API:LogDirectory"] ?? "logs/API").Replace(@"/", "\\");
                ApiLogger.InitApiLogger(logDirectory);
                ApiLogger.Info(_className, "RunAsync", "Configuração da API carregada, logger da API iniciado!");

                builder.Services.AddControllers();
                bool useSwagger = Convert.ToBoolean(Config.Get("API:UseSwagger"));
                if (useSwagger)
                {
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                }

                var app = builder.Build();

                if (useSwagger)
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                ApiLogger.Info(_className, "RunAsync", "Todos os parâmetros foram carregados, subindo a API...");
                await app.RunAsync(token!.Value);
                ApiLogger.Info(_className, "RunAsync", "Requisição para finalizar recebida, parando a API...");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
