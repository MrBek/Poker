Shader "Advanced SS/Bump/Transparent/Spec Map" {
   Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
      _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
      _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
      _SpecMap ("SpecMap (RGB) Heightmap (A)", 2D) = "white" {}
   }
   SubShader {

      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      LOD 400
      
      CGPROGRAM
      #pragma surface surf BlinnPhong alpha

      struct Input {
         float2 uv_MainTex;
         float2 uv_BumpMap;
      };

      sampler2D _MainTex;
      sampler2D _BumpMap;
      sampler2D _SpecMap;
      fixed4 _Color;
      half _Shininess;

      void surf (Input IN, inout SurfaceOutput o) {
         half4 tex = tex2D(_MainTex, IN.uv_MainTex);
         o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
	     o.Albedo = tex.rgb * _Color.rgb;
	     half3 specMapCol = tex2D(_SpecMap, IN.uv_BumpMap).rgb;
	     #if !SHADER_API_FLASH
	     o.Gloss = Luminance(specMapCol);
         #endif
	     _SpecColor *= float4(specMapCol,1);
	     o.Alpha = tex.a * _Color.a;
	     o.Specular = _Shininess;
      }
      ENDCG
   }
   
   FallBack "Transparent/Specular"
}