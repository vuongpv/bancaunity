using UnityEngine;
using System;

namespace GFramework
{
	struct VertexData
	{
		// Pointers to vertex data
		public Vector3[]	positions;
		public Vector3[]	normals;
		public BoneWeight[] boneWeights;
		public Color[]		colors;
		public Vector2[]	uv;
		public Vector2[]	uv1;
		public Vector2[]	uv2;
		public Vector4[]	tangents;
	};

	[Flags]
	public enum VertexFormat : long
	{
		Position = 0x01,
		Normal = 0x02,
		BoneWeight = 0x04,
		Color = 0x08,
		UV = 0x10,
		UV1 = 0x20,
		UV2 = 0x40,
		Tangent = 0x80,

		LitDiffuse = Position | Normal | Color,
		UnlitDiffuse = Position | Color,

		LitTexture = Position | Normal | UV,
		UnlitTexture = Position | UV,

		LitDiffuseTexture = Position | Normal | Color | UV,
		UnlitDiffuseTexture = Position | Color | UV,		
	};

	public class MeshBuilder
	{
		// Current mesh desc
		private VertexData	vertices;
		private int[]		indices;

		// Num indices and vertices is allowed to modify
		private int numVertices;
		private int numIndices;

		// First vertex index
		private int firstVertexIndex;

		// The current vertex and index
		private int	curVertex;
		private int	curIndex;

		// Vertex format
		public VertexFormat VertFormat { get; protected set; }

		// Is lock to modify
		private bool isLocked;

		public MeshBuilder()
		{
		}

		~MeshBuilder() 
		{
		}

		public bool vertexHasComponent(VertexFormat format)
		{
			return (this.VertFormat & format) == format;
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="numVertices"></param>
		public void beginOverwrite( VertexFormat vertFormat, int numVertices, int numTriangles )
		{
			isLocked = true;

			// Vertex format
			this.VertFormat = vertFormat | VertexFormat.Position;

			// Number
			this.firstVertexIndex = 0;
			this.numVertices = numVertices;
			this.numIndices	= numTriangles * 3;
			
			// Data
			this.vertices.positions = new Vector3[this.numVertices];

			if (vertexHasComponent(VertexFormat.Normal))
				this.vertices.normals = new Vector3[this.numVertices];

			if (vertexHasComponent(VertexFormat.BoneWeight))
				this.vertices.boneWeights = new BoneWeight[this.numVertices];

			if (vertexHasComponent(VertexFormat.Color))
				this.vertices.colors = new Color[this.numVertices];

			if (vertexHasComponent(VertexFormat.UV))
				this.vertices.uv = new Vector2[this.numVertices];

			if (vertexHasComponent(VertexFormat.UV1))
				this.vertices.uv1 = new Vector2[this.numVertices];

			if (vertexHasComponent(VertexFormat.UV2))
				this.vertices.uv2 = new Vector2[this.numVertices];

			if (vertexHasComponent(VertexFormat.Tangent))
				this.vertices.tangents = new Vector4[this.numVertices];
			
			this.indices = new int[this.numIndices];

			// reset to begin of buffer
			reset( 0, 0 );
		}

		public void end()
		{
			isLocked = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="numVertices"></param>
		/// <param name="firstVertex"></param>
		/// <param name="numIndices"></param>
		/// <param name="firstIndex"></param>
		public void beginUpdate( Mesh mesh, int firstVertex, int firstIndex ) 
		{
			isLocked = true;

			this.VertFormat = 0;
			this.firstVertexIndex = 0;

			this.vertices.positions = mesh.vertices;
			
			this.vertices.normals = mesh.normals;
			if (this.vertices.normals != null)
				this.VertFormat |= VertexFormat.Normal;

			this.vertices.boneWeights = mesh.boneWeights;
			if (this.vertices.boneWeights != null)
				this.VertFormat |= VertexFormat.BoneWeight;

			this.vertices.colors = mesh.colors;
			if (this.vertices.colors != null)
				this.VertFormat |= VertexFormat.Color;

			this.vertices.uv = mesh.uv;
			if (this.vertices.uv != null)
				this.VertFormat |= VertexFormat.UV;

			this.vertices.uv1 = mesh.uv1;
			if (this.vertices.uv1 != null)
				this.VertFormat |= VertexFormat.UV1;

			this.vertices.uv2 = mesh.uv2;
			if (this.vertices.uv2 != null)
				this.VertFormat |= VertexFormat.UV2;

			this.vertices.tangents = mesh.tangents;
			if (this.vertices.tangents != null)
				this.VertFormat |= VertexFormat.Tangent;

			this.indices = mesh.triangles;
			
			this.numVertices = this.vertices.positions.Length;
			this.numIndices	= this.indices.Length;

			// reset to begin of buffer
			reset(firstVertex, firstIndex);
		}

		public void beginAppend(VertexFormat vertFormat, int numVertices, int numTriangles)
		{
			if (this.numVertices == 0)
			{
				beginOverwrite(vertFormat, numVertices, numTriangles);
				return;
			}

			isLocked = true;

			// reset to begin of buffer
			reset(this.numVertices, this.numIndices);

			// Number
			this.firstVertexIndex = this.numVertices;
			this.numVertices += numVertices;
			this.numIndices += numTriangles * 3;

			// Data
			Array.Resize(ref this.vertices.positions, this.numVertices);
			
			if (vertexHasComponent(VertexFormat.Normal))
				Array.Resize(ref this.vertices.normals, this.numVertices);

			if (vertexHasComponent(VertexFormat.BoneWeight))
				Array.Resize(ref this.vertices.boneWeights, this.numVertices);

			if (vertexHasComponent(VertexFormat.Color))
				Array.Resize(ref this.vertices.colors, this.numVertices);

			if (vertexHasComponent(VertexFormat.UV))
				Array.Resize(ref this.vertices.uv, this.numVertices);

			if (vertexHasComponent(VertexFormat.UV1))
				Array.Resize(ref this.vertices.uv1, this.numVertices);

			if (vertexHasComponent(VertexFormat.UV2))
				Array.Resize(ref this.vertices.uv2, this.numVertices);

			if (vertexHasComponent(VertexFormat.Tangent))
				Array.Resize(ref this.vertices.tangents, this.numVertices);

			Array.Resize(ref this.indices, this.numIndices);
		}

		public bool commit( Mesh mesh )
		{
			if (isLocked)
				return false;

			if (this.vertices.positions == null)
				return true;
			//check mesh null
			if (mesh == null)
				return false;
			mesh.Clear();

			// Assign back data
			mesh.vertices = this.vertices.positions;
			
			if( vertexHasComponent(VertexFormat.Normal) )
				mesh.normals = this.vertices.normals;
			
			if( vertexHasComponent(VertexFormat.BoneWeight) )
				mesh.boneWeights = this.vertices.boneWeights;
			
			if( vertexHasComponent(VertexFormat.Color) )
				mesh.colors = this.vertices.colors;
			
			if( vertexHasComponent(VertexFormat.UV) )
				mesh.uv = this.vertices.uv;
			
			if( vertexHasComponent(VertexFormat.UV1) )
				mesh.uv1 = this.vertices.uv1;
			
			if( vertexHasComponent(VertexFormat.UV2) )
				mesh.uv2 = this.vertices.uv2;
			
			if( vertexHasComponent(VertexFormat.Tangent) )
				mesh.tangents = this.vertices.tangents;

			//Debug.Log("Num vertices = " + numVertices);
			mesh.triangles = this.indices;

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vertIdx"></param>
		/// <param name="indexIdx"></param>
		public void reset( int vertIdx, int indexIdx )
		{
			this.curVertex	= vertIdx;
			this.curIndex	= indexIdx;

			MathfEx.Clamp<int>(vertIdx, 0, numVertices);
			MathfEx.Clamp<int>(indexIdx, 0, numIndices);
		}

		public bool isEmpty()
		{
			return this.vertices.positions == null || this.vertices.positions.Length == 0;
		}

		public void clear()
		{
			vertices.positions = null;
			vertices.normals = null;
			vertices.boneWeights = null;
			vertices.colors = null;
			vertices.uv = null;
			vertices.uv1 = null;
			vertices.uv2 = null;
			vertices.tangents = null;
			
			indices = null;

			numVertices = 0;
			numIndices = 0;

			firstVertexIndex = 0;

			curVertex = 0;
			curIndex = 0;

			VertFormat = 0;
		}

		public int CurVertexPos
		{
			get
			{
				return curVertex;
			}
		}

		public int CurIndexPos
		{
			get
			{
				return curIndex;
			}
		}


		#region Accesing vertex data

		public Vector3 Position 
		{
			get {
				return this.vertices.positions[curVertex];
			}
		}

		public BoneWeight BoneWeight
		{
			get {
				if( this.vertices.boneWeights == null )
					return new BoneWeight();

				return this.vertices.boneWeights[curVertex];
			}
		}

		public Vector3 Normal
		{
			get {
				if( this.vertices.normals == null )
					return Vector3.zero;

				return this.vertices.normals[curVertex];
			}
		}

		public Color Color
		{
			get {
				if( this.vertices.colors == null )
					return Color.white;

				return this.vertices.colors[curVertex];
			}
		}

		public Vector2 UV
		{
			get {
				if( this.vertices.uv == null )
					return Vector2.zero;

				return this.vertices.uv[curVertex];
			}
		}

		public Vector2 UV1
		{
			get {
				if( this.vertices.uv1 == null )
					return Vector2.zero;

				return this.vertices.uv1[curVertex];
			}
		}

		public Vector2 UV2
		{
			get {
				if( this.vertices.uv2 == null )
					return Vector2.zero;

				return this.vertices.uv2[curVertex];
			}
		}

		public Vector3 Tangent
		{
			get {
				if( this.vertices.tangents == null )
					return Vector3.zero;

				return this.vertices.tangents[curVertex];
			}

		}
		

		//-----------------------------------------------------------------------------
		// Modifying
		//-----------------------------------------------------------------------------
		// Position
		public void position( float x, float y, float z )
		{
			if( this.vertices.positions == null ) return;

			this.vertices.positions[curVertex].x = x;
			this.vertices.positions[curVertex].y = y;
			this.vertices.positions[curVertex].z = z;
		}

		public void position(Vector3 v)
		{
			if (this.vertices.positions == null) return;

			this.vertices.positions[curVertex] = v;
		}

		//-----------------------------------------------------------------------------
		// Bone weights
		public void boneWeight( int i, float weight )
		{
			if( this.vertices.boneWeights == null ) return;

			if( i == 0 )
				this.vertices.boneWeights[curVertex].weight0 = weight;
			else if( i == 1 )
				this.vertices.boneWeights[curVertex].weight1 = weight;
			else if( i == 2 )
				this.vertices.boneWeights[curVertex].weight2 = weight;
			else if( i == 3 )
				this.vertices.boneWeights[curVertex].weight3 = weight;
		}

		// Bone weights
		public void boneIndex( int i, int boneIndex )
		{
			if( this.vertices.boneWeights == null ) return;

			if( i == 0 )
				this.vertices.boneWeights[curVertex].boneIndex0 = boneIndex;
			else if( i == 1 )
				this.vertices.boneWeights[curVertex].boneIndex1 = boneIndex;
			else if( i == 2 )
				this.vertices.boneWeights[curVertex].boneIndex2 = boneIndex;
			else if( i == 3 )
				this.vertices.boneWeights[curVertex].boneIndex3 = boneIndex;
		}

		//-----------------------------------------------------------------------------
		// Normal
		public void normal( float nx, float ny, float nz )
		{
			if( this.vertices.normals == null ) return;

			this.vertices.normals[curVertex].x = nx;
			this.vertices.normals[curVertex].y = ny;
			this.vertices.normals[curVertex].z = nz;
		}

		public void normal(Vector3 nv)
		{
			if (this.vertices.normals == null) return;

			this.vertices.normals[curVertex] = nv;
		}

		//-----------------------------------------------------------------------------
		// Color float
		public void color( float r, float g, float b )
		{
			if( this.vertices.colors == null ) return;

			this.vertices.colors[curVertex].r = r;
			this.vertices.colors[curVertex].g = g;
			this.vertices.colors[curVertex].b = b;
			this.vertices.colors[curVertex].a = 1.0f;
		}

		public void color( float r, float g, float b, float a )
		{
			if( this.vertices.colors == null ) return;

			this.vertices.colors[curVertex].r = r;
			this.vertices.colors[curVertex].g = g;
			this.vertices.colors[curVertex].b = b;
			this.vertices.colors[curVertex].a = a;
		}

		//-----------------------------------------------------------------------------
		// Color 255
		public void color255( byte r, byte g, byte b )
		{
			if( this.vertices.colors == null ) return;

			this.vertices.colors[curVertex].r = r / 255.0f;
			this.vertices.colors[curVertex].g = g / 255.0f;
			this.vertices.colors[curVertex].b = b / 255.0f;
			this.vertices.colors[curVertex].a = 1.0f;
		}

		public void color255( byte r, byte g, byte b, byte a )
		{
			if( this.vertices.colors == null ) return;

			this.vertices.colors[curVertex].r = r / 255.0f;
			this.vertices.colors[curVertex].g = g / 255.0f;
			this.vertices.colors[curVertex].b = b / 255.0f;
			this.vertices.colors[curVertex].a = a / 255.0f;
		}

		public void color(GColor c)
		{
			this.color(c.UnityColor);
		}

		public void color(Color c)
		{
			if (this.vertices.colors == null) return;

			this.vertices.colors[curVertex] = c;
		}

		//-----------------------------------------------------------------------------
		// UV
		public void uv( float u, float v )
		{
			if( vertices.uv == null ) return;

			vertices.uv[curVertex].x = u;  
			vertices.uv[curVertex].y = v;  
		}

		public void uv(Vector2 v)
		{
			if (vertices.uv == null) return;

			vertices.uv[curVertex] = v;
		}

		public void uv1( float u, float v )
		{
			if( vertices.uv1 == null ) return;

			vertices.uv1[curVertex].x = u;  
			vertices.uv1[curVertex].y = v;  
		}

		public void uv1(Vector2 v)
		{
			if (vertices.uv1 == null) return;

			vertices.uv1[curVertex] = v;
		}

		public void uv2( float u, float v )
		{
			if( vertices.uv2 == null ) return;

			vertices.uv2[curVertex].x = u;  
			vertices.uv2[curVertex].y = v;  
		}

		public void uv2(Vector2 v)
		{
			if (vertices.uv2 == null) return;

			vertices.uv2[curVertex] = v;
		}

		
		//-----------------------------------------------------------------------------
		// Tangent
		public void tangent( float tx, float ty, float tz )
		{
			if( vertices.tangents == null ) return;

			vertices.tangents[curVertex].x = tx;
			vertices.tangents[curVertex].y = ty;
			vertices.tangents[curVertex].z = tz;
		}

		public void tangent(Vector3 tv)
		{
			if (vertices.tangents == null) return;

			vertices.tangents[curVertex] = tv;
		}
		#endregion

		#region Indices
		//-----------------------------------------------------------------------------
		public int Index
		{
			get
			{
				return this.indices[curIndex];
			}
			set
			{
				this.indices[curIndex] = this.firstVertexIndex + value;
			}
		}

		public int IndexAuto
		{
			private get
			{
				return this.indices[curIndex];
			}
			set
			{
				this.indices[curIndex] = this.firstVertexIndex + value;
				nextIndex(1);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public int[] Indices
		{
			get
			{
				return (int[]) indices.Clone();
			}
			set
			{
				value.CopyTo(this.indices, 0);
				curIndex = value.Length;
				if (curIndex > numIndices)
					curIndex = numIndices;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public void appendIndices( int[] indices )
		{
			appendIndices(firstVertexIndex, indices);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public void appendIndices( int offset, int[] indices )
		{
			indices.CopyTo(this.indices, curIndex);
			if (offset != 0)
			{
				for (int i = 0; i < indices.Length; ++i)
					this.indices[curIndex + i] += offset;
			}

			curIndex += indices.Length;
			if (curIndex > numIndices)
				curIndex = numIndices;
		}

		/// <summary>
		/// 
		/// </summary>
		public void generateSequentialIndices()
		{
			if (indices == null) return;
			for (int i = 0; i < numIndices; ++i)
				indices[i] = i;
		}
		#endregion

		#region Loop and iteration
		public void nextVertex()
		{
			nextVertex(1);
		}

		public void nextVertex(int padding)
		{
			curVertex += padding;

			// check boundary (FIXME : out boundary or end of boundary)
			if (curVertex >= numVertices)
				curVertex = numVertices - 1;
		}

		public void nextIndex()
		{
			nextIndex(1);
		}

		public void nextIndex(int padding)
		{
			curIndex += padding;

			// check boundary (FIXME : out boundary or end of boundary)
			if (curIndex >= numIndices)
				curIndex = numIndices - 1;
		}

		public void selectVertex(int idx)
		{
			curVertex = idx;

			if (curVertex >= numVertices)
				curVertex = numVertices - 1;
		}

		public void selectIndex(int idx)
		{
			curIndex = idx;

			if (curIndex >= numIndices)
				curIndex = numIndices - 1;
		}
		#endregion

		#region Helpers
		public string dumpIndices()
		{
			string str = String.Format( "[{0}] {{ ", indices.Length );
			for (int i = 0; i < indices.Length; i++)
			{
				str += indices[i] + ", ";
			}
			str += " }";
			return str;
		}


		//-----------------------------------------------------------------------------
		// Compute normals
		public bool computeNormals()
		{
			if ((numIndices <= 0) || (numVertices <= 0))
				return false;

			if (vertices.normals == null) return false;

			// cal num faces
			int numFaces = numIndices / 3;

			int i, f1, f2, f3;

			// Set normal to zero
			for (i = 0; i < numVertices; ++i)
				vertices.normals[i] = Vector3.zero;

			// Calculate normal
			Vector3 n, a, b;
			for (int iFace = 0; iFace < numFaces; iFace++)
			{
				i = iFace * 3;
				f1 = indices[i];
				f2 = indices[i + 1];
				f3 = indices[i + 2];

				// Get v1, v2, v3 vector
				Vector3 v1 = vertices.positions[f1];
				Vector3 v2 = vertices.positions[f2];
				Vector3 v3 = vertices.positions[f3];

				a = new Vector3(v1.x - v1.x, v2.y - v1.y, v2.z - v1.z);
				b = new Vector3(v1.x - v3.x, v1.y - v3.y, v1.z - v3.z);
				n = Vector3.Cross(a, b);

				// accumulate to final normal
				vertices.normals[f1] += n;
				vertices.normals[f2] += n;
				vertices.normals[f2] += n;
			}

			// Normal lize all normal
			for (i = 0; i < numVertices; ++i)
				vertices.normals[i].Normalize();

			return true;
		}

		//-----------------------------------------------------------------------------
		// Compute tangents
		public bool computeTangents()
		{
			/*if( (numIndices <= 0) || (numVertices <= 0) )
				return false;

			if( !m_curVertexData.pTangent || 
				!m_curVertexData.pBinormal ||
				!m_curVertexData.pTexCoord[0] ) return false;

			// cal num faces
			int numFaces = m_numIndices / 3;
			int size = m_meshDesc.vertexStride;
			int i, f1, f2, f3;
			Vector3_t tangent, binormal, e0, e1;
			
			// check all face
			for( int iFace=0; iFace<numFaces; iFace++ )
			{
				i = iFace * 3;
				f1 = m_meshDesc.pIndices[i];
				f2 = m_meshDesc.pIndices[i+1];
				f3 = m_meshDesc.pIndices[i+2];

				// Get v1, v2, v3 texcoord 0
				float* pV1 = m_meshDesc.vertexData.pTexCoord[0];
				float* pV2 = pV1;
				float* pV3 = pV1;
				increaseFloatPointer( pV1, size * f1 );
				increaseFloatPointer( pV2, size * f2 );
				increaseFloatPointer( pV3, size * f3 );

				e0.set( 0.0f, pV2[0] - pV1[0], pV2[1] - pV1[1] );
				e1.set( 0.0f, pV3[0] - pV1[0], pV3[1] - pV1[1] );

				// Get v1, v2, v3 psoition
				pV1 = pV2 = pV3 = m_meshDesc.vertexData.pPosition;
				increaseFloatPointer( pV1, size * f1 );
				increaseFloatPointer( pV2, size * f2 );
				increaseFloatPointer( pV3, size * f3 );

				for( i=0; i<3; i++ )
				{
					e0.x = pV2[i] - pV1[i];
					e1.x = pV3[i] - pV1[i];

					Vector3_t cp;
					vec3Cross( &cp, &e0, &e1 );

					tangent[i]  = -cp.y / cp.x;
					binormal[i] = -cp.z / cp.x;
				}

				vec3Normalize( &tangent, &tangent ); 
				vec3Normalize( &binormal, &binormal );

				//vec3Cross( &normal, &tangent, &binormal );
				//vec3Cross( &binormal, &normal, &tangent );

				// fill tangent
				pV1 = pV2 = pV3 = m_meshDesc.vertexData.pTangent;
				increaseFloatPointer( pV1, size * f1 );
				increaseFloatPointer( pV2, size * f2 );
				increaseFloatPointer( pV3, size * f3 );

				*((Vector3_t*) pV1) = 
				*((Vector3_t*) pV2) = 
				*((Vector3_t*) pV3) = tangent;

				// fill binormal
				pV1 = pV2 = pV3 = m_meshDesc.vertexData.pBinormal;
				increaseFloatPointer( pV1, size * f1 );
				increaseFloatPointer( pV2, size * f2 );
				increaseFloatPointer( pV3, size * f3 );

				*((Vector3_t*) pV1) = 
				*((Vector3_t*) pV2) = 
				*((Vector3_t*) pV3) = binormal;
			}*/

			return true;
		}

		#endregion
	}

} 