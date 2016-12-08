using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Webilim.XmlRpc
{
    public enum MemberValueTypes
    {
        @int,
        @string,
        boolean
    }

    public static class ParserHelper
    {
        /// <summary>
        /// parse value from template <member><name>term_id</name><value><string>2</string></value></member>
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="name"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static string ParseInMember(this IEnumerable<XElement> elements, string name, MemberValueTypes valueType)
        {
            return elements
                .Where(x => x.Element("name").Value.Equals(name))
                .Select(x => x.Element("value").Element(valueType.ToString()).Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// parse value from template <member><name>term_id</name><value><string>2</string></value></member>
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="name"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static DateTime ParseInMemberDateTime(this IEnumerable<XElement> elements, string name)
        {
            string value = elements
                .Where(x => x.Element("name").Value.Equals(name))
                .Select(x => x.Element("value").Element("dateTime.iso8601").Value)
                .FirstOrDefault();

            int year = int.Parse(value.Substring(0, 4));
            int month = int.Parse(value.Substring(4, 2));
            int day = int.Parse(value.Substring(6, 2));
            int hour = int.Parse(value.Substring(9, 2));
            int minute = int.Parse(value.Substring(12, 2));
            int second = int.Parse(value.Substring(15, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }

        /// <summary>
        /// Wrap element with <methodResponse><params><param>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static XDocument WrapWithMethodResponse(XElement element)
        { 
            return new XDocument(
                new XElement("methodResponse"
                    , new XElement("params"
                        , new XElement("param", element)
                       )
                    )
            );
        }

        public static int ToInt32(this string value, int defaultValue = 0)
        {
            int returnValue = defaultValue;
            if (!int.TryParse(value, out returnValue))
                return defaultValue;

            return returnValue;
        }
    }
}
