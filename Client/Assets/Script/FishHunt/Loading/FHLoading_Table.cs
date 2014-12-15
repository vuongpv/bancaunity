using UnityEngine;
using System.Collections;

public class FHLoading_Table : FHLoading
{

		public FHLoading_Table (FHLoadingManager _manager)
		: base(_manager)
		{
				numberLoadingSteps = 6;
		}
	
		public override void Update (int step)
		{
				switch (step) {
				case 1:
						LoadingStep_1 ();
						break;
			
				case 2:
						LoadingStep_2 ();
						break;
			
				case 3:
						LoadingStep_3 ();
						break;
				}
		}
	
		void LoadingStep_1 ()
		{
				//GameObject.Instantiate((GameObject)Resources.Load("Prefabs/System/FishManager", typeof(GameObject)));
		}
	
		void LoadingStep_2 ()
		{
				//GameObject.Instantiate((GameObject)Resources.Load("Prefabs/System/GunManager", typeof(GameObject)));
				//GameObject.Instantiate((GameObject)Resources.Load("Prefabs/System/SeasonManager", typeof(GameObject)));
		}
	
		void LoadingStep_3 ()
		{
				Application.LoadLevel (FHScenes.Tables);
		}
}
