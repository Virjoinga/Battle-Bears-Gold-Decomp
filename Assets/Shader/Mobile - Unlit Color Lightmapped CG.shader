Shader "CPX_Custom/Mobile/Unlit Color Lightmapped CG" 
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
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
            float4 _Color;
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
                float4 tmpvar_2;
                tmpvar_2.w = 1.0;
                tmpvar_2.xyz = (2.0 * UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord0.zw).xyz);
                lightMap_1 = tmpvar_2;
                float4 tmpvar_3;
                tmpvar_3.xyz = (_Color.xyz * lightMap_1.xyz);
                tmpvar_3.w = _Color.w;
                return tmpvar_3;
            }
            ENDCG
        }
    }
    
}