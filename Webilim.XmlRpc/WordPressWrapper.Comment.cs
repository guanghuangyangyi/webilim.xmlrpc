using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Webilim.XmlRpc.Model;

namespace Webilim.XmlRpc
{
    //Comment
    public partial class WordPressWrapper
    {
        #region Comments

        public CommentCount GetCommentCount(int postId)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getCommentCount")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", postId))))
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            CommentCount count = new CommentCount();
            List<XElement> parameters = xDocResponse.Root.Element("params")
                .Element("param")
                .Element("value")
                .Element("struct")
                .Elements("member").ToList();

            count.approved = parameters.ParseInMember("approved", MemberValueTypes.@int).ToInt32();
            count.awaiting_moderation = parameters.ParseInMember("awaiting_moderation", MemberValueTypes.@string).ToInt32();
            count.spam = parameters.ParseInMember("spam", MemberValueTypes.@int).ToInt32();
            count.total_comments = parameters.ParseInMember("total_comments", MemberValueTypes.@int).ToInt32();

            return count;
        }

        public Comment GetComment(int commentId)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getComment")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", commentId))))
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            List<XElement> members = xDocResponse.Root.Element("params")
                .Element("param")
                .Element("value")
                .Element("struct")
                .Elements("member").ToList();

            return Comment.ParseInMembers(members);
        }

        public IEnumerable<Comment> GetComments(CommentFilter filter)
        {
            filter.number = filter.number > 0 ? filter.number : 10;
            filter.offset = filter.offset > 0 ? filter.offset : 0;

            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getComments")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "status"), new XElement("value", filter.comment_status.ToString())),
                        new XElement("member", new XElement("name", "number"), new XElement("value", filter.number)),
                        new XElement("member", new XElement("name", "offset"), new XElement("value", filter.offset))
                    )
               )
            ));

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            //parse terms elements
            IEnumerable<XElement> elements = xDocResponse.Root.Element("params")
                .Element("param")
                .Element("value")
                .Element("array")
                .Element("data")
                .Elements("value");

            List<Comment> commentList = new List<Comment>();
            foreach (XElement element in elements)
            {
                List<XElement> members = element
                    .Element("struct")
                    .Elements("member").ToList();

                commentList.Add(Comment.ParseInMembers(members));
            }

            return commentList;
        }

        public string NewComment(Comment comment)
        {
            //int post_id, string content, string author, string author_url, string author_email

            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.newComment")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", comment.post_id))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "content"), new XElement("value", comment.content)),
                        new XElement("member", new XElement("name", "author"), new XElement("value", comment.author)),
                        new XElement("member", new XElement("name", "author_url"), new XElement("value", comment.author_url)),
                        new XElement("member", new XElement("name", "author_email"), new XElement("value", comment.author_email))
                    )
               )
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            string comment_id = xDocResponse.Root
                .Element("params")
                .Element("param")
                .Element("value")
                .Element("int").Value;

            //maalesef wordpress new comment sırasında gonderilen administrator bilgileri ile yorum yapildigini dusunmekte,
            //bu yuzden ekledikten sonra isim ve email adrsini degistirmek icin ek islem yapmak gerekiyor
            comment = GetComment(comment_id.ToInt32());

            //to edit author, author_email, author_url
            EditComment(comment);

            return comment_id;
        }

        public bool EditComment(Comment comment)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.editComment")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", comment.comment_id))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "status"), new XElement("value", comment.status.ToString())),
                        new XElement("member", new XElement("name", "content"), new XElement("value", comment.content)),
                        new XElement("member", new XElement("name", "author"), new XElement("value", comment.author)),
                        new XElement("member", new XElement("name", "author_url"), new XElement("value", comment.author_url)),
                        new XElement("member", new XElement("name", "author_email"), new XElement("value", comment.author_email))
                    )
               )
            ));

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            bool result = xDocResponse.Root
                .Element("params")
                .Element("param")
                .Element("value")
                .Element("boolean").Value != "0";

            return result;
        }

        public bool DeleteComment(int commentId)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.deleteComment")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", commentId))))
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

        #endregion
    }
}
