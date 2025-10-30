using Serilog;

namespace Service.API.Logging
{
    internal static class ApiLogger
    {
        #region Atributes
        private const string _className = "ApiLogger";
        private static Serilog.Core.Logger? logger;
        #endregion

        #region Methods
        // Inicializa o logger da API com padrão de level debug
        internal static void InitApiLogger(string logDirectory)
        {
            try
            {
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                logger = new LoggerConfiguration()
                                    .MinimumLevel.Debug()
                                    //.WriteTo.Console()
                                    .WriteTo.File(Path.Combine(logDirectory, $"API_log_.txt"),
                                    rollingInterval: RollingInterval.Day, // Um arquivo de log por dia
                                    retainedFileCountLimit: null, // Null mantém os arquivos indefinidamente
                                    shared: true // Permite acompanhar em tempo real a escrita no log
                                    )
                                    .CreateLogger();
            }
            catch (Exception)
            {

                throw;
            }
        }
        internal static Serilog.Core.Logger GetLoggerInstance()
        {
            try
            {
                if (logger is null)
                    throw new InvalidOperationException($"{DateTime.Now} - {_className} - GetLoggerInstance - Não foi possível obter a instância do logger da API porque ele não foi iniciado!");

                return logger;
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal static void Debug(string className, string methodName, string message)
        {
            try
            {
                logger!.Debug($"{className} - {methodName} - {message}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal static void Info(string className, string methodName, string message)
        {
            try
            {
                logger!.Information($"{className} - {methodName} - {message}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal static void Error(string className, string methodName, string message)
        {
            try
            {
                logger!.Error($"{className} - {methodName} - {message}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
