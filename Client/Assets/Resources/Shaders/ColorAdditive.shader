Shader "FishHunt/ColorAdditive" {
    Properties {
        _Color ("Color", Color ) = ( 1, 1, 1, 1 )    
		_MainTex ("MainTex", 2D) = "white"
   } 
   Subshader {
        Tags {
            "Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		Pass {
            ZWrite off 
			Fog { Mode Off }
			ColorMask RGB 
			Cull Off
			Lighting Off
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
         
			SetTexture [_MainTex] {
                constantColor [_Color]
				combine constant, texture * constant
			} 
		}
	}
}