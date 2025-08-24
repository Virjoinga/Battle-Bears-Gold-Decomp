Shader "CPX_Custom/Mobile/Unlit Transparent (Gold)" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
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
                float4 color_1;
                float4 tmpvar_2;
                float2 P_3;
                P_3 = ((i.texcoord0 * _MainTex_ST.xy) + _MainTex_ST.zw);
                tmpvar_2 = tex2D (_MainTex, P_3);
                color_1 = tmpvar_2;
                color_1.xyz = (color_1.xyz * _Color.xyz);
                color_1.x = ((0.85 * color_1.y) + 0.15);
                color_1.y = (color_1.y * 0.8);
                return color_1;
            }
            ENDCG
        }
    }
    
}