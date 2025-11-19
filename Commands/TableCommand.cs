using System.CommandLine;
using System.Xml;

namespace XMLe.Commands;

public class TableCommand : Command
{
    string Fit(string s) => s.Length <= 30 ? s.PadRight(30) : s.Substring(0, 29) + " ";

    private readonly Option<string[]> columnNamesOption = new Option<string[]>("colNames", "-c", "--cols")
    {
        Required = true,
        CustomParser = res => res.Tokens.Select(t => t.Value).ToArray()
    };

    private readonly Option<string> rootXPathOption = new Option<string>("rootXpath", "-r", "--root")
    {
        Required = true
    };

    public TableCommand() : base("table", "Creates a table")
    {
        Add(columnNamesOption);
        Add(rootXPathOption);
        SetAction(ActionHandler);
    }

    private async Task ActionHandler(ParseResult result)
    {
        var columnNames = result.GetValue(columnNamesOption)!;
        var rootXPath = result.GetValue(rootXPathOption)!;
        var xmlPath = result.GetValue<string>("xmlPath");

        xmlPath = Path.GetFullPath(xmlPath);
        if (!Path.Exists(xmlPath))
        {
            Console.WriteLine("XML path does not exist");
            return;
        }

        var xml = new XmlDocument();
        xml.Load(xmlPath);

        var table = new List<List<string>>();

        var nodes = xml.SelectNodes(rootXPath);

        Console.Write("\n");
        foreach (var node in nodes)
        {
            if (node is XmlElement)
            {
                var el = (XmlElement)node;
                var list = new List<string>();

                foreach (var col in columnNames)
                {
                    var element = el.SelectSingleNode(col);
                    if (element is null) Console.Write(Fit("IS NULL!"));
                    else if (element is XmlAttribute) { Console.Write(Fit(element.Value)); }
                    else if (element is XmlElement) { Console.Write(Fit(element.InnerXml)); }
                    Console.Write("\t");
                }

                Console.Write("\n");
            }
        }
    }
}
