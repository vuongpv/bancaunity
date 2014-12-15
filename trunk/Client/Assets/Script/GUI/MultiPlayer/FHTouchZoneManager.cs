using UnityEngine;
using System.Collections;

public class FHTouchZoneManager : SingletonMono<FHTouchZoneManager>
{
    public UITouchZone[] touchZones;

    public UITouchZone zoneHover = null;

    public void Init()
    {
        touchZones = (UITouchZone[])GameObject.FindSceneObjectsOfType(typeof(UITouchZone));

        // Calculate player touch zones radius
        UIRoot root = NGUITools.FindInParents<UIRoot>(GuiManager.instance.gameObject);
        float pixelAdjustment = root.GetPixelSizeAdjustment(Screen.height);

        SphereCollider collider = touchZones[0].gameObject.GetComponent<SphereCollider>();
        Vector3 pos1 = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, Camera.main.transform.position.y));
        Vector3 pos2 = Camera.main.ScreenToWorldPoint(new Vector3(collider.radius / pixelAdjustment, 0.0f, Camera.main.transform.position.y));
        float colliderRadius = Mathf.Abs(pos2.x - pos1.x) * touchZones[0].gameObject.transform.localScale.x;

        for (int i = 0; i < touchZones.Length; i++)
        {
            SphereCollider playerCollider = touchZones[i].player.gameObject.GetComponent<SphereCollider>();
            playerCollider.radius = colliderRadius;
        }

        Reset();
    }
    
    public void Reset()
    {
        for (int i = 0; i < touchZones.Length; i++)
        {
            touchZones[i].Reset();
            if (!touchZones[i].player.isActive)
                touchZones[i].gameObject.SetActiveRecursively(false);
        }

        zoneHover = null;
    }

    public void EnableColliders(FHPlayerMultiController player)
    {
        for (int i = 0; i < touchZones.Length; i++)
            if (touchZones[i].player != player && touchZones[i].player.isActive)
                touchZones[i].StartSharingCoin();
    }

    public void CheckHover()
    {
        GameObject hoveredObject = UICamera.hoveredObject;
        UITouchZone touchZone = (hoveredObject != null) ? hoveredObject.GetComponent<UITouchZone>() : null;
        if (touchZone != null)
        {
            if (touchZone != zoneHover)
            {
                if (zoneHover != null)
                    zoneHover.StopFlick();

                touchZone.Flick();
                zoneHover = touchZone;
            }
        }
        else
        if (zoneHover != null)
        {
            zoneHover.StopFlick();
            zoneHover = null;
        }
    }
}