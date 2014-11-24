//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Flag waving in wind effect.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoSpeed          - Speed of ripple
//# _echoHeight         - Height/size of each ripple
//# _echoAmount         - Amount of ripples
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/D-Light/30-flag"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)								= "black" {} 
       	_echoUV("UV Offset u1 v1", Vector )						= ( 0, 0, 0, 0 )
   		_echoSpeed ("Wave Speed", Range ( 0.0, 256.0 ) )		= 16
   		_echoHeight ("Wave Height", Range ( 0.0, 8.0 ) )		= 1
   		_echoAmount ("Wave Amount", Range ( 1.0, 32.0 ) )		= 4
   	}
   	
	//=========================================================================
	SubShader 
	{
        Tags { "Queue" = "Geometry"  "IgnoreProjector" = "True" } 

 		// PASS FOR FRONT ======================================================================
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

			sampler2D 	_MainTex;
			float4		_MainTex_ST;
			float4 		_MainTex_TexelSize;
			float4		_echoUV;
			float       _echoSpeed;
			float       _echoHeight;
			float       _echoAmount;

         	struct VertInput
            {
                float4 vertex	 : POSITION;
                float2 texcoord	 : TEXCOORD0;
			  	float3 normal    : NORMAL;
            };

           	struct Varys
            {
            	half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed3 dcolor	: TEXCOORD1;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;
				float3 dcolor = UNITY_LIGHTMODEL_AMBIENT.xyz;
				float ripple;

#if ( ECHO_POINT || ECHO_POINTANDDIRECTIONAL )
				dcolor += EchoCalcLight_Point ( ad.normal, ad.vertex, 0 );
#endif
				
#if ( ECHO_DIRECTIONAL || ECHO_POINTANDDIRECTIONAL )
				dcolor += EchoCalcLight_Directional ( ad.normal, ad.vertex );
#endif

				ripple 			= EchoWave ( ad.texcoord.x, _echoAmount, _echoSpeed, _echoHeight );
				ad.vertex.xyz 	= ad.vertex.xyz + ( half3 ( ripple, ripple, ripple ) * ad.normal );

				v.dcolor        = dcolor;
	   			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 			= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
    			return v;
			}

			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				return fixed4 ( tex2D ( _MainTex, v.tc1 ).xyz * v.dcolor, 1 );
			}

			ENDCG
		}
		
		// PASS FOR BACK ======================================================================
    	Pass 
		{    
			Tags { "LightMode" = "ForwardBase" }
 
      		Cull Front
     		
			CGPROGRAM
			
			#include "EchoLogin.cginc"
			
 			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D 	_MainTex;
			float4		_MainTex_ST;
			float4		_echoUV;
			float       _echoSpeed;
			float       _echoHeight;
			float       _echoAmount;

#if UNITY_UV_STARTS_AT_TOP
			half4 _MainTex_TexelSize;
#endif	
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
			  	fixed3 dcolor	: TEXCOORD1;
            };

			// ============================================= 	
			Varys vert ( VertInput ad )
			{
				Varys v;
				float3 dcolor = EchoLightAmbient();

				if ( unity_LightColor[0].w > 0.0 )
					dcolor += EchoCalcLight_Point ( ad.normal, ad.vertex, 0 );
				else
					dcolor += EchoCalcLight_Directional ( ad.normal, ad.vertex );

				v.dcolor        = dcolor;
	   			v.pos			= mul ( UNITY_MATRIX_MVP, ad.vertex );
   				v.tc1 			= _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy );

				ad.vertex.z = EchoWave ( ad.texcoord.x, _echoAmount, _echoSpeed, _echoHeight );

#if UNITY_UV_STARTS_AT_TOP
				if ( _MainTex_TexelSize.y < 0 )
					v.tc1.y = 1.0-v.tc1.y;
#endif
    			return v;
			}

			// ============================================= 	
			fixed4 frag ( Varys v ):COLOR
			{
				return fixed4 ( tex2D ( _MainTex, v.tc1 ).xyz * v.dcolor, 1 );
			}

			ENDCG
		}
 	}
}
 
