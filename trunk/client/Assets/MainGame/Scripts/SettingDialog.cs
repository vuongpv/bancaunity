using UnityEngine;
using System.Collections;

public class SettingDialog : BaseDialog
{
		System.Action mCallback;
	
		public override void SetCallBack (System.Action callback)
		{
				mCallback = callback;
		}
		public void OnClick ()
		{
				if (mCallback != null)
						mCallback ();
		}
}
