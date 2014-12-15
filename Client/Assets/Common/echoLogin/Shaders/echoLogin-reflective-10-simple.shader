//$-----------------------------------------------------------------------------
//@ Reflective shader	- The fastest reflective shader of this group.
//@
//@ Properties/Uniforms
//@
//# _echoUV         	- The UV offset of texture Vector4 ( u1, v1, 0, 0 ) 
//#  
//&-----------------------------------------------------------------------------
Shader "echoLogin/C-reflective/20-simple"
{
   	Properties 
	{
    	_MainTex ("Texture", 2D)				= "black" 
       	_echoUV ("UV Offset u1 v1", Vector )		= ( 0.5, 0.5, 0, 0 )
   	}
   	   	
	//=========================================================================
	SubShader 
	{
    	Tags { "Queue"="Geometry+10" "IgnoreProjector" = "True" } 

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
			float4		_echoUV;

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
			Varys vert ( VertInput ad )
			{
				Varys v;
				
    			v.pos = mul ( UNITY_MATRIX_MVP, ad.vertex );
    			
    			float3 reflection 	= EchoReflect ( ad.vertex , ad.normal );
	 			float num 			= sqrt ( reflection.x * reflection.x + reflection.y * reflection.y + reflection.z * reflection.z ) * 2.0;

				v.tc1.x = _MainTex_ST.xy * ( reflection.x / num + _echoUV.x + _MainTex_ST.zw );
				v.tc1.y = _MainTex_ST.xy * ( reflection.y / num + _echoUV.y + _MainTex_ST.zw );
	
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
