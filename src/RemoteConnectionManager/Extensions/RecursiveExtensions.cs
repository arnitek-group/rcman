using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteConnectionManager.Extensions
{
    public static class RecursiveExtensions
    {
        public static IEnumerable<T> GetFlatList<T>(
            this IEnumerable<T> rootItems,
            Func<T, IEnumerable<T>> selector,
            Func<T, bool> filter)
        {
            var flatList = new List<T>();
            VisitChildrenRecursive(rootItems, selector, filter, ref flatList);

            return flatList;
        }

        private static void VisitChildrenRecursive<T>(
            IEnumerable<T> rootItems,
            Func<T, IEnumerable<T>> selector,
            Func<T, bool> filter,
            ref List<T> flatList)
        {
            foreach (var rootItem in rootItems)
            {
                if (filter != null && filter(rootItem))
                {
                    flatList.Add(rootItem);
                }
                if (selector != null)
                {
                    var childItems = selector(rootItem);
                    if (childItems != null && childItems.Any())
                    {
                        VisitChildrenRecursive(childItems, selector, filter, ref flatList);
                    }
                }
            }
        }
    }
}
