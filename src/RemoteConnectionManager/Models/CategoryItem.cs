using RemoteConnectionManager.Core;

namespace RemoteConnectionManager.Models
{
    public class CategoryItem
    {
        public string DisplayName { get; set; }

        public ConnectionSettings ConnectionSettings { get; set; }
        public Credentials Credentials { get; set; }
        public CategoryItem[] Items { get; set; }
    }
}
