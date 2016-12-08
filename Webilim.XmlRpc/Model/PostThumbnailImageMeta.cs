namespace Webilim.XmlRpc.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class PostThumbnailImageMeta
    {
        public int aperture;
        public string credit;
        public string camera;
        public string caption;
        public string copyright;
        public int focal_length;
        public int iso;
        public int shutter_speed;
        public string title;

        public static PostThumbnailImageMeta ParseInMembers(IEnumerable<XElement> members)
        {
            PostThumbnailImageMeta meta = new PostThumbnailImageMeta();

            meta.aperture = members.ParseInMember("aperture", MemberValueTypes.@int).ToInt32();
            meta.credit = members.ParseInMember("credit", MemberValueTypes.@string);
            meta.camera = members.ParseInMember("camera", MemberValueTypes.@string);
            meta.caption = members.ParseInMember("caption", MemberValueTypes.@string);
            meta.copyright = members.ParseInMember("copyright", MemberValueTypes.@string);
            meta.focal_length = members.ParseInMember("focal_length", MemberValueTypes.@int).ToInt32();
            meta.iso = members.ParseInMember("iso", MemberValueTypes.@int).ToInt32();
            meta.shutter_speed = members.ParseInMember("shutter_speed", MemberValueTypes.@int).ToInt32();
            meta.title = members.ParseInMember("title", MemberValueTypes.@string);

            return meta;
        }
    }
}
