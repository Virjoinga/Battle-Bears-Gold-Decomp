Shader "Hidden/GlobalFog" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "black" {}
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
            float4x4 _FrustumCornersWS;
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float _GlobalDensity;
            float4 _FogColor;
            float4 _StartDistance;
            float4 _Y;
            float4 _CameraWS;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1 = v.vertex;
                float2 tmpvar_2;
                tmpvar_2 = v.texcoord0.xy;
                float4 tmpvar_3;
                tmpvar_3.xyw = tmpvar_1.xyw;
                float index_4;
                float2 tmpvar_5;
                float2 tmpvar_6;
                float4 tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = tmpvar_1.z;
                index_4 = tmpvar_8;
                tmpvar_3.z = 0.1;
                tmpvar_5 = tmpvar_2;
                tmpvar_6 = tmpvar_2;
                int i_9;
                i_9 = int(index_4);
                float4 tmpvar_10;
                float4 v_11;
                v_11.x = _FrustumCornersWS[0][i_9];
                v_11.y = _FrustumCornersWS[1][i_9];
                v_11.z = _FrustumCornersWS[2][i_9];
                v_11.w = _FrustumCornersWS[3][i_9];
                tmpvar_10 = v_11;
                tmpvar_7.xyz = tmpvar_10.xyz;
                tmpvar_7.w = index_4;
                o.vertex = UnityObjectToClipPos(tmpvar_3);
                o.texcoord0 = tmpvar_5;
                o.texcoord1 = tmpvar_6;
                o.texcoord2 = tmpvar_7;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = ((1.0/((
                (_ZBufferParams.x * tex2D (_CameraDepthTexture, i.texcoord1).x)
                + _ZBufferParams.y))) * i.texcoord2);
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_MainTex, i.texcoord0);
                float tmpvar_4;
                tmpvar_4 = max (0.0, ((
                (_CameraWS + tmpvar_2)
                .y - _Y.x) * _Y.y));
                float4 tmpvar_5;
                tmpvar_5 = lerp (tmpvar_3, _FogColor, (((1.0 - 
                exp((-(_GlobalDensity) * (clamp (
                ((sqrt(dot (tmpvar_2.xyz, tmpvar_2.xyz)) * _StartDistance.x) - 1.0)
                , 0.0, 1.0) * _StartDistance.y)))
                ) * exp(
                -((tmpvar_4 * tmpvar_4))
                ))));
                tmpvar_1 = tmpvar_5;
                return tmpvar_1;
            }
            ENDCG
        }
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
            float4x4 _FrustumCornersWS;
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _FogColor;
            float4 _Y;
            float4 _CameraWS;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1 = v.vertex;
                float2 tmpvar_2;
                tmpvar_2 = v.texcoord0.xy;
                float4 tmpvar_3;
                tmpvar_3.xyw = tmpvar_1.xyw;
                float index_4;
                float2 tmpvar_5;
                float2 tmpvar_6;
                float4 tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = tmpvar_1.z;
                index_4 = tmpvar_8;
                tmpvar_3.z = 0.1;
                tmpvar_5 = tmpvar_2;
                tmpvar_6 = tmpvar_2;
                int i_9;
                i_9 = int(index_4);
                float4 tmpvar_10;
                float4 v_11;
                v_11.x = _FrustumCornersWS[0][i_9];
                v_11.y = _FrustumCornersWS[1][i_9];
                v_11.z = _FrustumCornersWS[2][i_9];
                v_11.w = _FrustumCornersWS[3][i_9];
                tmpvar_10 = v_11;
                tmpvar_7.xyz = tmpvar_10.xyz;
                tmpvar_7.w = index_4;
                o.vertex = UnityObjectToClipPos(tmpvar_3);
                o.texcoord0 = tmpvar_5;
                o.texcoord1 = tmpvar_6;
                o.texcoord2 = tmpvar_7;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                float tmpvar_2;
                tmpvar_2 = max (0.0, ((
                (_CameraWS + ((1.0/((
                (_ZBufferParams.x * tex2D (_CameraDepthTexture, i.texcoord1).x)
                + _ZBufferParams.y))) * i.texcoord2))
                .y - _Y.x) * _Y.y));
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_MainTex, i.texcoord0);
                float4 tmpvar_4;
                tmpvar_4 = lerp (tmpvar_3, _FogColor, exp(-(
                (tmpvar_2 * tmpvar_2)
                )));
                tmpvar_1 = tmpvar_4;
                return tmpvar_1;
            }
            ENDCG
        }
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
            float4x4 _FrustumCornersWS;
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float _GlobalDensity;
            float4 _FogColor;
            float4 _StartDistance;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1 = v.vertex;
                float2 tmpvar_2;
                tmpvar_2 = v.texcoord0.xy;
                float4 tmpvar_3;
                tmpvar_3.xyw = tmpvar_1.xyw;
                float index_4;
                float2 tmpvar_5;
                float2 tmpvar_6;
                float4 tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = tmpvar_1.z;
                index_4 = tmpvar_8;
                tmpvar_3.z = 0.1;
                tmpvar_5 = tmpvar_2;
                tmpvar_6 = tmpvar_2;
                int i_9;
                i_9 = int(index_4);
                float4 tmpvar_10;
                float4 v_11;
                v_11.x = _FrustumCornersWS[0][i_9];
                v_11.y = _FrustumCornersWS[1][i_9];
                v_11.z = _FrustumCornersWS[2][i_9];
                v_11.w = _FrustumCornersWS[3][i_9];
                tmpvar_10 = v_11;
                tmpvar_7.xyz = tmpvar_10.xyz;
                tmpvar_7.w = index_4;
                o.vertex = UnityObjectToClipPos(tmpvar_3);
                o.texcoord0 = tmpvar_5;
                o.texcoord1 = tmpvar_6;
                o.texcoord2 = tmpvar_7;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = ((1.0/((
                (_ZBufferParams.x * tex2D (_CameraDepthTexture, i.texcoord1).x)
                + _ZBufferParams.y))) * i.texcoord2);
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_MainTex, i.texcoord0);
                float4 tmpvar_4;
                tmpvar_4 = lerp (_FogColor, tmpvar_3, (exp((
                -(_GlobalDensity)
                * 
                (clamp (((
                sqrt(dot (tmpvar_2, tmpvar_2))
                * _StartDistance.x) - 1.0), 0.0, 1.0) * _StartDistance.y)
                ))));
                tmpvar_1 = tmpvar_4;
                return tmpvar_1;
            }
            ENDCG
        }
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
            float4x4 _FrustumCornersWS;
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float _GlobalDensity;
            float4 _FogColor;
            float4 _StartDistance;
            float4 _Y;
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };
            struct v2f
            {
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                float4 vertex : POSITION;
            };
            v2f vert(appdata_t v)
            {
                v2f o;
                float4 tmpvar_1;
                tmpvar_1 = v.vertex;
                float2 tmpvar_2;
                tmpvar_2 = v.texcoord0.xy;
                float4 tmpvar_3;
                tmpvar_3.xyw = tmpvar_1.xyw;
                float index_4;
                float2 tmpvar_5;
                float2 tmpvar_6;
                float4 tmpvar_7;
                float tmpvar_8;
                tmpvar_8 = tmpvar_1.z;
                index_4 = tmpvar_8;
                tmpvar_3.z = 0.1;
                tmpvar_5 = tmpvar_2;
                tmpvar_6 = tmpvar_2;
                int i_9;
                i_9 = int(index_4);
                float4 tmpvar_10;
                float4 v_11;
                v_11.x = _FrustumCornersWS[0][i_9];
                v_11.y = _FrustumCornersWS[1][i_9];
                v_11.z = _FrustumCornersWS[2][i_9];
                v_11.w = _FrustumCornersWS[3][i_9];
                tmpvar_10 = v_11;
                tmpvar_7.xyz = tmpvar_10.xyz;
                tmpvar_7.w = index_4;
                o.vertex = UnityObjectToClipPos(tmpvar_3);
                o.texcoord0 = tmpvar_5;
                o.texcoord1 = tmpvar_6;
                o.texcoord2 = tmpvar_7;
                return o;
            }
            float4 frag(v2f i) : SV_TARGET
            {
                float4 tmpvar_1;
                float4 tmpvar_2;
                tmpvar_2 = ((1.0/((
                (_ZBufferParams.x * tex2D (_CameraDepthTexture, i.texcoord1).x)
                + _ZBufferParams.y))) * i.texcoord2);
                float4 tmpvar_3;
                tmpvar_3 = tex2D (_MainTex, i.texcoord0);
                float tmpvar_4;
                tmpvar_4 = max (0.0, ((tmpvar_2.y - _Y.x) * _Y.y));
                float4 tmpvar_5;
                tmpvar_5 = lerp (tmpvar_3, _FogColor, (((1.0 - 
                exp((-(_GlobalDensity) * (clamp (
                ((sqrt(dot (tmpvar_2.xyz, tmpvar_2.xyz)) * _StartDistance.x) - 1.0)
                , 0.0, 1.0) * _StartDistance.y)))
                ) * exp(
                -((tmpvar_4 * tmpvar_4))
                ))));
                tmpvar_1 = tmpvar_5;
                return tmpvar_1;
            }
            ENDCG
        }
    }
    Fallback Off
}