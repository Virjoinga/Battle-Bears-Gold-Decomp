Shader "Custom/ColoredWeapon" 
{
    Properties
    {
        _WeaponColor ("Weapon Color", Color) = (1,0,0,1)
        _MainTex ("Texture (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _MainTex_ST;
            sampler2D _MainTex;
            float4 _WeaponColor;
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
                float3 texcoord2 : TEXCOORD2;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float3 tmpvar_1;
                float3x3 tmpvar_2;
                tmpvar_2[0] = unity_ObjectToWorld[0].xyz;
                tmpvar_2[1] = unity_ObjectToWorld[1].xyz;
                tmpvar_2[2] = unity_ObjectToWorld[2].xyz;
                float3 tmpvar_3;
                tmpvar_3 = mul(tmpvar_2, (normalize(v.normal) * 1.0));
                tmpvar_1 = tmpvar_3;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = ((v.texcoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
                o.texcoord1 = tmpvar_1;
                o.texcoord2 = float3(0.0, 0.0, 0.0);
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 c_1;
                float3 tmpvar_2;
                float4 c_3;
                float4 tmpvar_4;
                tmpvar_4 = tex2D (_MainTex, i.texcoord0);
                c_3 = tmpvar_4;
                float3 tmpvar_5;
                tmpvar_5 = lerp (_WeaponColor.xyz, c_3.xyz, c_3.www);
                tmpvar_2 = tmpvar_5;
                tmpvar_2 = (tmpvar_2 * 2.0);
                float4 c_6;
                c_6.xyz = (tmpvar_2 * 0.5);
                c_6.w = 0.0;
                c_1.w = c_6.w;
                c_1.xyz = (c_6.xyz + (tmpvar_2 * i.texcoord2));
                return c_1;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}