//$-----------------------------------------------------------------------------
//@ Lighted Shader		- The fastest textured shader of this group.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/D-Light/10-simple"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)				= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )		= ( 0, 0, 0, 0 )
   	}
   	
	//=========================================================================
	SubShader 
	{
        Tags { "Queue" = "Geometry" } 

		Pass 
		{    
			Tags { "LightMode" = "ForwardBase" }
 
      		Cull Back
     		
     		CGPROGRAM
			
			#include "EchoLogin.cginc"
			
			sampler2D 	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4		_echoUV;

 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile ECHO_POINT ECHO_DIRECTIONAL ECHO_POINTANDDIRECTIONAL

         	struct VertInput
            {
                float4 vertex	 : POSITION;
                float2 texcoord	 : TEXCOORD0;
			  	float3 normal    : NORMAL;
            };
        
           	struct Varys
            {
            	half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed3 dcolor	: TEXCOORD1;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;
				float3 dcolor =  EchoLightAmbient();

#if ( ECHO_POINT || ECHO_POINTANDDIRECTIONAL )
				dcolor += EchoCalcLight_Point ( ad.normal, ad.vertex, 0 );
#endif

#if ( ECHO_DIRECTIONAL || ECHO_POINTANDDIRECTIONAL )
				dcolor += EchoCalcLight_Directional ( ad.normal, ad.vertex );
#endif

				v.dcolor        = dcolor;
	   			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
    			return v;
			}

			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				return fixed4 ( tex2D ( _MainTex, v.tc1 ).xyz * v.dcolor, 1 );
			}

			ENDCG
		}

 	}
}
 
