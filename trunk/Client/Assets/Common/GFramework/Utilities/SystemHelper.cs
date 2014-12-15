using UnityEngine;
using System.Collections;
using GFramework;
using System;



public class SystemHelper {

	private static string _deviceUniqueID;

	public static string deviceUniqueID
	{
		get
		{
			if (_deviceUniqueID == null)
				computeDeviceUniqueID();

			return _deviceUniqueID;
		}
	}

	private static void computeDeviceUniqueID()
	{
		string systemID = SystemInfo.deviceUniqueIdentifier;
		_deviceUniqueID = systemID.Replace("-", "").ToLower();

		/*int len = systemID.Length;
		int numLong = len / 15;
		if (len % 15 > 0)
			numLong++;

		_deviceUniqueID = MathfEx.BaseConvert(16, 36, systemID);*/

		/*byte[] bytes = new Byte[len / 2];
		int numBytes = len / 2;
		for (int i = 0; i < numBytes; i++)
		{
			string hexByte = systemID.Substring(i * 2, 2);
			bytes[i] = Convert.ToByte(hexByte, 16);
		}
		_deviceUniqueID = System.Convert.ToBase64String(bytes);*/
	}
	
}