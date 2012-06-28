Shader "Advanced SS/Parallax/Transparent/Spec Map Rim" {
   Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
      _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
      _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	   _Parallax ("Height", Range (0.005, 0.08)) = 0.02
      _RimColor ("Rim Color", Color) = (0.75,0.75,0.75,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
	   _SpecMap ("SpecMap (RGB) Heightmap (A)", 2D) = "white" {}
   }
   SubShader {

      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      LOD 400
      
      CGPROGRAM
      #pragma surface surf BlinnPhong alpha
	  #pragma target 3.0

      struct Input {
         float2 uv_MainTex;
         float2 uv_BumpMap;
         float3 viewDir;
      };

      sampler2D _MainTex;
      sampler2D _BumpMap;
      sampler2D _SpecMap;
      fixed4 _Color;
      half _Shininess;
      float _Parallax;
      fixed4 _RimColor;
      half _RimPower;

      void surf (Input IN, inout SurfaceOutput o) {
         half h = tex2D (_SpecMap, IN.uv_BumpMap).w;
	     float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);
	     IN.uv_MainTex += offset;
	     IN.uv_BumpMap += offset;
      
         half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	     o.Albedo = tex.rgb * _Color.rgb;
	     half3 specMapCol = tex2D(_SpecMap, IN.uv_BumpMap).rgb;
	     o.Gloss = Luminance(specMapCol);
	     _SpecColor *= float4(specMapCol,1);
	     o.Alpha = tex.a * _Color.a;
	     o.Specular = _Shininess;
         o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
         half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
         o.Emission = _RimColor.rgb * pow (rim, _RimPower);
      }
      ENDCG
   }
   
   FallBack "Transparent/Specular"
}