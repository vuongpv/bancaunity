using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using X11;
using GFramework;

namespace X11
{
	public static partial class Extensions
	{
		#region Unity type helpers


		/// <summary>
		/// Check vector equals
		/// </summary>
		public static bool EqualsEpsilon(this float[] l, float[] r, float epsilon)
		{
			if (l == null || r == null)
				return l == r;

			if (l.Length != r.Length)
				return false;

			for (int i = 0; i < l.Length; i++)
			{
				if (!MathfEx.Approx(l[i], r[i], epsilon))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Check vector equals
		/// </summary>
		public static bool EqualsEpsilon(this Vector3 l, Vector3 r, float epsilon)
		{
			return
				MathfEx.Approx(l.x, r.x, epsilon) &&
				MathfEx.Approx(l.y, r.y, epsilon) &&
				MathfEx.Approx(l.z, r.z, epsilon);
		}

		/// <summary>
		/// Convert a vector to float array
		/// </summary>
		public static float[] ToFloatArray(this Vector3 v3)
		{
			return new float[3] { v3.x, v3.y, v3.z };
		}

		/// <summary>
		/// Convert a float 3 array to vector
		/// </summary>
		public static Vector3 ToVector3(this float[] floatArray)
		{
			return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
		}

		/// <summary>
		/// Convert a vector array to float array
		/// </summary>
		public static float[] ToFloatArray(this Vector3[] v3Array)
		{
			float[] result = new float[v3Array.Length * 3];
			for (int i = 0; i < v3Array.Length; i++)
			{
				result[i * 3] = v3Array[i].x;
				result[i * 3 + 1] = v3Array[i].y;
				result[i * 3 + 2] = v3Array[i].z;
			}

			return result;
		}

		/// <summary>
		/// Convert a float array to vector 3 array
		/// </summary>
		public static Vector3[] ToVector3Array(this float[] floatArray)
		{
			var v3Array = new Vector3[floatArray.Length / 3];
			for (int i = 0; i < v3Array.Length; i++)
			{
				v3Array[i] = new Vector3(floatArray[i * 3], floatArray[i * 3 + 1], floatArray[i * 3 + 2]);
			}
			return v3Array;
		}

		/// <summary>
		/// 
		/// </summary>
		public static Color ToColor(this UInt32 _color)
		{
			Color c = new Color();
			c.r = (_color >> 16) / 255.0f;
			_color %= (256 * 256);

			c.g = (_color >> 8) / 255.0f;
			_color %= 256;

			c.b = (_color >> 0) / 255.0f;
			return c;
		}

		#endregion
	}
}
