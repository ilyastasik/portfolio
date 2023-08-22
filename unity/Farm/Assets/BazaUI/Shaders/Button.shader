Shader "Baza/UI/Button"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData] _Color ("Color", Color) = (0, 0, 0, 1)
        [PerRendererData] _uv ("UV", Vector) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Always
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float4, _uv)
            UNITY_INSTANCING_BUFFER_END(Props)

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float4 uv = UNITY_ACCESS_INSTANCED_PROP(Props, _uv);
                fixed4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                fixed4 textureColor = tex2D(_MainTex, lerp(uv.xy,uv.zw,i.uv.xy));
                //color.a = textureColor.r;
                //return fixed4(i.uv.xy,0,1);
                return color * textureColor.a;
                //return float4(textureColor.a,0,0,1);
            }
            ENDCG
        }
    }
}
