#include "UnityCG.cginc"
#include "Lighting.cginc"

// ============================================= 	
inline float3 EchoLightAmbient()
{
 	return UNITY_LIGHTMODEL_AMBIENT.xyz + UNITY_LIGHTMODEL_AMBIENT.xyz;
}

// ============================================= 	
inline float3 EchoLightDir_Point ( float4 ivertex, int index )
{
 	return normalize ( float3 ( unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index] ) - mul ( _Object2World, ivertex ).xyz );
}

// ============================================= 	
inline float3 EchoLightDir_Directional ( float4 ivertexpos )
{
	return  _WorldSpaceLightPos0.xyz;
}

// =============================================
inline float3 EchoNormalDir ( float3 ivertex )
{
//	return mul ( ivertex , float3x3( _World2Object[0].xyz, _World2Object[1].xyz, _World2Object[2].xyz) );
	return float3 ( mul ( ivertex , (float3x3)_World2Object ) );
}

// =============================================
inline float3 EchoCalcLight_Directional ( float3 inormal, float4 ivertex )
{
	return _LightColor0.xyz * max ( 0.0, dot ( EchoNormalDir ( inormal ), EchoLightDir_Directional ( ivertex ) ) );
} 	

// =============================================
inline float3 EchoCalcLight_Directional ( float idotprod )
{
	return _LightColor0.xyz * max ( 0.0, idotprod );
} 	

// =============================================
inline float3 EchoCalcLight_Point (  float3 inormal, float4 ivertex, int index )
{
	return unity_LightColor[index].xyz * max ( 0.0, dot ( EchoNormalDir ( inormal ), EchoLightDir_Point ( ivertex, index ) ) );
} 	

// =============================================
inline float3 EchoCalcLight_Point (  float idotprod, int index )
{
	return unity_LightColor[index].xyz * max ( 0.0, idotprod );
} 	

// =============================================
inline float4 EchoVertexPos ( float4 ivertexpos )
{
	return mul ( _Object2World, ivertexpos );
}

// =============================================
inline float3 EchoViewDir ( float3 ivertexpos )
{
	return normalize ( _WorldSpaceCameraPos - ivertexpos );
}

// =============================================
inline float3 EchoSpecular ( float3 icolor, float ishine, float3 ilightdir, float3 inormaldir, float3 iviewdir )
{
	return icolor * pow ( max ( 0.0, dot ( reflect ( -ilightdir, inormaldir ), iviewdir ) ), float3 ( ishine, ishine, ishine ) ) ;
}

// =============================================
inline float EchoShieldHit ( float4 ihitvec, float ihitmix, float3 inormaldir )
{
	if ( ihitvec.w > 0.0 )
	{
		float dotprod = dot ( inormaldir, normalize ( ihitvec.xyz ) );
		
		if ( dotprod <= ihitmix )
			return lerp ( 0.0, 1.0, clamp ( ( ihitmix - dotprod ) * 10.0, 0.0, 2.0 ) );
			
		return 0.0;
	}
	
	return 0.0;
}

// =============================================
inline float EchoWave ( float itexu, float iamount, float ispeed, float iheight )
{
	return  ( itexu * sin ( itexu * iamount - ( _Time * ispeed ) ) ) * iheight;
}

// =============================================
inline float EchoRipple ( float2 itexuv, float iamount, float ispeed, float iheight, float icenterx, float icentery )
{
	itexuv.x += icenterx;
	itexuv.y += icentery;

	return sin ( length ( itexuv ) * iamount - ispeed * _Time ) * iheight;
}

// =============================================
inline float3 EchoReflect ( float4 ivertex, float3 ivnormal )
{
	float3 reflection = reflect ( normalize ( mul ( UNITY_MATRIX_MV , ivertex ) ), float3 ( normalize ( mul ( (float3x3)UNITY_MATRIX_MV , ivnormal ) ) ) );
	
	reflection.z += 1.0;
	
	return reflection;
}

// =============================================
inline float echoRand ( float2 inv )
{
	return (sin(dot(inv.xy ,float2(12.9898,78.233))) * 43758.5453);
}
