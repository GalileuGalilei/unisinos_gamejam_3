#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Parâmetros da Onda
float ScreenWidth;
float ScreenHeight;
float CenterY;
float Amplitude;
float Frequency;
float Phase;

// Customização Visual
float4 WaveColor;
float Thickness;
float Dotted;      // 0.0 para sólida, 1.0 para pontilhada
float DotSpacing;  // Tamanho do padrão de pontos (ex: 15.0)

sampler TextureSampler : register(s0);

float4 SpritePixelShader(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    // Recupera a posição do pixel atual na tela
    float pixelX = texCoord.x * ScreenWidth;
    float pixelY = texCoord.y * ScreenHeight;

    // Calcula a posição Y correspondente na curva senoidal para este X
    // 2 * PI = 6.2831853
    float waveY = Amplitude * sin((6.2831853 * Frequency * pixelX) + Phase) + CenterY;

    // Distância vertical do pixel atual até a onda
    float dist = abs(pixelY - waveY);

    // Determina a intensidade da cor com base na distância (suavização/anti-aliasing)
    // Se estiver dentro da metade da espessura, a intensidade é 1.0, caindo gradativamente
    float alpha = smoothstep(Thickness, Thickness - 1.5, dist);

    // Se for pontilhada, descarta pixels baseados em uma onda quadrada no eixo X
    if (Dotted > 0.5)
    {
        // Se o resto da divisão do X pelo espaçamento for menor que a metade, "esconde" o pixel
        // Adicionamos a Fase para os pontilhados andarem junto com o movimento da onda
        float dotPattern = sin((pixelX - Phase * 50.0) / DotSpacing);
        if (dotPattern < 0.0)
        {
            alpha = 0.0;
        }
    }

    // Retorna a cor da onda multiplicada pela intensidade do traço
    return WaveColor * alpha;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
};
