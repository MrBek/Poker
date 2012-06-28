Shader "Advanced SS/Specular/Specular Emissive" {
   Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
      _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
      _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
      _MainTex ("Texture", 2D) = "white" {}
	   _EmissiveMap ("EmissiveMap (RGB)", 2D) = "black" {}
   }
   SubShader {

      Tags { "RenderType" = "Opaque" }
      
      CGPROGRAM
      #pragma surface surf BlinnPhong

      struct Input {
         float2 uv_MainTex;
         float3 viewDir;
      };

      sampler2D _MainTex;
      sampler2D _EmissiveMap;
      fixed4 _Color;
      half _Shininess;

      void surf (Input IN, inout SurfaceOutput o) {
         half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	      o.Albedo = tex.rgb * _Color.rgb;
	      o.Gloss = tex.a;
	      o.Alpha = tex.a * _Color.a;
	      o.Specular = _Shininess;
         o.Emission = tex2D(_EmissiveMap, IN.uv_MainTex).rgb;
      }
      ENDCG
   }
   
   Fallback "Specular"
}