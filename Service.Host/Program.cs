using Service.API;
using Service.Business;
using Service.Business.Configuration;
using Service.Business.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using Topshelf;

namespace Service.Host
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Config.LoadConfig();

                // Inicia o TopShelf
                var exitCode = HostFactory.Run(hostConfig =>
                {
                    hostConfig.Service<ServiceWork>(serviceConfig =>
                    {
                        serviceConfig.ConstructUsing(serviceWork => new ServiceWork());
                        serviceConfig.WhenStarted((serviceWork, _) =>
                        {
                            Logger.Info("Program.cs", "WhenStarted", "Iniciando serviço...");
                            serviceWork.StartService();

                            if (Convert.ToBoolean(Config.Get("API:UseAPI")))
                            {
                                Logger.Info("Program.cs", "WhenStarted", "Iniciando API...");
                                serviceWork._apiCancellation = new CancellationTokenSource();
                                serviceWork._apiTask = ApiHost.RunAsync(args, serviceWork._apiCancellation.Token);
                                Logger.Info("Program.cs", "WhenStarted", "API iniciada!");
                            }

                            return true;
                        });

                        serviceConfig.WhenStopped((serviceWork, _) => 
                        {
                            serviceWork.StopService();

                            if(serviceWork._apiCancellation != null)
                            {
                                Logger.Info("Program.cs", "WhenStopped", "Requisição para finalizar recebida, parando API...");
                                serviceWork._apiCancellation.Cancel();

                                try
                                {
                                    serviceWork._apiTask?.Wait(3000);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error("Program.cs", "WhenStopped", $"Uma exceção foi gerada ao tentar encerrar a API: {ex.GetType().Name} - {ex.Message}");
                                }
                                finally
                                {
                                    Logger.Info("Program.cs", "WhenStopped", "API encerrada!");
                                }
                            }

                            Logger.Info("Program.cs", "WhenStopped", $"Serviço encerrado!");

                            return true;
                        });
                    });

                    hostConfig.RunAsLocalSystem();
                    hostConfig.SetServiceName("ServiceWithAPI");
                    hostConfig.SetDisplayName("ServiceWithAPI");
                    hostConfig.SetDescription("Service integrated with an API");

                });

                // Para casos genéricos: (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
                int exitCodeValue = (int)exitCode;
                Environment.ExitCode = exitCodeValue;
                Console.WriteLine($"ExitCode: {Environment.ExitCode}");
            }
            catch (Exception ex)
            {
                // Encerra o serviço diante de erros não pegos pelo Topshelf no startup
                HandleStartupError(ex);
                Console.WriteLine($"{DateTime.Now} - Erro: {ex}");
                Environment.Exit(1);
            }
        }

        private static void HandleStartupError(Exception exception)
        {
            // Cria um arquivo devido a chance do Logger não ter sido iniciado

            string fatalErrorDirectory = Path.Combine(AppContext.BaseDirectory, "StartupErrors");
            if (!Directory.Exists(fatalErrorDirectory))
                Directory.CreateDirectory(fatalErrorDirectory);

            string timeStamp = DateTime.Now.Date.ToString("yyyyMMdd");
            string file = Path.Combine(fatalErrorDirectory, $"{timeStamp}_ERROR_.txt");
            string errorMsg = $"{DateTime.Now} - Erro durante o startup do serviço: {exception.ToString()}{Environment.NewLine}";
            File.AppendAllText(file, errorMsg);
        }
    }
}

