using System.CommandLine;

using xmle.Services;

namespace xmle.Commands;

public class UpdateCommand : Command
{
    private readonly Option<string> xpathOption;
    private readonly Option<string> valueOption;
    private readonly IXmlService xmlService;
    private readonly TextWriter writer;

    public UpdateCommand(IXmlService xmlService, TextWriter writer) : base("update", "Updates the xml xpath value pair")
    {
        this.xmlService = xmlService;
        this.writer = writer;

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
            writer.WriteLine("Xml file does not exist!");
            return;
        }

        var xpath = parseResult.GetValue(xpathOption)!;
        var value = parseResult.GetValue(valueOption)!;

        var xml = xmlService.GetRootXml(xmlPath);

        try
        {
            xmlService.UpdateValueForXpath(xml, xpath, value);
            writer.WriteLine("Updated the value");

            xml.Save(xmlPath);
        }
        catch (Exception e)
        {
            writer.WriteLine(e.Message);
        }
    }
}
