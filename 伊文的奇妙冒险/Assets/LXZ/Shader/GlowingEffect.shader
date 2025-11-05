Shader "Unlit/GlowingEffect"
{
    Properties {
        _Color ("主颜色", Color) = (1, 1, 1, 1)
        _MainTex ("纹理", 2D) = "white" {}
        _Glossiness ("光滑度", Range(0, 1)) = 0.5
        _Metallic ("金属度", Range(0, 1)) = 0.0

        // 自发光属性
        _EmissionColor ("自发光颜色", Color) = (0, 0, 0, 1)
        _EmissionMap ("自发光纹理", 2D) = "white" {}
        _EmissionStrength ("自发光强度", Range(0, 5)) = 1.0

        // 边缘发光属性
        _RimColor ("边缘发光颜色", Color) = (1, 1, 1, 1)
        _RimPower ("边缘发光强度", Range(0.1, 10.0)) = 2.0
        _RimThreshold ("边缘发光范围", Range(0, 1)) = 0.5
    }

    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // 使用表面着色器，并启用高光反射
        #pragma surface surf Standard fullforwardshadows

        // 编译目标为3.0，支持更多特性
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _EmissionMap;

        struct Input {
            float2 uv_MainTex;
            float2 uv_EmissionMap;
            float3 viewDir; // 用于计算边缘发光
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;
        half _EmissionStrength;
        fixed4 _RimColor;
        half _RimPower;
        half _RimThreshold;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // 采样主纹理并应用主颜色
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // 设置金属度和光滑度
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // 计算自发光
            fixed4 emission = tex2D(_EmissionMap, IN.uv_EmissionMap) * _EmissionColor * _EmissionStrength;

            // 计算边缘发光
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            // 应用阈值，控制边缘发光范围
            rim = step(_RimThreshold, rim) * rim;
            fixed4 rimLight = _RimColor * pow(rim, _RimPower);

            // 将自发光和边缘发光叠加
            o.Emission = emission.rgb + rimLight.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
