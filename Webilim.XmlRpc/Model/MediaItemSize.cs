namespace Webilim.XmlRpc.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class MediaItemSize
    {
        public string file;
        public int width;
        public int height;
        public string mime_type;

        public static MediaItemSize ParseInMembers(IEnumerable<XElement> members)
        {
            MediaItemSize size = new MediaItemSize();

            size.file = members.ParseInMember("file", MemberValueTypes.@string);
            size.width = members.ParseInMember("width", MemberValueTypes.@int).ToInt32();
            size.height = members.ParseInMember("height", MemberValueTypes.@int).ToInt32();
            size.mime_type = members.ParseInMember("mime-type", MemberValueTypes.@string);


            return size;
        }
    }
}
