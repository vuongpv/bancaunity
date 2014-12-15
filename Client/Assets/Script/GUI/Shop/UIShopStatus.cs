using UnityEngine;
using System.Collections;

public class UIShopStatus : MonoBehaviour
{
    public UILabel message;
    public UILabel countdown;

    float timeout;

    public void Setup(float _timeout)
    {
        timeout = _timeout;
        message.text = FHLocalization.instance.GetString(FHStringConst.PAYMENT_WAITING);
        countdown.text = timeout.ToString();

        StopAllCoroutines();
        StartCoroutine(CountDown());
    }

    public void Reset()
    {
        message.text = "";
        countdown.text = "";
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(1.0f);

        if (timeout > 0)
        {
            timeout = timeout - 1;
            countdown.text = timeout.ToString();

            StartCoroutine(CountDown());
        }
    }
}