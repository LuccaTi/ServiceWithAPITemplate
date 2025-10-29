
using System.Reflection;

namespace Service.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ApiHost.RunAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Encerra a API quando há erros no startup ao usar ela isoladamente
                HandleStartupError(ex);
                Console.WriteLine($"{DateTime.Now} - Erro: {ex}");
                Environment.Exit(1);
            }
        }

        private static void HandleStartupError(Exception exception)
        {
            // Cria um arquivo devido a chance do Logger não ter sido iniciado

            string apiDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string fatalErrorDirectory = Path.Combine(apiDirectory, "StartupErrors");
            if (!Directory.Exists(fatalErrorDirectory))
                Directory.CreateDirectory(fatalErrorDirectory);

            string timeStamp = DateTime.Now.Date.ToString("yyyyMMdd");
            string file = Path.Combine(fatalErrorDirectory, $"{timeStamp}_ERROR_.txt");
            string errorMsg = $"{DateTime.Now} - Erro durante a inicialização da API: {exception.ToString()}{Environment.NewLine}";
            File.AppendAllText(file, errorMsg);
        }
    }
}
