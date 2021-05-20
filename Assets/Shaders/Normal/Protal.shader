Shader "Custom/Normal/Protal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint("Tint",Color) = (1,1,1,1)
        _Direction("Direction",Vector) = (0,0,0,0)
        _Speed("Speed",float) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"  }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 _Direction;
            float _Speed;
            float4 _Tint;

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv + _Direction.xy*_Time.y*_Speed;
                float4 col = tex2D(_MainTex, uv);
                col.a -= col.r;
                col*=_Tint; 
                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
