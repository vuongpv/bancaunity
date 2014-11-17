using UnityEngine;
using System.Collections;

public class ShopDialog : BaseDialog
{
		System.Action mCallback;

		public void Show (System.Action callback)
		{
				mCallback = callback;
		}
		public void OnClick ()
		{
				if (mCallback != null)
						mCallback ();
		}
}
