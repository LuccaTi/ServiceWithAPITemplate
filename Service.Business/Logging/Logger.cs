using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Sinks.File;
using Serilog.Events;

namespace Service.Business.Logging
{
    public static class Logger
    {
        #region Atributes
        private const string _className = "Logger";
        private static ILogger? _logger;
        #endregion

        #region Methods
        // Inicializa o logger com padrão de level debug
        public static void InitLogger(string logDirectory)
        {
            try
            {
                if (!Directory.Exists(logDirectory))
                    Directory.CreateDirectory(logDirectory);

                _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Console()
                .WriteTo.File(
                    Path.Combine(logDirectory, $"system_log_.txt"),
                    rollingInterval: RollingInterval.Day, // Um arquivo de log por dia
                    retainedFileCountLimit: null, // Null mantém os arquivos indefinidamente
                    shared: true // Permite acompanhar em tempo real a escrita no log
                    )
                .CreateLogger();

                // Cria o logger universal do serilog
                Log.Logger = _logger;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public static void Debug(string className, string methodName, string message)
        {
            try
            {
                _logger!.Debug($"{className} - {methodName} - {message}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void Info(string className, string methodName, string message)
        {
            try
            {
                _logger!.Information($"{className} - {methodName} - {message}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void Error(string className, string methodName, string message)
        {
            try
            {
                _logger!.Error($"{className} - {methodName} - {message}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
