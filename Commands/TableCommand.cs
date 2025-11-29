using System.CommandLine;
using System.CommandLine.Parsing;
using System.Xml;
using xmle.Services;

namespace xmle.Commands;


public class TableCommand : Command
{
    private readonly IConfigService config;
    private readonly TextWriter writer;

    private string Fit(string s) => s.Length <= 30 ? s.PadRight(30) : s.Substring(0, 29) + " ";

    private readonly Option<string[]> columnNamesOption;
    private readonly Option<string[]> headingsOption;
    private readonly Option<string> rootXPathOption;
    private readonly Argument<string> xmlPathArgument = new Argument<string>("xmlPath")
    {
        Description = "Path to XML file",
    };

    public TableCommand(IConfigService config, TextWriter writer) : base("table", "Creates a table")
    {
        this.config = config;
        this.writer = writer;

        columnNamesOption = new Option<string[]>("colNames", "-c", "--cols")
        {
            DefaultValueFactory = (ArgumentResult res) => { return config.GetConfig()?.Columns ?? null; },
        };

        rootXPathOption = new Option<string>("rootXpath", "-r", "--root")
        {
            DefaultValueFactory = (ArgumentResult res) => { return config.GetConfig()?.Table ?? null; },
        };

        headingsOption = new Option<string[]>("headings", "-h", "--headings")
        {
            DefaultValueFactory = (ArgumentResult res) => { return config.GetConfig()?.Headings ?? null; }
        };

        Add(columnNamesOption);
        Add(rootXPathOption);
        Add(headingsOption);
        Add(xmlPathArgument);
        SetAction(ActionHandler);
    }

    private async Task ActionHandler(ParseResult result)
    {
        var columnNames = result.GetValue(columnNamesOption);
        var headings = result.GetValue(headingsOption);
        var rootXPath = result.GetValue(rootXPathOption);

        if (columnNames is null)
        {
            writer.WriteLine("Columns are not given!");
            return;
        }

        if (rootXPath is null)
        {
            writer.WriteLine("Root Xpath is not given!");
            return;
        }

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

        if (headings is not null)
        {
            if (headings.Length != columnNames.Length) { throw new Exception("Headings and Columns don't match!"); }
            writer.WriteLine(string.Join(",", headings));
        }

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
