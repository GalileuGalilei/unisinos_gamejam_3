#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
float BandCount;
float Opacity;
float Phase;
float4 BaseColor;
float ErrorLevel;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	output.Position = mul(input.Position, WorldViewProjection);
	output.TexCoord = input.TexCoord;
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TexCoord;

    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
    {
        return float4(0, 0, 0, 0.0); 
    }

    float noiseValue = random(float2(uv.y, Phase * 100.0));
    
    noiseValue = (noiseValue * 2.0) - 1.0;
    
    float distortedX = uv.x + (noiseValue * ErrorLevel * 0.2);

    float bandIndex = floor((distortedX * BandCount) - (Phase / 3.14159265));
    float isBand = fmod(bandIndex, 2.0f);

    float finalAlpha = isBand * Opacity;
    float r = BaseColor.r * isBand * finalAlpha;
    float g = BaseColor.g * isBand * finalAlpha;
    float b = BaseColor.b * isBand * finalAlpha;

    return float4(r, g, b, finalAlpha);
}

technique TubeTVSnow
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}