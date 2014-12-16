//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Dissolve with edge burn color to nothing.
//@
//@ NOTE: The alpha channel contains dissolve data. 
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoBurnSize       - Size of edge burn
//# _echoBurnColor      - Color of burn
//# _echoMix            - Dissolve amount 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/B-fullbright/75-dissolve-burn"
{
   	Properties 
	{
    	_MainTex ("Texture Main", 2D )					= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
    	_echoBurnSize("BurnSize", Range ( 0.0, 1.0 ) )	= 0.1
    	_echoBurnColor ( "BurnColor", Color )			= ( 0.8, 0.4, 0.0, 1 )
    	_echoMix("Mix", Range ( -0.3, 1.3 ) )			= 0
   	}

 	//=========================================================================
	SubShader 
	{
 		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

    	Pass 
		{    
      	 	Blend SrcAlpha OneMinusSrcAlpha
      		Cull Back

 			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
	
			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4		_MainTex_TexelSize;
			float4		_echoUV;
			fixed		_echoMix;
			fixed		_echoBurnSize;
			fixed4		_echoBurnColor;

         	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
            };
	
           	struct Varys
           	{
				half4 pos : SV_POSITION;
                half2 tc1 : TEXCOORD0;
           	};

			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;

    			v.pos	= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
    			return v;
			}
 	
			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
    			fixed4 fcolor	= tex2D ( _MainTex, v.tc1 );
 				fixed tcolor	= fcolor.w;

				if ( tcolor <= _echoMix )
				{
					fcolor = fixed4 ( 0.0, 0.0, 0.0, 0.0 );
					
					if ( ( _echoMix - tcolor ) < _echoBurnSize )
						fcolor = _echoBurnColor;
				}
				else
				{
					fcolor.w = 1.0;
				}
					
    			return fcolor;
			}

			ENDCG
		}
 	}
}