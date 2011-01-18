Shader "GUI/Vertex Colored" {
Properties {
	_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
	_Emission ("Emmisive Color", Color) = (0,0,0,0)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

Category {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Tags { "LightMode" = "Vertex" }
	Cull Off
	ZWrite Off
	Alphatest Greater 0
	Blend SrcAlpha OneMinusSrcAlpha 
	SubShader {
		Material {
            // DISABLE { 
            // Diffuse [_Color]
            // Ambient [_Color]
            // } DISABLE end 
			Emission [_Emission]	
		}
		Pass {
            ColorMaterial AmbientAndDiffuse
			Lighting Off
            Cull Off
            SetTexture [_MainTex] {
                Combine texture * primary
                // DISABLE { 
                // Combine texture * primary, texture * primary
                // } DISABLE end 
            }
            // DISABLE { 
            // SetTexture [_MainTex] {
            //     constantColor [_Color]
            //     Combine previous * constant DOUBLE, previous * constant
            // }  
            // } DISABLE end 
		}
	} 
}
}
