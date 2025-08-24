Shader "Special/Skins/Gold Reflective (toony)" 
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { TexGen CubeNormal }
        _ReflectColorTex ("Reflect Color", 2D) = "white" {}
        _ReflectMap ("Reflection Map", CUBE) = "dummy.jpg" { TexGen CubeReflect }
        _Color ("Base Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Name "BASE"
            Tags { "RenderType"="Opaque" }
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _MainTex_ST;
            sampler2D _MainTex;
            samplerCUBE _ToonShade;
            sampler2D _ReflectColorTex;
            samplerCUBE _ReflectMap;
            float4 _Color;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float3 texcoord1 : TEXCOORD1;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1.w = 0.0;
                tmpvar_1.xyz = normalize(v.normal);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.texcoord1 = UnityObjectToViewPos(tmpvar_1).xyz;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 reflectMask_1;
                float4 reflection_2;
                float4 cube_3;
                float4 tmpvar_4;
                tmpvar_4 = tex2D (_MainTex, i.texcoord0);
                float4 tmpvar_5;
                tmpvar_5 = (_Color * tmpvar_4);
                float4 tmpvar_6;
                tmpvar_6 = texCUBE (_ToonShade, i.texcoord1);
                cube_3 = tmpvar_6;
                float4 tmpvar_7;
                tmpvar_7 = texCUBE (_ReflectMap, i.texcoord1);
                reflection_2 = tmpvar_7;
                float4 tmpvar_8;
                tmpvar_8 = tex2D (_ReflectColorTex, i.texcoord0);
                reflectMask_1 = tmpvar_8;
                float4 tmpvar_9;
                tmpvar_9.xyz = (((2.0 * cube_3.xyz) * tmpvar_5.xyz) + (reflection_2.xyz * reflectMask_1.xyz));
                tmpvar_9.w = tmpvar_5.w;
                return tmpvar_9;
            }
            ENDCG
        }
    }
    Fallback "Special/Skins/Gold Reflective"
}