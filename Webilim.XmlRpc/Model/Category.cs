namespace Webilim.XmlRpc.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// Same as term
    /// </summary>
    public class Category
    {
        public string cat_id;
        public string name;
        public string slug;
        public string term_group;
        public string term_taxonomy_id;
        public string description;
        public string parent;
        public int count;

        /// <summary>
        /// Convert a term to category
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public static Category FromTerm(Term term)
        {
            Category c = new Category();

            c.count = term.count;
            c.description = term.description;
            c.name = term.name;
            c.parent = term.parent;
            c.slug = term.slug;
            c.term_group = term.term_group;
            c.cat_id = term.term_id;
            c.term_taxonomy_id = term.term_taxonomy_id;

            return c;
        }

        /// <summary>
        /// Convert a term to category
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public static Term ToTerm(Category cat)
        {
            Term t = new Term();

            t.count = cat.count;
            t.description = cat.description;
            t.name = cat.name;
            t.parent = cat.parent;
            t.slug = cat.slug;
            t.term_group = cat.term_group;
            t.term_id = cat.cat_id;
            t.term_taxonomy_id = cat.term_taxonomy_id;
            t.taxonomy = Taxonomy.TaxonomyType.category.ToString();

            return t;
        }
    }
}
