Shader "CPX_Custom/Mobile/Unlit Color GLSL" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    { 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex; 
            float4 _Color;
            float4 _MainTex_ST;
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
                o.texcoord0 = v.texcoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 color = tex2D(_MainTex, i.texcoord0);
                return (color + color) * _Color;
            }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}
