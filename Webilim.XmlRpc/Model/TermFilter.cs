namespace Webilim.XmlRpc.Model
{
    public struct TermFilter
    {
        public Taxonomy.TaxonomyType taxonomy_type;
        public string search;
        public int number;
        public int offset;
        public string orderby;
        public string order;
    }
}