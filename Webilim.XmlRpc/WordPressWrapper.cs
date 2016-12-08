using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Webilim.XmlRpc.Model;

namespace Webilim.XmlRpc
{
    public partial class WordPressWrapper
    {
        protected int BlogID { get; set; }
        protected static string Username { get; set; }
        protected static string Password { get; set; }
        protected static string Url { get; set; }

        public WordPressWrapper(string url, string username, string password) : this(url, username, password, 0) {
            System.Net.ServicePointManager.Expect100Continue = false;
        }
        
        public WordPressWrapper(string url, string username, string password, int blogID)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            Url = url;
            Username = username;
            Password = password;

            BlogID = blogID;

            if (BlogID == 0)
            {
                try { BlogID = int.Parse(GetUserBlogs().First().blogid); }
                catch { BlogID = 1; }
            }
        }

        public IEnumerable<UserBlog> GetUserBlogs()
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getUsersBlogs")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password)))
               )
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            List<UserBlog> userBlogs = new List<UserBlog>();

            List<XElement> parameters = xDocResponse.Root.Element("params")
                    .Element("param")
                    .Element("value")
                    .Element("array")
                    .Element("data")
                    .Elements("value")
                .ToList();

            foreach (XElement elem in parameters)
            {
                List<XElement> temp = elem.Element("struct").Elements("member").ToList();

                UserBlog blg = new UserBlog();

                blg.blogid = temp.ParseInMember("blogid", MemberValueTypes.@string);
                blg.blogName = temp.ParseInMember("blogName", MemberValueTypes.@string);
                blg.isAdmin = temp.ParseInMember("isAdmin", MemberValueTypes.boolean) != "0";
                blg.url = temp.ParseInMember("url", MemberValueTypes.@string);
                blg.xmlrpc = temp.ParseInMember("xmlrpc", MemberValueTypes.@string);

                userBlogs.Add(blg);
            }

            return userBlogs;
        }

        /// <summary>
        /// Get a user details by its id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User GetUser(int userId)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getUser")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", userId))))
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            List<XElement> members = xDocResponse.Root.Element("params")
                .Element("param")
                .Element("value")
                .Element("struct")
                .Elements("member").ToList();

            User user = User.ParseInMembers(members);

            return user;
        }

        /// <summary>
        /// Get users list
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<User> GetUsers(UserFilter filter)
        {
            filter.number = filter.number > 0 ? filter.number : 10;
            filter.offset = filter.offset > 0 ? filter.offset : 0;
            filter.order = string.IsNullOrEmpty(filter.order) ? "" : filter.order;
            filter.orderby = string.IsNullOrEmpty(filter.orderby) ? "" : filter.orderby;
            filter.who = string.IsNullOrEmpty(filter.who) ? "" : filter.who;
            filter.role = string.IsNullOrEmpty(filter.role) ? "" : filter.role;

            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getUsers")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "number"), new XElement("value", filter.number)),
                        new XElement("member", new XElement("name", "offset"), new XElement("value", filter.offset)),
                        new XElement("member", new XElement("name", "orderby"), new XElement("value", filter.orderby)),
                        new XElement("member", new XElement("name", "order"), new XElement("value", filter.order)),
                        new XElement("member", new XElement("name", "role"), new XElement("role", filter.role)),
                        new XElement("member", new XElement("name", "who"), new XElement("who", filter.who))
                    )
               )
            ));

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            //parse user elements
            IEnumerable<XElement> elements = xDocResponse.Root.Element("params")
                .Element("param")
                .Element("value")
                .Element("array")
                .Element("data")
                .Elements("value");

            List<User> userList = new List<User>();
            foreach (XElement element in elements)
            {
                List<XElement> members = element
                        .Element("struct")
                        .Elements("member").ToList();

                userList.Add(User.ParseInMembers(members));
            }

            return userList;
        }

        private XDocument RequestResponse(XDocument xDocRequest, string Url)
        {
            using (WebClient wc = new WebClient())
            {
                byte[] responseBytes = wc.UploadData(Url, Encoding.UTF8.GetBytes(xDocRequest.ToString()));
                string response = Encoding.UTF8.GetString(responseBytes);

                XDocument xDocResponse = XDocument.Parse(response);

                XElement faultXElement = xDocResponse.Root.Element("fault");
                if (faultXElement != null)
                {
                    List<XElement> members = faultXElement.Element("value").Element("struct").Elements("member").ToList();

                    string faultCode = members
                        .Where(x => x.Element("name").Value.Equals("faultCode"))
                        .Select(x => x.Element("value").Element("int").Value)
                        .FirstOrDefault();

                    string faultString = members
                        .Where(x => x.Element("name").Value.Equals("faultString"))
                        .Select(x => x.Element("value").Element("string").Value)
                        .FirstOrDefault();

                    throw new Exception(string.Format("{0} ({1})", faultString, faultCode));
                }

                return xDocResponse;
            }
        }
    }
}