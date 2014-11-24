using UnityEngine;
using System.Collections;

public class Gold : _MyGameObject
{

		private float price = 1;


		

		public void Init ()
		{
				SetSpeed (0.07f);
			
		}


		public void SetPrice (float p)
		{
				price = p;
		}

		public void Action (bool b)
		{
				if (!b)
						return;

				MoveTo (FHFishSeasonManager.controlGold.transform.position.x, FHFishSeasonManager.controlGold.transform.position.y);

		}


		public float GetPrice ()
		{
				return price;
		}
	
		public override void AtTheTargetPosition ()
		{
				
				FHFishSeasonManager.UpdateGold (price);
				gameObject.SetActive (false);
		}

		
}
