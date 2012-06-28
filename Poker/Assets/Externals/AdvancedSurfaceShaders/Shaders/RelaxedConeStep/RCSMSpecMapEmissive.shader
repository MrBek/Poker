Shader "Advanced SS/Relaxed Cone Stepping/Spec Map Emissive" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_Parallax ("Height", Range (0.005, 0.16)) = 0.02
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_RelaxedConeMap ("RelaxedConeMap", 2D) = "white" {}
	_SpecMap ("SpecMap (RGB)", 2D) = "white" {}
	_EmissiveMap ("EmissiveMap (RGB)", 2D) = "black" {}
	_ClipTiling ("Clip Tiling U,V", Vector) = (1, 1, 0, 0)
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 500

CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0
#include "../AdvancedSS.cginc"

sampler2D _MainTex;
sampler2D _RelaxedConeMap;
sampler2D _SpecMap;
sampler2D _EmissiveMap;
fixed4 _Color;
half _Shininess;
float _Parallax;
float _NumSamples;
fixed4 _ClipTiling;

float depth_bias;
float border_clamp;

struct Input {
	float2 uv_MainTex;
	float2 uv_RelaxedConeMap;
	float2 uv_SpecMap;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {

    float2 p = RelaxedConeStep( IN.viewDir, _Parallax, IN.uv_RelaxedConeMap, _RelaxedConeMap, _ClipTiling );
    float2 offset = p - IN.uv_RelaxedConeMap;
    
	IN.uv_MainTex += offset;
	IN.uv_SpecMap += offset;
	
	half4 tex = tex2D(_MainTex,IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	half3 specMapCol = tex2D(_SpecMap, IN.uv_SpecMap).rgb;
	o.Gloss = Luminance(specMapCol);
	_SpecColor *= float4(specMapCol,1);
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = tex2D(_RelaxedConeMap,p.xy).rgb;
	o.Normal.xy = 2*o.Normal.xy - 1;
	o.Normal.z = sqrt(1.0 - dot(o.Normal.xy,o.Normal.xy));
	
    o.Emission = tex2D(_EmissiveMap, IN.uv_MainTex).rgb;
}
ENDCG
}

FallBack "Specular"
}