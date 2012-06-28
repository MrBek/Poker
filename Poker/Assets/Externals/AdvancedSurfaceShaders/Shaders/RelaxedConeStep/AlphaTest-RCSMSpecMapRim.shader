Shader "Advanced SS/Relaxed Cone Stepping/Transparent/Cutout/Spec Map Rim" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_Parallax ("Height", Range (0.005, 0.16)) = 0.02
    _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_RelaxedConeMap ("RelaxedConeMap", 2D) = "white" {}
	_SpecMap ("SpecMap (RGB)", 2D) = "white" {}
	_ClipTiling ("Clip Tiling U,V", Vector) = (1, 1, 0, 0)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
      LOD 400

CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_Cutoff
#pragma target 3.0
#include "../AdvancedSS.cginc"

sampler2D _MainTex;
sampler2D _RelaxedConeMap;
sampler2D _SpecMap;
fixed4 _Color;
half _Shininess;
float _Parallax;
float _NumSamples;
fixed4 _ClipTiling;
fixed4 _RimColor;
half _RimPower;

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
	o.Normal = ConeStepNormal( tex2D(_RelaxedConeMap, p).rgb );
	
	half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
    o.Emission = (_RimColor.rgb * pow (rim, _RimPower));
}
ENDCG
}

FallBack "Transparent/Cutout/Specular"
}