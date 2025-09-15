Shader "CPX_Custom/Mobile/Unlit (Custom Lightmap 1 Support)" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _LightMap ("Lightmap", 2D) = "white" {}   // manual lightmap slot
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_ST;

        sampler2D _LightMap;
        float4 _LightMap_ST;

        struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;   // main texture UV
            float2 uv2 : TEXCOORD1;  // secondary UVs (lightmap)
        };

        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
        };

        v2f vert (appdata v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.uv2 = TRANSFORM_TEX(v.uv2, _LightMap);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target {
            fixed4 baseCol = tex2D(_MainTex, i.uv);
            fixed4 lmCol   = tex2D(_LightMap, i.uv2);
            return baseCol * lmCol; // multiply base texture by lightmap
        }
        ENDCG
    }
}
Fallback "Mobile/VertexLit"
}
