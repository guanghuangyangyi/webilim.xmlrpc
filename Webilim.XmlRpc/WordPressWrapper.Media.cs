using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Webilim.XmlRpc.Model;

namespace Webilim.XmlRpc
{
    //Media
    public partial class WordPressWrapper
    {
        public MediaItem GetMediaItem(int mediaItemId)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getMediaItem")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", mediaItemId))))
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);
            List<XElement> parameters = xDocResponse.Root.Element("params")
                .Element("param")
                .Element("value")
                .Element("struct")
                .Elements("member").ToList();

            MediaItem media = MediaItem.ParseInMembers(parameters);

            return media;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Test.jpg</param>
        /// <param name="filePath">C://Images/Test.jpg</param>
        /// <param name="mimeType">image/jpeg, image/jpg, application/pdf</param>
        /// <param name="postId">58</param>
        /// <returns>path of new image</returns>
        public string UploadFile(string fileName, string filePath, string mimeType, int postId)
        {
            Image image = Image.FromFile(filePath);
            string base64String;
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, image.RawFormat);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                base64String = Convert.ToBase64String(imageBytes);

                ms.Flush();
                ms.Dispose();
            }

            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.uploadFile")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "name"), new XElement("value", new XElement("string", fileName))),
                        new XElement("member", new XElement("name", "type"), new XElement("value", new XElement("string", mimeType))),
                        new XElement("member", new XElement("name", "bits"), new XElement("value", new XElement("base64", base64String))),
                        new XElement("member", new XElement("name", "overwrite"), new XElement("value", new XElement("boolean", "1"))),
                        new XElement("member", new XElement("name", "post_id"), new XElement("value", new XElement("int", postId)))
                    )
               )
            ));

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            try
            {
                //parse terms elements
                IEnumerable<XElement> parameters
                    = xDocResponse.XPathSelectElements("//methodResponse/params/param/value/struct/member");

                return parameters.ParseInMember("url", MemberValueTypes.@string);
            }
            catch
            {
                return "";
            }
            finally
            {
                image.Dispose();

            }
        }
    }
}
