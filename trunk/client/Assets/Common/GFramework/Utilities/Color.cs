using UnityEngine;
using System;

namespace GFramework
{
	/// <summary>
	/// Color class
	/// </summary>
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
	public struct GColor
	{
		// access by colorref
		[System.Runtime.InteropServices.FieldOffset(0)]
		public UInt32 color;

		// access by individual element
		[System.Runtime.InteropServices.FieldOffset(0)]
		public byte b;

		[System.Runtime.InteropServices.FieldOffset(1)]
		public byte g;
		
		[System.Runtime.InteropServices.FieldOffset(2)]
		public byte r;
		
		[System.Runtime.InteropServices.FieldOffset(3)]
		public byte a;


		/// <summary>
		/// Implicit cast to Unity Color
		/// </summary>
		public static implicit operator UnityEngine.Color(GColor color)
		{
			return color.UnityColor;
		}

		/// <summary>
		/// Implicit cast to GColor from Unity color
		/// </summary>
		public static implicit operator GColor(UnityEngine.Color color)
		{
			return new GColor(color);
		}

		/// <summary>
		/// Implicit cast to GColor from int
		/// </summary>
		public static implicit operator GColor(UInt32 color)
		{
			return new GColor(color);
		}


		//
		public GColor( UInt32 cl )
		{
			a = r = g = b = 0;
			color = cl;
		}

		//
		public GColor( byte r, byte g, byte b ) {
			color = 0;
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 0xFF;
		}

		//
		public GColor( byte r, byte g, byte b, byte a ) {
			color = 0;
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		//
		public GColor(UnityEngine.Color cl)
		{
			color = 0;
			a = (byte)(cl.a * 255);
			r = (byte)(cl.r * 255);
			g = (byte)(cl.g * 255);
			b = (byte)(cl.b * 255);
		}

		//
		public static bool operator == (GColor a, GColor b)
		{
			return a.color == b.color;
		}

		//
		public static bool operator != (GColor a, GColor b)
		{
			return a.color != b.color;
		}

		//
		public override int GetHashCode()
		{
			return (int) color;
		}


		//
		public override bool Equals ( object other ) 
		{
			return color == ((GColor)other).color;
		}


		//
		void Set(byte r, byte g, byte b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 0xFF;
		}

		//
		void Set( byte r, byte g, byte b, byte a ) {
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		//
		public UnityEngine.Color UnityColor
		{
			get 
			{
				return new UnityEngine.Color(r/(float)255, g/(float)255, b/(float)255, a/(float)255);
			}
		}

		public override string ToString()
		{
			return "_RGBA( " + r + ", " + g + ", " + b + ", " + a + " )";
		}

		
		public static readonly GColor AliceBlue		= new GColor( 0xFFF0F8FF );
		public static readonly GColor AntiqueWhite	= new GColor( 0xFFFAEBD7 );
		public static readonly GColor Aqua			= new GColor( 0xFF00FFFF );
		public static readonly GColor Aquamarine		= new GColor( 0xFF7FFFD4 );
		public static readonly GColor Azure			= new GColor( 0xFFF0FFFF );
		public static readonly GColor Beige			= new GColor( 0xFFF5F5DC );
		public static readonly GColor Bisque			= new GColor( 0xFFFFE4C4 );
		public static readonly GColor Black			= new GColor( 0xFF000000 );
		public static readonly GColor BlanchedAlmond	= new GColor( 0xFFFFEBCD );
		public static readonly GColor Blue			= new GColor( 0xFF0000FF );
		public static readonly GColor BlueViolet		= new GColor( 0xFF8A2BE2 );
		public static readonly GColor Brown			= new GColor( 0xFFA52A2A );
		public static readonly GColor BurlyWood		= new GColor( 0xFFDEB887 );
		public static readonly GColor CadetBlue		= new GColor( 0xFF5F9EA0 );
		public static readonly GColor Chartreuse		= new GColor( 0xFF7FFF00 );
		public static readonly GColor Chocolate		= new GColor( 0xFFD2691E );
		public static readonly GColor Coral			= new GColor( 0xFFFF7F50 );
		public static readonly GColor CornflowerBlue	= new GColor( 0xFF6495ED );
		public static readonly GColor Cornsilk		= new GColor( 0xFFFFF8DC );
		public static readonly GColor Crimson		= new GColor( 0xFFDC143C );
		public static readonly GColor Cyan			= new GColor( 0xFF00FFFF );
		public static readonly GColor DarkBlue		= new GColor( 0xFF00008B );
		public static readonly GColor DarkCyan		= new GColor( 0xFF008B8B );
		public static readonly GColor DarkGoldenRod	= new GColor( 0xFFB8860B );
		public static readonly GColor DarkGray		= new GColor( 0xFFA9A9A9 );
		public static readonly GColor DarkGreen		= new GColor( 0xFF006400 );
		public static readonly GColor DarkKhaki		= new GColor( 0xFFBDB76B );
		public static readonly GColor DarkMagenta	= new GColor( 0xFF8B008B );
		public static readonly GColor DarkOliveGreen	= new GColor( 0xFF556B2F );
		public static readonly GColor Darkorange		= new GColor( 0xFFFF8C00 );
		public static readonly GColor DarkOrchid		= new GColor( 0xFF9932CC );
		public static readonly GColor DarkRed		= new GColor( 0xFF8B0000 );
		public static readonly GColor DarkSalmon		= new GColor( 0xFFE9967A );
		public static readonly GColor DarkSeaGreen	= new GColor( 0xFF8FBC8F );
		public static readonly GColor DarkSlateBlue	= new GColor( 0xFF483D8B );
		public static readonly GColor DarkSlateGray	= new GColor( 0xFF2F4F4F );
		public static readonly GColor DarkTurquoise	= new GColor( 0xFF00CED1 );
		public static readonly GColor DarkViolet		= new GColor( 0xFF9400D3 );
		public static readonly GColor DeepPink		= new GColor( 0xFFFF1493 );
		public static readonly GColor DeepSkyBlue	= new GColor( 0xFF00BFFF );
		public static readonly GColor DimGray		= new GColor( 0xFF696969 );
		public static readonly GColor DodgerBlue		= new GColor( 0xFF1E90FF );
		public static readonly GColor FireBrick		= new GColor( 0xFFB22222 );
		public static readonly GColor FloralWhite	= new GColor( 0xFFFFFAF0 );
		public static readonly GColor ForestGreen	= new GColor( 0xFF228B22 );
		public static readonly GColor Fuchsia		= new GColor( 0xFFFF00FF );
		public static readonly GColor Gainsboro		= new GColor( 0xFFDCDCDC );
		public static readonly GColor GhostWhite		= new GColor( 0xFFF8F8FF );
		public static readonly GColor Gold			= new GColor( 0xFFFFD700 );
		public static readonly GColor GoldenRod		= new GColor( 0xFFDAA520 );
		public static readonly GColor Gray			= new GColor( 0xFF808080 );
		public static readonly GColor Green			= new GColor( 0xFF008000 );
		public static readonly GColor GreenYellow	= new GColor( 0xFFADFF2F );
		public static readonly GColor HoneyDew		= new GColor( 0xFFF0FFF0 );
		public static readonly GColor HotPink		= new GColor( 0xFFFF69B4 );
		public static readonly GColor IndianRed		= new GColor( 0xFFCD5C5C );
		public static readonly GColor Indigo			= new GColor( 0xFF4B0082 );
		public static readonly GColor Ivory			= new GColor( 0xFFFFFFF0 );
		public static readonly GColor Khaki			= new GColor( 0xFFF0E68C );
		public static readonly GColor Lavender		= new GColor( 0xFFE6E6FA );
		public static readonly GColor LavenderBlush	= new GColor( 0xFFFFF0F5 );
		public static readonly GColor LawnGreen		= new GColor( 0xFF7CFC00 );
		public static readonly GColor LemonChiffon	= new GColor( 0xFFFFFACD );
		public static readonly GColor LightBlue		= new GColor( 0xFFADD8E6 );
		public static readonly GColor LightCoral		= new GColor( 0xFFF08080 );
		public static readonly GColor LightCyan		= new GColor( 0xFFE0FFFF );
		public static readonly GColor LightGoldenRodYellow  = new GColor( 0xFFFAFAD2 );
		public static readonly GColor LightGrey		= new GColor( 0xFFD3D3D3 );
		public static readonly GColor LightGreen		= new GColor( 0xFF90EE90 );
		public static readonly GColor LightPink		= new GColor( 0xFFFFB6C1 );
		public static readonly GColor LightSalmon	= new GColor( 0xFFFFA07A );
		public static readonly GColor LightSeaGreen	= new GColor( 0xFF20B2AA );
		public static readonly GColor LightSkyBlue	= new GColor( 0xFF87CEFA );
		public static readonly GColor LightSlateBlue	= new GColor( 0xFF8470FF );
		public static readonly GColor LightSlateGray	= new GColor( 0xFF778899 );
		public static readonly GColor LightSteelBlue	= new GColor( 0xFFB0C4DE );
		public static readonly GColor LightYellow	= new GColor( 0xFFFFFFE0 );
		public static readonly GColor Lime			= new GColor( 0xFF00FF00 );
		public static readonly GColor LimeGreen		= new GColor( 0xFF32CD32 );
		public static readonly GColor Linen			= new GColor( 0xFFFAF0E6 );
		public static readonly GColor Magenta		= new GColor( 0xFFFF00FF );
		public static readonly GColor Maroon			= new GColor( 0xFF800000 );
		public static readonly GColor MediumAquaMarine  = new GColor( 0xFF66CDAA );
		public static readonly GColor MediumBlue		= new GColor( 0xFF0000CD );
		public static readonly GColor MediumOrchid	= new GColor( 0xFFBA55D3 );
		public static readonly GColor MediumPurple	= new GColor( 0xFF9370D8 );
		public static readonly GColor MediumSeaGreen	= new GColor( 0xFF3CB371 );
		public static readonly GColor MediumSlateBlue	= new GColor( 0xFF7B68EE );
		public static readonly GColor MediumSpringGreen  = new GColor( 0xFF00FA9A );
		public static readonly GColor MediumTurquoise	= new GColor( 0xFF48D1CC );
		public static readonly GColor MediumVioletRed	= new GColor( 0xFFC71585 );
		public static readonly GColor MidnightBlue	= new GColor( 0xFF191970 );
		public static readonly GColor MintCream		= new GColor( 0xFFF5FFFA );
		public static readonly GColor MistyRose		= new GColor( 0xFFFFE4E1 );
		public static readonly GColor Moccasin		= new GColor( 0xFFFFE4B5 );
		public static readonly GColor NavajoWhite	= new GColor( 0xFFFFDEAD );
		public static readonly GColor Navy			= new GColor( 0xFF000080 );
		public static readonly GColor OldLace		= new GColor( 0xFFFDF5E6 );
		public static readonly GColor Olive			= new GColor( 0xFF808000 );
		public static readonly GColor OliveDrab		= new GColor( 0xFF6B8E23 );
		public static readonly GColor Orange			= new GColor( 0xFFFFA500 );
		public static readonly GColor OrangeRed		= new GColor( 0xFFFF4500 );
		public static readonly GColor Orchid			= new GColor( 0xFFDA70D6 );
		public static readonly GColor PaleGoldenRod	= new GColor( 0xFFEEE8AA );
		public static readonly GColor PaleGreen		= new GColor( 0xFF98FB98 );
		public static readonly GColor PaleTurquoise	= new GColor( 0xFFAFEEEE );
		public static readonly GColor PaleVioletRed	= new GColor( 0xFFD87093 );
		public static readonly GColor PapayaWhip		= new GColor( 0xFFFFEFD5 );
		public static readonly GColor PeachPuff		= new GColor( 0xFFFFDAB9 );
		public static readonly GColor Peru			= new GColor( 0xFFCD853F );
		public static readonly GColor Pink			= new GColor( 0xFFFFC0CB );
		public static readonly GColor Plum			= new GColor( 0xFFDDA0DD );
		public static readonly GColor PowderBlue		= new GColor( 0xFFB0E0E6 );
		public static readonly GColor Purple			= new GColor( 0xFF800080 );
		public static readonly GColor Red			= new GColor( 0xFFFF0000 );
		public static readonly GColor RosyBrown		= new GColor( 0xFFBC8F8F );
		public static readonly GColor RoyalBlue		= new GColor( 0xFF4169E1 );
		public static readonly GColor SaddleBrown	= new GColor( 0xFF8B4513 );
		public static readonly GColor Salmon			= new GColor( 0xFFFA8072 );
		public static readonly GColor SandyBrown		= new GColor( 0xFFF4A460 );
		public static readonly GColor SeaGreen		= new GColor( 0xFF2E8B57 );
		public static readonly GColor SeaShell		= new GColor( 0xFFFFF5EE );
		public static readonly GColor Sienna			= new GColor( 0xFFA0522D );
		public static readonly GColor Silver			= new GColor( 0xFFC0C0C0 );
		public static readonly GColor SkyBlue		= new GColor( 0xFF87CEEB );
		public static readonly GColor SlateBlue		= new GColor( 0xFF6A5ACD );
		public static readonly GColor SlateGray		= new GColor( 0xFF708090 );
		public static readonly GColor Snow			= new GColor( 0xFFFFFAFA );
		public static readonly GColor SpringGreen	= new GColor( 0xFF00FF7F );
		public static readonly GColor SteelBlue		= new GColor( 0xFF4682B4 );
		public static readonly GColor Tan			= new GColor( 0xFFD2B48C );
		public static readonly GColor Teal			= new GColor( 0xFF008080 );
		public static readonly GColor Thistle		= new GColor( 0xFFD8BFD8 );
		public static readonly GColor Tomato			= new GColor( 0xFFFF6347 );
		public static readonly GColor Turquoise		= new GColor( 0xFF40E0D0 );
		public static readonly GColor Violet			= new GColor( 0xFFEE82EE );
		public static readonly GColor VioletRed		= new GColor( 0xFFD02090 );
		public static readonly GColor Wheat			= new GColor( 0xFFF5DEB3 );
		public static readonly GColor White			= new GColor( 0xFFFFFFFF );
		public static readonly GColor WhiteSmoke		= new GColor( 0xFFF5F5F5 );
		public static readonly GColor Yellow			= new GColor( 0xFFFFFF00 );
		public static readonly GColor YellowGreen	= new GColor( 0xFF9ACD32 );
		public static readonly GColor GrassGreen		= new GColor( 0xFF408080 );
	}
}