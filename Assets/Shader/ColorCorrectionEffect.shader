Shader "Hidden/Color Correction Effect" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _RampTex ("Base (RGB)", 2D) = "grayscaleRamp" {}
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
            sampler2D _MainTex;
            sampler2D _RampTex;
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
                float2 tmpvar_1;
                tmpvar_1 = v.texcoord0.xy;
                float2 tmpvar_2;
                float2 tmpvar_3;
                float2 inUV_4;
                inUV_4 = tmpvar_1;
                float4 tmpvar_5;
                tmpvar_5.zw = float2(0.0, 0.0);
                tmpvar_5.xy = inUV_4;
                tmpvar_3 = (glstate_matrix_texture0 * tmpvar_5).xy;
                tmpvar_2 = tmpvar_3;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_2;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                tmpvar_1 = tex2D (_MainTex, i.texcoord0);
                float4 tmpvar_2;
                tmpvar_2.x = (tex2D (_RampTex, tmpvar_1.xx).x + 1e-05);
                tmpvar_2.y = (tex2D (_RampTex, tmpvar_1.yy).y + 2e-05);
                tmpvar_2.z = (tex2D (_RampTex, tmpvar_1.zz).z + 3e-05);
                tmpvar_2.w = tmpvar_1.w;
                return tmpvar_2;
            }
            ENDCG
        }
    }
    Fallback Off
}