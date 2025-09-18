Shader "Custom/GoldReflectiveToonOutline_Bright"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" {}
        _ReflectColorTex ("Reflect Color", 2D) = "white" {}
        _ReflectMap ("Reflection Cubemap", CUBE) = "" {}
        _Color ("Base Color", Color) = (1.2,1.2,1.2,1)  // slightly brighter
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline Width", Range(0.01,0.3)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ReflectColorTex;
        samplerCUBE _ReflectMap;
        samplerCUBE _ToonShade;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;   // World-space view direction
            float3 worldNormal; // World-space normal
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Base texture and color
            fixed4 baseCol = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Toon shading via cubemap
            fixed3 toon = texCUBE(_ToonShade, normalize(IN.worldNormal)).rgb;

            // Reflection
            float3 reflDir = reflect(-normalize(IN.viewDir), normalize(IN.worldNormal));
            fixed3 reflCol = texCUBE(_ReflectMap, reflDir).rgb;
            fixed3 reflColorTex = tex2D(_ReflectColorTex, IN.uv_MainTex).rgb;

            // Combine everything with slightly stronger reflection
            o.Albedo = baseCol.rgb * toon;
            o.Emission = reflCol * reflColorTex * 0.8; // brighter reflection
            o.Alpha = baseCol.a;
        }
        ENDCG

        // Outline pass
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }

            Cull Front
            ZWrite On
            ZTest LEqual

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float _Outline;
            uniform float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                v.vertex.xyz += norm * _Outline;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
