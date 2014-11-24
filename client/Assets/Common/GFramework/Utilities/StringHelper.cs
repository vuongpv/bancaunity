using System;
using System.Text;
using System.Globalization;

namespace GFramework
{
	public class StringHelper
	{
		/// <summary>
		/// To convert a byte Array of Unicode values (UTF-8 encoded) to a complete string.
		/// </summary>
		/// <param name="characters">Unicode byte Array to be converted to string</param>
		/// <returns>string converted from Unicode byte Array</returns>
		private static string Utf8ByteArrayToString(byte[] characters)
		{
			UTF8Encoding encoding = new UTF8Encoding();
			string constructedstring = encoding.GetString(characters);
			return (constructedstring);
		}

		/// <summary>
		/// Converts the string to UTF8 byte array and is used in De serialization
		/// </summary>
		/// <param name="pXmlstring"></param>
		/// <returns></returns>
		private static byte[] StringToUtf8ByteArray(string pXmlstring)
		{
			UTF8Encoding encoding = new UTF8Encoding();
			byte[] byteArray = encoding.GetBytes(pXmlstring);
			return byteArray;
		}
	}
}