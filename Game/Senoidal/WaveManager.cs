using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

namespace game.Senoidal
{
    public class WaveManager
    {
        private const int MIN_AMPLITUDE = 10;
        private const int MAX_AMPLITUDE = 200;
        private const int MIN_FREQUENCY = 1;
        private const int MAX_FREQUENCY = 10;

        private const float WAVE_SPEED = 1.5f;

        private GraphicsDevice _graphicsDevice;
        private Random _random;

        private Effect _waveShader;
        private Wave _targetWave;
        private Wave _playerWave;

        public WaveManager(GraphicsDevice graphicsDevice, Effect waveShader)
        {
            _graphicsDevice = graphicsDevice;
            _random = new Random();
            _waveShader = waveShader;

            Reset();      
        }

        public void Update(GameTime gameTime)
        {
            _targetWave.Update(gameTime);
            _playerWave.Update(gameTime);
        }

        public void Reset()
        {
            float targetAmplitude = _random.Next(MIN_AMPLITUDE, MAX_AMPLITUDE);
            float targetFrequency = _random.Next(MIN_FREQUENCY, MAX_FREQUENCY) / 1000f;
            float targetPhaseOffset = (float)(_random.NextDouble() * Math.PI * 2);

            float dotSpacing = targetAmplitude / 10f + 1;

            float playerStarterAmplitude = _random.Next(MIN_AMPLITUDE, MAX_AMPLITUDE);
            float playerStarterFrequency = _random.Next(MIN_FREQUENCY, MAX_FREQUENCY) / 1000f;
            float playerStarterPhaseOffset = (float)(_random.NextDouble() * Math.PI * 2);

            _targetWave = new Wave(Core.GraphicsDevice, _waveShader)
            {
                Amplitude = targetAmplitude,
                Frequency = targetFrequency,
                PhaseOffset = targetPhaseOffset,
                Speed = WAVE_SPEED,
                Color = Color.Silver,
                Thickness = 4.0f,
                IsDotted = true,
                DotSpacing = dotSpacing
            };

            _playerWave = new Wave(Core.GraphicsDevice, _waveShader)
            {
                Amplitude = playerStarterAmplitude,
                Frequency = playerStarterFrequency,
                PhaseOffset = playerStarterPhaseOffset,
                Speed = WAVE_SPEED,
                Color = Color.White,
                Thickness = 4.0f,
                IsDotted = false
            };   
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _targetWave.Draw(Core.SpriteBatch);
            _playerWave.Draw(Core.SpriteBatch);
        }
    }
}