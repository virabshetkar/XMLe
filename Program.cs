using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;

using xmle.Commands;
using xmle.Services;


namespace xmle;

public class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var provider = services.BuildServiceProvider();

        var rootCommand = RootCommandBuilder.Build(provider);

        foreach (var commandType in RootCommandBuilder.GetAllCommands())
        {
            rootCommand.Add((Command)provider.GetRequiredService(commandType));
        }

        await rootCommand.Parse(args).InvokeAsync();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ICsvParser, CsvParser>();
        services.AddSingleton<IXmlService, XmlService>();

        foreach (var commandType in RootCommandBuilder.GetAllCommands())
        {
            services.AddTransient(commandType);
        }
    }
}
