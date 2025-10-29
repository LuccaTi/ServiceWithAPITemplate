using Service.Business.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Business.Configuration
{
    public static class Config
    {
        #region Atributes
        private const string _className = "Config";
        private static IConfiguration? _config;
        #endregion

        #region Methods
        public static void LoadConfig()
        {
            try
            {
                // 1. Carregar appsettings.json
                _config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                // 2. Obter configs do Log
                string logDirectory = _config["ServiceLogging:LogDirectory"] ?? "logs/Host".Replace(@"/", "\\");

                // 3. Configurar Serilog e inicializar logger
                Logger.InitLogger(logDirectory);
                Logger.Info(_className, "LoadConfig", "Logger iniciado, carregando configurações...");

            } 
            catch (Exception ex)
            {
                throw new Exception("Ocorreu um erro ao carregar as configurações do sistema!", ex);
            }
        }

        public static string Get(string parameter)
        {
            try
            {
                return _config?[parameter]!;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Get", $"{ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
