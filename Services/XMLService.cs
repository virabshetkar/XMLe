using System.Xml;

namespace XMLe.Services;

public class XMLService
{
    public XmlDocument GetBuffer(string filepath)
    {
        var doc = new XmlDocument();
        doc.Load(filepath);
        return doc;
    }

    private XmlElement GetDataValue(XmlDocument xml, string id)
    {
        var el = xml.SelectSingleNode($"//data[@id='{id}']//value");
        if (el == null) throw new Exception($"Could not find {id}");
        if (el is not XmlElement) throw new Exception($"{id} is not an XmlElement");
        return (XmlElement)el;
    }

    public string GetValue(XmlDocument xml, string id)
    {
        return GetDataValue(xml, id).InnerXml;
    }

    public void UpdateValue(XmlDocument xml, string id, string value)
    {
        var el = GetDataValue(xml, id);
        el.InnerText = value;
    }
}
