using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

namespace GFramework
{
	public class XmlHelper
	{
		public static Quaternion ToQuaternion(XmlNode node)
		{
			return new Quaternion(XmlConvert.ToSingle(node.Attributes["x"].Value), XmlConvert.ToSingle(node.Attributes["y"].Value), XmlConvert.ToSingle(node.Attributes["z"].Value), XmlConvert.ToSingle(node.Attributes["w"].Value));
		}

		public static Vector3 ToVector3(XmlNode node)
		{
			return new Vector3(XmlConvert.ToSingle(node.Attributes["x"].Value), XmlConvert.ToSingle(node.Attributes["y"].Value), XmlConvert.ToSingle(node.Attributes["z"].Value));
		}


		/* public static T XmlDeserialize<T>(DataWarehouse data) where T: class
		 {
			 return XmlDeserialize<T>(data, new XmlSerializer(typeof(T)));
		 }*/

		public static T XmlDeserialize<T>(string xml)
		{
			using (StringReader reader = new StringReader(xml))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				return (T)serializer.Deserialize(reader);
			}
		}

		public static T XmlDeserialize<T>(XmlReader xml)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(xml);
		}

		/* public static T XmlDeserialize<T>(DataWarehouse data, XmlSerializer serializer) where T: class
		 {
			 return (serializer.Deserialize(data.Navigator.ReadSubtree()) as T);
		 }*/

		public static T XmlDeserializeElement<T>(XmlReader xml, string element)
		{
			xml.ReadStartElement(element);
			T local = XmlDeserialize<T>(xml);
			xml.ReadEndElement();
			return local;
		}

		public static T XmlDeserializeInner<T>(string innerXml)
		{
			return XmlDeserializeInner<T>(innerXml, new XmlSerializer(typeof(T)));
		}

		public static T XmlDeserializeInner<T>(string innerXml, XmlSerializer serializer)
		{
			string elementName = null;
			Attribute[] customAttributes = Attribute.GetCustomAttributes(typeof(T), typeof(XmlRootAttribute));
			if ((customAttributes != null) && (customAttributes.Length > 0))
			{
				elementName = ((XmlRootAttribute)customAttributes[0]).ElementName;
			}
			else
			{
				elementName = typeof(T).Name;
			}
			string s = string.Format("<{0}>{1}</{0}>", elementName, innerXml);
			return (T)serializer.Deserialize(new StringReader(s));
		}

		public static T XmlDeserializeValue<T>(XmlReader xml, string element, T def)
		{
			try
			{
				xml.ReadStartElement(element);
				T local = (T)xml.ReadContentAs(typeof(T), null);
				xml.ReadEndElement();
				return local;
			}
			catch
			{
				return def;
			}
		}

		public static T XmlReadAttribute<T>(XmlReader xml, string attribute, T def)
		{
			xml.MoveToAttribute(attribute);
			T local = (T)xml.ReadContentAs(typeof(T), null);
			xml.MoveToElement();
			return local;
		}

		public static void XmlSerialize<T>(XmlWriter xml, T obj)
		{
			new XmlSerializer(typeof(T)).Serialize(xml, obj);
		}

		public static void XmlSerializeElement<T>(XmlWriter xml, string element, T obj)
		{
			xml.WriteStartElement(element);
			XmlSerialize<T>(xml, obj);
			xml.WriteEndElement();
		}

		public static void XmlSerializeValue(XmlWriter xml, string element, Quaternion q)
		{
			xml.WriteStartElement(element);
			XmlWriteAttribute<double>(xml, "x", (double)q.x);
			XmlWriteAttribute<double>(xml, "y", (double)q.y);
			XmlWriteAttribute<double>(xml, "z", (double)q.z);
			XmlWriteAttribute<double>(xml, "w", (double)q.w);
			xml.WriteEndElement();
		}

		public static void XmlSerializeValue(XmlWriter xml, string element, Vector2 v)
		{
			xml.WriteStartElement(element);
			XmlWriteAttribute<double>(xml, "x", (double)v.x);
			XmlWriteAttribute<double>(xml, "y", (double)v.y);
			xml.WriteEndElement();
		}

		public static void XmlSerializeValue<T>(XmlWriter xml, string element, T obj)
		{
			xml.WriteStartElement(element);
			xml.WriteValue(obj);
			xml.WriteEndElement();
		}

		public static void XmlSerializeValue(XmlWriter xml, string element, Vector3 v)
		{
			xml.WriteStartElement(element);
			XmlWriteAttribute<double>(xml, "x", (double)v.x);
			XmlWriteAttribute<double>(xml, "y", (double)v.y);
			XmlWriteAttribute<double>(xml, "z", (double)v.z);
			xml.WriteEndElement();
		}

		public static void XmlSerializeValue(XmlWriter xml, string element, Vector4 v)
		{
			xml.WriteStartElement(element);
			XmlWriteAttribute<double>(xml, "x", (double)v.x);
			XmlWriteAttribute<double>(xml, "y", (double)v.y);
			XmlWriteAttribute<double>(xml, "z", (double)v.z);
			XmlWriteAttribute<double>(xml, "w", (double)v.w);
			xml.WriteEndElement();
		}

		public static void XmlWriteAttribute<T>(XmlWriter xml, string attribute, T value)
		{
			xml.WriteStartAttribute(attribute);
			xml.WriteValue(value);
			xml.WriteEndAttribute();
		}

		public static void XmlWriteAttribute(XmlWriter xml, string attribute, float value)
		{
			XmlWriteAttribute<double>(xml, attribute, (double)value);
		}
	}

}