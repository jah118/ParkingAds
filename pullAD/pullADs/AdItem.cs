namespace pullADs
{
    public class AdItem
    {
        public AdItem()
        {
        }

        public AdItem(string content) : this()
        {
            Content = content;
        }

        private string Content { get; set; } = null!;
    }
}