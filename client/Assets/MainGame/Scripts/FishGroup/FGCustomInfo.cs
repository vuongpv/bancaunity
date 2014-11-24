using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum FGType
{
    GROUP_NORMAL = 0,// type custom line, sin, group by using element
    GROUP_LOOP,// type custom line, sin group by using a group element
    GROUP_ACTION,// type drag fish position in game (base on type line or sin movement)
}

[Serializable]
public enum FGKindEvent
{
    NONE = 0,// khong co event
    VELOCITY=1,// tang toc
    STOP=3,// dung lai
    START=4,// dung lai
    LOSE=5,// bien mat
    CURVE=6,// di chuyen hinh sin
}

[Serializable]
public class FGCustomEvent
{
    [SerializeField]
    public FGKindEvent fGKindEvent = FGKindEvent.NONE;
    [SerializeField]
    public float velocityChange=1;
    [SerializeField]
    public float distanceEvent = 0;

    [SerializeField]
    public float heightSin  = 0;

    [SerializeField]
    public int loopSin  = 0;
    public FGCustomEvent()
    {
        fGKindEvent = FGKindEvent.NONE;
        distanceEvent = 0;
        heightSin = 0;
        loopSin = 0;
    }
}

[Serializable]
public class FGFishCollect
{
    [SerializeField]
    public string fishName = "";
    [SerializeField]
    public bool isActive;

    public FGFishCollect(string _fishName, bool _isActive)
    {
        fishName = _fishName;
        isActive = _isActive;
    }
    
}
[Serializable]
public partial class FGCustomInfo
        #region Override 
            //: Spline
            :MonoBehaviour
        #endregion
{
    [SerializeField]
    public FGType fgType = FGType.GROUP_NORMAL;
    [SerializeField]
    public float timeRespawn=0;
    [SerializeField]
    public int loopCountRespawn=1;
    [SerializeField]
    public FishSpeed baseSpeed=FishSpeed.NORMAL;
    //util
    [SerializeField]
    public float sizeGizmos = 0.01f;
    [SerializeField]
    public Color colorGizmos = new Color(1f, 0f, 0.2f, 0.8f);
    [SerializeField]
    public Color colorLine = new Color(1f, 1.0f, 0.2f, 0.7f);
    [SerializeField]
    public bool isSimulation=false;
    [SerializeField]
    public int countCustomEvent = 0;
    [SerializeField]
    public List<FGCustomEvent> customEvent=new List<FGCustomEvent>();
    
    public FGCustomNode[] GetNodes()
    {
        FGCustomNode[] _elements = this.gameObject.GetComponentsInChildren<FGCustomNode>();
        return _elements;
    }

    #region Draw Gizmos
    public void OnDrawGizmos()
    {
        DrawSplineGizmo(colorLine);

        Plane screen = new Plane();
        screen.SetNormalAndPosition(Camera.current.transform.forward, Camera.current.transform.position);
        Gizmos.color = colorGizmos;
        FGCustomNode[] _elements = gameObject.GetComponentsInChildren<FGCustomNode>();
        for (int i = 0; i < _elements.Length; i++)
        {
            float sizeMultiplier = 0f;

            if (Camera.current.orthographic)
                sizeMultiplier = Camera.current.orthographicSize * 2.5f;
            else
                screen.Raycast(new Ray(_elements[i].transform.position, Camera.current.transform.forward), out sizeMultiplier);

            Gizmos.DrawSphere(_elements[i].transform.position, sizeMultiplier * sizeGizmos);
        }
        OnDrawGizmosCenter();
    }
    public void OnDrawGizmosCenter()
    {
        Plane screen = new Plane();
        screen.SetNormalAndPosition(Camera.current.transform.forward, Camera.current.transform.position);
        Gizmos.color = new Color(1,1,1,1);
       
        float sizeMultiplier = 0f;

        if (Camera.current.orthographic)
            sizeMultiplier = Camera.current.orthographicSize * 2.5f;
        else
            screen.Raycast(new Ray(gameObject.transform.position, Camera.current.transform.forward), out sizeMultiplier);

        Vector3 _center = gameObject.transform.position;
        Vector3 left, right, top, down;
        left= right= top= down = _center;
        left.x-= 2;
        right.x += 2;
        top.z -= 2;
        down.z += 2;
        Gizmos.DrawLine(left, right);
        Gizmos.DrawLine(top, down);
        Gizmos.DrawSphere(gameObject.transform.position, sizeMultiplier * 0.002f);

        
    }
    void OnDrawGizmosSelected()
    {
        Plane screen = new Plane();
        Gizmos.color = new Color(1f, 1.0f, 0f, 1.0f);
        screen.SetNormalAndPosition(Camera.current.transform.forward, Camera.current.transform.position);
        FGCustomNode[] _elements = gameObject.GetComponentsInChildren<FGCustomNode>();
        for (int i = 0; i < _elements.Length; i++)
        {
            float sizeMultiplier = 0f;

            if (Camera.current.orthographic)
                sizeMultiplier = Camera.current.orthographicSize * 2.5f;
            else
                screen.Raycast(new Ray(_elements[i].transform.position, Camera.current.transform.forward), out sizeMultiplier);

            Gizmos.DrawSphere(_elements[i].transform.position, sizeMultiplier * sizeGizmos);
        }
    }
    public void DrawSplineGizmo(Color curveColor)
    {
        if (!isSimulation)
            return;
        Gizmos.color = curveColor;
        FGCustomNode[] _elements = gameObject.GetComponentsInChildren<FGCustomNode>();
        for (int i = 0; i < _elements.Length; i++)
        {
            FGCustomNode node = _elements[i];
            if (node != null)
            {
                Vector3 lastPos = node.gameObject.transform.position;
                Vector3 indentifyPos = node.gameObject.transform.position;

                for (float x = 0; x < 40; x += 0.1f)
                {
                    Vector3 curPos = indentifyPos;
                    curPos.x += x;
                    curPos.z += node.heightSin * Mathf.Sin(x / 40 * Mathf.PI * 2 * node.loopSin);
                    Gizmos.DrawLine(lastPos, curPos);
                    lastPos = curPos;

                }
            }
        }

    }
    #endregion
}
