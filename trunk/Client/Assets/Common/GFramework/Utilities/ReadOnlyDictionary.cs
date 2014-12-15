using System;
using System.Collections.Generic;
using System.Linq;

namespace GFramework
{
	/// <summary>
	/// 
	/// </summary>
	public static class IDictionaryExtensions
	{
		/// <summary>
		/// Get all relate keys by value
		/// </summary>
		public static bool TryGetKeysByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, out TKey[] keys)
		{
			if (dictionary == null)
				throw new ArgumentNullException("Dictionary is null");

			keys = dictionary.Where(p => p.Value.Equals(value)).Select(p => p.Key).ToArray();
			if (keys.Length == 0)
				return false;

			return true;
		}

		/// <summary>
		/// Get single key by value
		/// </summary>
		public static bool TryGetKeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, out TKey key)
		{
			if (dictionary == null)
				throw new ArgumentNullException("Dictionary is null");

			IEnumerable<TKey> find = dictionary.Where(p => p.Value.Equals(value)).Select(p => p.Key);
			if (find.Any() == false)
			{
				key = default(TKey);
				return false;
			}

			key = find.First();
			return true;
		}
	}

	public class ReadOnlyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		private bool _locked = false;

		public ReadOnlyDictionary()
		{
		}

		public ReadOnlyDictionary(Dictionary<TKey, TValue> dictionary) : base (dictionary)
		{

		}

		public new void Add(TKey key, TValue value)
		{
			if (!_locked)
			{
				base.Add(key, value);
			}
			else
			{
				throw new AccessViolationException();
			}
		}

		public new TValue this[TKey key]
		{
			get
			{
				return base[key];
			}
			set
			{
				if (!_locked)
				{
					base[key] = value;
				}
				else
				{
					throw new AccessViolationException();
				}
			}
		}

		public void Lock()
		{
			_locked = true;
		}
	}
}
