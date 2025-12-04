sampler2D inputSampler : register(s0);

float2 Center   : register(c0);
float  Radius   : register(c1);
float  Strength : register(c2);
float  Aspect   : register(c3);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 pos = uv - Center;
    
    pos.x *= Aspect;

    float dist = length(pos);


    float mask = 1.0 - smoothstep(Radius - 0.05, Radius, dist);

    if (mask <= 0.001) return tex2D(inputSampler, uv);

    float t = dist / Radius;
    
    t = max(0.0001, t); 

    float t_new = pow(t, Strength); 
    
    float scale = t_new / t;
    
    float2 newPos = pos * scale;
    newPos.x /= Aspect;
    
    float2 sampleUv = Center + newPos;

    return lerp(tex2D(inputSampler, uv), tex2D(inputSampler, sampleUv), mask);
}