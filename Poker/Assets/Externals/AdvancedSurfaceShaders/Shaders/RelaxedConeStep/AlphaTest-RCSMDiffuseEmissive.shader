Shader "Advanced SS/Relaxed Cone Stepping/Transparent/Cutout/Diffuse Emissive" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Parallax ("Height", Range (0.005, 0.16)) = 0.02
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_RelaxedConeMap ("RelaxedConeMap", 2D) = "white" {}
	_EmissiveMap ("EmissiveMap (RGB)", 2D) = "black" {}
	_ClipTiling ("Clip Tiling U,V", Vector) = (1, 1, 0, 0)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
      LOD 400

CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff
#pragma target 3.0
#include "../AdvancedSS.cginc"

sampler2D _MainTex;
sampler2D _RelaxedConeMap;
sampler2D _EmissiveMap;
fixed4 _Color;
float _Parallax;
float _NumSamples;
fixed4 _ClipTiling;

float depth_bias;
float border_clamp;

struct Input {
	float2 uv_MainTex;
	float2 uv_RelaxedConeMap;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {

    float2 p = RelaxedConeStep( IN.viewDir, _Parallax, IN.uv_RelaxedConeMap, _RelaxedConeMap, _ClipTiling );
    float2 offset = p - IN.uv_RelaxedConeMap;
    
	IN.uv_MainTex += offset;
	
	half4 tex = tex2D(_MainTex,IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Alpha = tex.a * _Color.a;
	o.Normal = ConeStepNormal( tex2D(_RelaxedConeMap, p).rgb );
	
    o.Emission = tex2D(_EmissiveMap, IN.uv_MainTex).rgb;
}
ENDCG
}

FallBack "Transparent/Cutout/Diffuse"
}