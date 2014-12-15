using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHInputController : MonoBehaviour {

	private Dictionary<int, FHPlayerController> mapFingerToPlayers = new Dictionary<int, FHPlayerController>();

	public FHPlayerController[] playerControllers;

	// Cache
	Collider _collider;    
    Vector3 screenRayOrigin;
    float camPosY;

	void Awake()
	{
		_collider = collider;

        camPosY = Camera.main.transform.position.y;
	}

	void Start()
	{
		playerControllers = (FHPlayerController[]) GameObject.FindSceneObjectsOfType(typeof(FHPlayerController));
	}

	public void OnFingerDown(FingerDownEvent e)
	{
        Ray uiRay = GuiManager.instance.uiCamera.ScreenPointToRay(e.Position);
        if (Physics.Raycast(uiRay, Mathf.Infinity, GlobalLayers.UIMask | GlobalLayers.GunUIObjectsMask))
            return;

        RaycastHit[] hits = Physics.RaycastAll(GetRayOrigin(e.Position), -Vector3.up, Mathf.Infinity, GlobalLayers.PlayersMask);

		foreach(var hit in hits)
		{
			foreach (var player in playerControllers)
			{
				if (player.gameObject == hit.collider.gameObject && !player.isDragging)
				{
					player.OnFingerDown(hit.point);
					mapFingerToPlayers[e.Finger.Index] = player;
				}
			}
		}
	}

	public void OnFingerUp(FingerUpEvent e)
	{
		FHPlayerController player;
		if (!mapFingerToPlayers.TryGetValue(e.Finger.Index, out player))
			return;

		player.OnFingerUp();
		mapFingerToPlayers.Remove(e.Finger.Index);
	}

	public void OnFingerMove(FingerMotionEvent e)
	{
		FHPlayerController player;
		if (!mapFingerToPlayers.TryGetValue(e.Finger.Index, out player))
			return;

		Ray ray = Camera.main.ScreenPointToRay(new Vector3(e.Position.x, e.Position.y, 0));
		RaycastHit hit;
		if (_collider.Raycast(ray, out hit, float.MaxValue))
			player.OnFingerMove(hit.point);
	}

    Vector3 GetRayOrigin(Vector3 screenPos)
    {
        screenRayOrigin.x = screenPos.x;
        screenRayOrigin.y = screenPos.y;
        screenRayOrigin.z = camPosY;

        Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(screenRayOrigin);
        rayOrigin.y = camPosY;

        return rayOrigin;
    }
}