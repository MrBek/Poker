Shader "Advanced SS/Diffuse/Transparent/Cutout/Emissive Rim" {
   Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      _MainTex ("Texture", 2D) = "white" {}
	  _EmissiveMap ("EmissiveMap (RGB)", 2D) = "black" {}
	  _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
   }
   SubShader {

      Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
      LOD 400
      
      CGPROGRAM
      #pragma surface surf Lambert alphatest:_Cutoff

      struct Input {
         float2 uv_MainTex;
         float3 viewDir;
      };

      sampler2D _MainTex;
      sampler2D _EmissiveMap;
      fixed4 _Color;
      fixed4 _RimColor;
      half _RimPower;

      void surf (Input IN, inout SurfaceOutput o) {
         half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	     o.Albedo = tex.rgb * _Color.rgb;
         half rim = 1.0 - saturate(dot (normalize(IN.viewDir), normalize(o.Normal)));
         o.Emission = tex2D(_EmissiveMap, IN.uv_MainTex).rgb + (_RimColor.rgb * pow (rim, _RimPower));
      }
      ENDCG
   }
   
   FallBack "Transparent/Cutout/Specular"
}