using System.Xml.Linq;
namespace Webilim.XmlRpc.Model
{
    public class Taxonomy
    {
        public enum TaxonomyType 
        {
            category,
            tag
        }

        public string name;
        public string label;
        public bool hierarchical;
        public bool @public;
        public bool show_ui;
        public bool _builtin;

        //struct labels;
        //struct cap;
        //array object_type
    }
}
