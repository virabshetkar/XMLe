using System.CommandLine;
using System.Xml;

namespace xmle.Commands;


public class TableCommand : Command
{
    private string Fit(string s) => s.Length <= 30 ? s.PadRight(30) : s.Substring(0, 29) + " ";

    private readonly Option<string[]> columnNamesOption = new Option<string[]>("colNames", "-c", "--cols")
    {
        Required = true,
        CustomParser = res => res.Tokens.Select(t => t.Value).ToArray()
    };

    private readonly Option<string> rootXPathOption = new Option<string>("rootXpath", "-r", "--root")
    {
        Required = true
    };

    private readonly Argument<string> xmlPathArgument = new Argument<string>("xmlPath")
    {
        Description = "Path to XML file",
    };

    private readonly TextWriter writer;

    public TableCommand(TextWriter writer) : base("table", "Creates a table")
    {
        Add(columnNamesOption);
        Add(rootXPathOption);
        Add(xmlPathArgument);
        SetAction(ActionHandler);
        this.writer = writer;
    }

    private async Task ActionHandler(ParseResult result)
    {
        var columnNames = result.GetValue(columnNamesOption)!;
        var rootXPath = result.GetValue(rootXPathOption)!;
        var xmlPath = result.GetValue<string>("xmlPath");
        if (xmlPath == null) return;

        xmlPath = Path.GetFullPath(xmlPath);
        if (!Path.Exists(xmlPath))
        {
            writer.WriteLine("XML path does not exist");
            return;
        }

        var xml = new XmlDocument();
        xml.Load(xmlPath);

        var table = new List<List<string>>();

        var nodes = xml.SelectNodes(rootXPath);
        if (nodes is null) return;

        foreach (var node in nodes)
        {
            if (node is XmlElement)
            {
                var el = (XmlElement)node;
                var list = new List<string>();

                for (int i = 0; i < columnNames.Length - 1; i++)
                {
                    var element = el.SelectSingleNode(columnNames[i]);
                    writer.Write($"{GetValue(element)},");
                }

                writer.WriteLine(GetValue(el.SelectSingleNode(columnNames[columnNames.Length - 1])));
            }
        }
    }

    private string GetValue(XmlNode? element)
    {
        if (element is null) return "";
        else if (element is XmlAttribute) return element.Value ?? "";
        else if (element is XmlElement) return element.InnerXml;
        return "";
    }
}
