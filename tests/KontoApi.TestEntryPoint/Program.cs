using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace KontoApi.TestEntryPoint
{
    public class Program
    {
        // Provide proxies so WebApplicationFactory can locate host/web app builders
        public static Microsoft.Extensions.Hosting.IHostBuilder CreateHostBuilder(string[] args)
        {
            try
            {
                System.IO.File.AppendAllText("/tmp/konto_test_entrypoint.log", DateTime.UtcNow + " - CreateHostBuilder invoked\n");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"CreateHostBuilder: failed to write test entrypoint log: {ex}");
            }
            return KontoApi.Api.Program.CreateHostBuilder(args);
        }

        public static Microsoft.AspNetCore.Builder.WebApplication CreateWebApplication(string[] args)
        {
            try
            {
                System.IO.File.AppendAllText("/tmp/konto_test_entrypoint.log", DateTime.UtcNow + " - CreateWebApplication invoked\n");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"CreateWebApplication: failed to write test entrypoint log: {ex}");
            }
            return KontoApi.Api.Program.CreateWebApplication(args);
        }

        public static Task Main(string[] args)
        {
            // Ensure KontoApi.Api assembly is copied to test output
            _ = typeof(KontoApi.Api.Program);
            return Task.CompletedTask;
        }
    }
}
