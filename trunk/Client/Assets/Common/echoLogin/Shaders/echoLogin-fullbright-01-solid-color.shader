//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Solid color shader which also uses vertex color.
//@
//@ Properties/Uniforms
//@
//# _echoColor             - Object color 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/B-fullbright/01-solid-color"
{
   	Properties 
	{
    	_echoColor ("Color", Color )	= ( 1, 1, 1, 1 )
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
			float4      _echoColor;

          	struct VertInput
            {
                float4 vertex	: POSITION;
			  	float4 color	: COLOR;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
 			  	fixed3 vcolor   : TEXCOORD0;
           	};

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;
				
    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
				v.vcolor		= ad.color.xyz * _echoColor.xyz;

				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
    			return fixed4 ( v.vcolor, 1.0 );
			}

			ENDCG

		}
 	}
 }
