Shader "UI/DottedLine"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _DotSpacing ("Dot Spacing", Float) = 0.1
        _DotSize ("Dot Size", Float) = 0.05
        _MainTex ("Sprite Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ColorMask RGB
        Cull Off

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

            float4 _Color;
            float _DotSpacing;
            float _DotSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate the distance from the center of the dot
                float d = distance(frac(i.uv.x / _DotSpacing), 0.5);

                // Determine if the pixel is within the dot
                float alpha = smoothstep(_DotSize, _DotSize + 0.01, d);

                // Return the color with the calculated alpha
                return float4(_Color.rgb, 1.0 - alpha);
            }
            ENDCG
        }
    }
}
