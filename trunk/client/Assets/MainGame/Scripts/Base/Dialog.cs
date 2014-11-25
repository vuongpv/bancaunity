using UnityEngine;
using System.Collections;
using System;

public class Dialog : BaseDialog
{

		UILabel title, message, textLeft, textRight, textCenter;
		GameObject leftButton, rightButton, centerButton;
		Action mCallback;
	
		void Awake ()
		{
				title = transform.FindChild ("Title").GetComponent<UILabel> ();
				message = transform.FindChild ("Message").GetComponent<UILabel> ();
				textLeft = transform.FindChild ("ButtonBlue").FindChild ("TextButton").GetComponent<UILabel> ();
				textRight = transform.FindChild ("ButtonRed").FindChild ("TextButton").GetComponent<UILabel> ();
				textCenter = transform.FindChild ("ButtonCenterDialog").FindChild ("TextButton").GetComponent<UILabel> ();
		
				leftButton = transform.FindChild ("ButtonBlue").gameObject;
				rightButton = transform.FindChild ("ButtonRed").gameObject;
				centerButton = transform.FindChild ("ButtonCenterDialog").gameObject;
		
				leftButton.SetActive (false);
				rightButton.SetActive (false);
				centerButton.SetActive (false);
		}
	
		private void _ShowDialog (string title, string messager)
		{
				this.title.text = title;
				this.message.text = messager;
		}
	
		public void ShowDialog (string title, string messager, string button, Action callback)
		{
				centerButton.name = button;
				centerButton.SetActive (true);
				_ShowDialog (title, messager);
				textCenter.text = button;
				SetCallBack (callback);
		}
	
		public void ShowDialog (string title, string messager, string leftButton, string rightButton, string centerButton, Action callback)
		{
				ShowDialog (title, messager, centerButton, callback);
				this.leftButton.SetActive (true);
				this.rightButton.SetActive (true);
				this.leftButton.name = leftButton;
				this.rightButton.name = rightButton;
				textLeft.text = leftButton;
				textRight.text = rightButton;
		}
	
		public void ShowDialog (string title, string messager, string leftButton, string rightButton, Action callback)
		{
				_ShowDialog (title, messager);
				this.leftButton.SetActive (true);
				this.rightButton.SetActive (true);
				this.leftButton.name = leftButton;
				this.rightButton.name = rightButton;
				textLeft.text = leftButton;
				textRight.text = rightButton;
		}

		public void CloseDialog (Dialog dl)
		{
				Destroy (dl);
		}

		public override void SetCallBack (System.Action callback)
		{
				this.mCallback = callback;
		}

		public void OnClickButton ()
		{
//				Debug.LogWarning ("======================OnClickButton: " + UICamera.selectedObject.name);
				if (mCallback != null)
						mCallback ();
		}

		
}
