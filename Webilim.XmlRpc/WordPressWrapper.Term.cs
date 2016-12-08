using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Webilim.XmlRpc.Model;

namespace Webilim.XmlRpc
{
    //Term and Category
    public partial class WordPressWrapper
    {
        #region Category - Term But taxonomy is category

        /// <summary>
        /// Get a category by categoryId - Same as GetTerm
        /// </summary>
        /// <param name="catId"></param>
        /// <returns></returns>
        public Category GetCategory(int catId)
        {
            return Category.FromTerm(GetTerm(Taxonomy.TaxonomyType.category, catId));
        }

        /// <summary>
        /// Get Categories by search  - Same as GetTerms
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Category> GetCategories(TermFilter filter)
        {
            return GetTerms(filter).Select(t => Category.FromTerm(t)).ToList();
        }

        /// <summary>
        /// Create new category - same as NewTerm but accept category model
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public string NewCategory(Category category)
        {
            return NewTerm(Category.ToTerm(category));
        }

        /// <summary>
        /// Delete Category over DeleteTerm as taxonomy = category and termId = catId
        /// </summary>
        /// <param name="catId"></param>
        /// <returns></returns>
        public bool DeleteCategory(int catId)
        {
            return DeleteTerm(Taxonomy.TaxonomyType.category, catId);
        }

        #endregion

        #region Terms - For Categories, Tags and Custom Taxonomies

        public Term GetTerm(Taxonomy.TaxonomyType taxonomy, int termId)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getTerm")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("string", taxonomy.ToString()))),
                    new XElement("param", new XElement("value", new XElement("int", termId))))
               )
            );

            XDocument xDocResponse = RequestResponse(xDocRequest, Url);

            List<XElement> members = xDocResponse.Root.Element("params")
                .Element("param")
                .Element("value")
                .Element("struct")
                .Elements("member").ToList();

            Term term = Term.ParseInMembers(members);

            return term;
        }

        public IEnumerable<Term> GetTerms(TermFilter filter)
        {
            filter.number = filter.number > 0 ? filter.number : 10;
            filter.offset = filter.offset > 0 ? filter.offset : 0;
            filter.search = string.IsNullOrEmpty(filter.search) ? "" : filter.search;
            filter.order = string.IsNullOrEmpty(filter.order) ? "" : filter.order;
            filter.orderby = string.IsNullOrEmpty(filter.orderby) ? "" : filter.orderby;

            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.getTerms")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("string", filter.taxonomy_type.ToString()))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "number"), new XElement("value", filter.number)),
                        new XElement("member", new XElement("name", "offset"), new XElement("value", filter.offset)),
                        new XElement("member", new XElement("name", "orderby"), new XElement("value", filter.orderby)),
                        new XElement("member", new XElement("name", "order"), new XElement("value", filter.order)),
                        new XElement("member", new XElement("name", "hide_empty"), new XElement("value", "0")),
                        new XElement("member", new XElement("name", "search"), new XElement("value", filter.search))
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

            List<Term> termList = new List<Term>();
            foreach (XElement element in elements)
            {
                List<XElement> members = element
                        .Element("struct")
                        .Elements("member").ToList();

                termList.Add(Term.ParseInMembers(members));
            }

            return termList;
        }

        public string NewTerm(Term term)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.newTerm")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "name"), new XElement("value", term.name)),
                        new XElement("member", new XElement("name", "taxonomy"), new XElement("value", term.taxonomy.ToString())),
                        new XElement("member", new XElement("name", "slug"), new XElement("value", term.slug)),
                        new XElement("member", new XElement("name", "description"), new XElement("value", term.description))
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

        public bool EditTerm(Term term)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.editTerm")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("int", term.term_id))),
                    new XElement("struct",
                        new XElement("member", new XElement("name", "taxonomy"), new XElement("value", term.taxonomy)),
                        new XElement("member", new XElement("name", "name"), new XElement("value", term.name)),
                        new XElement("member", new XElement("name", "slug"), new XElement("value", term.slug)),
                        new XElement("member", new XElement("name", "description"), new XElement("value", term.description))
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

        public bool DeleteTerm(Taxonomy.TaxonomyType taxonomy, int termId)
        {
            XDocument xDocRequest = new XDocument(new XElement("methodCall"
                , new XElement("methodName", "wp.deleteTerm")
                , new XElement("params",
                    new XElement("param", new XElement("value", new XElement("string", BlogID))),
                    new XElement("param", new XElement("value", new XElement("string", Username))),
                    new XElement("param", new XElement("value", new XElement("string", Password))),
                    new XElement("param", new XElement("value", new XElement("string", taxonomy.ToString()))),
                    new XElement("param", new XElement("value", new XElement("int", termId))))
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
