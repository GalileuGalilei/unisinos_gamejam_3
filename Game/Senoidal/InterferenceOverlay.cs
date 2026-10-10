using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace game.Senoidal
{
    public class InterferenceOverlay
    {
        private GraphicsDevice _graphicsDevice;
        private Effect _interferenceShader;
        private Texture2D _blankTexture;

        public Wave TargetWave { get; set; }
        
        public float Opacity { get; set; } = 0.015f;
        public Color BaseColor { get; set; } = Color.White;
        public float ErrorLevel { get; set; } = 0f;

        public InterferenceOverlay(GraphicsDevice graphicsDevice, Effect interferenceShader)
        {
            _graphicsDevice = graphicsDevice;
            _interferenceShader = interferenceShader;
            
            _blankTexture = new Texture2D(_graphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (TargetWave == null) return;

            int width = _graphicsDevice.Viewport.Width;
            int height = _graphicsDevice.Viewport.Height;
            Rectangle viewportRect = new Rectangle(0, 0, width, height);

            float bandCount = width * TargetWave.Parameters.Frequency * 2.0f;
            float totalPhase = TargetWave.Phase + TargetWave.Parameters.PhaseOffset;

            _interferenceShader.Parameters["BandCount"]?.SetValue(bandCount);
            _interferenceShader.Parameters["Phase"]?.SetValue(-totalPhase);
            _interferenceShader.Parameters["Opacity"]?.SetValue(Opacity);
            _interferenceShader.Parameters["ErrorLevel"]?.SetValue(ErrorLevel);
            
            _interferenceShader.Parameters["BaseColor"]?.SetValue(BaseColor.ToVector4());

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);
            _interferenceShader.Parameters["WorldViewProjection"]?.SetValue(projection);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, _interferenceShader);
            spriteBatch.Draw(_blankTexture, viewportRect, Color.White);
            spriteBatch.End();
        }
    }
}