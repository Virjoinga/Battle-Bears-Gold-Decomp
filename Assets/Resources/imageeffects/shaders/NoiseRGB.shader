Shader "BBR/Noise Shader RGB" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _GrainTex ("Base (RGB)", 2D) = "gray" {}
        _ScratchTex ("Base (RGB)", 2D) = "gray" {}
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _GrainOffsetScale;
            float4 _ScratchOffsetScale;
            sampler2D _MainTex;
            sampler2D _GrainTex;
            sampler2D _ScratchTex;
            float4 _Intensity;
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
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float2 tmpvar_1;
                tmpvar_1 = v.texcoord0.xy;
                float2 inUV_2;
                inUV_2 = tmpvar_1;
                float4 tmpvar_3;
                tmpvar_3.zw = float2(0.0, 0.0);
                tmpvar_3.xy = inUV_2;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = (glstate_matrix_texture0 * tmpvar_3).xy;
                o.texcoord1 = ((v.texcoord0.xy * _GrainOffsetScale.zw) + _GrainOffsetScale.xy);
                o.texcoord2 = ((v.texcoord0.xy * _ScratchOffsetScale.zw) + _ScratchOffsetScale.xy);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 col_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.texcoord0);
                col_1.w = tmpvar_2.w;
                col_1.xyz = (tmpvar_2.xyz + ((
                (tex2D (_GrainTex, i.texcoord1).xyz * 2.0)
                - 1.0) * _Intensity.x));
                col_1.xyz = (col_1.xyz + ((
                (tex2D (_ScratchTex, i.texcoord2).xyz * 2.0)
                - 1.0) * _Intensity.y));
                return col_1;
            }
            ENDCG
        }
    }
    Fallback Off
}