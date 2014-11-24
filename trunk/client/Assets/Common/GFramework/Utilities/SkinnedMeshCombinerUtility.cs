using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SkinnedMeshCombinerUtility
{
	/// <summary>
	/// 
	/// </summary>
	public enum CombineMode
	{
		// Medium speed, largest number of bones, accurate for all type of skinned mesh
		// Duplicate bone/bindpose for every mesh instances processing, so each instance has 
		// separate range of bone/bindpose
		DuplicateBoneBindpose,

		// Fastest speed, smallest number of bones, need standard bindpose in 3D editor tool
		// All instances share the same bone/bindpose hierachy structure, so the bindpose must *BE*
		// freezed to identity in 3D modeling tool for this option to work
		SharedBoneBindpose,

		// Slowest speed, medium number of bones, accurate for all type of skinned mesh
		// Auto calculate overlapped bone/bindpose to share and prevent create new as much as possible,
		// this calculation comsumes time for searching and matrix-comparation
		CalculateOverlappedBoneBindpose,
	}

	/// <summary>
	/// What to do when end combine
	/// </summary>
	public enum EndCombine
	{
		DestroyChild,
		DeactiveChild,
		DisableRenderer,
		Nothing,
	}

	// Optimize type
	public CombineMode optimizeType { get; private set; }

	// End combine behaviour
	public EndCombine endCombine { get; private set; }

	// Hint for alloc optimize: maximum number of vertex per model
	public int maxVertexHint { get; private set; }

	// Hint for alloc optimize: maximum number of bones per model
	public int maxBoneHint { get; private set; }

	// Shared bone/bindpose (for OptimizeType.CalculateOverlappedBoneBindpose)
	public SharedBoneBindposeGroups sharedGroups;

	// Shared bone/bindpose (for OptimizeType.SharedBoneBindpose)
	public Dictionary<Transform, int> boneIndexLookup;

	// Combined bones list
	public List<Transform> bones;

	// Combined bonepose list, the same size with bones list
	public List<Matrix4x4> bindposes;

	// Combined boneweights, the same size with number of vertices
	public List<BoneWeight> boneWeights;

	// List mesh instance
	public List<MeshInstance> meshInstances;

	//Setting my Arrays for copies		
	public List<Material> materials;


	/// <summary>
	/// Mesh instance
	/// </summary>
	public struct MeshInstance
	{
		public Mesh mesh;
		public int subMeshIndex;
	}

	/// <summary>
	/// Bone/bindpose combination to share
	/// </summary>
	public class SharedBoneBindpose
	{
		public int index;
		public Transform bone;
		public Matrix4x4 bindpose;
	}

	/// <summary>
	/// Manage to find item
	/// </summary>
	public class SharedBoneBindposeGroups
	{
		private Dictionary<Transform, List<SharedBoneBindpose>> lookup = new Dictionary<Transform, List<SharedBoneBindpose>>();

		/// <summary>
		/// Finds the specified bone/bindpose.
		/// </summary>
		public SharedBoneBindpose Find(Transform bone, Matrix4x4 bindpose)
		{
			// New bone/bindpose combination
			SharedBoneBindpose boneBindpose = null;
			List<SharedBoneBindpose> sharedGroups;

			// Find
			if (lookup.TryGetValue(bone, out sharedGroups))
			{
				boneBindpose = sharedGroups.Find(b => b.bindpose == bindpose);
			}

			return boneBindpose;
		}

		/// <summary>
		/// Creates the specified bone.
		/// </summary>
		public SharedBoneBindpose Create(Transform bone, Matrix4x4 bindpose, int index)
		{
			SharedBoneBindpose boneBindpose = null;
			List<SharedBoneBindpose> sharedGroups;

			// Find
			if (lookup.TryGetValue(bone, out sharedGroups))
			{
				boneBindpose = sharedGroups.Find(b => b.bindpose == bindpose);
			}
			else // Create groups
			{
				sharedGroups = new List<SharedBoneBindpose>();
				lookup.Add(bone, sharedGroups);
			}

			// Create bone/bindpose
			if (boneBindpose == null)
			{
				boneBindpose = new SharedBoneBindpose();
				sharedGroups.Add(boneBindpose);
			}

			boneBindpose.bone = bone;
			boneBindpose.bindpose = bindpose;
			boneBindpose.index = index;

			return boneBindpose;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SkinnedMeshCombineUtility"/> class.
	/// </summary>
	public SkinnedMeshCombinerUtility(CombineMode optimizeType, EndCombine endCombine, int maxVertexHint, int maxBoneHint)
	{
		this.optimizeType = optimizeType;
		this.endCombine = endCombine;

		if (this.optimizeType == CombineMode.CalculateOverlappedBoneBindpose)
			sharedGroups = new SharedBoneBindposeGroups();
		else if (this.optimizeType == CombineMode.SharedBoneBindpose)
			boneIndexLookup = new Dictionary<Transform, int>();

		this.bones = new List<Transform>(maxBoneHint);
		this.bindposes = new List<Matrix4x4>(maxBoneHint);
		this.boneWeights = new List<BoneWeight>(maxVertexHint);
		this.meshInstances = new List<MeshInstance>();
		this.materials = new List<Material>();
	}


	/// <summary>
	/// Applies the specified sm renderer.
	/// </summary>
	private void Build(SkinnedMeshRenderer smRenderer)
	{
		//Setting Mesh
		smRenderer.sharedMesh = CombineMeshInstance(meshInstances);

		//Setting Bindposes
		smRenderer.sharedMesh.bindposes = bindposes.ToArray();

		//Setting BoneWeights
		smRenderer.sharedMesh.boneWeights = boneWeights.ToArray();

		//Setting bones
		smRenderer.bones = bones.ToArray();

		//Setting Materials
		smRenderer.sharedMaterials = materials.ToArray();

		//objRenderer.sharedMesh.RecalculateNormals();
		//objRenderer.sharedMesh.RecalculateBounds();

		// Log
		//Debug.Log("Combine skinned mesh: type: " + optimizeType + "\nBones: " + bones.Count + ", bindpose: " + bindposes.Count);
	}

	/// <summary>
	/// Combines the mesh instance.
	/// </summary>
	private Mesh CombineMeshInstance(List<MeshInstance> instances)
	{
		// Get total vertex count
		int totalVertexCount = 0;
		foreach (MeshInstance ins in instances)
			totalVertexCount += ins.mesh.vertexCount;

		// Extract resource
		Vector3[] vertices = new Vector3[totalVertexCount];
		Vector3[] normals = new Vector3[totalVertexCount];
		Vector4[] tangents = new Vector4[totalVertexCount];
		Vector2[] uv = new Vector2[totalVertexCount];
		Vector2[] uv1 = new Vector2[totalVertexCount];

		// Copy in to result
		int vertexOffset = 0;
		foreach (MeshInstance ins in instances)
		{
			int count = ins.mesh.vertexCount;
			Array.Copy(ins.mesh.vertices, 0, vertices, vertexOffset, count);
			Array.Copy(ins.mesh.normals, 0, normals, vertexOffset, count);
			Array.Copy(ins.mesh.tangents, 0, tangents, vertexOffset, count);
			Array.Copy(ins.mesh.uv, 0, uv, vertexOffset, count);
			Array.Copy(ins.mesh.uv1, 0, uv1, vertexOffset, count);
			vertexOffset += count;
		}

		// New mesh
		Mesh mesh = new Mesh();
        mesh.name = "SkinnedMeshCombiner";
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.uv1 = uv1;
		mesh.tangents = tangents;

		// Setting sub-meshes base on number of instances
		mesh.subMeshCount = instances.Count;

		// Set triangle index
		vertexOffset = 0;
		for (int subMesh = 0; subMesh < instances.Count; subMesh++)
		{
			MeshInstance ins = instances[subMesh];

			// Get source triangle index
			int[] srcTriangles = ins.mesh.GetTriangles(ins.subMeshIndex);
			int[] dstTriangles = new int[srcTriangles.Length];

			// Increase index by vertex offset
			for (int i = 0; i < srcTriangles.Length; i++)
				dstTriangles[i] = srcTriangles[i] + vertexOffset;

			// Set submesh triangles
			mesh.SetTriangles(dstTriangles, subMesh);

			// Increase offset
			vertexOffset += ins.mesh.vertexCount;
		}

		// Mesh name
		mesh.name = "Combined Mesh";

		return mesh;
	}

	/// <summary>
	/// Clears this instance.
	/// </summary>
	public void Clear(SkinnedMeshRenderer smRenderer)
	{
		smRenderer.sharedMesh = null;
		smRenderer.bones = null;
		smRenderer.sharedMaterials = null;
	}

	/// <summary>
	/// Combines the specified sm renderers.
	/// </summary>
	public void Combine(SkinnedMeshRenderer resultTarget, SkinnedMeshRenderer[] smRenderers)
	{
		if (smRenderers == null || smRenderers.Length == 0)
		{
			Debug.LogWarning("Combine skinned mesh: child renderers are null or empty");
			return;
		}

		//Debug.Log("Combine skinned mesh: child " + smRenderers.Length );

		// Temp values
		int modeDuplicate_boneOffset = 0; // for CombineMode.SharedBoneBindpose

		// Iterate all sub renderers
		for (int i = 0; i < smRenderers.Length; i++)
		{
			//Getting one by one
			SkinnedMeshRenderer smRenderer = smRenderers[i];

			// Making changes to the Skinned Renderer
			MeshInstance instance = new MeshInstance();

			// Setting the Mesh for the instance
			instance.mesh = smRenderer.sharedMesh;

			// Getting all materials
			Material[] sharedMaterials = smRenderer.sharedMaterials;
			for (int t = 0; t < sharedMaterials.Length; t++)
				materials.Add(sharedMaterials[t]);

			// 
			if (smRenderer != null && smRenderer.sharedMesh != null)
			{
				//instance.transform = Matrix4x4.identity; // myTransform /*transform.worldToLocalMatrix */ * smRenderer.transform.localToWorldMatrix;

				//Getting  subMesh
				for (int t = 0; t < smRenderer.sharedMesh.subMeshCount; t++)
				{
					instance.subMeshIndex = t;
					meshInstances.Add(instance);
				}

				//Copying Bones
				Transform[] smBones = smRenderer.bones;
				Matrix4x4[] smBindposes = smRenderer.sharedMesh.bindposes;
				BoneWeight[] smBoneweights = smRenderer.sharedMesh.boneWeights;

				switch (optimizeType)
				{
					case CombineMode.CalculateOverlappedBoneBindpose:
						{
							//Copying Bones
							for (int b = 0; b < smBones.Length; b++)
							{
								// New bone/bindpose combination
								SharedBoneBindpose sharedBoneBindpose = sharedGroups.Find(smBones[b], smBindposes[b]);
								if (sharedBoneBindpose != null)
									continue;

								// Create new bone/bindpose
								sharedBoneBindpose = sharedGroups.Create(smBones[b], smBindposes[b], bones.Count);

								//inserting bones in totalBones
								bones.Add(smBones[b]);
								//Recalculating BindPoses
								bindposes.Add(smBindposes[b]);
								//totalBindPoses[offset] = smRenderer.bones[x].worldToLocalMatrix * transform.localToWorldMatrix;
							}

							//RecalculateBoneWeights
							for (int bw = 0; bw < smBoneweights.Length; bw++)
							{
								//Just Copying and changing the Bones Indexes !!						
								boneWeights.Add(RecalculateBoneIndexes(smBoneweights[bw], sharedGroups, smBones, smBindposes));
							}
						}
						break;

					case CombineMode.SharedBoneBindpose:
						{
							//Copying Bones
							for (int b = 0; b < smBones.Length; b++)
							{
								// New bone/bindpose combination
								if (boneIndexLookup.ContainsKey(smBones[b]))
									continue;

								// Add to lookup
								boneIndexLookup.Add(smBones[b], bones.Count);

								//inserting bones in totalBones
								bones.Add(smBones[b]);
								//Recalculating BindPoses
								bindposes.Add(smBindposes[b]);
							}

							//RecalculateBoneWeights
							for (int bw = 0; bw < smBoneweights.Length; bw++)
							{
								//Just Copying and changing the Bones Indexes !!						
								boneWeights.Add(RecalculateBoneIndexes(smBoneweights[bw], boneIndexLookup, smBones, smBindposes));
							}
						}
						break;

					case CombineMode.DuplicateBoneBindpose:
						{
							// May want to modify this if the renderer shares bones as unnecessary bones will get added.
							foreach (BoneWeight bw in smBoneweights)
							{
								BoneWeight _bw = bw;

								_bw.boneIndex0 += modeDuplicate_boneOffset;
								_bw.boneIndex1 += modeDuplicate_boneOffset;
								_bw.boneIndex2 += modeDuplicate_boneOffset;
								_bw.boneIndex3 += modeDuplicate_boneOffset;

								boneWeights.Add(_bw);
							}
							modeDuplicate_boneOffset += smBones.Length;

							foreach (Transform bone in smBones)
								bones.Add(bone);

							for (int b = 0; b < smBones.Length; b++)
							{
								bindposes.Add(smBindposes[b]); // bones[b].worldToLocalMatrix * transform.localToWorldMatrix);
							}
						}
						break;
				}

				// Disabling current SkinnedMeshRenderer
				EndCombineChildRenderer(smRenderer);
			}
		}

		// Build final mesh
		Build(resultTarget);
	}

	/// <summary>
	/// Called when [end combine].
	/// </summary>
	private void EndCombineChildRenderer(SkinnedMeshRenderer smRenderer)
	{
		switch (endCombine)
		{
			case EndCombine.DestroyChild:
				UnityEngine.Object.Destroy(smRenderer.gameObject);
				break;

			case EndCombine.DeactiveChild:
				smRenderer.gameObject.active = false;
				break;

			case EndCombine.DisableRenderer:
				smRenderer.enabled = false;
				break;
		}
	}

	/// <summary>
	/// Recalculates the bone indexes.
	/// </summary>
	private BoneWeight RecalculateBoneIndexes(BoneWeight bw, SharedBoneBindposeGroups sharedGroups, Transform[] smBones, Matrix4x4[] smBindposes)
	{
		Func<Transform, Matrix4x4, int> GetBoneIdx = (bone, bindpose) =>
		{
			SharedBoneBindpose shareBoneBindpose = sharedGroups.Find(bone, bindpose);
			if (shareBoneBindpose != null)
				return shareBoneBindpose.index;

			return 0;
		};

		BoneWeight retBw = bw;
		retBw.boneIndex0 = GetBoneIdx(smBones[bw.boneIndex0], smBindposes[bw.boneIndex0]);
		retBw.boneIndex1 = GetBoneIdx(smBones[bw.boneIndex1], smBindposes[bw.boneIndex1]);
		retBw.boneIndex2 = GetBoneIdx(smBones[bw.boneIndex2], smBindposes[bw.boneIndex2]);
		retBw.boneIndex3 = GetBoneIdx(smBones[bw.boneIndex3], smBindposes[bw.boneIndex3]);

		//retBw.boneIndex0 = 0;
		//retBw.weight0 = 1;
		//retBw.weight1 = retBw.weight2 = retBw.weight3 = 0;

		//Debug.Log(bw.boneIndex0+ " " + bw.boneIndex1+ " "  + bw.boneIndex2+ " " + bw.boneIndex3+ " " + ">" +
		//	retBw.boneIndex0 + " " + retBw.boneIndex1 + " " + retBw.boneIndex2 + " " + retBw.boneIndex3 + " " + ">" +
		//	retBw.weight0 + " " + retBw.weight1 + " " + retBw.weight2 + " " + retBw.weight3);
		return retBw;
	}

	/// <summary>
	/// Recalculates the bone indexes.
	/// </summary>
	private BoneWeight RecalculateBoneIndexes(BoneWeight bw, Dictionary<Transform, int> boneIndexLookup, Transform[] smBones, Matrix4x4[] smBindposes)
	{
		BoneWeight retBw = bw;
		retBw.boneIndex0 = boneIndexLookup[smBones[bw.boneIndex0]];
		retBw.boneIndex1 = boneIndexLookup[smBones[bw.boneIndex1]];
		retBw.boneIndex2 = boneIndexLookup[smBones[bw.boneIndex2]];
		retBw.boneIndex3 = boneIndexLookup[smBones[bw.boneIndex3]];

		return retBw;
	}


	// 	public bool Approx(Vector3 val, Vector3 about)
	// 	{
	// 		return Mathf.Approximately(val.x, about.x) &&
	// 			Mathf.Approximately(val.y, about.y) &&
	// 			Mathf.Approximately(val.z, about.z);
	// 	}
}
