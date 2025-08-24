Shader "Toon/Basic Outline" 
{
    Properties
    {
        _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline width", Range(0.1,2)) = 0.15
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { TexGen CubeNormal }
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        UsePass "Toon/Basic/BASE"
        Pass
        {
            Name "OUTLINE"
            Tags { "LIGHTMODE"="Always" "RenderType"="Opaque" }
            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float _Outline;
            float4 _OutlineColor;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float4 color : COLOR;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = UnityObjectToClipPos(v.vertex);
                tmpvar_1.zw = tmpvar_2.zw;
                float3x3 tmpvar_3;
                tmpvar_3[0] = UNITY_MATRIX_IT_MV[0].xyz;
                tmpvar_3[1] = UNITY_MATRIX_IT_MV[1].xyz;
                tmpvar_3[2] = UNITY_MATRIX_IT_MV[2].xyz;
                float2x2 tmpvar_4;
                tmpvar_4[0] = UNITY_MATRIX_P[0].xy;
                tmpvar_4[1] = UNITY_MATRIX_P[1].xy;
                tmpvar_1.xy = (tmpvar_2.xy + ((
                (clamp (mul(tmpvar_4, mul(tmpvar_3, 
                normalize(v.normal)
                ).xy), float2(-1.0, -1.0), float2(1.0, 1.0)) * tmpvar_2.z)
                * _Outline) / tmpvar_2.z));
                o.vertex = tmpvar_1;
                o.color = _OutlineColor;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1 = i.color;
                return tmpvar_1;
            }
            ENDCG
        }
    }
    Fallback "Toon/Basic"
}