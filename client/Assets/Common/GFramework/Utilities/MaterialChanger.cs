using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using GFramework;

[Serializable]
public class AlternativeMaterial
{
		public string name;
		public Material[] materials;
}

[AddComponentMenu("GFramework/Material Changer")]
public class MaterialChanger : MonoBehaviour
{
		private Renderer _renderer;
		// Multi materials
		private Material[] originals;
		private List<AlternativeMaterial> alternates;

		void Awake ()
		{
				Debug.LogError ("MaterialChanger awake");
				_renderer = renderer;
		}

		public void ResetOriginal ()
		{
				if (_renderer != null)
						originals = _renderer.sharedMaterials;
		}

		public Material GetAlternate (string name)
		{
				return GetAlternate (name, null);
		}

		public Material GetAlternate (string name, string shader)
		{
				Material[] mats = GetAlternates (name, shader);
				if (mats != null && mats.Length > 0)
						return mats [0];

				return null;
		}

		public Material[] GetAlternates (string name)
		{
				return GetAlternates (name, null);
		}

		public Material[] GetAlternates (string name, string shader)
		{
				if (_renderer == null)
						return null;

				if (alternates == null)
						alternates = new List<AlternativeMaterial> ();

				if (originals == null)
						originals = _renderer.sharedMaterials;

				int found = alternates.FindIndex (m => m.name == name);
		
				if (found >= 0)
						return alternates [found].materials;

				AlternativeMaterial amat = new AlternativeMaterial ();
				amat.name = name;
				amat.materials = originals.Select (m => 
				{
						var mat = new Material (m);
						mat.name = "# " + m.name;

						if (!string.IsNullOrEmpty (shader))
								mat.shader = Shader.Find (shader);
						return mat;
				}).ToArray ();
				alternates.Add (amat);

				return amat.materials;
		}

		public void CleanUp ()
		{
				if (alternates != null) {
						foreach (var amat in alternates) {
								//Debug.LogError("Destroy " + amat.name);
								if (amat.materials != null) {
										foreach (Material mat in amat.materials) {
												if (mat != null) {
														//Debug.LogError("Destroy 2 " + mat.name);
														UnityEngine.Object.Destroy (mat);
												}
										}
								}
						}

						alternates.Clear ();
				}

				originals = null;
		}

		public void SetAlternate (string name)
		{
				SetAlternate (name, null);
		}

		public void SetAlternate (string name, string shader)
		{
				if (_renderer == null)
						return;

				renderer.sharedMaterial = GetAlternate (name, shader);
		}

		public void SetAlternates (string name)
		{
				SetAlternates (name, null);
		}

		public void SetAlternates (string name, string shader)
		{
				if (_renderer == null)
						return;

				renderer.sharedMaterials = GetAlternates (name, shader);
		}

		public bool CanChangeMaterial ()
		{
				return _renderer != null && _renderer.sharedMaterial != null;
		}

		public static bool CanChangeMaterial (GameObject go)
		{
				Renderer rd = go.renderer;
				return rd != null && rd.sharedMaterial != null;
		}

		void OnDestroy ()
		{
				CleanUp ();
		}

		public void RestoreMaterial ()
		{
				if (_renderer == null || originals == null)
						return;

				renderer.sharedMaterials = originals;
		}
		//------------------------------------
		// Doan nay can chinh sua lai
		private Material[] temp = null;

		public void ShowWall ()
		{
				if (_renderer == null || originals == null)
						return;

				temp = renderer.sharedMaterials;
				renderer.sharedMaterials = originals;        
		}

		public void RestoreWall ()
		{
				if (_renderer == null || originals == null)
						return;
        
				renderer.sharedMaterials = temp;   
		}
		//------------------------------------

}


