
float globalTime;
float4 baseColor; 

sampler2D particleTex : register(s0);

float4 PS(float2 uv : TEXCOORD0) : COLOR0
{
    float2 centered = uv - 0.5;
    float dist = length(centered) * 2.0;
    float alpha = saturate(1.0 - dist);

    float flicker = 0.7 + 0.3 * sin(globalTime * 8.0 + uv.x * 10.0);

    float4 tex = tex2D(particleTex, uv);
    return baseColor * tex * alpha * flicker;
}

technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_2_0 PS();
    }
}
