
float4x4 uWorldViewProjection;
float Time;

struct VS_INPUT {
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VS_OUTPUT {
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT VS(VS_INPUT input) {
    VS_OUTPUT output;
    output.Position = mul(input.Position, uWorldViewProjection);
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS(VS_OUTPUT input) : COLOR0 {
    float2 uv = input.TexCoord;
    float dist = distance(uv, float2(0.5, 0.5));

    float glow = 1.0 - smoothstep(0.2, 0.6, dist);

    glow *= 0.7 + 0.3 * sin(Time * 5);

    return float4(0.3, 0.6, 1.0, glow); 
}

technique Technique1 {
    pass P0 {
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 PS();
    }
}
