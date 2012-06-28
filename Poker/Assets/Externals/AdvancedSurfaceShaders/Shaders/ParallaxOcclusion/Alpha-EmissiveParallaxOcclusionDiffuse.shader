Shader "Advanced SS/Parallax Occlusion (D3D)/Transparent/Diffuse Emissive" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Parallax ("Height", Range (0.005, 0.16)) = 0.02
	_NumSamples ("NumSamples", Float) = 40
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_SpecMap ("Heightmap (A)", 2D) = "black" {}
	_EmissiveMap ("EmissiveMap (RGB)", 2D) = "black" {}
}

SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      LOD 400

CGPROGRAM
#pragma surface surf Lambert alpha
#pragma target 3.0
#pragma only_renderers d3d9
#include "../AdvancedSS.cginc"

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _SpecMap;
sampler2D _EmissiveMap;
fixed4 _Color;
float _Parallax;
float _NumSamples;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 viewDir;
	float3 normal;
	float3 viewDirWS;
};

void vert (inout appdata_full v, out Input o)
{
	o.viewDirWS = normalize(WorldSpaceViewDir( v.vertex ));
	o.normal = normalize(v.normal);
}

void surf (Input IN, inout SurfaceOutput o) {
	
	float2 offset = ParallaxOcclusionOffset( IN.viewDir, IN.viewDirWS, _Parallax, IN.normal, IN.uv_BumpMap, _SpecMap, _NumSamples );
    
	IN.uv_MainTex -= offset;
	IN.uv_BumpMap -= offset;
	
	half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Alpha = tex.a * _Color.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	o.Emission = tex2D(_EmissiveMap, IN.uv_MainTex).rgb;
}
ENDCG
}

FallBack "Transparent/VertexLit"
}