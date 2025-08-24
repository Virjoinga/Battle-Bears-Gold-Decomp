Shader "CPX_Custom/Mobile/Unlit Lightmapped CG" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        LOD 100
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Tags { "RenderType"="Opaque" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _MainTex_ST;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };
            struct v2f
            {
                float4 texcoord0 : TEXCOORD0;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1.xy = v.texcoord0.xy;
                tmpvar_1.zw = ((v.texcoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_1;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 lightMap_1;
                float4 color_2;
                float4 tmpvar_3;
                float2 P_4;
                P_4 = ((i.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                tmpvar_3 = tex2D (_MainTex, P_4);
                color_2 = tmpvar_3;
                float4 tmpvar_5;
                tmpvar_5.w = 1.0;
                tmpvar_5.xyz = (2.0 * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord0.zw).xyz);
                lightMap_1 = tmpvar_5;
                float4 tmpvar_6;
                tmpvar_6.xyz = (color_2.xyz * lightMap_1.xyz);
                tmpvar_6.w = color_2.w;
                return tmpvar_6;
            }
            ENDCG
        }
    }
    
}