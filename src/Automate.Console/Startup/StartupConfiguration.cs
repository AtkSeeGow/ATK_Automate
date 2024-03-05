using Automate.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Automate.Console
{
    /// <summary>
    /// 
    /// </summary>
    public class StartupConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void ConfigureServices(ServiceCollection serviceCollection)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var convertEncodingToBeatXOptions = new ConvertEncodingToBeatXOptions();
            configuration.GetSection("ConvertEncodingToBeatXOptions").Bind(convertEncodingToBeatXOptions);
            serviceCollection.AddTransient<ConvertEncodingToBeatXOptions>(i => convertEncodingToBeatXOptions);
        }
    }
}