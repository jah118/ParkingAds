using PullADsToMQ.Facade;

namespace PullADsToMQ
{
    public class AdItem : IAdItem
    {
        public AdItem()
        {
        }

        public AdItem(string content) : this()
        {
            Content = content;
        }

        public bool Success { get; set; }
        public string Content { get; set; } = null!;
    }
}