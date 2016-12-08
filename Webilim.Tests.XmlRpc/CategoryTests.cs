using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Webilim.XmlRpc;
using Webilim.XmlRpc.Model;

namespace Webilim.Tests.XmlRpc
{
    [TestClass]
    public class CategoryTests
    {
        [TestMethod]
        public void GetCategories()
        {
            var siteName = "";
            var username = "";
            var password = "";

            WordPressWrapper wrap = new WordPressWrapper($"http://{siteName}/xmlrpc.php", username, password);
            var cats = wrap.GetCategories(new TermFilter()
            {
                number = 10,
                offset = 0,
                order = "",
                orderby = "",
                search = "",
                taxonomy_type = Taxonomy.TaxonomyType.category
            });


            Assert.AreNotEqual(cats.Count, 0);
        }
    }
}
