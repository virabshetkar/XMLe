using System.CommandLine;
using System.CommandLine.Parsing;
using xmle.Services;

namespace xmle.Commands;

public class ViewCommand : Command
{
    private readonly IXmlService xmlService;

    private Option<string> xpathOption;
    private Argument<string> xmlPathArgument;

    public ViewCommand(IXmlService xmlService) : base("view", "View data in JSON format")
    {
        xpathOption = new Option<string>("xpath", "-x")
        {
            Description = "XPath to find the first XML Element",
            DefaultValueFactory = (ArgumentResult res) => { return "/"; }
        };

        xmlPathArgument = new Argument<string>("xmlPath")
        {
            Description = "Path to XML file",
        };

        Add(xmlPathArgument);
        Add(xpathOption);

        SetAction(ActionHandler);
        this.xmlService = xmlService;
    }

    private async Task ActionHandler(ParseResult result)
    {
        var xmlPath = result.GetValue(xmlPathArgument);

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

        var xpath = result.GetValue(xpathOption);
        if (xpath == null)
        {
            Console.WriteLine("XPath is not given");
            return;
        }

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
