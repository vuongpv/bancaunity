using UnityEngine;
using System.Collections;
using System;

public class RippleMesh : MonoBehaviour
{
	public float dampner = 0.999f;
	public float maxWaveHeight = 2.0f;
	public int splashForce = 1000;
	public int cols = 128;
	public int rows = 128;

	private int[] buffer1;
	private int[] buffer2;
	private int[] vertexIndices;

	private Mesh mesh;
	private Vector3[] vertices;
	//private Vector3[] normals ;

	//public int slowdown = 20;
	//private int slowdownCount = 0;

	private bool swapMe = true;

	// Cache
	private Collider _collider;

	void Awake()
	{
		_collider = GetComponentInChildren<Collider>();
	}

	// Use this for initialization
	void Start()
	{
		MeshFilter mf = (MeshFilter)GetComponent(typeof(MeshFilter));
		mesh = mf.mesh;
		vertices = mesh.vertices;
		buffer1 = new int[vertices.Length];
		buffer2 = new int[vertices.Length];

		Bounds bounds = mesh.bounds;

		float xStep = (bounds.max.x - bounds.min.x) / cols;
		float zStep = (bounds.max.z - bounds.min.z) / rows;

		vertexIndices = new int[vertices.Length];
		int i = 0;
		for (i = 0; i < vertices.Length; i++)
		{
			vertexIndices[i] = -1;
			buffer1[i] = 0;
			buffer2[i] = 0;
		}

		// this will produce a list of indices that are sorted the way I need them to 
		// be for the algo to work right
		for (i = 0; i < vertices.Length; i++)
		{
			float column = ((vertices[i].x - bounds.min.x) / xStep);// + 0.5;
			float row = ((vertices[i].z - bounds.min.z) / zStep);// + 0.5;
			float position = (row * (cols + 1)) + column + 0.5f;
			if (vertexIndices[(int)position] >= 0) print("smash");
			vertexIndices[(int)position] = i;
		}
		SplashAtGridPoint(cols / 2, rows / 2);
	}


	public void SplashAtGridPoint(int x, int y)
	{
		x = Mathf.Clamp(x, 2, cols - 3);
		y = Mathf.Clamp(y, 2, rows - 3);

		int position = ((y * (cols + 1)) + x);
	
		buffer1[position] = splashForce;
		buffer1[position - 1] = splashForce;
		buffer1[position + 1] = splashForce;
		buffer1[position + (cols + 1)] = splashForce;
		buffer1[position + (cols + 1) + 1] = splashForce;
		buffer1[position + (cols + 1) - 1] = splashForce;
		buffer1[position - (cols + 1)] = splashForce;
		buffer1[position - (cols + 1) + 1] = splashForce;
		buffer1[position - (cols + 1) - 1] = splashForce;
	}

	public void SplashAtTexCoordPoint(Vector2 point)
	{
		Bounds bounds = mesh.bounds;
		float xStep = (bounds.max.x - bounds.min.x) / cols;
		float zStep = (bounds.max.z - bounds.min.z) / rows;
		float xCoord = (bounds.max.x - bounds.min.x) * point.x;
		float zCoord = (bounds.max.z - bounds.min.z) * point.y;
		float column = (xCoord / xStep);// + 0.5;
		float row = (zCoord / zStep);// + 0.5;
		SplashAtGridPoint((int)column, (int)row);
	}

	// Update is called once per frame
	void Update()
	{

		int[] currentBuffer;
		if (swapMe)
		{
			// process the ripples for this frame
			ProcessRipples(buffer1, buffer2);
			currentBuffer = buffer2;
		}
		else
		{
			ProcessRipples(buffer2, buffer1);
			currentBuffer = buffer1;
		}
		swapMe = !swapMe;
		// apply the ripples to our buffer
		Vector3[] theseVertices = new Vector3[vertices.Length];
		int vertIndex;
		int i = 0;
		for (i = 0; i < currentBuffer.Length; i++)
		{
			vertIndex = vertexIndices[i];
			theseVertices[vertIndex] = vertices[vertIndex];
			theseVertices[vertIndex].y += (currentBuffer[i] * 1.0f / splashForce) * maxWaveHeight;
		}
		mesh.vertices = theseVertices;


		// swap buffers		
	}

	void ProcessRipples(int[] source, int[] dest)
	{
		int x = 0;
		int y = 0;
		int position = 0;
		for (y = 1; y < rows; y++)
		{
			for (x = 1; x < cols; x++)
			{
				position = (y * (cols + 1)) + x;
				dest[position] = (((source[position - 1] +
									 source[position + 1] +
									 source[position - (cols + 1)] +
									 source[position + (cols + 1)]) >> 1) - dest[position]);
				dest[position] = (int)(dest[position] * dampner);
			}
		}
	}

}

