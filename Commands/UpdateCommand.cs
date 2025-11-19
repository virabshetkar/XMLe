using System.Xml;
using System.CommandLine;

using XMLe.Services;

namespace XMLe.Commands;

public class UpdateCommand : Command
{
    private readonly Option<string> xpathOption;
    private readonly Option<string> valueOption;

    public UpdateCommand() : base("update", "Updates the xml xpath value pair")
    {
        xpathOption = new Option<string>("xpath", "-x")
        {
            Description = "XPath to find the first XML Element",
            Required = true
        };

        valueOption = new Option<string>("value", "-v")
        {
            Description = "Value to update the found XML Element",
            Required = true
        };

        SetAction(ActionHandler);

        Add(xpathOption);
        Add(valueOption);
    }

    private async Task ActionHandler(ParseResult parseResult)
    {
        var xmlPath = parseResult.GetValue<string>("xmlPath");

        xmlPath = Path.GetFullPath(xmlPath!);
        if (!Path.Exists(xmlPath))
        {
            Console.WriteLine("Xml file does not exist!");
            return;
        }

        var xpath = parseResult.GetValue(xpathOption)!;
        var value = parseResult.GetValue(valueOption)!;

        var xmlService = new XMLService();
        var xml = xmlService.GetRootXml(xmlPath);

        try
        {
            xmlService.UpdateValueForXpath(xml, xpath, value);
            Console.WriteLine("Updated the value");

            xml.Save(xmlPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
