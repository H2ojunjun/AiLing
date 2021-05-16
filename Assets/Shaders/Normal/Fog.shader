Shader "Custom/Fog"
{
    Properties
    {
        [Header(Textures and color)]
        [Space]
        _MainTex ("Fog texture", 2D) = "white" {}
        [NoScaleOffset] _Mask ("Mask", 2D) = "white" {}
        _Color ("Color", color) = (1., 1., 1., 1.)
        [Space(10)]
 
        [Header(Behaviour)]
        [Space]
        _ScrollDirX ("Scroll along X", Range(-1., 1.)) = 1.
        _ScrollDirY ("Scroll along Y", Range(-1., 1.)) = 1.
        _Speed ("Speed", float) = 1.
        _Distance ("Fading distance", Range(1., 10.)) = 1.
        _AlphaChangeRatio("AlphaChangeRatio",Range(0,2))=1
    }
 
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 vertCol : COLOR0;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _uvChangeValueX;
            float _uvChangeValueY;

            v2f vert(appdata_full v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.x *= 1 + _uvChangeValueX;
                o.uv.y *= 1 + _uvChangeValueY;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv2 = v.texcoord;
                o.vertCol = v.color;
                return o;
            }
 
            float _Distance;
            sampler2D _Mask;
            float _Speed;
            fixed _ScrollDirX;
            fixed _ScrollDirY;
            fixed4 _Color;
            float _AlphaChangeRatio;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv + fixed2(_ScrollDirX, _ScrollDirY) * _Speed * _Time.x;
                fixed4 col = tex2D(_MainTex, uv) * _Color * i.vertCol;
                col.a *= min(1,tex2D(_Mask, i.uv2).r*_AlphaChangeRatio);
                col.a *= 1 - (abs(i.pos.z) * _Distance);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
