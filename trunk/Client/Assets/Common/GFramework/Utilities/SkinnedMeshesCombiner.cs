using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SkinnedMeshesCombiner : MonoBehaviour
{
	/// Usually rendering with triangle strips is faster.
	/// However when combining objects with very low triangle counts, it can be faster to use triangles.
	/// Best is to try out which value is faster in practice.
	public bool castShadows = true;
	public bool receiveShadows = true;

	// Combine mode
	public SkinnedMeshCombinerUtility.CombineMode combineMode = SkinnedMeshCombinerUtility.CombineMode.DuplicateBoneBindpose;
	// End combine behaviour
	public SkinnedMeshCombinerUtility.EndCombine endCombine = SkinnedMeshCombinerUtility.EndCombine.DestroyChild;

	// Filters options
	// Name start with filters
	public string[] startNameFilters;
	// Include inactive
	public bool includeInactive;
	// Direct children only
	public bool directChildOnly;

	// Interval to check 
	public float delay = 0;
	public float pollingInterval = 0;

	// Rebuild now
	public bool rebuildNow = false;

	// Current combine group
	private Dictionary<SkinnedMeshRenderer, Material[]> currCombinedKey;
	
	// Use this for initialization
	void Start()
	{
		StartCoroutine(PollingCombine());
	}

    void OnDestroy()
    {
        SkinnedMeshRenderer thisRenderer = GetComponent<SkinnedMeshRenderer>();
		if (thisRenderer != null && thisRenderer.sharedMesh != null)
        {
            Destroy(thisRenderer.sharedMesh);
            thisRenderer.sharedMesh = null;
        };
    }

	public void Combine(bool force)
	{

		//Getting all Skinned Renderer from Children
		SkinnedMeshRenderer[] newCombinedChildren = GetChildSkinnedMeshRenderer();

		if (newCombinedChildren.Length > 0)
		{
			if (force || NeedUpdateCombinedKey(newCombinedChildren))
			{
				//Debug.Log("******* Rebuild combine skinned mesh - Reason: Sub skinned mesh changed. *******");

				// Make sure we have a SkinnedMeshRenderer
				SkinnedMeshRenderer thisRenderer = GetComponent<SkinnedMeshRenderer>();
				if (thisRenderer == null)
				{
					thisRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
					thisRenderer.castShadows = castShadows;
					thisRenderer.receiveShadows = receiveShadows;
				}

				SkinnedMeshCombinerUtility combiner = new SkinnedMeshCombinerUtility(combineMode, endCombine, 2000, 64);
                if (thisRenderer.sharedMesh != null)
                {
                    Destroy(thisRenderer.sharedMesh);
                    thisRenderer.sharedMesh = null;
                }
				combiner.Combine(thisRenderer, newCombinedChildren);

				// Update combine key
				UpdateCombinedKey(newCombinedChildren);
			}
		}
		else
		{
			SkinnedMeshRenderer thisRenderer = GetComponent<SkinnedMeshRenderer>();
			if (thisRenderer != null)
				UnityEngine.Object.Destroy(thisRenderer);
		}
	}

	public IEnumerator PollingCombine()
	{
		yield return new WaitForSeconds(delay);

		do
		{
			Combine(false);

			if (pollingInterval > 0)
				yield return new WaitForSeconds(pollingInterval);
		}
		while (pollingInterval > 0);
	}

	/// <summary>
	/// Determines whether [is skinned mesh renderer the same] [the specified sm renderer1].
	/// </summary>
	private bool NeedUpdateCombinedKey(SkinnedMeshRenderer[] newSmRenderers)
	{
		if (currCombinedKey == null)
			return true;

		if (currCombinedKey.Count != newSmRenderers.Length)
			return true;

		// Check top level renderer
		foreach (var smRenderer in newSmRenderers)
		{
			if (!currCombinedKey.ContainsKey(smRenderer))
				return true;
		}

		// Check second level renderer material
		foreach (var smRenderer in newSmRenderers)
		{
			if(!smRenderer.sharedMaterials.SequenceEqual(currCombinedKey[smRenderer]))
				return true;
		}

		return false;

		/*foreach (var mat in smRenderer1.materials)
		{
			Debug.LogError("[1]" + smRenderer1.name + " " + mat.name);
		}
		foreach (var mat in smRenderer2.materials)
		{
			Debug.LogError("[2]" + smRenderer2.name + " " + mat.name);
		}

		if (smRenderer1.sharedMaterials.SequenceEqual(smRenderer2.materials))
			return true;*/
		
		//Debug.LogError(">>>>>>>> Material is changed");
		//return false;
	}

	/// <summary>
	/// Updates the combined key.
	/// </summary>
	private void UpdateCombinedKey(SkinnedMeshRenderer[] newSmRenderers)
	{
		if (currCombinedKey == null)
			currCombinedKey = new Dictionary<SkinnedMeshRenderer, Material[]>();

		currCombinedKey.Clear();
		foreach (var smRenderer in newSmRenderers)
		{
			currCombinedKey.Add(smRenderer, smRenderer.sharedMaterials);
		}
	}
	
	/// <summary>
	/// Gets the child skinned mesh renderer.
	/// </summary>
	private SkinnedMeshRenderer[] GetChildSkinnedMeshRenderer()
	{
		SkinnedMeshRenderer[] smRenderers;
		if (directChildOnly)
		{
			smRenderers = transform.Cast<Transform>().Select(t => t.GetComponent<SkinnedMeshRenderer>()).Where(sm => ValidateSkinnedMesh(sm)).ToArray();
		}
		else
		{
			smRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true).Where(sm => ValidateSkinnedMesh(sm)).ToArray();
		}

		if (startNameFilters == null || startNameFilters.Length == 0)
			return smRenderers;

		return smRenderers.Where(sm =>
		{
			if (sm.gameObject == gameObject)
				return false;

			foreach (var str in startNameFilters)
			{
				if (sm.name.StartsWith(str))
					return true;
			}

			return false;
		}).ToArray();
	}


	/// <summary>
	/// Validates the skinned mesh.
	/// </summary>
	private bool ValidateSkinnedMesh(SkinnedMeshRenderer smRenderer)
	{
		if (smRenderer == null || smRenderer.sharedMesh == null)
			return false;

		if (!includeInactive && smRenderer.gameObject.active == false)
			return false;

		foreach (var material in smRenderer.sharedMaterials)
		{
			if (material.mainTexture == null)
				return false;
		}

		return true;
	}

#if UNITY_EDITOR
	void Update()
	{
		if (rebuildNow)
		{
			Combine(true);
			rebuildNow = false;
		}
	}
#endif
}