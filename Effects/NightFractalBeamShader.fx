
sampler2D noiseTexture : register(s1);


float globalTime;
float4 coreColor;    
float4 glowColor;    
float4 flareColor;   
float4x4 uWorldViewProjection;

struct VS_INPUT
{
    float4 Position  : POSITION0;
    float4 Color     : COLOR0;
    float2 TexCoord  : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Position  : POSITION0;
    float4 Color     : COLOR0;
    float2 TexCoord  : TEXCOORD0;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT output;
    output.Position = mul(input.Position, uWorldViewProjection);
    output.Color    = input.Color;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float2 uv = input.TexCoord;

    float2 noiseUV = float2(uv.x * 5.0 + globalTime * 3.0, uv.y * 5.0);
    float noise = tex2D(noiseTexture, noiseUV).r;

    float dist = abs(uv.y - 0.5);


    float core = exp(-dist * dist * 1600.0) * 1.5;

    float glow = exp(-dist * dist * 50.0) * (0.7 + 0.3 * noise);

    float pulse = 0.9 + 0.3 * sin(globalTime * 25.0 + uv.x * 12.0);

    float flare = exp(-uv.x * 6.0) * (1.0 - dist * 1.5);

    float startFade = saturate(uv.x * 10.0);

    float3 rgb = coreColor.rgb * core + glowColor.rgb * glow;
    float alpha = saturate(core + glow) * startFade;

    float4 color = float4(rgb, alpha) * pulse;
    color += flareColor * flare * startFade;

    return saturate(color);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}
