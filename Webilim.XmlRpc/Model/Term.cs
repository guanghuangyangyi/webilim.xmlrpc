namespace Webilim.XmlRpc.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class Term
    {
        public string term_id;
        public string name;
        public string slug;
        public string term_group;
        public string term_taxonomy_id;
        public string taxonomy;
        public string description;
        public string parent;
        public int count;

        public static Term ParseInMembers(IEnumerable<XElement> members)
        {
            Term term = new Term();

            term.count = members.ParseInMember("count", MemberValueTypes.@int).ToInt32();
            term.description = members.ParseInMember("description", MemberValueTypes.@string);
            term.name = members.ParseInMember("name", MemberValueTypes.@string);
            term.parent = members.ParseInMember("parent", MemberValueTypes.@string);
            term.slug = members.ParseInMember("slug", MemberValueTypes.@string);
            term.taxonomy = members.ParseInMember("taxonomy", MemberValueTypes.@string);
            term.term_group = members.ParseInMember("term_group", MemberValueTypes.@string);
            term.term_id = members.ParseInMember("term_id", MemberValueTypes.@string);
            term.term_taxonomy_id = members.ParseInMember("term_taxonomy_id", MemberValueTypes.@string);

            return term;
        }
    }
}
