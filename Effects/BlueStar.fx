
float Time;                     
float4x4 uWorldViewProjection;  
float Intensity;                
sampler2D NoiseTex : register(s0); 

struct VS_INPUT
{
    float4 Position  : POSITION0;
    float2 TexCoord  : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Position  : POSITION0;
    float2 TexCoord  : TEXCOORD0;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT output;
    output.Position = mul(input.Position, uWorldViewProjection);
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float2 uv = input.TexCoord * 2.0 - 1.0; 
    float dist = length(uv);

    float core = 1.0 - smoothstep(0.0, 0.25, dist);

    float2 noiseUV = uv * 3.0 + float2(Time * 0.2, 0.0);
    float noise = tex2D(NoiseTex, noiseUV * 0.5).r;

    float corona = (1.0 - smoothstep(0.2, 1.0, dist)) * (0.5 + 0.5 * noise);

    float3 coreColor   = float3(1.0, 1.0, 1.0) * core * 2.0;
    float3 coronaColor = float3(0.2, 0.5, 1.0) * corona;

    float3 finalColor = coreColor + coronaColor;

    float pulse = 0.85 + 0.15 * sin(Time * 2.0);
    finalColor *= pulse * Intensity;

    float alpha = saturate(core + corona * 0.5);

    return float4(finalColor, alpha);
}

technique Technique1
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}
