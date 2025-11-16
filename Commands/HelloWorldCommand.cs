using System.CommandLine;

namespace XMLe.Commands;


public class HelloWorldCommand
{
    private readonly Command command = new Command("hello");

    public Command Command
    {
        get
        {
            return command;
        }
    }

    public HelloWorldCommand()
    {
        command.SetAction(this.Action);
    }

    private async Task Action(ParseResult parseResult)
    {
        Console.WriteLine("Hello World!");
    }
}

