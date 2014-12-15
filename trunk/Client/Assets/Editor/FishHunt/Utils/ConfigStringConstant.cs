using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.IO;

public class ConfigStringConstant {

	[MenuItem("FishHunt/Gen String Constant")]
	static void GenContants()
	{
		string file = "Assets/Resources/Config/ConfigString.txt";
		try
		{
			var configString = new ConfigString();

			configString.BeginLoadAppend();
			configString.LoadFromString(((TextAsset)AssetDatabase.LoadAssetAtPath(file, typeof(TextAsset))).text);
			configString.EndLoadAppend();


			StringBuilder sb = new StringBuilder();
			sb.AppendLine("public static class FHStringConst {");
			foreach (var record in configString.records)
			{
				if( !string.IsNullOrEmpty(record.alias) )
					sb.AppendLine("\tpublic const int " + record.alias + " = " + record.id + ";");
			}
			sb.AppendLine("}");

			File.WriteAllText("Assets/Script/FishHunt/FHStringConst.cs", sb.ToString());
			AssetDatabase.ImportAsset(file);
			AssetDatabase.Refresh();
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Gen string constant failed. " + ex.ToString());
		}
	}
}
