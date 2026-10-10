using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace game.Senoidal
{
    public class Wave
    {
        private const int MIN_AMPLITUDE = 10;
        private const int MAX_AMPLITUDE = 200;
        private const int MIN_FREQUENCY = 1;
        private const int MAX_FREQUENCY = 10;

        private GraphicsDevice _graphicsDevice;
        private Texture2D _pixel;
        private Random _random;

        // Parâmetros da Onda Senoidal
        private float _amplitude = 100f;   // Altura em pixels
        private float _frequency = 0.010f; // Controla o comprimento de onda (menor = mais larga)
        private float _phase = 0f;         // Deslocamento horizontal
        private float _speed = 2f;         // Velocidade da animação da onda

        public Wave(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _pixel = new Texture2D(_graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            
            _random = new Random();            
        }

        public void Update(GameTime gameTime)
        {
            _phase += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Reset()
        {
            _amplitude = _random.Next(MIN_AMPLITUDE, MAX_AMPLITUDE);
            _frequency = _random.Next(MIN_FREQUENCY, MAX_FREQUENCY) / 1000f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int screenWidth = _graphicsDevice.Viewport.Width;
            int centerY = _graphicsDevice.Viewport.Height / 2;

            Vector2? previousPoint = null;

            for (int x = 0; x < screenWidth; x++)
            {
                float y = _amplitude * (float)Math.Sin((2 * Math.PI * _frequency * x) + _phase) + centerY;
                Vector2 currentPoint = new Vector2(x, y);

                if (previousPoint.HasValue)
                {
                    DrawLine(spriteBatch, previousPoint.Value, currentPoint, Color.White, 2f);
                }

                previousPoint = currentPoint;
            }
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness = 1f)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);

            spriteBatch.Draw(
                _pixel,
                start,
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(edge.Length(), thickness),
                SpriteEffects.None,
                0f
            );
        }
    }
}