//$-----------------------------------------------------------------------------
//@ Transparent Shader	- Textured with vertex color and _echoRGBA color.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoRGBA         	- Vector4 ( r, g, b, a ) 
//# _echoScale          - Scale mesh in shader
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/E-transparent/20-color"
{
	Properties 
   	{
    	_MainTex ("Texture", 2D )						= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
 		_echoRGBA ( "RGB Multiply", Vector )			= ( 1, 1, 1, 1 )    
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
			float4		_MainTex_ST;
			float4		_MainTex_TexelSize;
			float4      _echoUV;
			float4		_echoRGBA;
			float4      _echoScale;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float4 color	: COLOR;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed4 vcolor	: TEXCOORD1;
            };

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;
				
 				v.vcolor		= ad.color * _echoRGBA;
   				v.pos 			= mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );
  				v.tc1 			= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
    			return tex2D ( _MainTex, v.tc1 ) * v.vcolor;
			}

			ENDCG
		}
 	}
}
