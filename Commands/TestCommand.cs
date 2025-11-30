using System.CommandLine;

using xmle.Services;

namespace xmle.Commands;

public class TestCommand : Command
{
    private readonly IConfigService config;

    public TestCommand(IConfigService config) : base("test", "Temporary Command")
    {
        this.config = config;

        SetAction(ActionHandler);
    }

    private void ActionHandler(ParseResult result)
    {
        Console.WriteLine("Table: " + config.GetConfig()?.Table);
        Console.WriteLine("Headings: " + string.Join(", ", config.GetConfig()?.Headings ?? []));
        Console.WriteLine("XPaths: " + string.Join(", ", config.GetConfig()?.Columns ?? []));
    }
}