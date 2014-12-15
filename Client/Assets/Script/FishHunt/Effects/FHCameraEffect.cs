using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class FHCameraEffect : MonoBehaviour
{
		public float shakingTime = 3.0f;
		public float shakingAmplitude = 3.0f;
		public float shakingPeriod = 0.05f;

		private UIAnchor[] gunContainers;
		private bool isShaking = false;

		public float posX {
				get {
						return transform.position.x;
				}

				set {
						transform.position = new Vector3 (value, transform.position.y, transform.position.z);
				}
		}

		public float posZ {
				get {
						return transform.position.z;
				}

				set {
						transform.position = new Vector3 (transform.position.x, transform.position.y, value);
				}
		}

		void Start ()
		{
				FHPlayerController[] playerControllers = (FHPlayerController[])GameObject.FindSceneObjectsOfType (typeof(FHPlayerController));
				if (playerControllers == null || playerControllers.Length <= 0)
						return;

				gunContainers = new UIAnchor[playerControllers.Length];

				for (int i = 0; i < playerControllers.Length; i++)
						gunContainers [i] = playerControllers [i].transform.GetComponentInChildren<UIAnchor> ();

				isShaking = false;
		}
    
		public void Shake ()
		{
				if (isShaking)
						return;

				for (int i = 0; i < gunContainers.Length; i++)
						gunContainers [i].enabled = false;

				HOTween.Shake (this, shakingTime, new TweenParms ()
            .Prop ("posX", 0)
            .Prop ("posZ", 0)
            .OnComplete (OnShakingComplete)
            , shakingAmplitude, shakingPeriod);

				isShaking = true;
		}

		void OnShakingComplete ()
		{
				for (int i = 0; i < gunContainers.Length; i++)
						gunContainers [i].enabled = true;

				isShaking = false;
		}
}
