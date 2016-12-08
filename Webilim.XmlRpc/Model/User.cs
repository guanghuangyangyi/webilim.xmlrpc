namespace Webilim.XmlRpc.Model
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class User
    {
        public string user_id;
        public string username;
        public string first_name;
        public string last_name;
        public string bio;
        public string email;
        public string nickname;
        public string nicename;
        public string url;
        public string display_name;
        public DateTime registered;

        public static User ParseInMembers(IEnumerable<XElement> members)
        {
            User user = new User();

            user.user_id = members.ParseInMember("user_id", MemberValueTypes.@string);
            user.username = members.ParseInMember("username", MemberValueTypes.@string);
            user.first_name = members.ParseInMember("first_name", MemberValueTypes.@string);
            user.last_name = members.ParseInMember("last_name", MemberValueTypes.@string);
            user.bio = members.ParseInMember("bio", MemberValueTypes.@string);
            user.email = members.ParseInMember("email", MemberValueTypes.@string);
            user.nickname = members.ParseInMember("nickname", MemberValueTypes.@string);
            user.nicename = members.ParseInMember("nicename", MemberValueTypes.@string);
            user.url = members.ParseInMember("url", MemberValueTypes.@string);
            user.display_name = members.ParseInMember("display_name", MemberValueTypes.@string);
            user.registered = members.ParseInMemberDateTime("registered");

            return user;
        }
    }
}