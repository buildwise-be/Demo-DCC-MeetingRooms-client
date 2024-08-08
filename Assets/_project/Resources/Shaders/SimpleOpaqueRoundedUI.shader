Shader"Custom/SimpleOpaqueRoundedUI" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Range(0, 0.5)) = 0.1
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Transparent+1" }

        Pass {
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

fixed4 _Color;
float _Radius;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    float2 uv = i.uv;
    float2 center = float2(0.5, 0.5);
    float2 dist = abs(uv - center);
    float cornerDist = max(dist.x - (0.5 - _Radius), dist.y - (0.5 - _Radius));
    float alpha = smoothstep(0.01, 0.0, cornerDist);
    return fixed4(_Color.rgb, _Color.a * alpha);
}
            ENDCG
        }
    }
}
