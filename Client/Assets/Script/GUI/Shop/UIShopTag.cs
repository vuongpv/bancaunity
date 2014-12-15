using UnityEngine;
using System.Collections;

public class UIShopTag : MonoBehaviour
{
    const float SWITCH_TAG_INTERVAL = 1.5f;

    UISprite sprite;

    string[] tags = null;
    int currentTag = -1;

    public void Setup(string _tags)
    {
        UIHelper.DisableWidget(gameObject);

        if (_tags == "")
            return;

        tags = _tags.Split(';');
        if (tags == null || tags.Length <= 0)
            return;

        UIHelper.EnableWidget(gameObject);
        sprite = gameObject.GetComponent<UISprite>();
        SetTag(tags[0]);

        if (tags.Length == 1)
            return;

        currentTag = 0;
        StopAllCoroutines();
        StartCoroutine(SwitchTag());
    }

    IEnumerator SwitchTag()
    {
        yield return new WaitForSeconds(SWITCH_TAG_INTERVAL);

        currentTag++;
        if (currentTag >= tags.Length)
            currentTag = 0;

        SetTag(tags[currentTag]);

        StartCoroutine(SwitchTag());
    }

    void SetTag(string tag)
    {
        sprite.spriteName = "shop_tag_" + tag;
        sprite.MakePixelPerfect();
    }
}