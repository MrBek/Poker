Shader "Advanced SS/Bump/Diffuse Rim" {
   Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
      _RimColor ("Rim Color", Color) = (0.75,0.75,0.75,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
   }
   SubShader {

      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert

      struct Input {
         float2 uv_MainTex;
         float2 uv_BumpMap;
         float3 viewDir;
      };

      sampler2D _MainTex;
      sampler2D _BumpMap;
      fixed4 _Color;
      fixed4 _RimColor;
      half _RimPower;

      void surf (Input IN, inout SurfaceOutput o) {
         half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	     o.Albedo = tex.rgb * _Color.rgb;
         o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
         half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
         o.Emission = _RimColor.rgb * pow (rim, _RimPower);
      }
      ENDCG
   }
   
   Fallback "Diffuse"
}