float appearProgress;
float4 starColor;
sampler2D starTex : register(s0);

float4 PS(float2 uv : TEXCOORD0) : COLOR0
{
    float4 tex = tex2D(starTex, uv);

    tex.a *= appearProgress;

    return tex * starColor;
}

technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_2_0 PS();
    }
}
