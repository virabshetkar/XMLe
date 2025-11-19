using Newtonsoft.Json;
using System.Xml;

namespace XMLe.Services;

public class XMLService
{
    public XmlDocument GetRootXml(string filepath)
    {
        var xrs = new XmlReaderSettings()
        {
            IgnoreWhitespace = true,
        };

        var reader = XmlReader.Create(filepath, xrs);

        var doc = new XmlDocument();

        doc.Load(reader);
        return doc;
    }

    public string GetValueFromXpath(XmlDocument xml, string xpath)
    {
        var el = xml.SelectNodes(xpath);
        if (el == null) throw new Exception("No element found!");

        return JsonConvert.SerializeObject(el, Newtonsoft.Json.Formatting.Indented);
    }

    public void UpdateValueForXpath(XmlDocument xml, string xpath, string value)
    {
        var el = xml.SelectSingleNode(xpath);
        if (el == null) throw new Exception("No element found!");

        if (el is XmlAttribute)
        {
            el.Value = value;
        }
        else if (el is XmlElement)
        {
            el.InnerXml = value;
        }
    }
}
