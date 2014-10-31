using UnityEngine;
using System.Collections;

public class Util_Funtion  {
	/**Convert position to view Camera*/
	public static Vector2 convertPositionToCamera(Vector2 v,Camera c)
	{
		return c.ScreenToWorldPoint(v);
	}
	
	public static Vector3 convertPositionToCamera(Vector3 v,Camera c)
	{
		return c.ScreenToWorldPoint(v);
	}

}
