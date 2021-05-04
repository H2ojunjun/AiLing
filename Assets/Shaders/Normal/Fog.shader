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
            float4 _Dir;
            float4 _Offset;
            float _OriginWidth;
            float _OriginHeight;

            v2f vert(appdata_full v)
            {
                v2f o;
                float xTransition;
                float yTransition;
                float zTransition;
                float uvChangeValueX = 0;
                float uvChangeValueY = 0;
                if(v.vertex.x*_Dir.x > 0){
                    xTransition = 1;
                    if(v.vertex.x<0){
                        uvChangeValueX = -_Offset.x/_OriginWidth;
                    }
                    else
                        uvChangeValueX = _Offset.x/_OriginWidth;
                }
                else{
                    xTransition = 0;
                }
                if(v.vertex.y*_Dir.y > 0){
                    yTransition = 1;
                    if(v.vertex.y<0){
                        uvChangeValueY = -_Offset.y/_OriginHeight;
                    }
                    else
                        uvChangeValueY = _Offset.y/_OriginHeight;
                }
                else{
                    yTransition = 0;
                }               
                if(v.vertex.z*_Dir.z > 0){
                    zTransition = 1;
                }
                else{
                    zTransition = 0;
                }                   
                v.vertex += float4(_Offset.x*xTransition,_Offset.y*yTransition,_Offset.z*zTransition,0);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.x *= 1 + uvChangeValueX;
                o.uv.y *= 1 + uvChangeValueY;
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
 
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv + fixed2(_ScrollDirX, _ScrollDirY) * _Speed * _Time.x;
                fixed4 col = tex2D(_MainTex, uv) * _Color * i.vertCol;
                col.a *= 0.75 - distance(uv,float2(0.5,0.5));
                col.a *= 1 - (abs(i.pos.z) * _Distance);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
