using UnityEngine;
using System;

namespace GFramework
{
	// Point
	public struct GPoint
	{
		public float x;
		public float y;

		public GPoint(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public GPoint(GPoint src)
		{
			this.x = src.x;
			this.y = src.y;
		}

		public GPoint(Vector2 src)
		{
			this.x = src.x;
			this.y = src.y;
		}

		public void Set(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return x + "," + y;
		}

		public override bool Equals(object o)
		{
			try
			{
				GPoint other = (GPoint)o;
				return (Mathf.Approximately(x, other.x) &&
						Mathf.Approximately(y, other.y));
			}
			catch
			{
				return false;
			}
		}
	}

	// Size
	public struct GSize
	{
		public float width;
		public float height;

		public GSize(float width, float height)
		{
			this.width = width;
			this.height = height;
		}

		public GSize(GSize src)
		{
			this.width = src.width;
			this.height = src.width;
		}

		public GSize(Vector2 src)
		{
			this.width = src.x;
			this.height = src.y;
		}

		public void Set(float width, float height)
		{
			this.width = width;
			this.height = height;
		}

		public override string ToString()
		{
			return width + "," + height;
		}

		public override bool Equals(object o)
		{
			try
			{
				GSize other = (GSize)o;
				return (Mathf.Approximately(width, other.width) &&
						Mathf.Approximately(height, other.height));
			}
			catch
			{
				return false;
			}
		}
	}

	// Rect
	public struct GRect
	{
		public static readonly GRect Invalid = new GRect(0.0f, 0.0f, -1.0f, -1.0f);

		// Properties
		public float left;
		public float top;
		public float width;
		public float height;
		
		public GPoint leftTop 
		{
			get 
			{
				return new GPoint(left, top);
			}
			set
			{
				this.left = value.x;
				this.top = value.y;
			}
		}

		public GSize size 
		{
			get 
			{
				return new GSize(width, height);
			}
			set
			{
				this.width = value.width;
				this.height = value.height;
			}
		}
		
		public float x { get { return left; } set { left = value; } }
		public float y { get { return top; } set { top = value; } }
		public float right { get { return left + width; } }
		public float bottom { get { return top + height; } }

		public GPoint center { get { return new GPoint(left + (width / 2), top + (height / 2)); } }

		// Point helper
		public GPoint upperLeft { get { return new GPoint(left, top); } }
		public GPoint upperCenter { get { return new GPoint(left + (width / 2), top); } }
		public GPoint upperRight { get { return new GPoint(right, top); } }
		public GPoint middleLeft { get { return new GPoint(left, top + (height / 2)); } }
		public GPoint middleCenter { get { return new GPoint(left + (width / 2), top + (height / 2)); } }
		public GPoint middleRight { get { return new GPoint(right, top + (height / 2)); } }
		public GPoint lowerLeft { get { return new GPoint(left, top + height); } }
		public GPoint lowerCenter { get { return new GPoint(left + (width / 2), top + height); } }
		public GPoint lowerRight { get { return new GPoint(right, top + height); } }

		public GRect(float left, float top, float width, float height)
		{
			this.left = left;
			this.top = top;
			this.width = width;
			this.height = height;
		}

		public GRect(GRect rct)
		{
			this.left = rct.left;
			this.top = rct.top;
			this.width = rct.width;
			this.height = rct.height;
		}

		public GRect(UnityEngine.Rect rct)
		{
			this.left = rct.xMin;
			this.top = rct.yMin;
			this.width = rct.width;
			this.height = rct.height;
		}

		public GRect(GPoint topleft, GPoint rightbottom)
		{
			this.left = topleft.x;
			this.top = topleft.y;
			this.width = rightbottom.x - topleft.x;
			this.height = rightbottom.y - topleft.y;
		}

		public GRect(GPoint pos, GSize size)
		{
			this.left = pos.x;
			this.top = pos.y;
			this.width = size.width;
			this.height = size.height;
		}

		public override bool Equals(object o)
		{
			try
			{
				GRect other = (GRect)o;
				return (Mathf.Approximately(left, other.left) &&
						Mathf.Approximately(top, other.top) &&
						Mathf.Approximately(width, other.width) &&
						Mathf.Approximately(height, other.height));
			}
			catch
			{
				return false;
			}
		}

		// Set
		public void Set(float left, float top, float width, float height)
		{
			this.left = left;
			this.top = top;
			this.width = width;
			this.height = height;
		}

		public void Set(GRect rct)
		{
			this.left = rct.left;
			this.top = rct.top;
			this.width = rct.width;
			this.height = rct.height;
		}

		public void Set(GPoint topleft, GPoint rightbottom)
		{
			this.left = topleft.x;
			this.top = topleft.y;
			this.width = rightbottom.x - topleft.x;
			this.height = rightbottom.y - topleft.y;
		}

		public void Set(GPoint pos, GSize size)
		{
			this.left = pos.x;
			this.top = pos.y;
			this.width = size.width;
			this.height = size.height;
		}

		//
		public void SetPos(float left, float top)
		{
			this.left = left;
			this.top = top;
		}

		//
		public void SetSize(float width, float height)
		{
			this.width = width;
			this.height = height;
		}

		// Intersect
		public bool IsIntersect(GRect rect)
		{
			if (this.left > rect.right ||
				this.top > rect.bottom ||
				this.right < rect.left ||
				this.bottom < rect.top)
				return false;

			return true;
		}

		public static GRect Intersect(GRect rect1, GRect rect2)
		{
			GRect rct = new GRect(rect1);
			rct.IntersectWith(rect2);
			return rct;
		}

		public bool IntersectWith(GRect rect)
		{
			float r1 = right;
			float b1 = bottom;
			float r2 = rect.right;
			float b2 = rect.bottom;

			if (this.left > r2 ||
				this.top > b2 ||
				r1 < rect.left ||
				b1 < rect.top)
			{
				Zero();
				return false;
			}

			if (rect.left > this.left) this.left = rect.left;
			if (rect.top > this.top) this.top = rect.top;
			this.width = (r1 < r2 ? r1 : r2) - this.left;
			this.height = (b1 < b2 ? b1 : b2) - this.top;
			return true;
		}

		// Union
		public static GRect Union(GRect rect1, GRect rect2)
		{
			GRect rct = new GRect(rect1);
			rct.UnionWith(rect2);
			return rct;
		}

		public bool UnionWith(GRect rect)
		{
			float r1 = this.right;
			float b1 = this.bottom;
			float r2 = rect.right;
			float b2 = rect.bottom;

			if (this.left > rect.left) this.left = rect.left;
			if (this.top > rect.top) this.top = rect.top;
			this.width = (r1 > r2 ? r1 : r2) - this.left;
			this.height = (b1 > b2 ? b1 : b2) - this.top;
			return true;
		}


		// Contains ?
		public bool Contains(GPoint pt)
		{
			if (this.left < pt.x && pt.x < this.right &&
				this.top < pt.y && pt.y < this.bottom)
				return true;
			return false;
		}

		// Rect is zero point
		public bool IsValid()
		{
			return this.width >= 0 && this.height >= 0;
		}

		public bool IsInvalidOrZeroArea()
		{
			return this.width <= 0 || this.height <= 0;
		}

		//
		public void Offset(float offX, float offY)
		{
			this.left += offX;
			this.top += offY;
		}

		//
		public void Expand(float width, float height)
		{
			this.left -= width;
			this.top -= height;
			this.width += width * 2;
			this.height += height * 2;
		}

		//
		public void Expand(float left, float top, float right, float bottom)
		{
			this.left -= left;
			this.top -= top;
			this.width += left + right;
			this.height += top + bottom;
		}

		//
		public void Zero()
		{
			this.left = this.top = this.width = this.height = 0;
		}

		public override string ToString()
		{
			return "LTWH( " + this.left + ", " + this.top + ", " + this.width + ", " + this.height + " )";
		}

		public UnityEngine.Rect UnityRect
		{
			get
			{
				return new UnityEngine.Rect(this.left, this.top, this.width, this.height);
			}
		}

		public static GRect zero = new GRect(0, 0, 0, 0);
	}

}

