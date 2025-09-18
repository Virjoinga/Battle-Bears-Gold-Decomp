Shader "CPX_Custom/Mobile/Lightmapped + Realtime + Shadows"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // ---------- BASE PASS (Baked GI + Sun Light + Shadows) ----------
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1; // lightmap UV
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvLM : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float4 pos : SV_POSITION;
                SHADOW_COORDS(3) // store shadow coords
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv0, _MainTex);
                o.uvLM = v.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                TRANSFER_SHADOW(o); // pass shadow data
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // baked GI
                fixed3 baked = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM));

                // main directional light
                fixed3 n = normalize(i.worldNormal);
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                // apply shadow attenuation
                fixed shadow = SHADOW_ATTENUATION(i);

                fixed3 realtime = _LightColor0.rgb * saturate(dot(n, lightDir)) * shadow;

                col.rgb *= baked + realtime;
                return col;
            }
            ENDCG
        }

        // ---------- ADD PASS (Point/Spot Lights + Shadows) ----------
        Pass
        {
            Tags { "LightMode"="ForwardAdd" }
            Blend One One // additive
            CGPROGRAM
            #pragma vertex vertAdd
            #pragma fragment fragAdd
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 pos : SV_POSITION;
                SHADOW_COORDS(3)
            };

            v2f vertAdd(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv0, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 fragAdd(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed3 n = normalize(i.worldNormal);

                // light dir for point/spot/directional
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos * _WorldSpaceLightPos0.w);

                // diffuse
                fixed3 diff = _LightColor0.rgb * saturate(dot(n, lightDir));

                // apply shadow attenuation
                fixed shadow = SHADOW_ATTENUATION(i);

                return fixed4(col.rgb * diff * shadow, 0);
            }
            ENDCG
        }
    }
}
