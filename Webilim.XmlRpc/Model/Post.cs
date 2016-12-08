namespace Webilim.XmlRpc.Model
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class Post
    {
        public enum PostType
        { 
            post,
            page
        }

        public enum PostStatus
        {
            draft,
            pending,
            @private,
            publish
        }

        public enum CommentStatus
        {
            open,
            closed
        }

        public enum PingStatus
        {
            open,
            closed
        }

        public string id;
        public string title;

        public DateTime date;
        public DateTime date_gmt;
        public DateTime modified;
        public DateTime modified_gmt;
        public string status;
        public string type;
        
        public string format;
        public string name;
        public int author;// author_id
        public string password;
        public string excerpt;
        public string content;
        public string parent;
        public string mime_type;
        public string link;
        public string guid;
        public int menu_order;
        public string comment_status;
        public string ping_status;
        public bool sticky;
        public MediaItem post_thumbnail;
        public Term[] terms;
        public Term[] tags;
        public CustomField[] custom_fields;

        public static Post ParseInMembers(IEnumerable<XElement> members)
        {
            Post post = new Post();

            post.comment_status = members.ParseInMember("comment_status", MemberValueTypes.@string);
            post.guid = members.ParseInMember("guid", MemberValueTypes.@string);
            post.link = members.ParseInMember("link", MemberValueTypes.@string);
            post.menu_order = members.ParseInMember("menu_order", MemberValueTypes.@int).ToInt32();
            post.ping_status = members.ParseInMember("ping_status", MemberValueTypes.@string);
            post.author = members.ParseInMember("post_author", MemberValueTypes.@string).ToInt32();
            post.content = members.ParseInMember("post_content", MemberValueTypes.@string);
            post.date = members.ParseInMemberDateTime("post_date");
            post.date_gmt = members.ParseInMemberDateTime("post_date_gmt");
            post.excerpt = members.ParseInMember("post_excerpt", MemberValueTypes.@string);
            post.format = members.ParseInMember("post_format", MemberValueTypes.@string);
            post.id = members.ParseInMember("post_id", MemberValueTypes.@string);
            post.mime_type = members.ParseInMember("post_mime_type", MemberValueTypes.@string);
            post.modified = members.ParseInMemberDateTime("post_modified");
            post.modified_gmt = members.ParseInMemberDateTime("post_modified_gmt");
            post.name = members.ParseInMember("post_name", MemberValueTypes.@string);
            post.parent = members.ParseInMember("post_parent", MemberValueTypes.@string);
            post.password = members.ParseInMember("post_password", MemberValueTypes.@string);
            post.status = members.ParseInMember("post_status", MemberValueTypes.@string);
            post.title = members.ParseInMember("post_title", MemberValueTypes.@string);
            post.type = members.ParseInMember("post_type", MemberValueTypes.@string);
            post.sticky = members.ParseInMember("sticky", MemberValueTypes.boolean) != "0";

            //terms
            List<XElement> terms = members
                .Where(x => x.Element("name").Value.Equals("terms"))
                .FirstOrDefault()
                .Element("value").Element("array").Element("data").Elements("value").ToList();
            List<Term> termsList = new List<Term>();
            foreach (XElement elem in terms)
                termsList.Add(Term.ParseInMembers(elem.Element("struct").Elements("member")));

            post.terms = termsList.ToArray();

            //custom fields
            List<XElement> custom_fields = members
                .Where(x => x.Element("name").Value.Equals("custom_fields"))
                .FirstOrDefault()
                .Element("value").Element("array").Element("data").Elements("value").ToList();
            List<CustomField> customFieldList = new List<CustomField>();
            foreach (XElement elem in custom_fields)
                customFieldList.Add(CustomField.ParseInMembers(elem.Element("struct").Elements("member")));

            post.custom_fields = customFieldList.ToArray();

            //thumbnail
            XElement elemThumbnail = members
                .Where(x => x.Element("name").Value.Equals("post_thumbnail"))
                .FirstOrDefault();

            post.post_thumbnail = MediaItem.ParseInMembers(elemThumbnail.Element("value").Element("struct").Elements("member"));

            return post;
        }
    }
}