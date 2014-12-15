using System;
using UnityEngine;

namespace GFramework
{
	public class SinCosTable
	{
		private const int TABLE_SIZE = 3600;
		private const float TABLE_CONSTANT = TABLE_SIZE / (2 * Mathf.PI);
		private static float[] sinTable;
		private static float[] cosTable;


		static SinCosTable()
		{
			Prebuild();
		}

		public static void Prebuild()
		{
			if( sinTable == null )
				BuildSinTable();

			if( cosTable == null )
				BuildCosTable();
		}

		private static void BuildSinTable()
		{
			// init arrays
			sinTable = new float[TABLE_SIZE];
			// build lookup tables
			float angle = 0;
			for (int i = 0; i < TABLE_SIZE; i++)
			{
				sinTable[i] = Mathf.Sin(angle);
				angle += 2 * Mathf.PI / TABLE_SIZE;
			}
		}

		private static void BuildCosTable()
		{
			// init arrays
			cosTable = new float[TABLE_SIZE];
			// build lookup tables
			float angle = 0;
			for (int i = 0; i < TABLE_SIZE; i++)
			{
				cosTable[i] = Mathf.Cos(angle);
				angle += 2 * Mathf.PI / TABLE_SIZE;
			}
		}

		public static float Sin(float radians)
		{
			int idx = (int)(radians * TABLE_CONSTANT);
			idx %= TABLE_SIZE;
			if (idx < 0)
				idx = TABLE_SIZE + idx;
			return sinTable[idx];
		}

		public static float Cos(float radians)
		{
			int idx = (int)(radians * TABLE_CONSTANT);
			idx %= TABLE_SIZE;
			if (idx < 0)
				idx = TABLE_SIZE + idx;
			return cosTable[idx];
		}
	}


	public class MathfEx
	{
		public const float PI = Mathf.PI;
		public const float HALF_PI = Mathf.PI * 0.5f;
		public const float TWO_PI = Mathf.PI * 2f;
		public const float DEGREE_PER_PI = 180f / Mathf.PI;
		public const float SMALL_NUM = 0.000001f;

		public static bool Approx(float val, float about, float range)
		{
			return (Mathf.Abs((float)(val - about)) < range);
		}

		public static bool Approx(Vector3 val, Vector3 about, float range)
		{
			Vector3 vector = val - about;
			return (vector.sqrMagnitude < (range * range));
		}

		public static float Berp(float start, float end, float t)
		{
			t = Mathf.Clamp01(t);
			t = ((Mathf.Sin((t * 3.141593f) * (0.2f + (((2.5f * t) * t) * t))) * Mathf.Pow(1f - t, 2.2f)) + t) * (1f + (1.2f * (1f - t)));
			return (start + ((end - start) * t));
		}

		public static Vector3 Berp(Vector3 start, Vector3 end, float t)
		{
			float x = Berp(start.x, end.x, t);
			float y = Berp(start.y, end.y, t);
			return new Vector3(x, y, Berp(start.z, end.z, t));
		}

		public static float Bounce(float x)
		{
			return Mathf.Abs((float)(Mathf.Sin((6.28f * (x + 1f)) * (x + 1f)) * (1f - x)));
		}

		public static T Clamp<T>(T Value, T Min, T Max) where T : IComparable<T>
		{
			if (Value.CompareTo(Max) > 0)
			{
				return Max;
			}
			if (Value.CompareTo(Min) < 0)
			{
				return Min;
			}
			return Value;
		}

		public static float Clerp(float start, float end, float value)
		{
			float num = 0f;
			float num2 = 360f;
			float num3 = Mathf.Abs((float)((num2 - num) / 2f));
			float num5 = 0f;
			if ((end - start) < -num3)
			{
				num5 = ((num2 - start) + end) * value;
				return (start + num5);
			}
			if ((end - start) > num3)
			{
				num5 = -((num2 - end) + start) * value;
				return (start + num5);
			}
			return (start + ((end - start) * value));
		}

		public static float Coserp(float start, float end, float t)
		{
			return Mathf.Lerp(start, end, 1f - Mathf.Cos((t * 3.141593f) * 0.5f));
		}

		public static Vector3 Coserp(Vector3 start, Vector3 end, float t)
		{
			float x = Coserp(start.x, end.x, t);
			float y = Coserp(start.y, end.y, t);
			return new Vector3(x, y, Coserp(start.z, end.z, t));
		}

		public static Plane[] GetFrustrumPlanes(ref Matrix4x4 mat)
		{
			Plane[] planeArray = new Plane[6];
			Vector4 v = mat.GetRow(3) + mat.GetRow(0);
			planeArray[0] = NormalizedPlaneFromVector(ref v);
			v = mat.GetRow(3) - mat.GetRow(0);
			planeArray[1] = NormalizedPlaneFromVector(ref v);
			v = mat.GetRow(3) + mat.GetRow(1);
			planeArray[2] = NormalizedPlaneFromVector(ref v);
			v = mat.GetRow(3) - mat.GetRow(1);
			planeArray[3] = NormalizedPlaneFromVector(ref v);
			v = mat.GetRow(3) + mat.GetRow(2);
			planeArray[4] = NormalizedPlaneFromVector(ref v);
			v = mat.GetRow(3) - mat.GetRow(2);
			planeArray[5] = NormalizedPlaneFromVector(ref v);
			return planeArray;
		}

		public static float Hermite(float start, float end, float t)
		{
			return Mathf.Lerp(start, end, (t * t) * (3f - (2f * t)));
		}

		public static Vector3 Hermite(Vector3 start, Vector3 end, float value)
		{
			float x = Hermite(start.x, end.x, value);
			float y = Hermite(start.y, end.y, value);
			return new Vector3(x, y, Hermite(start.z, end.z, value));
		}

		public static float Lerp(float start, float end, float t)
		{
			return (((1f - t) * start) + (t * end));
		}

		public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
		{
			Vector3 rhs = Vector3.Normalize(lineEnd - lineStart);
			float num = Vector3.Dot(point - lineStart, rhs) / Vector3.Dot(rhs, rhs);
			return (lineStart + ((Vector3)(num * rhs)));
		}


		public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
		{
			Vector3 vector = lineEnd - lineStart;
			Vector3 rhs = Vector3.Normalize(vector);
			float num = Vector3.Dot(point - lineStart, rhs) / Vector3.Dot(rhs, rhs);
			return (lineStart + ((Vector3)(Mathf.Clamp(num, 0f, Vector3.Magnitude(vector)) * rhs)));
		}

		public static Plane NormalizedPlaneFromVector(ref Vector4 v)
		{
			float num = 1f / Mathf.Sqrt(((v.x * v.x) + (v.y * v.y)) + (v.z * v.z));
			v.x *= num;
			v.y *= num;
			v.z *= num;
			v.w *= num;
			return new Plane((Vector3)v, v.w);
		}

		public static float Sinerp(float start, float end, float t)
		{
			return Mathf.Lerp(start, end, Mathf.Sin((t * 3.141593f) * 0.5f));
		}

		public static Vector3 Sinerp(Vector3 start, Vector3 end, float value)
		{
			float x = Sinerp(start.x, end.x, value);
			float y = Sinerp(start.y, end.y, value);
			return new Vector3(x, y, Sinerp(start.z, end.z, value));
		}

		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref float currentSpeed, float smoothTime, float maxSpeed, float deltaTime)
		{
			Vector3 vector = target - current;
			float num = Mathf.SmoothDamp(vector.magnitude, 0f, ref currentSpeed, smoothTime, maxSpeed, deltaTime);
			return (current + ((Vector3)(vector.normalized * num)));
		}

		/// <summary>
		/// Find angle between 2 vector
		/// </summary>
		public static float AngleBetweenVectors(Vector2 vector1, Vector2 vector2)
		{
			float angle = Mathf.Atan2(vector2.y, vector2.x) - Mathf.Atan2(vector1.y, vector1.x);
			while (angle > Mathf.PI)
			{
				angle -= Mathf.PI * 2;
			}
			while (angle < -Mathf.PI)
			{
				angle += Mathf.PI * 2;
			}
			return angle * (180f / Mathf.PI);
		}

		public static float GetPlanarDistance(Vector3 vector1, Vector3 vector2)
		{
			return Vector3.Distance(vector1, new Vector3(vector2.x, vector1.y, vector2.z));
		}

		public static Vector3 GetPlanarDirection(Vector3 position, Vector3 target)
		{
			Vector3 dir = target - position;
			dir.y = 0;
			return dir.normalized;
		}

		public static Vector3 GetForwardVector(Quaternion q)
		{
			return new Vector3(2 * (q.x * q.z + q.w * q.y),
							2 * (q.y * q.x - q.w * q.x),
							1 - 2 * (q.x * q.x + q.y * q.y));
		}

		public static Vector3 GetUpVector(Quaternion q)
		{
			return new Vector3(2 * (q.x * q.y - q.w * q.z),
							1 - 2 * (q.x * q.x + q.z * q.z),
							2 * (q.y * q.z + q.w * q.x));
		}

		public static Vector3 GetRightVector(Quaternion q)
		{
			return new Vector3(1 - 2 * (q.y * q.y + q.z * q.z),
							2 * (q.x * q.y + q.w * q.z),
							2 * (q.x * q.z - q.w * q.y));
		}

		/// <summary>
		/// Converts the given decimal number to the numeral system with the
		/// specified radix (in the range [2, 36]).
		/// </summary>
		/// <param name="decimalNumber">The number to convert.</param>
		/// <param name="radix">The radix of the destination numeral system
		/// (in the range [2, 36]).</param>
		/// <returns></returns>
		public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
		{
			const int BitsInLong = 64;
			const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			if (radix < 2 || radix > Digits.Length)
				throw new ArgumentException("The radix must be >= 2 and <= " +
					Digits.Length.ToString());

			if (decimalNumber == 0)
				return "0";

			int index = BitsInLong - 1;
			long currentNumber = Math.Abs(decimalNumber);
			char[] charArray = new char[BitsInLong];

			while (currentNumber != 0)
			{
				int remainder = (int)(currentNumber % radix);
				charArray[index--] = Digits[remainder];
				currentNumber = currentNumber / radix;
			}

			string result = new String(charArray, index + 1, BitsInLong - index - 1);
			if (decimalNumber < 0)
			{
				result = "-" + result;
			}

			return result;
		}

		//Convert number in string representation from base:from to base:to. 
		//Return result as a string
		public static String BaseConvert(int from, int to, String s)
		{
			//Return error if input is empty
			if (String.IsNullOrEmpty(s))
			{
				return ("Error: Nothing in Input String");
			}
			//only allow uppercase input characters in string
			s = s.ToUpper();

			//only do base 2 to base 36 (digit represented by characters 0-Z)"
			if (from < 2 || from > 36 || to < 2 || to > 36)
			{ return ("Base requested outside range"); }

			//convert string to an array of integer digits representing number in base:from
			int il = s.Length;
			int[] fs = new int[il];
			int k = 0;
			for (int i = s.Length - 1; i >= 0; i--)
			{
				if (s[i] >= '0' && s[i] <= '9') { fs[k++] = (int)(s[i] - '0'); }
				else
				{
					if (s[i] >= 'A' && s[i] <= 'Z') { fs[k++] = 10 + (int)(s[i] - 'A'); }
					else
					{ return ("Error: Input string must only contain any of 0-9 or A-Z"); } //only allow 0-9 A-Z characters
				}
			}

			//check the input for digits that exceed the allowable for base:from
			foreach (int i in fs)
			{
				if (i >= from) { return ("Error: Not a valid number for this input base"); }
			}

			//find how many digits the output needs
			int ol = il * (from / to + 1);
			int[] ts = new int[ol + 10]; //assign accumulation array
			int[] cums = new int[ol + 10]; //assign the result array
			ts[0] = 1; //initialize array with number 1 

			//evaluate the output
			for (int i = 0; i < il; i++) //for each input digit
			{
				for (int j = 0; j < ol; j++) //add the input digit 
				// times (base:to from^i) to the output cumulator
				{
					cums[j] += ts[j] * fs[i];
					int temp = cums[j];
					int rem = 0;
					int ip = j;
					do // fix up any remainders in base:to
					{
						rem = temp / to;
						cums[ip] = temp - rem * to; ip++;
						cums[ip] += rem;
						temp = cums[ip];
					}
					while (temp >= to);
				}

				//calculate the next power from^i) in base:to format
				for (int j = 0; j < ol; j++)
				{
					ts[j] = ts[j] * from;
				}
				for (int j = 0; j < ol; j++) //check for any remainders
				{
					int temp = ts[j];
					int rem = 0;
					int ip = j;
					do  //fix up any remainders
					{
						rem = temp / to;
						ts[ip] = temp - rem * to; ip++;
						ts[ip] += rem;
						temp = ts[ip];
					}
					while (temp >= to);
				}
			}

			//convert the output to string format (digits 0,to-1 converted to 0-Z characters) 
			String sout = String.Empty; //initialize output string
			bool first = false; //leading zero flag
			for (int i = ol; i >= 0; i--)
			{
				if (cums[i] != 0) { first = true; }
				if (!first) { continue; }
				if (cums[i] < 10) { sout += (char)(cums[i] + '0'); }
				else { sout += (char)(cums[i] + 'A' - 10); }
			}
			if (String.IsNullOrEmpty(sout)) { return "0"; } //input was zero, return 0
			//return the converted string
			return sout;
		}

		public static bool FasterLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
		{
			Vector2 a = p2 - p1;
			Vector2 b = p3 - p4;
			Vector2 c = p1 - p3;

			float alphaNumerator = b.y * c.x - b.x * c.y;
			float alphaDenominator = a.y * b.x - a.x * b.y;
			float betaNumerator = a.x * c.y - a.y * c.x;
			float betaDenominator = a.y * b.x - a.x * b.y;

			bool doIntersect = true;

			if (alphaDenominator == 0 || betaDenominator == 0)
			{
				doIntersect = false;
			}
			else
			{
				if (alphaDenominator > 0)
				{
					if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
					{
						doIntersect = false;
					}
				}
				else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
				{
					doIntersect = false;
				}

				if (doIntersect && betaDenominator > 0)
				{
					if (betaNumerator < 0 || betaNumerator > betaDenominator)
					{
						doIntersect = false;
					}
				}
				else if (betaNumerator > 0 || betaNumerator < betaDenominator)
				{
					doIntersect = false;
				}
			}

			return doIntersect;
		}

		public static float SqrDistancePointToSegment(Vector3 point, Vector3 lineP1, Vector3 lineP2, out int closestPoint)
		{

			Vector3 v = lineP2 - lineP1;
			Vector3 w = point - lineP1;

			float c1 = Vector3.Dot(w, v);

			if (c1 <= 0) //closest point is p1
			{
				closestPoint = 1;
				return Vector3.SqrMagnitude(point - lineP1);
			}

			float c2 = Vector3.Dot(v, v);
			if (c2 <= c1) //closest point is p2
			{
				closestPoint = 2;
				return Vector3.SqrMagnitude(point - lineP2);
			}


			float b = c1 / c2;

			Vector3 pb = lineP1 + b * v;
			{
				closestPoint = 4;
				return Vector3.SqrMagnitude(point - pb);
			}
		}

		public static float SqrDistanceSegmentToSegment(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out Vector3 closestPt1, out Vector3 closestPt2)
		{
			Vector3 u = end1 - start1;
			Vector3 v = end2 - start2;
			Vector3 w = start1 - start2;
			float a = Vector3.Dot(u, u);         // always >= 0
			float b = Vector3.Dot(u, v);
			float c = Vector3.Dot(v, v);         // always >= 0
			float d = Vector3.Dot(u, w);
			float e = Vector3.Dot(v, w);
			float D = a * c - b * b;        // always >= 0
			float sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
			float tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

			// compute the line parameters of the two closest points
			if (D < SMALL_NUM)
			{ // the lines are almost parallel
				sN = 0.0f;         // force using point P0 on segment S1
				sD = 1.0f;         // to prevent possible division by 0.0 later
				tN = e;
				tD = c;
			}
			else
			{                 // get the closest points on the infinite lines
				sN = (b * e - c * d);
				tN = (a * e - b * d);
				if (sN < 0.0)
				{        // sc < 0 => the s=0 edge is visible
					sN = 0.0f;
					tN = e;
					tD = c;
				}
				else if (sN > sD)
				{  // sc > 1  => the s=1 edge is visible
					sN = sD;
					tN = e + b;
					tD = c;
				}
			}

			if (tN < 0.0f)
			{            // tc < 0 => the t=0 edge is visible
				tN = 0.0f;
				// recompute sc for this edge
				if (-d < 0.0f)
					sN = 0.0f;
				else if (-d > a)
					sN = sD;
				else
				{
					sN = -d;
					sD = a;
				}
			}
			else if (tN > tD)
			{      // tc > 1  => the t=1 edge is visible
				tN = tD;
				// recompute sc for this edge
				if ((-d + b) < 0.0)
					sN = 0;
				else if ((-d + b) > a)
					sN = sD;
				else
				{
					sN = (-d + b);
					sD = a;
				}
			}
			// finally do the division to get sc and tc
			sc = (Mathf.Abs(sN) < SMALL_NUM ? 0.0f : sN / sD);
			tc = (Mathf.Abs(tN) < SMALL_NUM ? 0.0f : tN / tD);

			u *= sc;
			v *= tc;

			// get the difference of the two closest points
			Vector3 dP = w + u - v;  // =  S1(sc) - S2(tc)
			closestPt1 = start1 + u;
			closestPt2 = start2 + v;

			return dP.sqrMagnitude;   // return the closest sqr distance
		}


	}
}

