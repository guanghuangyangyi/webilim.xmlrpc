using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Webilim.XmlRpc.Model;

namespace Webilim.XmlRpc
{
    //Post
    public partial class WordPressWrapper
    {
        /// <summary>
        /// Get post with postId,
        /// Contains terms, custom fields and thumbnail
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public Post GetPost(int postId)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getPost")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", postId))))
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            List<XElement> members
                = xDocResponse.XPathSelectElements("/methodResponse/params/param/value/struct/member").ToList();

            Post post = Post.ParseInMembers(members);

            return post;
        }

        /// <summary>
        /// Get post by search
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<Post> GetPosts(PostFilter filter)
        {
            filter.number = filter.number > 0 ? filter.number : 10;
            filter.offset = filter.offset > 0 ? filter.offset : 0;
            filter.search = string.IsNullOrEmpty(filter.search) ? "" : filter.search;
            filter.order = string.IsNullOrEmpty(filter.order) ? "" : filter.order;
            filter.orderby = string.IsNullOrEmpty(filter.orderby) ? "" : filter.orderby;

            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getPosts")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "number"), new XElement("value", filter.number)),
                        new XElement("member", new XElement("name", "offset"), new XElement("value", filter.offset)),
                        new XElement("member", new XElement("name", "orderby"), new XElement("value", filter.orderby)),
                        new XElement("member", new XElement("name", "order"), new XElement("value", filter.order)),
                        new XElement("member", new XElement("name", "search"), new XElement("value", filter.search)),
                        new XElement("member", new XElement("name", "post_type"), new XElement("value", filter.post_type.ToString())),
                        new XElement("member", new XElement("name", "post_status"), new XElement("value", filter.post_status.ToString()))
                    )
               )
            ));

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            //parse terms elements
            IEnumerable<XElement> elements
                = xDocResponse.XPathSelectElements("//methodResponse/params/param/value/array/data/value");

            List<Post> postList = new List<Post>();
            foreach (XElement element in elements)
            {
                List<XElement> members = element
                    .Element("struct")
                    .Elements("member").ToList();

                postList.Add(Post.ParseInMembers(members));
            }

            return postList;
        }

        /// <summary>
        /// Crate new Post,Page or Custom post types
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public string NewPost(Post post)
        {
            //categories
            /*
             <struct>
               <member>
               <name>categories</name>
               <value><array><data><value>Category 1</value><value>Category 2</value></data></array></value>
               </member>
             </struct>
            */
            XElement catElements = new XElement("member", new XElement("name", "terms_names"),
                new XElement("struct",
                    new XElement("member",
                        new XElement("name", "category"),
                        new XElement("value",
                            new XElement("array",
                                new XElement("data",
                                    post.terms
                                       .Where(x => x.taxonomy.Equals("Category"))
                                       .Select(x => new XElement("value", x.name)).ToArray()
                                )
                            )
                        )
                    )
                )
            );

            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.newPost")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "post_type"), new XElement("value", post.type)),
                        new XElement("member", new XElement("name", "post_status"), new XElement("value", post.status)),
                        new XElement("member", new XElement("name", "post_title"), new XElement("value", post.title)),
                        new XElement("member", new XElement("name", "post_author"), new XElement("value", post.author)),
                        new XElement("member", new XElement("name", "post_excerpt"), new XElement("value", post.excerpt)),
                        new XElement("member", new XElement("name", "post_content"), new XElement("value", post.content)),
                        //new XElement("member", new XElement("name", "post_date"), new XElement("value", p.post_date)),
                        new XElement("member", new XElement("name", "post_format"), new XElement("value", post.format)),
                        new XElement("member", new XElement("name", "post_name"), new XElement("value", post.name)), //slug
                        new XElement("member", new XElement("name", "post_password"), new XElement("value", post.password)),
                        new XElement("member", new XElement("name", "comment_status"), new XElement("value", post.comment_status)),
                        new XElement("member", new XElement("name", "ping_status"), new XElement("value", post.ping_status)),
                        new XElement("member", new XElement("name", "sticky"), new XElement("value", post.sticky ? "1" : "0")),
                        //new XElement("member", new XElement("name", "post_thumbnail"), new XElement("value", p.post_thumbnail)),//int
                        new XElement("member", new XElement("name", "post_parent"), new XElement("value", post.parent)),
                        catElements//terms_names
                        //custom fields
                        //thumbnails
                    )
                )
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            return xDocResponse.Root
                .Element("params")
                .Element("param")
                .Element("value")
                .Element("string").Value;
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public bool EditPost(Post post)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.editPost")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("string", post.id))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "post_type"), new XElement("value", post.type)),
                        new XElement("member", new XElement("name", "post_status"), new XElement("value", post.status)),
                        new XElement("member", new XElement("name", "post_title"), new XElement("value", post.title)),
                        new XElement("member", new XElement("name", "post_author"), new XElement("value", post.author)),
                        new XElement("member", new XElement("name", "post_excerpt"), new XElement("value", post.excerpt)),
                        new XElement("member", new XElement("name", "post_content"), new XElement("value", post.content)),
                        //new XElement("member", new XElement("name", "post_date"), new XElement("value", p.post_date)),
                        new XElement("member", new XElement("name", "post_format"), new XElement("value", post.format)),
                        new XElement("member", new XElement("name", "post_name"), new XElement("value", post.name)), //slug
                        new XElement("member", new XElement("name", "post_password"), new XElement("value", post.password)),
                        new XElement("member", new XElement("name", "comment_status"), new XElement("value", post.comment_status)),
                        new XElement("member", new XElement("name", "ping_status"), new XElement("value", post.ping_status)),
                        new XElement("member", new XElement("name", "sticky"), new XElement("value", post.sticky)),
                        //new XElement("member", new XElement("name", "post_thumbnail"), new XElement("value", p.post_thumbnail)),//int
                        new XElement("member", new XElement("name", "post_parent"), new XElement("value", post.parent))
                        //custom fields
                        //terms
                        //thumbnails
                    )
               )
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            bool result = xDocResponse.Root
                .Element("params")
                .Element("param")
                .Element("value")
                .Element("boolean").Value != "0";

            return result;
        }

        /// <summary>
        /// Delete a post with post id
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public bool DeletePost(int postId)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.deletePost")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", postId))))
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            bool result = xDocResponse.Root
                .Element("params")
                .Element("param")
                .Element("value")
                .Element("boolean").Value != "0";

            return result;
        }
    }
}
