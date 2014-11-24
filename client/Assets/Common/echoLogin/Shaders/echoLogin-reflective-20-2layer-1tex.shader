//$-----------------------------------------------------------------------------
//@ Reflective shader	- Base texture plus reflection from the alpha channel.
//@
//@ NOTE:  Puts greyscale image on alpha channel for reflection.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/C-reflective/30-2layer-1tex"
{
   	Properties 
	{
 		_MainTex ("Texture Base Alpha is Reflection", 2D)		= "black" {}
       	_echoUV ("UV Offset u1 v1 u2 v2", Vector )				= ( 0, 0, 0.5, 0.5 )
    	_echoMixRGBA ( "Mix RGBA", Color )						= ( 0.5, 0.5, 0.5, 0.5 )
   	}
   	
	//=========================================================================
	SubShader 
	{
    	Tags { "Queue"="Geometry+10" "IgnoreProjector" = "True" } 

    	Pass 
		{    
      		Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "EchoLogin.cginc"

			sampler2D	_EnvMap;
			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4		_echoUV;
			fixed4      _echoMixRGBA;

         	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
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
				
    			v.pos = mul ( UNITY_MATRIX_MVP, ad.vertex );
    			
    			float3 reflection 	= EchoReflect ( ad.vertex , ad.normal );
	 			float num 			= sqrt ( reflection.x * reflection.x + reflection.y * reflection.y + reflection.z * reflection.z ) * 2.0;
	 
				v.tc2.x = _MainTex_ST.xy * ( reflection.x / num + _echoUV.z + _MainTex_ST.z );
				v.tc2.y = _MainTex_ST.xy * ( reflection.y / num + _echoUV.w + _MainTex_ST.w );
				
    			v.tc1 	= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy );

	
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
				fixed4 fcolor1 = tex2D ( _MainTex, v.tc1  );
				fixed3 fcolor2 = tex2D ( _MainTex, v.tc2  ).www;
				
				fcolor1.w  = 1.0;
				fcolor1.xyz  = lerp ( fcolor1.xyz, fcolor2, _echoMixRGBA.xyz );
				
    			return fcolor1;
			}

			ENDCG
		}
 	}
}
