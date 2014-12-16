//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Textured shader that uses Vertex Color and _echoRGBA coloring.
//@
//@ Properties/Uniforms
//@
//# _echoUV         		- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoRGBA         		- Vector4 ( r, g, b, a ) 
//&-----------------------------------------------------------------------------
Shader "echoLogin/B-fullbright/20-color"
{
	Properties 
	{
		_MainTex ("Texture", 2D)				= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
		_echoRGBA ( "RGB Multiply", Vector )	= ( 1, 1, 1, 1 )    
 	}

	//=========================================================================
	SubShader 
	{
    	Tags { "Queue"="Geometry" "IgnoreProjector" = "True" } 

    	Pass 
		{    
      		Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4      _echoUV;
			float4		_echoRGBA;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float4 color	: COLOR;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed3 vcolor   : TEXCOORD1;
            };

			//=============================================
			Varys vert ( VertInput  ad )
			{
				Varys v;

				v.vcolor		= ad.color.xyz * _echoRGBA.xyz;
    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
    			return fixed4 ( tex2D ( _MainTex, v.tc1 ).xyz * v.vcolor, 1.0 );
			}

			ENDCG
		}
 	}
 }