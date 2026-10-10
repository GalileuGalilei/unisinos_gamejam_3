using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace game.Senoidal
{
    public class Wave
    {
        private GraphicsDevice _graphicsDevice;
        private Texture2D _blankTexture; // Textura invisível para servir de "tela de pintura" para o shader
        private Effect _waveEffect;

        // Propriedades físicas da onda
        public WaveParameters Parameters; 
        public float Phase { get; set; } = 0f;
        public float Speed { get; set; } = 2f;

        // Customizações Visuais
        public Color Color { get; set; } = Color.White;
        public float Thickness { get; set; } = 1.5f;
        public bool IsDotted { get; set; } = false;
        public float DotSpacing { get; set; } = 5.0f; // Quanto menor, mais pontilhados por segmento

        public Wave(GraphicsDevice graphicsDevice, Effect waveEffect)
        {
            _graphicsDevice = graphicsDevice;
            _waveEffect = waveEffect;

            // Criamos uma textura 1x1 transparente para podermos esticar 
            // e renderizar o shader sobre a tela toda
            _blankTexture = new Texture2D(_graphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            Phase += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int screenWidth = _graphicsDevice.Viewport.Width;
            int screenHeight = _graphicsDevice.Viewport.Height;
            float centerY = screenHeight / 2f;

            // Passa os parâmetros de posicionamento e formato para o shader
            _waveEffect.Parameters["ScreenWidth"]?.SetValue((float)screenWidth);
            _waveEffect.Parameters["ScreenHeight"]?.SetValue((float)screenHeight);
            _waveEffect.Parameters["CenterY"]?.SetValue(centerY);
            _waveEffect.Parameters["Amplitude"]?.SetValue(Parameters.Amplitude);
            _waveEffect.Parameters["Frequency"]?.SetValue(Parameters.Frequency);
            _waveEffect.Parameters["Phase"]?.SetValue(Phase + Parameters.PhaseOffset);

            // Passa as configurações de estilização
            _waveEffect.Parameters["WaveColor"]?.SetValue(Color.ToVector4());
            _waveEffect.Parameters["Thickness"]?.SetValue(Thickness);
            _waveEffect.Parameters["Dotted"]?.SetValue(IsDotted ? 1.0f : 0.0f);
            _waveEffect.Parameters["DotSpacing"]?.SetValue(DotSpacing);

            // Desenha um retângulo do tamanho exato da tela usando o shader.
            // O shader irá pintar apenas onde a onda senoidal passa, ignorando o resto.
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, _waveEffect);
            spriteBatch.Draw(_blankTexture, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
            spriteBatch.End();
        }
    }
}