Shader "Advanced SS//Bump/Transparent/Diffuse Rim Emissive" {
   Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
      _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
	   _EmissiveMap ("EmissiveMap (RGB)", 2D) = "black" {}
   }
   SubShader {

      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      LOD 400
      
      CGPROGRAM
      #pragma surface surf Lambert alpha

      struct Input {
         float2 uv_MainTex;
         float2 uv_BumpMap;
         float3 viewDir;
      };

      sampler2D _MainTex;
      sampler2D _BumpMap;
      sampler2D _EmissiveMap;
      fixed4 _Color;
      fixed4 _RimColor;
      half _RimPower;

      void surf (Input IN, inout SurfaceOutput o) {
         half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	     o.Albedo = tex.rgb * _Color.rgb;
         o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
         half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
         o.Emission = tex2D(_EmissiveMap, IN.uv_MainTex).rgb + (_RimColor.rgb * pow (rim, _RimPower));
      }
      ENDCG
   }
   
   FallBack "Transparent/Diffuse"
}