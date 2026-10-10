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
        private const float AMPLITUDE_PER_RADIAN = 20f;
        private const float FREQUENCY_PER_RADIAN = 0.001f;

        private Random _random;

        private Effect _waveShader;
        private Wave _targetWave;
        private Wave _playerWave;

        public WaveParameters TargetParameters;
        public WaveParameters PlayerParameters;

        public float AmplitudeDiff;
        public float FrequencyDiff;
        public float PhaseDiff;

        public void RotateAmplitude(float rotationDelta)
        {
            PlayerParameters.Amplitude += rotationDelta * AMPLITUDE_PER_RADIAN;
            PlayerParameters.Amplitude = MathHelper.Clamp(PlayerParameters.Amplitude, MIN_AMPLITUDE, MAX_AMPLITUDE);
            UpdatePlayerWaveParameters();
        }

        public void RotateFrequency(float rotationDelta)
        {
            PlayerParameters.Frequency += rotationDelta * FREQUENCY_PER_RADIAN;
            PlayerParameters.Frequency = MathHelper.Clamp(PlayerParameters.Frequency, MIN_FREQUENCY / 1000f, MAX_FREQUENCY / 1000f);
            UpdatePlayerWaveParameters();
        }

        public void RotatePhaseOffset(float rotationDelta)
        {
            float twoPi = (float)(Math.PI * 2);
            PlayerParameters.PhaseOffset = (PlayerParameters.PhaseOffset + rotationDelta + twoPi) % twoPi;
            UpdatePlayerWaveParameters();
        }

        private void UpdatePlayerWaveParameters()
        {
            _playerWave.Parameters = PlayerParameters;
            TargetParameters.CalculateDifference(PlayerParameters, out AmplitudeDiff, out FrequencyDiff, out PhaseDiff);
        }

        public WaveManager(Effect waveShader)
        {
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

            TargetParameters = new WaveParameters(targetAmplitude, targetFrequency, targetPhaseOffset);

            float baseDotSpacing = 12f;
            float maxSlope = (float)(targetAmplitude * 2 * Math.PI * targetFrequency);
            float averageSlope = maxSlope / (float)Math.Sqrt(2);
            float stretchFactor = (float)Math.Sqrt(1 + (averageSlope * averageSlope));
            float dotSpacing = baseDotSpacing / stretchFactor;

            float playerStarterAmplitude = _random.Next(MIN_AMPLITUDE, MAX_AMPLITUDE);
            float playerStarterFrequency = _random.Next(MIN_FREQUENCY, MAX_FREQUENCY) / 1000f;
            float playerStarterPhaseOffset = (float)(_random.NextDouble() * Math.PI * 2);

            PlayerParameters = new WaveParameters(playerStarterAmplitude, playerStarterFrequency, playerStarterPhaseOffset);

            _targetWave = new Wave(Core.GraphicsDevice, _waveShader)
            {
                Parameters = TargetParameters,
                Speed = WAVE_SPEED,
                Color = Color.WhiteSmoke,
                Thickness = 4.0f,
                IsDotted = true,
                DotSpacing = dotSpacing
            };

            _playerWave = new Wave(Core.GraphicsDevice, _waveShader)
            {
                Parameters = PlayerParameters,
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