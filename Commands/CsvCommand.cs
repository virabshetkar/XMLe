using System.CommandLine;
using XMLe.Services;

namespace XMLe.Commands;


public class CsvCommand
{
    private readonly Option<string> xmlFilePathOption = new Option<string>("xmlFile", "-f", "--file");
    private readonly Option<string> csvFilePathOption = new Option<string>("csvFile", "-o", "--output");

    private readonly Command command = new Command("csv", "CSV based operation");

    public Command Command { get { return command; } }

    public CsvCommand()
    {
        command.Add(xmlFilePathOption);
        command.Add(csvFilePathOption);

        command.SetAction(this.Action);
    }

    public async Task Action(ParseResult parseResult)
    {
        var xmlFilePath = parseResult.GetValue(xmlFilePathOption);
        var csvFilePath = parseResult.GetValue(csvFilePathOption);

        if (xmlFilePath is null)
        {
            Console.WriteLine("No xml path given!");
            return;
        }

        if (csvFilePath is null)
        {
            Console.WriteLine("No csv path given!");
            return;
        }

        xmlFilePath = Path.GetFullPath(xmlFilePath);
        csvFilePath = Path.GetFullPath(csvFilePath);

        var csvParser = new CsvParser();

        csvParser.ToCsv(xmlFilePath, csvFilePath);
    }
}
