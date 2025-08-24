Shader "BBR/Edge Detect" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Treshold ("Treshold", Float) = 0.2
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
            float4 _MainTex_TexelSize;
            sampler2D _MainTex;
            float _Treshold;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord0_1 : TEXCOORD0_1;
                float2 texcoord0_2 : TEXCOORD0_2;
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
                float4 tmpvar_4;
                tmpvar_4 = mul(UNITY_MATRIX_TEXTURE0, tmpvar_3);
                float2 tmpvar_5;
                tmpvar_5.x = -(_MainTex_TexelSize.x);
                float cse_6;
                cse_6 = -(_MainTex_TexelSize.y);
                tmpvar_5.y = cse_6;
                float2 tmpvar_7;
                tmpvar_7.x = _MainTex_TexelSize.x;
                tmpvar_7.y = cse_6;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_4.xy;
                o.texcoord0_1 = (tmpvar_4.xy + tmpvar_5);
                o.texcoord0_2 = (tmpvar_4.xy + tmpvar_7);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float3 p3_1;
                float3 p2_2;
                float4 original_3;
                float4 tmpvar_4;
                tmpvar_4 = tex2D (_MainTex, i.texcoord0);
                original_3 = tmpvar_4;
                float3 tmpvar_5;
                tmpvar_5 = tex2D (_MainTex, i.texcoord0_1).xyz;
                p2_2 = tmpvar_5;
                float3 tmpvar_6;
                tmpvar_6 = tex2D (_MainTex, i.texcoord0_2).xyz;
                p3_1 = tmpvar_6;
                float3 tmpvar_7;
                tmpvar_7 = (((original_3.xyz * 2.0) - p2_2) - p3_1);
                float tmpvar_8;
                tmpvar_8 = dot (tmpvar_7, tmpvar_7);
                if ((tmpvar_8 >= _Treshold)) {
                original_3.xyz = float3(0.0, 0.0, 0.0);
                }
                return original_3;
            }
        ENDCG
    }   
}
Fallback Off
}