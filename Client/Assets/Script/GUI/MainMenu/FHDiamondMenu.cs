using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GFramework;
public class FHDiamondMenu : MonoBehaviour {
    public class FHDiamondEventTime
    {
        public int timeStart;
        public int timeEnd;
        public bool Init(string str)
        {
            try
            {
                string[] split = new string[] { "-" };
                string[] data = str.Split(split, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 2)
                {
                    timeStart = (int)(float.Parse(data[0])*3600);// convert to second
                    timeEnd = (int)(float.Parse(data[1])* 3600);
                    //Debug.LogError(timeStart + "," + timeEnd);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool CheckActiveEvent(int _time)
        {
            if(_time>=timeStart&&_time<=timeEnd)
            {
                return true;
            }
            return false;
        }
        public int GetTimeToDiamondEvent(int _time)
        {
            if(_time>timeEnd)
            {
                return 24*3600-_time+timeStart;
            }
            else if(_time<timeStart)
            {
                return timeStart-_time;
            }
            return 0;
        }
    }
    public GameObject objReady;
    public GameObject objWaiting;
    public UILabel labelWaiting;
    public UILabel labelPlay;
    private float timeStartCount;
    private List<FHDiamondEventTime> listEvent = new List<FHDiamondEventTime>();
    private int currentTimeServer=0;
    private float getTimeStartCount;
    protected JobScheduler scheduler = new JobScheduler();
	// Use this for initialization
	void Start () {
        Init();
         
        FHHttpClient.GetCurrentDiamond((code, json) =>
        {
            if (code == FHResultCode.OK)
            {
                Debug.LogWarning("FH Diamond Server:"+ json.ToString());
                int diamond = int.Parse((string)json["diamond"]);
                FHPlayerProfile.instance.diamond = diamond;
                FHDiamondHudPanel.instance.UpdateDiamond();
            }
            else
            {
                Debug.LogError("AAAAAA");
            }
        });
        FHHttpClient.GetCurrentTimeServer((code, json) =>
        {
            if (code == FHResultCode.OK)
            {
                //Debug.LogError("ABBBBBBB:" + code);
                //Debug.LogError("aaaaaa:" + json.ToString());
                long _timeTick = long.Parse((string)json["timeServer"]);
                long _timeZone = long.Parse((string)json["timeZone"]) * -60 * 1000;// add time zone server
                string diamondEventStr = (string)json["timeEvent"];
                string[] split = new string[] { ",", ";" };
                string[] sub = diamondEventStr.Split(split, StringSplitOptions.RemoveEmptyEntries);
                listEvent.Clear();
                for (int i = 0; i < sub.Length; i++)
                {
                    FHDiamondEventTime _evt = new FHDiamondEventTime();
                    if (_evt.Init(sub[i]))
                    {
                        listEvent.Add(_evt);
                    }
                }
                DateTime _Date = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(_timeTick + _timeZone);
                //Debug.LogError("ACCCCCCCCC:" + _Date.ToString());
                currentTimeServer = _Date.Hour * 3600 + _Date.Minute * 60 + _Date.Second;
                getTimeStartCount = Time.realtimeSinceStartup;
                if(listEvent.Count>0)
                {
                    scheduler.AddJob("check_update", OnUpdateDiamondEvent, 1000, 1000, null);
                }
            }
        });
	}
    public void Init()
    {
        objReady.SetActiveRecursively(false);
        objWaiting.SetActiveRecursively(true);
    }
    protected bool OnUpdateDiamondEvent(object param)
    {
        if( listEvent.Count<1)
            return false;
        float now=Time.realtimeSinceStartup;
        float timeCheck=now-getTimeStartCount;
        int current=currentTimeServer+(int)timeCheck;
        FHDiamondEventTime _evt= listEvent[0];
        bool active=false;
        for(int i=0;i<listEvent.Count;i++)
        {
            if(listEvent[i].CheckActiveEvent(current))
            {
                active=true;
                break;
            }
            else
            {
                //Debug.LogError("aaa:" + listEvent[i].GetTimeToDiamondEvent(current));
                if(listEvent[i].GetTimeToDiamondEvent(current)<_evt.GetTimeToDiamondEvent(current))
                {
                    _evt=listEvent[i];
                }
            }
        }
        SetStateEvent(active);
        if (active == false)
        {
            SetTimeWaiting(_evt.GetTimeToDiamondEvent(current));
        }
        return true;
    }
    public void SetStateEvent(bool active)
    {
        objReady.SetActiveRecursively(active);
        objWaiting.SetActiveRecursively(!active);
    }
    public void SetTimeWaiting(int _timeWait)
    {
        string _str="";
        int _hour=_timeWait/3600;
        _timeWait%=3600;
        int _minute=_timeWait/60;
        _timeWait%=60;
        int _second=_timeWait;

        _str+= (_hour>9?_hour.ToString():("0"+_hour.ToString()));
        _str+=":"+(_minute>9?_minute.ToString():("0"+_minute.ToString()));
        _str+=":"+(_second>9?_second.ToString():("0"+_second.ToString()));

        labelWaiting.text = _str;
    }
	// Update is called once per frame
	void Update () {
	    scheduler.Update();
	}
    public void OnPlayDiamond()
    {
        Debug.LogError("PlayDiamond");
    }
    public void Destroy()
    {

    }
}
