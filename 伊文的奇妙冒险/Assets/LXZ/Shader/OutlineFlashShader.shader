Shader "Unlit/OutlineFlashShader"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 0.1
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.0
        _GlowSpread ("Glow Spread", Range(1, 10)) = 3.0
    }
    SubShader {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _GlowIntensity;
            float _GlowSpread;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 计算像素偏移量
                float2 pixelSize = _MainTex_TexelSize.xy * _OutlineWidth;
                
                // 如果当前像素不透明，检查是否是边缘
                if (col.a > 0.5) {
                    // 检查周围像素是否有透明区域
                    bool isEdge = false;
                    
                    // 检查四个主要方向
                    fixed4 up = tex2D(_MainTex, i.uv + float2(0, pixelSize.y));
                    fixed4 down = tex2D(_MainTex, i.uv - float2(0, pixelSize.y));
                    fixed4 left = tex2D(_MainTex, i.uv - float2(pixelSize.x, 0));
                    fixed4 right = tex2D(_MainTex, i.uv + float2(pixelSize.x, 0));
                    
                    if (up.a < 0.5 || down.a < 0.5 || left.a < 0.5 || right.a < 0.5) {
                        isEdge = true;
                    }
                    
                    // 如果是边缘，添加基础轮廓
                    if (isEdge) {
                        col.rgb = lerp(col.rgb, _OutlineColor.rgb, 0.3);
                    }
                    
                    return col;
                }
                
                // 对于透明像素，计算发光效果
                float glow = 0;
                
                // 采样周围像素
                for (int x = -3; x <= 3; x++) {
                    for (int y = -3; y <= 3; y++) {
                        if (x == 0 && y == 0) continue;
                        
                        float2 offset = float2(x, y) * pixelSize;
                        fixed4 sampleCol = tex2D(_MainTex, i.uv + offset);
                        
                        // 如果采样到不透明像素，计算发光
                        if (sampleCol.a > 0.5) {
                            // 计算距离权重
                            float distance = length(float2(x, y));
                            float weight = exp(-distance * distance / (_GlowSpread * 0.5));
                            
                            glow += weight;
                        }
                    }
                }
                
                // 归一化发光强度
                glow = saturate(glow * _GlowIntensity / 20.0); // 20是近似归一化因子
                
                // 应用发光效果
                if (glow > 0) {
                    col.rgb = _OutlineColor.rgb * glow;
                    col.a = glow * _OutlineColor.a;
                }
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}