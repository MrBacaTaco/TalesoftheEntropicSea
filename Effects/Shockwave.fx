
float2 uTargetPosition;    
float  uProgress;          
float  uOpacity;           
float3 uColor;             
float2 uScreenResolution; 
float2 uScreenPosition;    

sampler2D uImage0 : register(s0);


struct VS_OUTPUT {
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};


float4 PS(VS_OUTPUT input) : COLOR0 {
    float2 coords = input.TexCoord; 

    float2 targetUV = (uTargetPosition - uScreenPosition) / uScreenResolution;


    float2 diff = coords - targetUV;
    float dist2 = dot(diff, diff);


    float ripple = dist2 * uColor.y * 3.14159 - uProgress * uColor.z;

    if (ripple < 0 && ripple > uColor.x * -2 * 3.14159) {
        ripple = saturate(sin(ripple));
    } else {
        ripple = 0;
    }


    float2 sampleCoords = coords + ripple * uOpacity * diff * 0.05;


    sampleCoords = clamp(sampleCoords, 0.0, 1.0);

    return tex2D(uImage0, sampleCoords);
}

technique Shockwave {
    pass P0 {
        PixelShader = compile ps_2_0 PS();
    }
}
