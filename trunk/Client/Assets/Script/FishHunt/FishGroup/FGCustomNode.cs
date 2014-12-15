using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FGCustomNode : MonoBehaviour{

    // x: toa do theo chieu doc
    // z : toa do theo chieu ngang
    // y : canh theo phia truoc hoac phia sau duong di
    [SerializeField]
    public float heightSin = 0;// do cao cua hinh sin
    [SerializeField]
    public int loopSin = 2;// so lan loop chu ky sin
    [SerializeField]
    public float timeAppear = 0;// thoi gian xuat hien
    [SerializeField]
    public FishSpeed fishSpeed = FishSpeed.PARENT;// toc do chay
    [SerializeField]
    public int countCustomEvent = 0;
    [SerializeField]
    public List<FGCustomEvent> customEvent = new List<FGCustomEvent>();
    public Vector3 GetPosition()
    {
        Vector3 temp=this.gameObject.transform.localPosition;
        //Vector3 vec = new Vector3(temp.x,0, temp.z);// function1
        Vector3 vec=new Vector3(temp.z,temp.x,0);// function2
        return vec;
    }
}
