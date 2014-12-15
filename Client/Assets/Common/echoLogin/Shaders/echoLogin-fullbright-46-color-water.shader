//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Water Wave effect. (Vertex alpha affects height of wave)
//@
//@ Properties/Uniforms
//@
//# _echoUV         		- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoRGBA         		- Vector4 ( r, g, b, a ) 
//# _echoSpeed         	 	- Speed of wave movement
//# _echoHeight         	- Height/size of each wave
//# _echoAmount         	- Amount of Waves
//# _echoCenterX        	- X/U position in texture to start Wave ( 0 - 1 )
//# _echoCenterY       	 	- Y/V position in texture to start Wave ( 0 - 1 )
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/B-fullbright/46-color-water"
{
	Properties 
   	{
    	_MainTex ("Texture", 2D )										= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )								= ( 0, 0, 0, 0 )
 		_echoRGBA ( "RGB Multiply", Vector )							= ( 1, 1, 1, 1 )    
   		_echoSpeed ("Wave Speed", Range ( -256.0, 256.0 ) )				= 16
   		_echoHeight ("Wave Height", Range ( 0.0, 16.0 ) )				= 1
   		_echoAmount ("Wave Amount", Range ( 1.0, 128.0 ) )				= 4
   		_echoCenterX ("Wave Center X", Range ( -2.0, 2.0 ) )			= 0.0
   		_echoCenterY ("Wave Center Y", Range ( -2.0, 2.0 ) )			= 0.0
   		_echoCrest ("Wave Crest", Range ( 0.0, 8.0 ) )					= 0.5
   		_echoCrestStrength ("Wave Crest Strength", Range ( 0.0, 1.0 ) )	= 0.1
  	}
   
	//=========================================================================
	SubShader 
	{
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" }

    	Pass 
		{    
      		Cull Back
     	
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "EchoLogin.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4		_MainTex_TexelSize;
			float4      _echoUV;
			float4		_echoRGBA;
			float       _echoSpeed;
			float       _echoHeight;
			float       _echoAmount;
			float       _echoCenterX;
			float       _echoCenterY;

           	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float4 color	: COLOR;
			  	float3 normal   : NORMAL;
            };

           	struct Varys
            {
                half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed3 vcolor	: TEXCOORD1;
			  	fixed3 acolor   : TEXCOORD2;
            };

 			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys v;

				v.acolor = fixed3 ( 0.0, 0.0, 0.0 );
								
				if ( _echoHeight != 0.0 )
				{
					float ripple;
					float height;
				
					height				=  _echoHeight * ad.color.w * 0.05;
					ripple				= EchoRipple ( ad.texcoord, _echoAmount, _echoSpeed, height, _echoCenterX, _echoCenterY );
					ad.vertex.xyz		=  ad.vertex.xyz + ( half3 ( ripple, ripple, ripple ) * ad.normal );
					v.acolor			= clamp ( ripple * 2, 0.0, 0.3 );
				}
				
 				v.vcolor		= ad.color.xyz * _echoRGBA.xyz;
 					
   				v.pos 			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  		= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
				return v;
			}
 	
 			//=============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = tex2D ( _MainTex, v.tc1 );
				return fixed4 ( fcolor.xyz * v.vcolor + v.acolor, 1.0 );
			}

			ENDCG
		}
 	}
}
