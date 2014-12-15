using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X11;

namespace X11
{
	public static partial class Extensions
	{
        /// <summary>
        /// Get property generic
        /// </summary>
        public static TType GetDataAs<TType>(this Dictionary<short, object> data, short propID)
        {
            // Get object type
            object obj;
			if (!data.TryGetValue(propID, out obj))
                return default(TType);

            return (TType)obj;
        }

		/// <summary>
		/// Get property generic
		/// </summary>
		public static TType GetDataAs<TType>(this Dictionary<short, object> data, short propID, TType defaultValue)
		{
			// Get object type
			object obj;
			if (!data.TryGetValue(propID, out obj))
				return defaultValue;

			return (TType)obj;
		}

		/// <summary>
		/// Change type of a data
		/// </summary>
		public static object ChangeType<TThis>(this TThis instance, System.Type toType)
		{
			if (toType.IsEnum)
				return Enum.ToObject(toType, instance);

			if (toType.IsArray)
			{
				System.Type elementType = toType.GetElementType();
				if (elementType.IsEnum)
				{
					IList des = Array.CreateInstance(elementType, ((IList)instance).Count);
					for (int i = 0; i < des.Count; i++)
					{
						des[i] = Enum.ToObject(elementType,((IList)instance)[i]);
					}
					return Convert.ChangeType(des, toType);
				}
			}
			return Convert.ChangeType(instance, toType);
		}

		/// <summary>
		/// Safe cache and check reference
		/// </summary>
		public static void SafeReference<TThis>(this TThis instance, Action<TThis> func) where TThis : class
		{
			if (instance != null)
			{
				func(instance);
			}
		}

		#region X11 property helpers

		/// <summary>
		/// Convert a short based disctionary to string base dictionary
		/// </summary>
		/*public static Dictionary<string, KeyValuePair<short, object>> _prop(this Dictionary<short, object> instance)
		{
			return instance.ToDictionary(p => p.Key._prop(), p => p);
		}

		/// <summary>
		/// Convert a int ID to propname
		/// </summary>
		public static string _prop(this int instance)
		{
			return XProperties.PropName((short)instance);
		}

		/// <summary>
		/// Convert a short ID to propname
		/// </summary>
		public static string _prop(this short instance)
		{
			return XProperties.PropName(instance);
		}

		/// <summary>
		/// Convert a prop name to short ID
		/// </summary>
		public static short _prop(this string instance)
		{
			return XProperties.PropID(instance);
		}*/

		#endregion

		/*public static bool Any(this IEnumerable source)
		{
			if (source == null)
			{
				throw new NullReferenceException("source");
			}
			using (IEnumerator enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return true;
				}
			}
			return false;
		}*/
  
	}
}
