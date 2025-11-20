using System.CommandLine;
using xmle.Services;

namespace xmle.Commands;

public class AppCommand : RootCommand
{
    private readonly Option<string> xpathOption;
    private readonly Argument<string> xmlPathArgument;

    private readonly UpdateCommand updateCommand = new UpdateCommand();
    private readonly TableCommand tableCommand = new TableCommand();

    public AppCommand() : base("xmle Command")
    {
        xpathOption = new Option<string>("xpath", "-x")
        {
            Description = "XPath to find the first XML Element",
            Required = true
        };

        xmlPathArgument = new Argument<string>("xmlPath")
        {
            Description = "XML file to edit",
        };

        SetAction(ActionHandler);

        Add(xmlPathArgument);
        Add(xpathOption);

        Add(updateCommand);
        Add(tableCommand);
    }

    private async Task ActionHandler(ParseResult parseResult)
    {
        var xmlPath = parseResult.GetValue(xmlPathArgument);

        if (xmlPath == null)
        {
            Console.WriteLine("You need to give a path");
            return;
        }

        xmlPath = Path.GetFullPath(xmlPath);
        if (!Path.Exists(xmlPath))
        {
            Console.WriteLine("Xml file does not exist!");
            return;
        }

        var xpath = parseResult.GetValue(xpathOption);
        if (xpath == null)
        {
            Console.WriteLine("XPath is not given");
            return;
        }

        var xmlService = new XMLService();
        var xml = xmlService.GetRootXml(xmlPath);

        try
        {
            var data = xmlService.GetValueFromXpath(xml, xpath);
            Console.WriteLine(data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
