//$-----------------------------------------------------------------------------
//@ Background Shader 	- Shader that fades from one texture to another.
//@
//@ Properties/Uniforms
//@
//# _echoUV             - The UV offsets of texture Vector4 ( u1, v1, u2, v2 ) 
//# _echoMix            - Amount to fade from _MainTex to _Layer1Tex
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/A-background/30-mix2tex"
{
	Properties 
	{
    	_MainTex ("Texture", 2D)						= "black" {} 
   	   	_Layer1Tex ("Texture Overlay", 2D)				= "black" {} 
      	_echoUV ( "UV Offset u1 v1 u2 v2", Vector )		= ( 0, 0, 0, 0 )
    	_echoMix("Mix", Color )							= ( 0.5, 0.5, 0.5 ,0.5 )
   	}

	//=========================================================================
	SubShader 
	{
		Tags { "Queue" = "Geometry+500" "IgnoreProjector"="True"}

    	Pass 
		{    
			ZWrite Off
      	 	Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			sampler2D	_Layer1Tex;
			float4		_Layer1Tex_ST;
			float4		_echoUV;
			fixed4		_echoMix;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
            };

           	struct Varys
            {
            	half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
                half2 tc2		: TEXCOORD1;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;

    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );
   				v.tc2 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.zw + _MainTex_ST.zw );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
				{
					v.tc1.y = 1.0-v.tc1.y;
					v.tc2.y = 1.0-v.tc2.y;
				}
#endif
    			return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor;
				
				fcolor.w 	= 1.0;
				fcolor.xyz 	= lerp ( tex2D ( _MainTex, v.tc1 ).xyz, tex2D ( _Layer1Tex, v.tc1 ).xyz, _echoMix.xyz ); 
				
				return fcolor;
			}

			ENDCG
		}
 	}
}
