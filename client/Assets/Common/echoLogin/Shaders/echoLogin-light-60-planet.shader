//$-----------------------------------------------------------------------------
//@ Lighted Shader	- Planet shader with clouds.
//@
//@ NOTE: Clouds are greyscale from alpha channel and each layer has its own UV set.
//@
//@ Properties/Uniforms
//@
//# _echoUV         - Two sets of UV offsets in one Vector4 ( u1, v1, u2, v2 ) 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/D-Light/60-planet"
{
   	Properties 
	{
    	_MainTex ("Texture BackGround", 2D)				= "black" {} 
       	_echoUV ("UV Offset u1 v1 u2 v2", Vector )		= ( 0, 0, 0, 0 )
 	}

	//=========================================================================
	SubShader 
	{
       	Tags { "Queue" = "Geometry+10"  "IgnoreProjector"="True" } 

    	Pass 
		{    
			Tags { "LightMode" = "ForwardBase" }
 
      		Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "EchoLogin.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4		_echoUV;

         	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal	: NORMAL;
            };

           	struct Varys
            {
            	half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD1;
                half2 tc2		: TEXCOORD2;
			  	fixed3 dcolor	: TEXCOORD4;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;
				float3 dcolor =  EchoLightAmbient();

				if ( unity_LightColor[0].w > 0.0 )
					dcolor += EchoCalcLight_Point ( ad.normal, ad.vertex, 0 );
				else
					dcolor += EchoCalcLight_Directional ( ad.normal, ad.vertex );

				v.dcolor    = dcolor;
	   			v.pos		= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  	= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );
   				v.tc2 	  	= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.zw + _MainTex_ST.zw );
 
#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
    			return v;
			}
 
  			// =============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = tex2D ( _MainTex, v.tc1 );
				fixed4 newcolor = tex2D ( _MainTex, v.tc2 );
				
				fcolor.w = 1;
				fcolor.xyz = ( fcolor.xyz + newcolor.www * 0.5 ) * v.dcolor;

				return fcolor;
			}

			ENDCG
		}
 	}
}
 
