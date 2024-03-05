using Automate.Domain.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            StartupConfiguration.ConfigureServices(serviceCollection);
            StartupHandle.ConfigureServices(serviceCollection);

            try
            {
                var command = args[0];
                if (string.IsNullOrEmpty(command))
                    return;

                var handle = serviceCollection.BuildServiceProvider().GetServices<IHandle>().Where(item => item.Command == command).FirstOrDefault();
                handle.Execution(args);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}