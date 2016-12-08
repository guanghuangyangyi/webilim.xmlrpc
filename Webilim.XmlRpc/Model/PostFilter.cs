namespace Webilim.XmlRpc.Model
{
    public struct PostFilter
    {
        public Post.PostType post_type;
        public Post.PostStatus post_status;
        public string search;
        public int number;
        public int offset;
        public string orderby;
        public string order;
    }
}