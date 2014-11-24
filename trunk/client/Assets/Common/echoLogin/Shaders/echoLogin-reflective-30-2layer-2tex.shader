//$-----------------------------------------------------------------------------
//@ Reflective shader	- The base texture plus reflection texture.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, u2, v2 ) 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/C-reflective/30-2layer-2tex"
{
   	Properties 
	{
 		_MainTex ("Texture Base Alpha si relfect amount", 2D)	= "blue" {}
     	_EnvMap ("Texture EnvMap", 2D)							= "blue"  {}
       	_echoUV ("UV Offset u1 v1 u2 v2", Vector )				= ( 0, 0, 0.5, 0.5 )
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
			float4		_EnvMap_ST;
			sampler2D	_MainTex;
			float4      _MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4		_echoUV;

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
    			
     			half3 reflection = EchoReflect ( ad.vertex , ad.normal );

	 			half num = sqrt ( reflection.x * reflection.x + reflection.y * reflection.y + reflection.z * reflection.z ) * 2.0;
	 
				v.tc2.x = _EnvMap_ST.xy * ( reflection.x / num + _echoUV.z + _EnvMap_ST.z );
				v.tc2.y = _EnvMap_ST.xy * ( reflection.y / num + _echoUV.w + _EnvMap_ST.w );
				
   				v.tc1 	= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );
	
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
				fixed3 tcolor;
				
				fcolor = tex2D ( _MainTex, v.tc1  );
				tcolor = tex2D ( _EnvMap, v.tc2 ).xyz;
				
				fcolor.xyz  = lerp ( fcolor.xyz, tcolor.xyz, fcolor.w );
				fcolor.w  	= 1.0;
				
    			return fcolor;
			}

			ENDCG
		}
 	}
}
