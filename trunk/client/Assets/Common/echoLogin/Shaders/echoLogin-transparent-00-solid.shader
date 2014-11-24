//$-----------------------------------------------------------------------------
//@ Transparent Shader	- Solid color transparent shader.
//@
//@ Properties/Uniforms
//@
//# _echoColor             - Object color 
//# _echoScale             - Scale mesh in shader
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/E-transparent/00-solid"
{
	Properties 
   	{
      	_echoColor ( "Color", Color )			= ( 1, 1, 1, 1 )    
       	_echoScale ("Scale XYZ", Vector )		= ( 1.0, 1.0, 1.0, 1.0 )
   	}

	//=========================================================================
	SubShader 
	{
 		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

    	Pass 
		{    
      	 	ZWrite Off
      	 	Blend SrcAlpha OneMinusSrcAlpha
      		Cull Back
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D	_MainTex;
			float4      _echoScale;
			half4       _echoColor;

          	struct VertInput
            {
                float4 vertex	: POSITION;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
           	};

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;
				
    			v.pos	= mul ( UNITY_MATRIX_MVP, ad.vertex *_echoScale );

				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
    			return _echoColor;
			}

			ENDCG
		}
 	}
}
