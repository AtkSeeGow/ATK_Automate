using Automate.Domain.Interface;
using Automate.Domain.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Automate.Console
{
    /// <summary>
    /// 
    /// </summary>
    public class StartupHandle
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IHandle>(i =>
            new ConvertEncodingToBeatX(
                "ConvertEncodingToBeatX",
                serviceCollection.BuildServiceProvider().GetService<ConvertEncodingToBeatXOptions>()!));
        }
    }

}