//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Flag waving in wind effect.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoSpeed          - Speed of ripple
//# _echoHeight         - Height/size of each ripple
//# _echoAmount         - Amount of ripples
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/B-fullbright/42-flag"
{
	Properties 
	{
		_MainTex ("Texture", 2D)								= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )						= ( 0, 0, 0, 0 )
   		_echoSpeed ("Wave Speed", Range ( -256.0, 256.0 ) )		= 16
   		_echoHeight ("Wave Height", Range ( 0.0, 4.0 ) )		= 0.25
   		_echoAmount ("Wave Amount", Range ( 1.0, 32.0 ) )		= 4
 	}

	//=========================================================================
	SubShader 
	{
    	Tags { "Queue"="Geometry" "IgnoreProjector" = "True" } 

    	Pass 
		{    
      		Cull Off
     		
			CGPROGRAM
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "EchoLogin.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4      _echoUV;
			float       _echoSpeed;
			float       _echoHeight;
			float       _echoAmount;

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
            };

			// ============================================= 	
			Varys vert ( VertInput  ad )
			{
				Varys v;
				float ripple;
				
				ripple			= EchoWave ( ad.texcoord.x, _echoAmount, _echoSpeed, _echoHeight );
				ad.vertex.xyz 	=  ad.vertex.xyz + ( half3(ripple,ripple,ripple) * ad.normal );
	
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
    			return tex2D ( _MainTex, v.tc1 );
			}

			ENDCG
		}
 	}
 }
