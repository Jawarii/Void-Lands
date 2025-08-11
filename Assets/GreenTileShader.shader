Shader "Custom/GreenTileShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _GreenColor("Green Color", Color) = (0,1,0,1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _GreenColor;

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 tex = tex2D(_MainTex, i.uv);

                // Check if the color is nearly black
                if (dot(tex.rgb, float3(1.0, 1.0, 1.0)) < 0.1) // Use dot product to check darkness
                {
                    tex.rgb = _GreenColor.rgb; // Replace black with green
                }

                return tex;
            }
            ENDCG
        }
        }
}
