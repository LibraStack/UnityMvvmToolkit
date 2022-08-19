using System.Collections.ObjectModel;

namespace Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void Update<T>(this ObservableCollection<T> collection, T item)
        {
            var index = collection.IndexOf(item);
            if (index != -1)
            {
                collection[index] = item;
            }
        }
    }
}