namespace Bussiness
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    public static class XmlExtends
    {
        public static string ToString(this XElement node, bool check)
        {
            StringBuilder output = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings {
                CheckCharacters = check,
                OmitXmlDeclaration = true,
                Indent = true
            };
            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                node.WriteTo(writer);
            }
            return output.ToString();
        }
    }
}

