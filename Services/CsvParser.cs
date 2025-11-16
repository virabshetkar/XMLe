using System.Xml;

namespace XMLe.Services;

public class CsvParser
{

    public void ToCsv(string xmlPath, string toPath)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlPath);

        var dataXmlList = xmlDoc.SelectNodes("//data");
        if (dataXmlList == null) throw new Exception("No data");

        string csvOutput = "id,en-US,de-AT\n";

        foreach (XmlNode dataXml in dataXmlList)
        {
            if (dataXml is null) continue;

            var value = dataXml.SelectSingleNode("value")?.InnerText;
            string? id = dataXml.Attributes?["id"]?.Value;
            var comment = dataXml.SelectSingleNode("comment")?.InnerText;

            csvOutput += $"{id},{comment},{value}\n";
        }

        File.WriteAllText(toPath, csvOutput);
    }
}
