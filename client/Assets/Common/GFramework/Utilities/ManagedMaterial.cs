using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// Renderer extension.
/// </summary>
public static class RendererExtension
{
    public static Material GetManagedMaterial(this Renderer renderer)
    {
        ManagedMaterial managedMaterial = renderer.GetComponent<ManagedMaterial>();
        if (managedMaterial == null)
            managedMaterial = renderer.gameObject.AddComponent<ManagedMaterial>();

        return managedMaterial.GetMaterial(renderer);
    }

	public static void SetManagedMaterial(this Renderer renderer, Material material)
	{
		ManagedMaterial managedMaterial = renderer.GetComponent<ManagedMaterial>();
		if (managedMaterial == null)
			managedMaterial = renderer.gameObject.AddComponent<ManagedMaterial>();

		managedMaterial.SetMaterial(renderer, material);
	}

    public static Material CreateManagedMaterial(this Renderer renderer, Shader shader)
    {
        ManagedMaterial managedMaterial = renderer.GetComponent<ManagedMaterial>();
        if (managedMaterial == null)
            managedMaterial = renderer.gameObject.AddComponent<ManagedMaterial>();

        return managedMaterial.CreateMaterial(renderer, shader);
    }

    public static Material[] GetManagedMaterials(this Renderer renderer)
    {
        ManagedMaterial managedMaterial = renderer.GetComponent<ManagedMaterial>();
        if (managedMaterial == null)
            managedMaterial = renderer.gameObject.AddComponent<ManagedMaterial>();

		return managedMaterial.GetMaterials(renderer);
    }

	public static void SetManagedMaterials(this Renderer renderer, Material[] materials)
	{
		ManagedMaterial managedMaterial = renderer.GetComponent<ManagedMaterial>();
		if (managedMaterial == null)
			managedMaterial = renderer.gameObject.AddComponent<ManagedMaterial>();

		managedMaterial.SetMaterials(renderer, materials);
	}

    public static Material[] CreateManagedMaterials(this Renderer renderer, int size, Shader shader)
    {
        ManagedMaterial managedMaterial = renderer.GetComponent<ManagedMaterial>();
        if (managedMaterial == null)
            managedMaterial = renderer.gameObject.AddComponent<ManagedMaterial>();

        return managedMaterial.CreateMaterials(renderer, size, shader);
    }

}

/// <summary>
/// Cloned material.
/// </summary>
public class ManagedMaterial : MonoBehaviour
{
	private Material[] _materials;

	public Material CreateMaterial(Renderer renderer, Shader shader)
    {
		var mats = CreateMaterials(renderer, 1, shader);
		return mats[0];
    }

    public Material GetMaterial(Renderer renderer)
    {
		var mats = GetMaterials(renderer);
		if (mats != null && mats.Length > 0)
			return mats[0];

		return null;
    }

	public void SetMaterial(Renderer renderer, Material material)
	{
		CleanUp();

		renderer.sharedMaterial = material;
	}

    public Material[] CreateMaterials(Renderer renderer, int size, Shader shader)
    {
        CleanUp();

        _materials = new Material[size];
		for (int i = 0; i < size; i++)
		{
			_materials[i] = new Material(shader);
			_materials[i].name = "@" + _materials[i].name;
		}

		renderer.sharedMaterials = _materials;

		return _materials;
    }

    public Material[] GetMaterials(Renderer renderer)
    {
		Material[] sharedMaterials = renderer.sharedMaterials;
		if (sharedMaterials == null || sharedMaterials.Length == 0)
		{
			CleanUp();
			return null;
		}

		if (_materials == null)
		{
			_materials = sharedMaterials.Select(m => 
				{
					if( m )
					{
						var mat = new Material(m);
						mat.name = "@" + m.name;
						return mat;
					}
					return null;
				}).ToArray();

			//if (_materials[0].name.StartsWith("@@")) 
				//Debug.LogError("Dupppppppppppppppppppppp", gameObject);
		}
		
		renderer.sharedMaterials = _materials;

		return _materials;
    }

	public void SetMaterials(Renderer renderer, Material[] materials)
	{
		CleanUp();

		renderer.sharedMaterials = materials;
	}

    void CleanUp()
    {
	    if (_materials != null)
        {
			foreach (var mat in _materials)
                Destroy(mat);
        }

		_materials = null;
    }

    void OnDestroy()
    {
        CleanUp();
    }
}