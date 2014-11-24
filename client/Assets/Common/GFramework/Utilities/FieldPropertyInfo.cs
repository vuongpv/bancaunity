using System;
using System.Reflection;
using UnityEngine;

namespace GFramework
{    
	/// <summary>
	/// Field and Property Info in one single class
	/// </summary>
    public class FieldPropertyInfo
	{
		#region Fields
		private MemberInfo memberInfo;
		#endregion

		#region Constructors
		private FieldPropertyInfo()
        {
        }

		public FieldPropertyInfo(FieldInfo fieldInfo)
		{
			this.memberInfo = fieldInfo;
		}

		public FieldPropertyInfo(PropertyInfo propertyInfo)
		{
			this.memberInfo = propertyInfo;
		}

        public FieldPropertyInfo(System.Type type, string fieldPropertyName)
        {
            this.memberInfo = null;
            FieldInfo field = type.GetField(fieldPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
            {
                this.memberInfo = field;
            }
            else
            {
                PropertyInfo property = type.GetProperty(fieldPropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    this.memberInfo = property;
                }
            }
            
			throw new Exception(string.Concat(new object[] { "FieldPropertyInfo: ", type, ", ", fieldPropertyName }));
		}
		#endregion //Constructors

		#region Public methods
		public object GetValue(object obj)
        {
            return this.GetValue(obj, null);
        }

        public object GetValue(object obj, object[] index)
        {
            if (this.memberInfo is FieldInfo)
            {
                FieldInfo memberInfo = this.memberInfo as FieldInfo;
                return memberInfo.GetValue(obj);
            }
            if (this.memberInfo is PropertyInfo)
            {
                PropertyInfo propInfo = this.memberInfo as PropertyInfo;
				return propInfo.GetValue(obj, index);
            }
            return null;
        }

        public bool IsInfoNull()
        {
            return (this.memberInfo == null);
        }

        public void SetValue(object obj, object value)
        {
            this.SetValue(obj, value, null);
        }

        public void SetValue(object obj, object value, object[] index)
        {
            if (this.memberInfo is FieldInfo)
            {
                (this.memberInfo as FieldInfo).SetValue(obj, value);
            }
            else if (this.memberInfo is PropertyInfo)
            {
                (this.memberInfo as PropertyInfo).SetValue(obj, value, index);
            }
        }

        public override string ToString()
        {
            return ((this.memberInfo != null) ? this.memberInfo.ToString() : "");
        }

        public System.Type FieldPropertyType
        {
            get
            {
                if (this.memberInfo is FieldInfo)
                {
                    FieldInfo memberInfo = this.memberInfo as FieldInfo;
                    return memberInfo.FieldType;
                }
                if (this.memberInfo is PropertyInfo)
                {
                    PropertyInfo info2 = this.memberInfo as PropertyInfo;
                    return info2.PropertyType;
                }
                return null;
            }
        }

        public string Name
        {
            get
            {
                return this.memberInfo.Name;
            }
		}
		#endregion
	}
}

