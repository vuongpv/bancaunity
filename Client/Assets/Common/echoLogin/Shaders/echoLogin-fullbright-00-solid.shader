//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Solid color shader.
//@
//@ Properties/Uniforms
//@
//# _echoColor              - Object color
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/B-fullbright/00-solid"
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
			fixed4		_echoColor; 
			
          	struct VertInput
            {
                float4 vertex	: POSITION;
           	};

           	struct Varys
            {
                half4 pos		: SV_POSITION;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;
				
    			v.pos 		= mul ( UNITY_MATRIX_MVP, ad.vertex );

				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
    			return _echoColor;
			}

			ENDCG
		}
 	}
 }
