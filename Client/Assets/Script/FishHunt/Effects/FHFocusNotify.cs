using UnityEngine;
using System.Collections;
using System;

public class FHFocusNotify : MonoBehaviour
{
    const float SCALE_STEP = 10.0f;
    const float MIN_SCALE = 1.0f;
    const float MAX_SCALE = 3.0f;

    Transform _transform;
    bool isScaling = false;
    float direction, currentScale;

    void Awake()
    {
        _transform = gameObject.transform;
    }

    public void Setup()
    {
        _transform.localScale = Vector3.one;
        currentScale = 1.0f;
        direction = 1;

        isScaling = true;
        _transform.localScale = Vector3.one;
    }

    public void Reset()
    {
        isScaling = false;
    }

    void Update()
    {
        if (!isScaling)
            return;

        currentScale += direction * SCALE_STEP * Time.deltaTime;

        if (direction > 0 && currentScale >= MAX_SCALE)
        {
            direction = -direction;
            currentScale = MAX_SCALE;
        }
        else
        if (direction < 0 && currentScale <= MIN_SCALE)
        {
            direction = -direction;
            currentScale = MIN_SCALE;
        }

        _transform.localScale = new Vector3(currentScale, currentScale, 1.0f);
    }
}
