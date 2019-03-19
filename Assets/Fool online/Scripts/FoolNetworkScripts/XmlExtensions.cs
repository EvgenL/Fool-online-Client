using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Assets.Fool_online.Scripts.FoolNetworkScripts
{
    static class XmlExtensions
    {
        /// <summary>
        /// Finds element nested in XML XElement by local name
        /// </summary>
        /// <param name="body">XElement which to look</param>
        /// <param name="elementLocalName">Target name</param>
        /// <returns>Found xelement. Null if none</returns>
        public static XElement GetChildElement(this XElement body, string elementLocalName)
        {
            foreach (var element in body.Elements())
            {
                if (element.Name.LocalName == elementLocalName)
                {
                    return element;
                }
            }

            return null;
        }
    }
}
