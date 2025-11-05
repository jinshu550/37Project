Shader "Unlit/CardBorder"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineSize("OutlineSize",Range(1.,50.))=25.//内层
        _OutlineSize2("OutlineSize2",Range(1.,50.))=50.//外层
        _BorderColor("Border Color", Color) = (1, 0, 0, 1) // 新增边框颜色属性
        _BorderColor2("Border Color2", Color) = (0, 1, 0, 0.7) // 第二个边框颜色，与第一个不同
    }
    SubShader
    {
        
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest On//关闭深度写入

        Pass
        {
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _OutlineSize;
            fixed4 _BorderColor; // 声明边框颜色变量

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                          
                float x=sqrt(_Time.y % 1)*_OutlineSize;

                float up=tex2D(_MainTex,i.uv+fixed2(0,1)*_MainTex_TexelSize.xy*x).a;
                float down=tex2D(_MainTex,i.uv+fixed2(0,-1)*_MainTex_TexelSize.xy*x).a;
                float left=tex2D(_MainTex,i.uv+fixed2(-1,0)*_MainTex_TexelSize.xy*x).a;
                float right=tex2D(_MainTex,i.uv+fixed2(1,0)*_MainTex_TexelSize.xy*x).a;

                float up_left=tex2D(_MainTex,i.uv+fixed2(-1,1)*_MainTex_TexelSize.xy*x).a;
                float up_right=tex2D(_MainTex,i.uv+fixed2(1,1)*_MainTex_TexelSize.xy*x).a;
                float down_left=tex2D(_MainTex,i.uv+fixed2(-1,-1)*_MainTex_TexelSize.xy*x).a;
                float down_right=tex2D(_MainTex,i.uv+fixed2(1,-1)*_MainTex_TexelSize.xy*x).a;
      

                if(col.a<0.1&&(up>0.1||down>0.1||left>0.1||right>0.1
                    ||up_left>0.1||up_right>0.1||down_left>0.1||down_right>0.1))
                    return fixed4(_BorderColor.r, _BorderColor.g, _BorderColor.b, lerp(0.8, 0.1, _Time.y % 1)); // 使用边框颜色，保留原透明度
                else if(col.a>0.1)
                {
                    return col;
                }
                else
                {
                    return col*0;
                }

                return col;
            }
            ENDCG
        }
        Pass
        {
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _OutlineSize2;
            fixed4 _BorderColor2; 


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                          
                float x = sqrt((_Time.y + 0.5) % 1) * _OutlineSize2;

                float up = tex2D(_MainTex, i.uv + fixed2(0, 1) * _MainTex_TexelSize.xy * x).a;
                float down = tex2D(_MainTex, i.uv + fixed2(0, -1) * _MainTex_TexelSize.xy * x).a;
                float left = tex2D(_MainTex, i.uv + fixed2(-1, 0) * _MainTex_TexelSize.xy * x).a;
                float right = tex2D(_MainTex, i.uv + fixed2(1, 0) * _MainTex_TexelSize.xy * x).a;

                float up_left = tex2D(_MainTex, i.uv + fixed2(-1, 1) * _MainTex_TexelSize.xy * x).a;
                float up_right = tex2D(_MainTex, i.uv + fixed2(1, 1) * _MainTex_TexelSize.xy * x).a;
                float down_left = tex2D(_MainTex, i.uv + fixed2(-1, -1) * _MainTex_TexelSize.xy * x).a;
                float down_right = tex2D(_MainTex, i.uv + fixed2(1, -1) * _MainTex_TexelSize.xy * x).a;
      

                if(col.a < 0.1 && (up > 0.1 || down > 0.1 || left > 0.1 || right > 0.1
                    || up_left > 0.1 || up_right > 0.1 || down_left > 0.1 || down_right > 0.1))
                    return fixed4(_BorderColor2.r, _BorderColor2.g, _BorderColor2.b, lerp(0.8, 0.1, (_Time.y + 0.5) % 1)); 
                else if(col.a > 0.1)
                {
                    return col;
                }
                else
                {
                    return col * 0;
                }

                return col; 
            }
            ENDCG
        }
    }
}