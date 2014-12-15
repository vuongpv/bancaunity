//$-----------------------------------------------------------------------------
//@ Lighted Shader		- Vertex specular.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//# _echoShine          - Shininess
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/D-Light/50-specular"
{
   	Properties 
	{
    	_MainTex ("RGB Texture Alpha is lightmap", 2D)	= "black" {}
       	_echoUV ("UV Offset u1 v1", Vector )				= ( 0, 0, 0, 0 )
      	_echoScale ("Scale XYZ", Vector )				= ( 1.0, 1.0, 1.0, 1.0 )
    	_echoShine ( "Shininess", Range ( 0.1, 32.0 ) )	= 0
   	}

	//=========================================================================
	SubShader 
	{
      	Tags { "Queue" = "Geometry"  "IgnoreProjector"="True" "RenderType"="Opaque" } 

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
			float      	_echoShine;
			float4		_echoRGBA;
			float4      _echoScale;

         	struct VertInput
            {
                float4 vertex	: POSITION;
                float2 texcoord	: TEXCOORD0;
			  	float3 normal   : NORMAL;
			  	float4 color	: COLOR;
            };
	
           	struct Varys
           	{
				half4 pos		: SV_POSITION;
                half2 tc1		: TEXCOORD0;
			  	fixed3 dcolor	: TEXCOORD1;
			  	fixed3 scolor  	: TEXCOORD2;
           	};

			//=============================================
			Varys vert ( VertInput ad )
			{
				Varys 	v;
				float3 	normalDir;
				float3 	lightDir;
				float 	dotprod;
				float4 	position;
				float3 	dcolor =  EchoLightAmbient();
				float3  scolor = fixed3 ( 0,0,0 );

				position 	= EchoVertexPos ( ad.vertex );

#if ( ECHO_POINT || ECHO_POINTANDDIRECTIONAL )
				lightDir 	= EchoLightDir_Point ( position, 0 );
				normalDir 	= EchoNormalDir ( ad.normal );
				dotprod 	= dot ( normalDir, lightDir );

				dcolor += EchoCalcLight_Point ( dotprod, 0 );

				if ( dotprod >= 0 )
					scolor += EchoSpecular ( unity_LightColor[0].xyz, _echoShine, lightDir, normalDir, EchoViewDir ( position ) );
#endif

#if ( ECHO_DIRECTIONAL || ECHO_POINTANDDIRECTIONAL )
				lightDir 	= EchoLightDir_Directional ( position );
				normalDir 	= EchoNormalDir ( ad.normal );
				dotprod 	= dot ( normalDir, lightDir );

				dcolor += EchoCalcLight_Directional ( dotprod );

				if ( dotprod >= 0 )
					scolor += EchoSpecular ( _LightColor0.xyz, _echoShine, lightDir, normalDir, EchoViewDir ( position ) );
#endif

    			v.dcolor  = clamp ( dcolor, 0, 2 );
    			v.scolor  = clamp ( scolor * 0.5, 0, 0.5 );
    			v.pos	  = mul ( UNITY_MATRIX_MVP, ad.vertex * _echoScale );
   				v.tc1 	  = _MainTex_ST.xy * ( ad.texcoord.xy + _echoUV.xy + _MainTex_ST.zw );
 
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
				
				fcolor.xyz 	= fcolor.xyz * v.dcolor + v.scolor;
								
				return fcolor;
			}

			ENDCG
		}
 	}
}
