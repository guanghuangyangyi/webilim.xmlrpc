namespace Webilim.XmlRpc.Model
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class CustomField
    {
        public string id;
        public string key;
        public string value;

        public static CustomField ParseInMembers(IEnumerable<XElement> members)
        {
            CustomField field = new CustomField();

            field.id = members.ParseInMember("id", MemberValueTypes.@string);
            field.key = members.ParseInMember("key", MemberValueTypes.@string);
            field.value = members.ParseInMember("value", MemberValueTypes.@string);

            return field;
        }
    }
}
