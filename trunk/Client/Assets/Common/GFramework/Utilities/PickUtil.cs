using UnityEngine;
using System.Collections;

public class PickUtil {

	public static bool PickObject(Camera camera, Vector2 screenPos, int layers, out RaycastHit hit)
	{
		Ray ray = camera.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
		return Physics.Raycast(ray, out hit, layers);
	}

	public static GameObject PickObject(Camera camera, Vector2 screenPos, int layers)
	{
		RaycastHit hit;
		if (!PickObject(camera, screenPos, layers, out hit))
			return null;

		return hit.collider.gameObject;
	}
}
