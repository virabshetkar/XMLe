using System.Xml;

namespace xmle.Services;

public interface ICsvParser
{
    void ToCsv(string xmlPath, string toPath);
}

public class CsvParser : ICsvParser
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
            string? name = dataXml.Attributes?["name"]?.Value;
            var comment = dataXml.SelectSingleNode("comment")?.InnerText;

            csvOutput += $"{name},{comment},{value}\n";
        }

        File.WriteAllText(toPath, csvOutput);
    }
}