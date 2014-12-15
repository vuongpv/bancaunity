using UnityEngine;
using System.Collections;

public class FHCoinText : MonoBehaviour
{
    const float FLY_SPEED = 0.5f;
	const float FLY_DISTANCE = 0.5f;
	const float FLY_BEGIN_FADEOUT_DISTANCE = 0.2f;

    public UILabel label;

    Transform _transform;
    Vector3 direction;
    bool canFly = false;

	Vector3 originScale;
	Color originColor;
	float distance;


	void Awake()
	{
		_transform = gameObject.transform;

		if (label == null)
			label = GetComponent<UILabel>();

		originScale = _transform.localScale;
		originColor = label.color;
	}

    public void Setup(int value)
    {
		_transform.localScale = originScale;
		label.color = originColor;
		distance = 0;

        direction = _transform.up;

        label.text = "+" + value.ToString();

        canFly = true;
    }

    void Update()
    {
        if (!canFly)
            return;

		distance += FLY_SPEED * Time.deltaTime;
        _transform.position += direction * FLY_SPEED * Time.deltaTime;
		if (distance > FLY_BEGIN_FADEOUT_DISTANCE)
		{
			Color color = originColor;
			color.a = 1 - (distance - FLY_BEGIN_FADEOUT_DISTANCE) / (FLY_DISTANCE - FLY_BEGIN_FADEOUT_DISTANCE);
			if (color.a < 0)
				color.a = 0;
			label.color = color;
		}

        if( distance > FLY_DISTANCE )
			FHGuiCollectibleManager.instance.Collect(_transform);
    }
}