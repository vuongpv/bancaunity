//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Solid color shader which also uses vertex color.
//@
//@ Properties/Uniforms
//@
//# _echoColor          - Object color 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/D-Light/00-solid-color"
{
   	Properties 
	{
 		_echoColor ( "Color", Color )	= ( 1, 1, 1, 1 )    
  	}

	//=========================================================================
	SubShader 
	{
        Tags { "Queue" = "Geometry"  "IgnoreProjector" = "True" } 

    	Pass 
		{    
			Tags { "LightMode" = "ForwardBase" }
 
      		Cull Back
     		
			CGPROGRAM
			
			#include "EchoLogin.cginc"
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile ECHO_POINT ECHO_DIRECTIONAL ECHO_POINTANDDIRECTIONAL

			float4	  _echoColor;

         	struct VertInput
            {
                float4 vertex	 : POSITION;
                float2 texcoord	 : TEXCOORD0;
			  	float3 normal    : NORMAL;
			  	float4 color     : COLOR;
            };

           	struct Varys
            {
            	half4 pos		: SV_POSITION;
			  	fixed3 dcolor	: TEXCOORD0;
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

				v.dcolor        = dcolor * ad.color * _echoColor;
	   			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );

    			return v;
			}

			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				return fixed4 ( v.dcolor, 1 );
			}

			ENDCG
		}
 	}
}
 
