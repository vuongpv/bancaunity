using System;
using System.Collections.Generic;

namespace GFramework
{
	/// <summary>
	/// Multimap
	/// </summary>
	/// <typeparam name="KeyType"></typeparam>
	/// <typeparam name="ValueType"></typeparam>
	public class SortedMultiMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
	{
		SortedDictionary<TKey, List<TValue>> _dictionary; 

		public SortedMultiMap()
		{
			_dictionary = new SortedDictionary<TKey, List<TValue>>();
		}

		public SortedMultiMap(IComparer<TKey> comparer)
		{
			_dictionary = new SortedDictionary<TKey, List<TValue>>(comparer);
		}

		public void Add(TKey key, TValue value)
		{
			List<TValue> list;
			if (this._dictionary.TryGetValue(key, out list))
			{
				list.Add(value);
			}
			else
			{
				list = new List<TValue>();
				list.Add(value);
				this._dictionary[key] = list;
			}
		}

		public IEnumerable<TKey> Keys
		{
			get
			{
				return this._dictionary.Keys;
			}
		}

		public IEnumerable<List<TValue>> Values
		{
			get
			{
				return this._dictionary.Values;
			}
		}

		public List<TValue> this[TKey key]
		{
			get
			{
				List<TValue> list;
				if (this._dictionary.TryGetValue(key, out list))
				{
					return list;
				}
				else
				{
					return new List<TValue>();
				}
			}
		}


		#region IEnumerable<KeyValuePair<TKey,List<TValue>>> Members

		public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	/// <summary>
	/// Multimap with priority
	/// </summary>
	/// <typeparam name="KeyType"></typeparam>
	/// <typeparam name="ValueType"></typeparam>
	public class MultiPriorityMap<TKey, TValue>
	{
		Dictionary<TKey, SortedMultiMap<int, TValue>> _dictionary;

		public MultiPriorityMap()
		{
			_dictionary = new Dictionary<TKey, SortedMultiMap<int, TValue>>();
		}

		public MultiPriorityMap(IEqualityComparer<TKey> comparer)
		{
			_dictionary = new Dictionary<TKey, SortedMultiMap<int, TValue>>(comparer);
		}

		public void Add(TKey key, int priority, TValue value)
		{
			SortedMultiMap<int, TValue> priorityMap;
			if (this._dictionary.TryGetValue(key, out priorityMap))
			{
				priorityMap.Add(priority, value);
			}
			else
			{
				priorityMap = new SortedMultiMap<int, TValue>();
				priorityMap.Add(priority, value);
				this._dictionary[key] = priorityMap;
			}
		}

		public bool ContainsKey(TKey key)
		{
			return _dictionary.ContainsKey(key);
		}

		public IEnumerable<TKey> Keys
		{
			get
			{
				return this._dictionary.Keys;
			}
		}

		public SortedMultiMap<int, TValue> this[TKey key]
		{
			get
			{
				SortedMultiMap<int, TValue> priorityMap;
				if (this._dictionary.TryGetValue(key, out priorityMap))
				{
					return priorityMap;
				}
				else
				{
					return new SortedMultiMap<int, TValue>();
				}
			}
		}

		public List<TValue> GetValueAsOrderedList(TKey key)
		{
			SortedMultiMap<int, TValue> priorityMap;
			List<TValue> result = new List<TValue>();
			if (this._dictionary.TryGetValue(key, out priorityMap))
			{
				foreach (KeyValuePair<int, List<TValue>> pair in priorityMap)
				{
					result.AddRange(pair.Value);
				}
			}
			return result;
		}

	}
}