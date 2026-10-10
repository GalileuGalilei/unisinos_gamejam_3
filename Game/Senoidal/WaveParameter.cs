using System;

namespace game.Senoidal
{
    public struct WaveParameters
    {
        public float Amplitude;
        public float Frequency;
        public float PhaseOffset;
        
        public WaveParameters(float amplitude, float frequency, float phaseOffset)
        {
            Amplitude = amplitude;
            Frequency = frequency;
            PhaseOffset = phaseOffset;
        }

        public void CalculateDifference(WaveParameters target, out float amplitudeDiff, out float frequencyDiff, out float phaseDiff)
        {
            amplitudeDiff = Math.Abs(Amplitude - target.Amplitude);
            frequencyDiff = Math.Abs(Frequency - target.Frequency);

            float twoPi = (float)(Math.PI * 2);
            phaseDiff = Math.Abs(PhaseOffset - target.PhaseOffset) % twoPi;
            if (phaseDiff > Math.PI)
            {
                phaseDiff = twoPi - phaseDiff;
            }
        }
    }
}