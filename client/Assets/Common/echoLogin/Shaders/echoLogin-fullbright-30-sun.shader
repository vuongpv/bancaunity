//$-----------------------------------------------------------------------------
//@ Fullbright/Unlit Shader	- Fades texture back and forth from different UV positions.
//@
//@ Properties/Uniforms
//@
//# _echoUV         		- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoColor         	    - Color of Rim lighting
//# _echoSpeed          	- Speed of texture changes
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/B-fullbright/30-sun"
{
	Properties 
	{
		_MainTex ("Texture", 2D)						= "black" {} 
      	_echoUV("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
     	_echoColor ("Rim Color", Color ) 				= ( 1.0,0.1,0.2, 1.0 )
   		_echoSpeed ("Fade Speed", Range ( 0.0, 64.0 ) )	= 16
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

			#include "EchoLogin.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4		_echoUV;
			float       _echoSpeed;
			fixed4      _echoColor;

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
                half2 tc2		: TEXCOORD1;
                fixed mixper    : TEXCOORD2;
                fixed rimmix    : TEXCOORD3;
            };

			// ============================================= 	
			Varys vert ( VertInput  ad )
			{
				Varys v;
				float3 	normalDir;
				float 	dotprod;
				float 	mixper;
			
				normalDir = EchoNormalDir ( ad.normal );
					
				dotprod = abs ( dot ( EchoViewDir ( EchoVertexPos ( ad.vertex ) ), normalDir ) );

				v.rimmix = 1.0 - clamp ( dotprod * 2.0, 0.0, 1.0 );

    			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 			= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy );
   				v.tc2.x         = 1.0 - v.tc1.x;
   				v.tc2.y         = 1.0 - v.tc1.y;
   				
				v.mixper = ( sin ( _Time * _echoSpeed ) * 0.5 ) + 0.5;
				
				return v;
			}
 	
			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = tex2D ( _MainTex, v.tc1 );
				fcolor.xyz = lerp ( fcolor.xyz, tex2D ( _MainTex, v.tc2 ).xyz, v.mixper );
				fcolor.xyz = lerp ( fcolor.xyz, _echoColor.xyz, v.rimmix );
    			return fcolor;
			}

			ENDCG
		}
 	}
 }
