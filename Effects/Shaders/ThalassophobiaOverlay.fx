sampler2D TextureSampler : register(s0);
float opacity;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TextureSampler, coords);
    float2 screenCenter = float2(0.5, 0.5);
    float dist = distance(coords, screenCenter);

    // Simple fade around center region
    float mask = smoothstep(0.35, 0.5, dist);

    float3 tint = float3(0.2, 0.4, 0.8); // Blue tint
    float strength = mask * opacity;

    color.rgb = lerp(color.rgb, tint, strength);
    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
