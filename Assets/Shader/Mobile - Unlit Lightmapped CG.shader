// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "CPX_Custom/Mobile/Unlit Lightmapped CG" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Lightmap ("Lightmap (RGB)", 2D) = "gray" {}   // 👈 added manual lightmap field
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 150

        CGPROGRAM
        #pragma surface surf Lambert noforwardadd

        sampler2D _MainTex;
        sampler2D _Lightmap;

        struct Input {
            float2 uv_MainTex;
            float2 uv2_Lightmap;   // secondary UV for lightmap
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 lm = tex2D(_Lightmap, IN.uv2_Lightmap);

            // Multiply albedo by lightmap color
            o.Albedo = c.rgb * lm.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    Fallback "Mobile/VertexLit"
}
