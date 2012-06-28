Shader "Advanced SS/Misc/Stylised Lighting" {
    Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
      _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	   _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
      _RimColor ("Rim Color", Color) = (0.75,0.75,0.75,0.0)
      _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
      _WarpRamp ("WarpRamp", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf WarpBlinnPhong
      
      sampler2D _MainTex;
      sampler2D _BumpMap;
      sampler2D _WarpRamp;
      fixed4 _Color;
      half _Shininess;
      fixed4 _RimColor;
      half _RimPower;
      
      inline fixed4 LightingWarpBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
      {
	      fixed3 h = normalize (lightDir + viewDir);
      	
	      half NdotL = dot (s.Normal, lightDir);
          fixed3 diff = tex2D (_WarpRamp, float2(NdotL * 0.5 + 0.5,0));
      	
	      float nh = max (0, dot (s.Normal, h));
	      float spec = pow (nh, s.Specular*128.0) * s.Gloss;
      	
	      fixed4 c;
	      c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
	      c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
	      return c;
      }
      
      struct Input {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 viewDir;
      };
      
      void surf (Input IN, inout SurfaceOutput o) {
          fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
          
          o.Albedo = tex.rgb * _Color.rgb;
          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
          half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          o.Emission = _RimColor.rgb * pow (rim, _RimPower);
          o.Gloss = tex.a;
	      o.Alpha = tex.a * _Color.a;
	      o.Specular = _Shininess;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }