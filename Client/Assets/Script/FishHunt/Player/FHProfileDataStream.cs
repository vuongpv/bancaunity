using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FHProfileDataStream
{
    public float saveInterval { get; private set; }

    public FHProfileDataStream()
    {
        saveInterval = -1.0f;
    }

    public FHProfileDataStream(float _saveInterval)
    {
        saveInterval = _saveInterval;
    }

    public void Set(string key, string value)
    {
        RawSet(key, Encrypt(value));
    }

    protected virtual void RawSet(string key, string value)
    {
    }

    public virtual string Get(string key, string defaultValue)
    {
        string value = RawGet(key, defaultValue);
        if (value != defaultValue)
            value = Decrypt(value);

        return value;
    }

    protected virtual string RawGet(string key, string defaultValue)
    {
        return "";
    }

    public virtual void Flush()
    {
    }

    protected string Encrypt(string value)
    {
		try
		{
			return Encryption.EncryptWithChecksum(value, SystemHelper.deviceUniqueID);
		}
		catch (Exception e)
		{
			return "";
		}
    }

    protected string Decrypt(string value)
    {
		try
		{
			string rawText;
			if (Encryption.DecryptWithChecksum(value, SystemHelper.deviceUniqueID, out rawText))
				return rawText;
		}
		catch (Exception e)
		{
			return null;
		}

		return null;
    }
}

public class FHProfileDataStream_Internal : FHProfileDataStream
{
    private bool hasChanged = false;

    public FHProfileDataStream_Internal()
        : base()
    {
    }

    public FHProfileDataStream_Internal(float _saveInterval)
        : base(_saveInterval)
    {
    }

    protected override void RawSet(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        hasChanged = true;
    }

    protected override string RawGet(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public override void Flush()
    {
        if (hasChanged)
            PlayerPrefs.Save();
        hasChanged = false;
    }
}

public class FHProfileDataStream_Network : FHProfileDataStream
{
    private Dictionary<string, string> rawData = new Dictionary<string, string>();
    private Dictionary<string, string> dataChanged = new Dictionary<string, string>();

    public FHProfileDataStream_Network(float _saveInterval)
        : base(_saveInterval)
    {
    }

    protected override void RawSet(string key, string value)
    {
        rawData[key] = value;
        dataChanged[key] = value;
    }

    protected override string RawGet(string key, string defaultValue)
    {
        if (rawData.ContainsKey(key))
        {
            Flush();
            return rawData[key];
        }
        else
            return Read(key, defaultValue);
    }

    public override void Flush()
    {
        string data = MiniJSON.Json.Serialize(dataChanged);

        // Send data to server, non-blocking
    }

    private void OnFlushCallback(int resultCode)
    {
        if (resultCode == FHResultCode.OK)
            dataChanged.Clear();
    }

    private string Read(string key, string defaultValue)
    {
        // Read data from server, blocking
        string value = defaultValue;

        rawData[key] = value;
        
        return value;
    }
}