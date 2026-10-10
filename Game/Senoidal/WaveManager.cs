using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

namespace game.Senoidal
{
    public class WaveManager
    {
        public enum EditableParameter
        {
            Amplitude,
            Frequency,
            PhaseOffset
        }

        private const int MIN_AMPLITUDE = 10;
        private const int MAX_AMPLITUDE = 200;
        private const int MIN_FREQUENCY = 1;
        private const int MAX_FREQUENCY = 10;

        private const float WAVE_SPEED = 1.5f;

        private Random _random;

        private Effect _waveShader;
        private Wave _targetWave;
        private Wave _playerWave;

        public WaveParameters TargetParameters;
        public WaveParameters PlayerParameters;

        private const float MAX_AMP_SPAN = MAX_AMPLITUDE - MIN_AMPLITUDE;
        private const float MAX_FRQ_SPAN = (MAX_FREQUENCY - MIN_FREQUENCY) / 1000f;
        private const float MAX_PHA_SPAN = (float)Math.PI;

        public float AmplitudeDiff;
        public float FrequencyDiff;
        public float PhaseDiff;

        public float InitialWeightedDiff { get; private set; }

        public EditableParameter CurrentParameter { get; private set; } = EditableParameter.Amplitude;

        public void ChangeParameter(int direction)
        {
            int totalParameters = Enum.GetValues(typeof(EditableParameter)).Length;
            int nextValue = ((int)CurrentParameter + direction + totalParameters) % totalParameters;
            CurrentParameter = (EditableParameter)nextValue;
            Console.WriteLine($"[CurrentParameter] {CurrentParameter}");
        }

        public void Increment(float amount)
        {
            switch (CurrentParameter)
            {
                case EditableParameter.Amplitude:
                    PlayerParameters.Amplitude += amount * 1.5f; 
                    PlayerParameters.Amplitude = MathHelper.Clamp(PlayerParameters.Amplitude, MIN_AMPLITUDE, MAX_AMPLITUDE);
                    break;

                case EditableParameter.Frequency:
                    PlayerParameters.Frequency += amount * 0.0001f;
                    PlayerParameters.Frequency = MathHelper.Clamp(PlayerParameters.Frequency, MIN_FREQUENCY / 1000f, MAX_FREQUENCY / 1000f);
                    break;

                case EditableParameter.PhaseOffset:
                    // Fase trabalha em radianos (escala de 0 a ~6.28)
                    PlayerParameters.PhaseOffset += amount * 0.05f;
                    
                    // Mantém o valor circular dentro de 0 a 2*PI
                    float twoPi = (float)(Math.PI * 2);
                    PlayerParameters.PhaseOffset = (PlayerParameters.PhaseOffset % twoPi + twoPi) % twoPi;
                    break;
            }

            _playerWave.Parameters = PlayerParameters;
            TargetParameters.CalculateDifference(PlayerParameters, out AmplitudeDiff, out FrequencyDiff, out PhaseDiff);
        }

        public float CalculateWeightedDiff()
        {
            float normAmp = MathHelper.Clamp(AmplitudeDiff / MAX_AMP_SPAN, 0f, 1f);
            float normFreq = MathHelper.Clamp(FrequencyDiff / MAX_FRQ_SPAN, 0f, 1f);
            float normPhase = MathHelper.Clamp(PhaseDiff / MAX_PHA_SPAN, 0f, 1f);

            return (normAmp * 0.33f) + (normFreq * 0.34f) + (normPhase * 0.33f);
        }

        public float GetMatchProgress()
        {
            if (InitialWeightedDiff <= 0.0001f) return 1f;

            float currentDiff = CalculateWeightedDiff();
            float progress = 1.0f - (currentDiff / InitialWeightedDiff);
            return MathHelper.Clamp(progress, 0.0f, 1.0f);
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

            TargetParameters.CalculateDifference(PlayerParameters, out AmplitudeDiff, out FrequencyDiff, out PhaseDiff);
            InitialWeightedDiff = CalculateWeightedDiff();
            if (InitialWeightedDiff < 0.001f) InitialWeightedDiff = 0.001f;

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