sampler2D TextureSampler : register(s0);

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR
{
    float4 color = tex2D(TextureSampler, texCoord);
    color.rgb *= float3(0.4, 0.6, 1.1); // dark blue tint
    return color;
}

technique SkyAbyssPass
{
    pass P0
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}
