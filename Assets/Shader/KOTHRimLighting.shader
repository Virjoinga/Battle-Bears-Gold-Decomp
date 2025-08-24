Shader "Rim Light" 
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0,1)) = 0.5
        _SpeedX ("Speed X", Float) = 0
        _SpeedY ("Speed Y", Float) = 0
        _FlashPeriod ("Flash Period", Range(1,10)) = 1
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _MainTex_ST;
            float4 _RimColor;
            float _SpeedX;
            float _SpeedY;
            sampler2D _MainTex;
            float4 _Color;
            float _Alpha;
            float _FlashPeriod;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float3 color : COLOR;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = _WorldSpaceCameraPos;
                float tmpvar_2;
                tmpvar_2 = clamp (((
                (1.0 - dot (normalize(v.normal), normalize((
                (mul(unity_WorldToObject, tmpvar_1).xyz * 1.0)
                - v.vertex.xyz))))
                - 0.3) / 0.7), 0.0, 1.0);
                float4 tmpvar_3;
                tmpvar_3.x = (v.texcoord0.x + (_Time.y * _SpeedX));
                tmpvar_3.y = (v.texcoord0.y + (_Time.y * _SpeedY));
                tmpvar_3.zw = v.texcoord0.zw;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = ((tmpvar_3.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.color = (((tmpvar_2 * (tmpvar_2 * 
                (3.0 - (2.0 * tmpvar_2))
                ))) * _RimColor.xyz);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 texcol_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.texcoord0);
                texcol_1 = tmpvar_2;
                texcol_1.xyz = ((texcol_1 * (
                (_Color + cos((_Time.y * _FlashPeriod)))
                + 1.0)).xyz + i.color);
                texcol_1.w = _Alpha;
                return texcol_1;
            }
            ENDCG
        }
    }
    
}