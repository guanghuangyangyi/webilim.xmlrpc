namespace Webilim.XmlRpc.Model
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class MediaItemMetadata
    {
        public int width;
        public int height;
        public string file;

        public MediaItemSize[] sizes;
        public PostThumbnailImageMeta imageMeta;

        public static MediaItemMetadata ParseInMembers(IEnumerable<XElement> members)
        {
            MediaItemMetadata meta = new MediaItemMetadata();

            meta.width = members.ParseInMember("width", MemberValueTypes.@int).ToInt32();
            meta.height = members.ParseInMember("height", MemberValueTypes.@int).ToInt32();
            meta.file = members.ParseInMember("file", MemberValueTypes.@string);

            //sizes
            List<XElement> sizes = members
                .Where(x => x.Element("name").Value.Equals("sizes"))
                .FirstOrDefault()
                .Element("value").Element("struct").Elements("member").ToList();

            List<MediaItemSize> sizeList = new List<MediaItemSize>();
            foreach (XElement elem in sizes)
                sizeList.Add(MediaItemSize.ParseInMembers(elem.Element("value").Element("struct").Elements("member")));

            meta.sizes = sizeList.ToArray();

            //Image Meta Data
            XElement imageMetaElement = members
                .Where(x => x.Element("name").Value.Equals("image_meta"))
                .FirstOrDefault();

            meta.imageMeta = PostThumbnailImageMeta.ParseInMembers(imageMetaElement.Element("value").Element("struct").Elements("member"));

            return meta;
        }
    }
}
