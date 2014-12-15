//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Planet shader with rim lighting.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture 
//# _echoColor          - Color of rim lighting
//# _echoLightSize      - Size of rim lighting on lighted side
//# _echoDarkSize       - Size of rim lighting on dark side 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/D-Light/62-planet-rimlight"
{
   	Properties 
	{
    	_MainTex ("Texture BackGround", 2D)								= "black" {} 
       	_echoUV("UV Offset u1 v1 u2 v2", Vector )						= ( 0, 0, 0, 0 )
     	_echoColor ("Rim Color", Color )								= ( 1,1,1,1)
     	_echoLightSize ("Lighted Side Rim Size", Range ( 0.0, 4.0 ) )	= 0.5
     	_echoDarkSize ("Dark Side Rim Size", Range ( 0.0, 1.0 ) )		= 0.14
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
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile ECHO_POINT ECHO_DIRECTIONAL ECHO_POINTANDDIRECTIONAL
  
			#include "EchoLogin.cginc"

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4		_echoUV;
			fixed4      _echoColor;
			float       _echoLightSize;
			float       _echoDarkSize;

         	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
			  	float4 color    : COLOR;
            };

           	struct Varys
            {
            	half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD1;
			  	fixed3 dcolor	: TEXCOORD4;
			  	fixed rimmix 	: TEXCOORD5;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys 	v;
				float3 	normalDir;
				float3 	lightDir;
				float3 	viewDir;
				float  	dotprod;
				float4 	position;
				float  	rimmix;
				float  	rimnum;
				float3 	dcolor =  EchoLightAmbient();

				position 	= EchoVertexPos ( ad.vertex );
				normalDir 	= EchoNormalDir ( ad.normal );
				rimmix 		= 0;

#if ( ECHO_POINT || ECHO_POINTANDDIRECTIONAL )
				lightDir 	= EchoLightDir_Point ( position, 0 );
				dotprod 	= dot ( normalDir, lightDir );
			
				dcolor += EchoCalcLight_Point ( dotprod, 0 );
				
				// rim lighting
				rimnum 		= lerp ( _echoDarkSize, _echoLightSize, max ( 0.0, dotprod ) );
				viewDir		= EchoViewDir ( position );
				dotprod 	= dot ( viewDir, normalDir );
				
				rimmix = 1.0 - ( dotprod * ( 1.0 / rimnum ) );
#endif

#if ( ECHO_DIRECTIONAL || ECHO_POINTANDDIRECTIONAL )
				lightDir 	= EchoLightDir_Directional ( position );
				dotprod 	= dot ( normalDir, lightDir );
				
				dcolor += EchoCalcLight_Directional ( dotprod );
				
				// rim lighting
				rimnum 		= lerp ( _echoDarkSize, _echoLightSize, max ( 0.0, dotprod ) );
				viewDir		= EchoViewDir ( position );
				dotprod 	= dot ( viewDir, normalDir );
				
				rimmix += ( 1.0 - ( dotprod * ( 1.0 / rimnum ) ) );
#endif
				
				v.rimmix    = clamp ( rimmix, 0.0, 0.9 );
				v.dcolor  	= clamp ( dcolor, 0.0, 2.0 );
	   			v.pos		= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 	  	= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );
 
#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
    			return v;
			}
 
  			// =============================================
			fixed4 frag ( Varys v ):COLOR
			{
				fixed4 fcolor = tex2D ( _MainTex, v.tc1 );
				
				fcolor.w = 1;
				fcolor.xyz = lerp ( fcolor.xyz * v.dcolor, _echoColor.xyz, v.rimmix );

				return fcolor;
			}

			ENDCG
		}
 	}
}
 
