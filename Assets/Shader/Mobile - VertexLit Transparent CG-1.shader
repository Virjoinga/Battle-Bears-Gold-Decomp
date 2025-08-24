Shader "CPX_Custom/Mobile/VertexLit Transparent CG-1" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent-1" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
        Pass
        {
            Tags { "QUEUE"="Transparent-1" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _MainTex_ST;
            sampler2D _MainTex;
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
                float4 tmpvar_1;
                tmpvar_1.w = 1.0;
                tmpvar_1.xyz = v.vertex.xyz;
                o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.vertex = UnityObjectToClipPos(tmpvar_1);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 color_1;
                float4 tmpvar_2;
                tmpvar_2 = tex2D (_MainTex, i.texcoord0);
                color_1 = tmpvar_2;
                color_1.xyz = (color_1.xyz * _Color.w);
                float4 tmpvar_3;
                tmpvar_3.xyz = ((color_1.xyz + color_1.xyz) * _Color.xyz);
                tmpvar_3.w = (color_1.w * _Color.w);
                return tmpvar_3;
            }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}