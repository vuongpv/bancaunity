using System;
using System.Collections;
using System.Collections.Generic;

namespace GFramework
{
	public class OrderedListItem<TKey> where TKey : IComparable<TKey>
	{
		public TKey key;

		public OrderedListItem()
		{
		}

		public OrderedListItem(TKey key)
		{
			this.key = key;
		}
	}

	public class OrderedList<TKey, TItem> 
		where TItem : OrderedListItem<TKey>, new()
		where TKey : IComparable<TKey>
	{
		class ItemComparer : IComparer<TItem>
		{
			public int Compare(TItem x, TItem y)
			{
				return x.key.CompareTo(y.key);
			}
		}
		private static ItemComparer comparer = new ItemComparer();

		public static void Sort(List<TItem> list)
		{
			list.Sort(comparer);
		}

		public static int FindItemIndex(List<TItem> list, TKey key)
		{
			TItem item = new TItem();
			item.key = key;
			return list.BinarySearch(item, comparer);
		}

		public static void Add(List<TItem> list, TKey key, TItem item)
		{
			item.key = key;
			list.Add(item);
			list.Sort(comparer);
		}

		public static void AddIgnoreOrder(List<TItem> list, TKey key, TItem item)
		{
			item.key = key;
			list.Add(item);
		}

		public static bool AddIfNotExisting(List<TItem> list, TKey key, TItem item)
		{
			int idx = FindItemIndex(list, key);

			// Not found
			if (idx < 0)
			{
				Add(list, key, item);
				return true;
			}

			return false;
		}

		public static bool TryGetValue(List<TItem> list, TKey key, out TItem item)
		{
			int idx = FindItemIndex(list, key);
			if (idx >= 0)
			{
				item = list[idx];
				return true;
			}

			item = default(TItem);
			return false;
		}
	}
}