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
        private const int MAX_AMPLITUDE = 50;
        private const int MIN_FREQUENCY = 10;
        private const int MAX_FREQUENCY = 40;

        private const float WAVE_SPEED = 1.5f;

        // Sensibilidade do teclado/scroll (por passo)
        private const float AMPLITUDE_PER_STEP = 1.5f;
        private const float FREQUENCY_PER_STEP = 0.0001f;
        private const float PHASE_PER_STEP = 0.05f;

        // Sensibilidade dos botões radiais (por radiano girado)
        private const float AMPLITUDE_PER_RADIAN = 20f;
        private const float FREQUENCY_PER_RADIAN = 0.001f;
        private const float PHASE_PER_RADIAN = 1f;

        private const float MAX_AMP_SPAN = MAX_AMPLITUDE - MIN_AMPLITUDE;
        private const float MAX_FRQ_SPAN = (MAX_FREQUENCY - MIN_FREQUENCY) / 1000f;
        private const float MAX_PHA_SPAN = (float)Math.PI;
        private const float TWO_PI = (float)(Math.PI * 2);

        private readonly Random _random = new Random();

        public Wave TargetWave { get; }
        public Wave PlayerWave { get; }

        public WaveParameters TargetParameters;
        public WaveParameters PlayerParameters;

        public float AmplitudeDiff;
        public float FrequencyDiff;
        public float PhaseDiff;

        public float InitialWeightedDiff { get; private set; }

        public EditableParameter CurrentParameter { get; private set; } = EditableParameter.Amplitude;

        public WaveManager(Effect waveShader)
        {
            // As ondas são criadas uma única vez; o Reset() só troca os parâmetros.
            TargetWave = new Wave(Core.GraphicsDevice, waveShader)
            {
                Speed = WAVE_SPEED,
                Color = Color.Silver,
                Thickness = 4.0f,
                IsDotted = true
            };

            PlayerWave = new Wave(Core.GraphicsDevice, waveShader)
            {
                Speed = WAVE_SPEED,
                Color = Color.White,
                Thickness = 4.0f,
                IsDotted = false
            };

            Reset();
        }

        // ---------- Controle: teclado/scroll (parâmetro selecionado) ----------

        public void ChangeParameter(int direction)
        {
            int totalParameters = Enum.GetValues(typeof(EditableParameter)).Length;
            int nextValue = ((int)CurrentParameter + direction + totalParameters) % totalParameters;
            CurrentParameter = (EditableParameter)nextValue;
        }

        public void Increment(float amount)
        {
            switch (CurrentParameter)
            {
                case EditableParameter.Amplitude:
                    AdjustAmplitude(amount * AMPLITUDE_PER_STEP);
                    break;
                case EditableParameter.Frequency:
                    AdjustFrequency(amount * FREQUENCY_PER_STEP);
                    break;
                case EditableParameter.PhaseOffset:
                    AdjustPhaseOffset(amount * PHASE_PER_STEP);
                    break;
            }
        }

        // ---------- Controle: botões radiais ----------

        public void RotateAmplitude(float rotationDelta) => AdjustAmplitude(rotationDelta * AMPLITUDE_PER_RADIAN);
        public void RotateFrequency(float rotationDelta) => AdjustFrequency(rotationDelta * FREQUENCY_PER_RADIAN);
        public void RotatePhaseOffset(float rotationDelta) => AdjustPhaseOffset(rotationDelta * PHASE_PER_RADIAN);

        // ---------- Lógica compartilhada ----------

        private void AdjustAmplitude(float delta)
        {
            if (delta == 0f) return;
            PlayerParameters.Amplitude = MathHelper.Clamp(PlayerParameters.Amplitude + delta, MIN_AMPLITUDE, MAX_AMPLITUDE);
            UpdatePlayerWaveParameters();
        }

        private void AdjustFrequency(float delta)
        {
            if (delta == 0f) return;
            PlayerParameters.Frequency = MathHelper.Clamp(PlayerParameters.Frequency + delta, MIN_FREQUENCY / 1000f, MAX_FREQUENCY / 1000f);
            UpdatePlayerWaveParameters();
        }

        private void AdjustPhaseOffset(float delta)
        {
            if (delta == 0f) return;
            // Fase trabalha em radianos; mantém o valor circular dentro de 0 a 2*PI
            float phase = PlayerParameters.PhaseOffset + delta;
            PlayerParameters.PhaseOffset = (phase % TWO_PI + TWO_PI) % TWO_PI;
            UpdatePlayerWaveParameters();
        }

        private void UpdatePlayerWaveParameters()
        {
            PlayerWave.Parameters = PlayerParameters;
            TargetParameters.CalculateDifference(PlayerParameters, out AmplitudeDiff, out FrequencyDiff, out PhaseDiff);
        }

        // ---------- Métricas ----------

        /// <summary>
        /// Erro normalizado (0 = idêntico, 1 = máximo) entre a onda do player e a alvo.
        /// </summary>
        public float CalculateWeightedDiff()
        {
            float normAmp = MathHelper.Clamp(AmplitudeDiff / MAX_AMP_SPAN, 0f, 1f);
            float normFreq = MathHelper.Clamp(FrequencyDiff / MAX_FRQ_SPAN, 0f, 1f);
            float normPhase = MathHelper.Clamp(PhaseDiff / MAX_PHA_SPAN, 0f, 1f);

            return (normAmp * 0.33f) + (normFreq * 0.34f) + (normPhase * 0.33f);
        }

        /// <summary>
        /// Progresso de 0 (tão longe quanto no início da fase, ou mais) até 1 (idêntico).
        /// </summary>
        public float GetMatchProgress()
        {
            if (InitialWeightedDiff <= 0.0001f) return 1f;

            float currentDiff = CalculateWeightedDiff();
            float progress = 1.0f - (currentDiff / InitialWeightedDiff);
            return MathHelper.Clamp(progress, 0.0f, 1.0f);
        }

        // ---------- Ciclo de vida ----------

        public void Update(GameTime gameTime)
        {
            TargetWave.Update(gameTime);
            PlayerWave.Update(gameTime);
        }

        public void Reset()
        {
            TargetParameters = RandomParameters();
            PlayerParameters = RandomParameters();

            TargetWave.Parameters = TargetParameters;
            TargetWave.DotSpacing = CalculateDotSpacing(TargetParameters);
            TargetWave.Phase = 0f;

            PlayerWave.Phase = 0f;
            UpdatePlayerWaveParameters();

            InitialWeightedDiff = CalculateWeightedDiff();
            if (InitialWeightedDiff < 0.001f) InitialWeightedDiff = 0.001f;
        }

        private WaveParameters RandomParameters()
        {
            float amplitude = _random.Next(MIN_AMPLITUDE, MAX_AMPLITUDE);
            float frequency = _random.Next(MIN_FREQUENCY, MAX_FREQUENCY) / 1000f;
            float phaseOffset = (float)(_random.NextDouble() * TWO_PI);
            return new WaveParameters(amplitude, frequency, phaseOffset);
        }

        // Compensa o "esticamento" dos pontos em ondas mais inclinadas
        private static float CalculateDotSpacing(WaveParameters parameters)
        {
            const float baseDotSpacing = 12f;
            float maxSlope = (float)(parameters.Amplitude * TWO_PI * parameters.Frequency);
            float averageSlope = maxSlope / (float)Math.Sqrt(2);
            float stretchFactor = (float)Math.Sqrt(1 + (averageSlope * averageSlope));
            return baseDotSpacing / stretchFactor;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TargetWave.Draw(spriteBatch);
            PlayerWave.Draw(spriteBatch);
        }
    }
}