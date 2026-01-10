using Microsoft.AspNetCore.Builder;

namespace KontoApi.TestEntryPoint
{
    public class Program
    {
        public static Microsoft.Extensions.Hosting.IHostBuilder CreateHostBuilder(string[] args)
        {
            return Api.Program.CreateHostBuilder(args);
        }

        public static WebApplication CreateWebApplication(string[] args)
        {
            try
            {
                File.AppendAllText("/tmp/konto_test_entrypoint.log", DateTime.UtcNow + " - CreateWebApplication invoked\n");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"CreateWebApplication: failed to write test entrypoint log: {ex}");
            }

            return Api.Program.CreateWebApplication(args);
        }

        public static async Task Main(string[] args)
            => await Api.Program.Main(args);
    }
}