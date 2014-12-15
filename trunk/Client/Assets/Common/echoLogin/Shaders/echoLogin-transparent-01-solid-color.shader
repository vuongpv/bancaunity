//$-----------------------------------------------------------------------------
//@ Transparent Shader		- Solid color transparent shader that uses vertex color.
//@
//@ Properties/Uniforms
//@
//# _echoColor             - Object color 
//# _echoScale             - Scale mesh in shader
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/E-transparent/01-solid-color"
{
	Properties 
   	{
      	_echoColor ( "Color", Color )					= ( 1, 1, 1, 1 )    
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
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
			float4      _echoColor;
			float4      _echoScale;

          	struct VertInput
            {
                float4 vertex	: POSITION;
			  	float4 color	: COLOR;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
 			  	fixed4 vcolor   : TEXCOORD0;
           	};

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;
				
    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );
				v.vcolor		= ad.color * _echoColor;

				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
    			return v.vcolor;
			}

			ENDCG
		}
 	}
}
