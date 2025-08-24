Shader "BBR/ImageEffects/Bloom" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Bloom ("Bloom (RGB)", 2D) = "black" {}
    }
    SubShader
    {
        Pass
        {
            ZTest False
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            sampler2D _Bloom;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = v.texcoord0.xy;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1 = (tex2D (_MainTex, i.texcoord0) + tex2D (_Bloom, i.texcoord0));
                return tmpvar_1;
            }
            ENDCG
        }
        Pass
        {
            ZTest False
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _OffsetsA;
            float4 _OffsetsB;
            sampler2D _MainTex;
            float4 _Parameter;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float2 texcoord3 : TEXCOORD3;
                float2 texcoord4 : TEXCOORD4;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = v.texcoord0.xy;
                o.texcoord1 = (v.texcoord0.xy + _OffsetsA.xy);
                o.texcoord2 = (v.texcoord0.xy + _OffsetsA.zw);
                o.texcoord3 = (v.texcoord0.xy + _OffsetsB.xy);
                o.texcoord4 = (v.texcoord0.xy + _OffsetsB.zw);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1 = (clamp ((
                max (max (max (max (tex2D (_MainTex, i.texcoord0), tex2D (_MainTex, i.texcoord1)), tex2D (_MainTex, i.texcoord2)), tex2D (_MainTex, i.texcoord3)), tex2D (_MainTex, i.texcoord4))
                - _Parameter.z), 0.0, 1.0) * _Parameter.w);
                return tmpvar_1;
            }
            ENDCG
        }
        Pass
        {
            ZTest False
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _OffsetsA;
            float4 _OffsetsB;
            sampler2D _MainTex;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float2 texcoord3 : TEXCOORD3;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = (v.texcoord0.xy + _OffsetsA.xy);
                o.texcoord1 = (v.texcoord0.xy + _OffsetsA.zw);
                o.texcoord2 = (v.texcoord0.xy + _OffsetsB.xy);
                o.texcoord3 = (v.texcoord0.xy + _OffsetsB.zw);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1 = (((
                (tex2D (_MainTex, i.texcoord0) + tex2D (_MainTex, i.texcoord1))
                + tex2D (_MainTex, i.texcoord2)) + tex2D (_MainTex, i.texcoord3)) * 0.25);
                return tmpvar_1;
            }
            ENDCG
        }
    }
    Fallback Off
}