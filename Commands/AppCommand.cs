using System.CommandLine;
using XMLe.Services;

namespace XMLe.Commands;

public class AppCommand
{
    private readonly Option<string> keyOption;
    private readonly Option<string> valueOption;
    private readonly Argument<string> xmlPathArgument;

    private readonly CsvCommand csvCommand = new CsvCommand();
    private readonly HelloWorldCommand hwCommand = new HelloWorldCommand();

    private readonly RootCommand command;

    public AppCommand()
    {
        keyOption = new Option<string>("key", "-k", "--key")
        {
            Description = "Id of the data to change"
        };
        valueOption = new Option<string>("value", "-v", "--value")
        {
            Description = "New value for Id specified"
        };
        xmlPathArgument = new Argument<string>("xmlPath")
        {
            Description = "XML file to edit"
        };

        command = new RootCommand("XMLe Command");
        command.SetAction(this.Action);

        command.Add(keyOption);
        command.Add(valueOption);
        command.Add(xmlPathArgument);

        command.Add(csvCommand.Command);
        command.Add(hwCommand.Command);
    }

    public async Task<int> InvokeAsync(string[] args)
    {
        return await command.Parse(args).InvokeAsync();
    }

    private async Task Action(ParseResult parseResult)
    {
        var xmlPath = parseResult.GetValue(xmlPathArgument);
        var keyOpt = parseResult.GetValue(keyOption);
        var valueOpt = parseResult.GetValue(valueOption);

        if (xmlPath == null)
        {
            Console.WriteLine("You need to give a path");
            return;
        }

        if (keyOpt == null)
        {
            Console.WriteLine("Key is not given");
            return;
        }

        xmlPath = Path.GetFullPath(xmlPath);
        var xmlService = new XMLService();
        var xml = xmlService.GetBuffer(xmlPath);

        if (valueOpt == null)
        {
            try
            {

                Console.WriteLine(xmlService.GetValue(xml, keyOpt));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            xmlService.UpdateValue(xml, keyOpt, valueOpt);

            // Save File
            xml.Save(xmlPath);
        }
    }
}
