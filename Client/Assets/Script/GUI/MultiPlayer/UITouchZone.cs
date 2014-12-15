using UnityEngine;
using System.Collections;

public class UITouchZone : MonoBehaviour
{
    const float NOTIFY_WAITING_TIME = 5.0f;

    public FHPlayerMultiController player;

    public UISprite background;

    bool isFlicking = false;

    Color normalColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);
    Color sharingCoinColor = new Color(1.0f, 0.87f, 0.34f, 1.0f);

    float lastShootingTime;

    public void Flick()
    {
        if (isFlicking)
            return;

        isFlicking = true;

        StopAllCoroutines();
        StartCoroutine(_Flick());
    }

    public void StopFlick()
    {
        StopAllCoroutines();

        background.alpha = 1.0f;

        isFlicking = false;
    }

    public void Reset()
    {
        StopAllCoroutines();

        gameObject.collider.enabled = false;
        background.alpha = 0.0f;

        isFlicking = false;

        if (player.isActive)
            CheckNotify();
    }

    public void Show()
    {
        gameObject.SetActiveRecursively(true);
        CheckNotify();
    }

    public void Hide()
    {
        gameObject.SetActiveRecursively(false);
    }

    public void StopNotify()
    {
        background.alpha = 0.0f;
        StopAllCoroutines();
    }

    public void CheckNotify()
    {
        background.alpha = 0.0f;
        lastShootingTime = Time.time;
        
        StopAllCoroutines();
        StartCoroutine(_CheckNotify());
    }

    IEnumerator _CheckNotify()
    {
        yield return new WaitForSeconds(NOTIFY_WAITING_TIME);

        Notify();
    }

    void Notify()
    {
        if (FHMultiPlayerManager.instance.sharingCoinObj != null)
            return;

        background.color = normalColor;
    }

    public void StartSharingCoin()
    {
        gameObject.collider.enabled = true;
        background.color = sharingCoinColor;
    }

    IEnumerator _Flick()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.9f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.8f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.7f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.6f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.5f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.4f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.3f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.2f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.1f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 0.0f;

            yield return new WaitForSeconds(0.1f);
            background.alpha = 1.0f;
        }
    }
}