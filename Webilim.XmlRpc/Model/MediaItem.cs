namespace Webilim.XmlRpc.Model
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using System.ComponentModel;

    public struct MediaItem
    {
        public enum mimetypes
        {
            jpeg,
            jpg,
            pdf
        }

        public string attachment_id;
        public int parent;
        public string link;
        public string title;
        public string caption;
        public string description;
        public string thumbnail;
        public MediaItemMetadata metaData;

        public static MediaItem ParseInMembers(IEnumerable<XElement> members)
        {
            MediaItem item = new MediaItem();

            item.attachment_id = members.ParseInMember("attachment_id", MemberValueTypes.@string);
            item.parent = members.ParseInMember("parent", MemberValueTypes.@int).ToInt32();
            item.link = members.ParseInMember("link", MemberValueTypes.@string);
            item.title = members.ParseInMember("title", MemberValueTypes.@string);
            item.caption = members.ParseInMember("caption", MemberValueTypes.@string);
            item.description = members.ParseInMember("description", MemberValueTypes.@string);
            item.thumbnail = members.ParseInMember("thumbnail", MemberValueTypes.@string);

            //metadatas
            List<XElement> meta = members
                .Where(x => x.Element("name").Value.Equals("metadata"))
                .FirstOrDefault()
                .Element("value")
                .Element("struct")
                .Elements("member").ToList();

            item.metaData = MediaItemMetadata.ParseInMembers(meta);

            return item;
        }
    }
}
