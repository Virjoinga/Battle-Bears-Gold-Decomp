Shader "Transparent Effects/CheapForcefield2" 
{
    Properties
    {
        _Scale ("_Scale", Range(0.1,4)) = 0.15
        _Color ("_Color", Color) = (0,1,0,1)
        _Texture ("_Texture", 2D) = "white" {}
        _Properties ("Rim, Strength, Speed, Tile ", Vector) = (1.2,1.5,0.5,5)
    }
    SubShader
    {
        Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            AlphaTest Greater 0
            ColorMask RGB
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4 _Texture_ST;
            float4 _Color;
            sampler2D _Texture;
            float4 _Properties;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord0 : TEXCOORD0;
                float4 tangent : TANGENT;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float3 texcoord1 : TEXCOORD1;
                float3 texcoord2 : TEXCOORD2;
                float3 texcoord3 : TEXCOORD3;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1.xyz = normalize(v.tangent.xyz);
                tmpvar_1.w = v.tangent.w;
                float3 tmpvar_2;
                tmpvar_2 = normalize(v.normal);
                float3 shlight_3;
                float2 tmpvar_4;
                float3 tmpvar_5;
                float3 tmpvar_6;
                float2 tmpvar_7;
                tmpvar_7 = ((v.texcoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
                tmpvar_4 = tmpvar_7;
                float3x3 tmpvar_8;
                tmpvar_8[0] = unity_ObjectToWorld[0].xyz;
                tmpvar_8[1] = unity_ObjectToWorld[1].xyz;
                tmpvar_8[2] = unity_ObjectToWorld[2].xyz;
                float3 tmpvar_9;
                float3 tmpvar_10;
                tmpvar_9 = tmpvar_1.xyz;
                tmpvar_10 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * v.tangent.w);
                float3x3 tmpvar_11;
                tmpvar_11[0].x = tmpvar_9.x;
                tmpvar_11[0].y = tmpvar_10.x;
                tmpvar_11[0].z = tmpvar_2.x;
                tmpvar_11[1].x = tmpvar_9.y;
                tmpvar_11[1].y = tmpvar_10.y;
                tmpvar_11[1].z = tmpvar_2.y;
                tmpvar_11[2].x = tmpvar_9.z;
                tmpvar_11[2].y = tmpvar_10.z;
                tmpvar_11[2].z = tmpvar_2.z;
                float3 tmpvar_12;
                tmpvar_12 = mul(tmpvar_11, mul(unity_WorldToObject, _WorldSpaceLightPos0).xyz);
                tmpvar_5 = tmpvar_12;
                float4 tmpvar_13;
                tmpvar_13.w = 1.0;
                tmpvar_13.xyz = _WorldSpaceCameraPos;
                float4 tmpvar_14;
                tmpvar_14.w = 1.0;
                tmpvar_14.xyz = mul(tmpvar_8, (tmpvar_2 * 1.0));
                float3 tmpvar_15;
                float4 normal_16;
                normal_16 = tmpvar_14;
                float vC_17;
                float3 x3_18;
                float3 x2_19;
                float3 x1_20;
                float tmpvar_21;
                tmpvar_21 = dot (unity_SHAr, normal_16);
                x1_20.x = tmpvar_21;
                float tmpvar_22;
                tmpvar_22 = dot (unity_SHAg, normal_16);
                x1_20.y = tmpvar_22;
                float tmpvar_23;
                tmpvar_23 = dot (unity_SHAb, normal_16);
                x1_20.z = tmpvar_23;
                float4 tmpvar_24;
                tmpvar_24 = (normal_16.xyzz * normal_16.yzzx);
                float tmpvar_25;
                tmpvar_25 = dot (unity_SHBr, tmpvar_24);
                x2_19.x = tmpvar_25;
                float tmpvar_26;
                tmpvar_26 = dot (unity_SHBg, tmpvar_24);
                x2_19.y = tmpvar_26;
                float tmpvar_27;
                tmpvar_27 = dot (unity_SHBb, tmpvar_24);
                x2_19.z = tmpvar_27;
                float tmpvar_28;
                tmpvar_28 = ((normal_16.x * normal_16.x) - (normal_16.y * normal_16.y));
                vC_17 = tmpvar_28;
                float3 tmpvar_29;
                tmpvar_29 = (unity_SHC.xyz * vC_17);
                x3_18 = tmpvar_29;
                tmpvar_15 = ((x1_20 + x2_19) + x3_18);
                shlight_3 = tmpvar_15;
                tmpvar_6 = shlight_3;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_4;
                o.texcoord1 = mul(tmpvar_11, ((
                mul(unity_WorldToObject, tmpvar_13)
                .xyz * 1.0) - v.vertex.xyz));
                o.texcoord2 = tmpvar_5;
                o.texcoord3 = tmpvar_6;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 c_1;
                float3 tmpvar_2;
                tmpvar_2 = i.texcoord1;
                float2 texCoords_3;
                float tmpvar_4;
                float tmpvar_5;
                tmpvar_5 = normalize(tmpvar_2).z;
                tmpvar_4 = (1.0 - tmpvar_5);
                float2 tmpvar_6;
                tmpvar_6.x = (i.texcoord0.x + (_Time.x * _Properties.z));
                tmpvar_6.y = i.texcoord0.y;
                float2 tmpvar_7;
                tmpvar_7 = (tmpvar_6 * _Properties.ww);
                texCoords_3 = tmpvar_7;
                c_1.xyz = _Color.xyz;
                c_1.w = (((tex2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
                return c_1;
            }
            ENDCG
        }
        Pass
        {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardAdd" "QUEUE"="Transparent" "RenderType"="Transparent" }
            ZWrite Off
            Fog {
            Color (0,0,0,0)
            }
            Blend SrcAlpha One
            AlphaTest Greater 0
            ColorMask RGB
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            float4x4 unity_WorldToLight;
            float4 _Texture_ST;
            float4 _Color;
            sampler2D _Texture;
            float4 _Properties;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord0 : TEXCOORD0;
                float4 tangent : TANGENT;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float3 texcoord1 : TEXCOORD1;
                float3 texcoord2 : TEXCOORD2;
                float3 texcoord3 : TEXCOORD3;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1.xyz = normalize(v.tangent.xyz);
                tmpvar_1.w = v.tangent.w;
                float3 tmpvar_2;
                tmpvar_2 = normalize(v.normal);
                float2 tmpvar_3;
                float3 tmpvar_4;
                float2 tmpvar_5;
                tmpvar_5 = ((v.texcoord0.xy * _Texture_ST.xy) + _Texture_ST.zw);
                tmpvar_3 = tmpvar_5;
                float3 tmpvar_6;
                float3 tmpvar_7;
                tmpvar_6 = tmpvar_1.xyz;
                tmpvar_7 = (((tmpvar_2.yzx * tmpvar_1.zxy) - (tmpvar_2.zxy * tmpvar_1.yzx)) * v.tangent.w);
                float3x3 tmpvar_8;
                tmpvar_8[0].x = tmpvar_6.x;
                tmpvar_8[0].y = tmpvar_7.x;
                tmpvar_8[0].z = tmpvar_2.x;
                tmpvar_8[1].x = tmpvar_6.y;
                tmpvar_8[1].y = tmpvar_7.y;
                tmpvar_8[1].z = tmpvar_2.y;
                tmpvar_8[2].x = tmpvar_6.z;
                tmpvar_8[2].y = tmpvar_7.z;
                tmpvar_8[2].z = tmpvar_2.z;
                float3 tmpvar_9;
                tmpvar_9 = mul(tmpvar_8, ((
                mul(unity_WorldToObject, _WorldSpaceLightPos0)
                .xyz * 1.0) - v.vertex.xyz));
                tmpvar_4 = tmpvar_9;
                float4 tmpvar_10;
                tmpvar_10.w = 1.0;
                tmpvar_10.xyz = _WorldSpaceCameraPos;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord0 = tmpvar_3;
                o.texcoord1 = mul(tmpvar_8, ((
                mul(unity_WorldToObject, tmpvar_10)
                .xyz * 1.0) - v.vertex.xyz));
                o.texcoord2 = tmpvar_4;
                o.texcoord3 = mul(unity_WorldToLight, mul(unity_ObjectToWorld, v.vertex)).xyz;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 c_1;
                float3 tmpvar_2;
                tmpvar_2 = i.texcoord1;
                float2 texCoords_3;
                float tmpvar_4;
                float tmpvar_5;
                tmpvar_5 = normalize(tmpvar_2).z;
                tmpvar_4 = (1.0 - tmpvar_5);
                float2 tmpvar_6;
                tmpvar_6.x = (i.texcoord0.x + (_Time.x * _Properties.z));
                tmpvar_6.y = i.texcoord0.y;
                float2 tmpvar_7;
                tmpvar_7 = (tmpvar_6 * _Properties.ww);
                texCoords_3 = tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = (((tex2D (_Texture, texCoords_3).x * _Properties.y) * pow (tmpvar_4, _Properties.x)) * _Color.w);
                float4 c_9;
                c_9.xyz = float3(0.0, 0.0, 0.0);
                float tmpvar_10;
                tmpvar_10 = tmpvar_8;
                c_9.w = tmpvar_10;
                c_1.xyz = c_9.xyz;
                c_1.w = tmpvar_8;
                return c_1;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}