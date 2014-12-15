using UnityEngine;
using System.Collections;

public class UIQuestRemainTime : MonoBehaviour
{
    public UILabel time;

    FHQuest quest = null;

    public void Setup(FHQuest _quest)
    {
        quest = _quest;
    }

    void Update()
    {
        int _seconds = 0;
        if (quest != null)
            _seconds = (int)quest.GetRemainTime();

        if (time != null)
            time.text = string.Format("{0:00}:{1:00}", _seconds / 60, _seconds % 60);
    }
}
