using UnityEngine;
using System.Collections;
using System;

public class GameBoard : MonoBehaviour
{

		public static bool isShowDialog = false;
		public static BaseDialog currenDialog;
		public static  BaseDialog  ShowDialog (string pathPrefabs)
		{
				if (isShowDialog)
						return null;
				isShowDialog = true;
				return	 (Instantiate (Resources.Load (pathPrefabs)) as GameObject).GetComponent<BaseDialog> ();

		}

		public static  void ShowDialog (string pathPrefabs, string title, string content, string centerButton, Action callback)
		{
				if (isShowDialog)
						return;
				isShowDialog = true;
				currenDialog = (Instantiate (Resources.Load (pathPrefabs + Constant.DefaultDialog)) as GameObject).GetComponent<Dialog> ();
				((Dialog)currenDialog).ShowDialog (title, content, centerButton, callback);
		}



		public static  void ShowDialog (string pathPrefabs, string title, string content, string leftButton, string rightButton, string centerButton)
		{
				if (isShowDialog)
						return;
				isShowDialog = true;
				currenDialog = (Instantiate (Resources.Load (pathPrefabs + Constant.DefaultDialog)) as GameObject).GetComponent<Dialog> ();
				((Dialog)currenDialog).ShowDialog (title, content, leftButton, rightButton, centerButton, null);
		}

		public static void ShowDialog (string pathPrefabs, string title, string content, string leftButton, string rightButton)
		{
				if (isShowDialog)
						return;
				isShowDialog = true;
				currenDialog = (Instantiate (Resources.Load (pathPrefabs + Constant.DefaultDialog)) as GameObject).GetComponent<Dialog> ();
				((Dialog)currenDialog).ShowDialog (title, content, leftButton, rightButton, null);
		}


		public static void CloseDialog (BaseDialog	 dl)
		{
				Debug.LogWarning ("Closedialog");
				GameObject.Destroy (dl.gameObject);
				isShowDialog = false;
		}


}
