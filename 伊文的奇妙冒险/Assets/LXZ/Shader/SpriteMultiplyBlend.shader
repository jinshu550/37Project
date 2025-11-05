Shader "Unlit/SpriteMultiplyBlend"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _BlendStrength ("叠加强度", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        // 关键混合模式：透明区域完全不影响背景，非透明区域正常混合
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            fixed4 _Color;
            float _BlendStrength;
            sampler2D _MainTex;
            float4 _MainTex_ST; 
            sampler2D _CameraOpaqueTexture; // 背景纹理（采样当前相机渲染的不透明层）

            v2f vert (appdata_t IN)
            {
                v2f OUT;
                // 顶点从模型空间转换到裁剪空间
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                // 纹理坐标应用缩放偏移
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                // 传递tint颜色
                OUT.color = IN.color * _Color;
                // 计算背景采样的屏幕坐标
                OUT.screenPos = ComputeScreenPos(OUT.vertex);
                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                // 采样精灵纹理颜色
                fixed4 spriteCol = tex2D(_MainTex, IN.texcoord);
                // 叠加tint颜色
                spriteCol.rgb *= IN.color.rgb;
                spriteCol.a *= IN.color.a; // 若tint颜色有Alpha，也一并叠加

                // 2. 透明区域判断：Alpha≤0.001视为完全透明，直接返回透明色（不影响背景）
                if (spriteCol.a <= 0.001)
                {
                    return fixed4(0, 0, 0, 0); // RGB=0不影响背景（因Alpha=0，混合后背景不变）
                }

                // 非透明区域：采样当前像素的背景色
                // 屏幕坐标转换为UV（除以w分量矫正透视）
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                // 采样背景颜色
                fixed3 bgCol = tex2D(_CameraOpaqueTexture, screenUV).rgb;

                // 计算正片叠底颜色：精灵RGB × 背景RGB（标准正片叠底公式）
                fixed3 multiplyColor = spriteCol.rgb * bgCol;

                // 根据叠加强度混合：原始精灵色 ↔ 正片叠底色
                // _BlendStrength=0：完全显示原始精灵色（仅叠加tint）
                // _BlendStrength=1：完全显示正片叠底色（精灵×背景）
                fixed3 finalRGB = lerp(spriteCol.rgb, multiplyColor, _BlendStrength);

                // 输出最终颜色
                return fixed4(finalRGB, spriteCol.a);
            }
            ENDCG
        }
    }
    // 降级方案：若设备不支持， fallback到Unity默认精灵Shader
    FallBack "Sprites/Default"
}