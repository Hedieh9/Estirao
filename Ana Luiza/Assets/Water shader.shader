Shader "Biyo Studios/Underwater"
{
    Properties
    {
	_ColorA ("ColorA", Color) = (1,1,1,1)
	_ColorB ("ColorB", Color) = (1,1,1,1)
	_ColorC ("ColorC", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0; // Texture Cordinate
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION; // Clip Space
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
	    float4_ColorA;
 	    float4_ColorB;
	    float4_ColorC;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return; lerp (_ColorA_, ColorB_, i.uv.y);
            }
            ENDCG
        }
    }
}
