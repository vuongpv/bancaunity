using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GFramework
{
	/// <summary>
	/// Generic object manager container
	/// Support superior finding on object key and flexible object type-based query 
	/// </summary>
	/// <typeparam name="TKey">Key of the object</typeparam>
	/// <typeparam name="TObj">The single object itself</typeparam>
	public class ObjectManager<TKey, TObj> where TObj : class
	{
		#region Fields
		protected Dictionary<TKey, TObj> objects = new Dictionary<TKey, TObj>();

		protected Dictionary<System.Type, Dictionary<TKey, TObj>> types = new Dictionary<System.Type, Dictionary<TKey, TObj>>();
		#endregion

		#region Utilities

		/// <summary>
		/// Return the object count
		/// </summary>
		public int count
		{
			get
			{
				return objects.Count;
			}
		}

		/// <summary>
		/// Return the object type count
		/// </summary>
		public int CountByType(System.Type type)
		{
			int count = 0;
			foreach (var dic in types.Where(p => p.Key == type || p.Key.IsSubclassOf(type)))
			{
				count += dic.Value.Count;
			}
			return count;
		}

		/// <summary>
		/// Return the object type count
		/// </summary>
		public int CountByType<TType>() where TType : TObj
		{
			int count = 0;
			foreach (var dic in types.Where(p => p.Key == typeof(TType) || p.Key.IsSubclassOf(typeof(TType))))
			{
				count += dic.Value.Count;
			}
			return count;
		}
		#endregion

		#region Query objects by type
		/// <summary>
		/// Get all objects of a runtime type
		/// </summary>
		public IEnumerable<TObj> GetObjectsOfType(System.Type type)
		{
			return types.
				Where(p => p.Key == type || p.Key.IsSubclassOf(type)).
				SelectMany(p => p.Value).Select(p => p.Value);
		}

		/// <summary>
		/// Get object of a runtime type
		/// </summary>
		public TObj GetObjectOfType(System.Type type)
		{
			return GetObjectsOfType(type).FirstOrDefault();
		}

		/// <summary>
		/// Get all objects of a generic type
		/// </summary>
		public IEnumerable<TType> GetObjectsOfType<TType>() where TType : TObj
		{
			return types.
				Where(p => p.Key == typeof(TType) || p.Key.IsSubclassOf(typeof(TType))).
				SelectMany(p => p.Value).Select(p => (TType) p.Value);
		}

		/// <summary>
		/// Get object of a generic type
		/// </summary>
		public TType GetObjectOfType<TType>() where TType : TObj
		{
			return GetObjectsOfType<TType>().FirstOrDefault();
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<TObj> GetAllObjects()
		{
			return objects.Select(p => p.Value);
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<KeyValuePair<TKey,TObj>> GetAllPairObjects()
		{
			return objects;
		}
		#endregion

		#region Search objects by criteria

		/// <summary>
		/// Check if object is valid
		/// </summary>
		public bool Contains(TKey key)
		{
			return objects.ContainsKey(key);
		}

		/// <summary>
		/// Find an object by its guid
		/// </summary>
		public TObj Find(TKey key)
		{
			TObj result = default(TObj);
			objects.TryGetValue(key, out result);
			return result;
		}

		/// <summary>
		/// Find an object by its guid
		/// </summary>
		public TObj Find(TKey key, System.Type objectType)
		{
			TObj result = default(TObj);
			objects.TryGetValue(key, out result);
			if (result.GetType() == objectType)
				return result;

			return null;
		}


		/// <summary>
		/// Find an object by its guid
		/// </summary>
		public TType Find<TType>(TKey key) where TType : TObj
		{
			TObj result = default(TObj);
			objects.TryGetValue(key, out result);
			if (result is TType)
				return (TType) result;

			return default(TType);
		}

		/// <summary>
		/// Search all objects using predicate criteria
		/// </summary>
		public IEnumerable<TObj> SearchObjects(Predicate<TObj> criteria)
		{
			return from p in objects
				   where criteria(p.Value)
				   select p.Value;
		}

		/// <summary>
		/// Search an object using predicate criteria
		/// </summary>
		public TObj SearchObject(Predicate<TObj> criteria)
		{
			return SearchObjects(criteria).FirstOrDefault();
		}

		/// <summary>
		/// Search all objects using predicate criteria
		/// </summary>
		public IEnumerable<TType> SearchObjects<TType>(Func<TType, bool> criteria) where TType : TObj
		{
			return GetObjectsOfType<TType>().Where(o => criteria(o));
		}

		/// <summary>
		/// Search an object using predicate criteria
		/// </summary>
		public TType SearchObject<TType>(Func<TType, bool> criteria) where TType : TObj
		{
			return SearchObjects<TType>(criteria).FirstOrDefault();
		}
		#endregion

		#region Instantiate
		/// <summary>
		/// Instantiate a object with a type name
		/// </summary>
		public TObj Instantiate(TKey key, string type, object data)
		{
			TObj obj = (TObj)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(type);
			if (!Add(key, obj, data))
				return null;
			return obj;
		}

		/// <summary>
		/// Instantiate a object with a type and call default constructor
		/// </summary>
		public TObj Instantiate(TKey key, System.Type type, object data)
		{
			TObj obj = Activator.CreateInstance(type) as TObj;
			if (!Add(key, obj, data))
				return null;
			return obj;
		}

		/// <summary>
		/// Instantiate a object with a type and call constructor with parameters
		/// </summary>
		public TObj InstantiateWithParams(TKey key, System.Type type, object[] @param, object data)
		{
			TObj obj = Activator.CreateInstance(type, @param) as TObj;
			if (!Add(key, obj, data))
				return null;
			return obj;
		}

		/// <summary>
		/// Instantiate a object with factory delegate
		/// </summary>
		public TObj InstantiateWithCustomCreator(TKey key, Func<TObj> creatorDelegate, object data)
		{
			TObj obj = creatorDelegate();
			if (!Add(key, obj, data))
				return null;
			return obj;
		}

		/// <summary>
		/// Instantiate a generic object and call default constructor
		/// </summary>
		public TType Instantiate<TType>(TKey key, object data) where TType : class, TObj, new()
		{
			TType obj = new TType();
			if (!Add(key, obj, data))
				return null;
			return obj;
		}

		/// <summary>
		/// Instantiate a generic object and call constructor with parameters 
		/// </summary>
		public TType InstantiateWithParams<TType>(TKey key, object[] @param, object data) where TType : class, TObj
		{
			TType obj = Activator.CreateInstance(typeof(TType), @param) as TType;
			if (!Add(key, obj, data))
				return null;
			return obj;
		}

		/// <summary>
		/// Instantiate a generic object with factory delegate
		/// </summary>
		public TType InstantiateWithCustomCreator<TType>(TKey key, Func<TType> creatorDelegate, object data) where TType : class, TObj
		{
			TType obj = creatorDelegate();
			if (!Add(key, obj, data))
				return null;
			return obj;
		}
		#endregion

		#region Add and remove
		/// <summary>
		/// Add object without pre callback
		/// Usage in sub class to call after async call
		/// </summary>
		public bool AddWithoutCallback(TKey key, TObj obj)
		{
			// Add to global search list
			objects.Add(key, obj);

			// Add to type search dictionaries
			Dictionary<TKey, TObj> typeObjects;
			if (!types.TryGetValue(obj.GetType(), out typeObjects))
			{
				typeObjects = new Dictionary<TKey, TObj>();
				types[obj.GetType()] = typeObjects;
			}
			// Add the object and notify the callbacks
			typeObjects.Add(key, obj);
			return true;
		}

		
		/// <summary>
		/// Add the object 
		/// </summary>
		public bool Add(TKey key, TObj obj)
		{
			return Add(key, obj, null);
		}

		/// <summary>
		/// Add the object 
		/// </summary>
		public bool Add(TKey key, TObj obj, object data)
		{
			// Pre add
			if (!OnObjectPreAdd(key, obj, data))
				return false;

			AddWithoutCallback(key, obj);

			// Post add
			OnObjectPostAdd(key, obj, data);
			return true;
		}

		/// <summary>
		/// Add object without pre callback
		/// Usage in sub class to call after async call
		/// </summary>
		public void RemoveWithoutCallback(TKey key)
		{
			TObj obj = Find(key);
			if (obj == null)
				return;

			// Actual remove
			objects.Remove(key);
			types[obj.GetType()].Remove(key);		
		}

		/// <summary>
		/// Remove the object
		/// </summary>
		public void Remove(TKey key)
		{
			TObj obj = Find(key);
			if (obj == null)
				return;

			// Pre remove
			OnObjectPreRemove(key, obj);

			// Actual remove
			objects.Remove(key);
			types[obj.GetType()].Remove(key);

			// Post remove
			OnObjectPostRemove(key, obj);
		}

		/// <summary>
		/// Clear all objects
		/// </summary>
		public virtual void Clear()
		{
			TKey[] keys = objects.Keys.ToArray();
			
			// Pre remove
			foreach (var key in keys)
				Remove(key);
		}

		#endregion

		#region Callbacks
		/// <summary>
		/// Call before an object about to be added
		/// </summary>
		/// <returns>return false to abort adding object</returns>
		protected virtual bool OnObjectPreAdd(TKey key, TObj obj, object data)
		{
			return true;
		}

		/// <summary>
		/// Call after an object was added
		/// </summary>
		protected virtual void OnObjectPostAdd(TKey key, TObj obj, object data)
		{
		}

		/// <summary>
		/// Call before an object about to be removed
		/// </summary>
		/// <returns>return false to abort removing object</returns>
		protected virtual void OnObjectPreRemove(TKey key, TObj obj)
		{
		}

		/// <summary>
		/// Call after an object was removed
		/// </summary>
		protected virtual void OnObjectPostRemove(TKey key, TObj obj)
		{
		}
		#endregion
	}

	public abstract class ObjectManagerSingleton<TThis, TKey, TObj> : ObjectManager<TKey, TObj>
		where TThis : ObjectManager<TKey, TObj>, new()
		where TObj : class
	{
		private static readonly TThis singleton = new TThis();

		public static TThis instance
		{
			get
			{
				return singleton;
			}
		}
	}
}
