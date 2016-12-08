namespace Webilim.XmlRpc.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class Comment
    {
        public enum CommentStatus
        {
            hold,
            approve,
            spam,
            unapprove
        }

        public string comment_id;
        public string parent;
        public string user_id;
        public DateTime dateCreated;
        public string status;
        public string content;
        public string link;
        public string post_id;
        public string post_title;
        public string author;
        public string author_url;
        public string author_email;
        public string author_ip;
        public string type;

        public static Comment ParseInMembers(IEnumerable<XElement> members)
        {
            Comment c = new Comment();

            c.author = members.ParseInMember("author", MemberValueTypes.@string);
            c.author_email = members.ParseInMember("author_email", MemberValueTypes.@string);
            c.author_ip = members.ParseInMember("author_ip", MemberValueTypes.@string);
            c.author_url = members.ParseInMember("author_url", MemberValueTypes.@string);
            c.comment_id = members.ParseInMember("comment_id", MemberValueTypes.@string);
            c.content = members.ParseInMember("content", MemberValueTypes.@string);
            c.dateCreated = members.ParseInMemberDateTime("date_created_gmt");
            c.link = members.ParseInMember("link", MemberValueTypes.@string);
            c.parent = members.ParseInMember("parent", MemberValueTypes.@string);
            c.post_id = members.ParseInMember("post_id", MemberValueTypes.@string);
            c.post_title = members.ParseInMember("post_title", MemberValueTypes.@string);
            c.status = members.ParseInMember("status", MemberValueTypes.@string);
            c.type = members.ParseInMember("type", MemberValueTypes.@string);
            c.user_id = members.ParseInMember("user_id", MemberValueTypes.@string);

            return c;
        }
    }
}