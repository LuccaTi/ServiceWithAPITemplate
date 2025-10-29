using Service.Business.Configuration;
using Service.Business.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection.Metadata.Ecma335;
using System.Timers;


namespace Service.Business
{
    public class ServiceWork
    {
        #region Atributes
        private const string _className = "ServiceWork";
        private System.Timers.Timer _timer;

        // Atributos que controlam o ciclo de vida da API
        public CancellationTokenSource? _apiCancellation;
        public Task? _apiTask;
        #endregion

        public ServiceWork()
        {
            try
            {
                var interval = int.Parse(Config.Get("ServiceConfig:Interval"));
                Logger.Info(_className, "Constructor", "Configurações carregadas!");

                // Intervalo usado antes de iniciar o trabalho
                _timer = new System.Timers.Timer(interval);
                _timer.Elapsed += CreateWorkThreads;
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Constructor", $"Erro: {ex.ToString()}{Environment.NewLine}Sistema será encerrado pelo TopShelf!");
                throw;
            }
        }

        #region Methods
        private void CreateWorkThreads(object? sender, ElapsedEventArgs e)
        {
            try
            {
                // Implementação de threads que vão executar as funcionalidades do serviço
                // Task.Run ou new Thread();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "CreateWorkThreads", $"Erro: {ex.Message}");
                throw;
            }
        }
        public void StartService()
        {
            try
            {
                Logger.Info(_className, "StartService", "Serviço iniciado com sucesso!");
                _timer.Start();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "StartService", $"Erro: {ex.Message}");
                throw;
            }

        }
        public void StopService()
        {
            try
            {
                Logger.Info(_className, "StopService", "Requisição para finalizar recebida, parando serviço...");
                _timer.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "StopService", $"Erro: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
