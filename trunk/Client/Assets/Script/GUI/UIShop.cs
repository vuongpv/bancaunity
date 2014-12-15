using UnityEngine;
using System.Collections;

public class UIshop : MonoBehaviour
{

		public UILabel numberGold1, numberGold2, numberGold3;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
		public void Onclick ()
		{
				switch (UICamera.selectedObject.name) {
				case "Button1":
						
						break;
				case "Button2":
						break;
				case "Button3":
						break;
				}
		}
}
