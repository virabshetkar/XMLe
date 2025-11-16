using System.CommandLine;
using XMLe.Services;

namespace XMLe;

public class Program
{
    public static async Task Main(string[] args)
    {
        var xmlFileArgs = new Argument<string>("xmlFile")
        {
            Description = "XML file to edit"
        };

        var keyOption = new Option<string>("key", "-k", "--key")
        {
            Description = "Id of the data to change"
        };

        var valueOption = new Option<string>("value", "-v", "--value")
        {
            Description = "New value for Id specified"
        };

        var root = new RootCommand("My example App");

        var toCsv = new Command("csv", "Covert to Csv");
        var xmlFilePathOption = new Option<string>("xmlFile", "-f", "--file");
        var csvFilePathOption = new Option<string>("csvFile", "-o", "--output");
        toCsv.Add(xmlFilePathOption);
        toCsv.Add(csvFilePathOption);


        toCsv.SetAction((res) =>
        {
            var xmlFilePath = res.GetValue(xmlFilePathOption);
            var csvFilePath = res.GetValue(csvFilePathOption);

            var csvParser = new CsvParser();

            csvParser.ToCsv(xmlFilePath, csvFilePath);
        });

        root.Add(xmlFileArgs);
        root.Add(keyOption);
        root.Add(valueOption);
        root.Add(toCsv);

        root.SetAction((res) =>
        {
            var xmlPath = res.GetValue(xmlFileArgs);
            var keyOpt = res.GetValue(keyOption);
            var valueOpt = res.GetValue(valueOption);

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
        });

        await root.Parse(args).InvokeAsync();
    }
}
