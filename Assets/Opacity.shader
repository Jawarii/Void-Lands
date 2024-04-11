Shader "Custom/VerticalGradientOpacity"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _OpacityStart("Start Opacity", Range(0, 1)) = 1.0
        _OpacityEnd("End Opacity", Range(0, 1)) = 0.0
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
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
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float2 texcoord : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 screenPos : TEXCOORD1;
                };

                sampler2D _MainTex;
                float _OpacityStart;
                float _OpacityEnd;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = v.texcoord;
                    o.screenPos = ComputeScreenPos(o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float lerpFactor = i.screenPos.y / i.screenPos.w;
                    float opacity = lerp(_OpacityStart, _OpacityEnd, lerpFactor);
                    fixed4 texColor = tex2D(_MainTex, i.texcoord);
                    fixed4 col = texColor * opacity;
                    return col;
                }
                ENDCG
            }
        }
}
