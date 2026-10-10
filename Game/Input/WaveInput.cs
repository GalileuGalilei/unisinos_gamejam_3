using System;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.UI;

namespace game.Input
{
    /// <summary>
    /// Traduz todas as formas de controle da cena de ondas (teclado, scroll e botões radiais)
    /// em comandos de alto nível. A cena só lê os resultados e repassa para os managers.
    /// </summary>
    public class WaveInput
    {
        private readonly RadialButton _amplitudeKnob;
        private readonly RadialButton _frequencyKnob;
        private readonly RadialButton _phaseKnob;

        // Comandos de cena
        public bool BackRequested { get; private set; }
        public bool SkipRequested { get; private set; }
        public bool ToggleDebugRequested { get; private set; }

        // Teclado/scroll: troca e ajuste do parâmetro selecionado
        public int ParameterChange { get; private set; }          // -1, 0 ou +1
        public float SelectedParameterDelta { get; private set; } // passos (-1, 0 ou +1)

        // Botões radiais: rotação em radianos neste frame
        public float AmplitudeRotation { get; private set; }
        public float FrequencyRotation { get; private set; }
        public float PhaseRotation { get; private set; }

        /// <summary>
        /// Verdadeiro se o jogador mexeu na onda neste frame, por qualquer meio.
        /// </summary>
        public bool HasWaveAdjustment =>
            SelectedParameterDelta != 0f ||
            AmplitudeRotation != 0f ||
            FrequencyRotation != 0f ||
            PhaseRotation != 0f;

        public WaveInput(RadialButton amplitudeKnob, RadialButton frequencyKnob, RadialButton phaseKnob)
        {
            _amplitudeKnob = amplitudeKnob;
            _frequencyKnob = frequencyKnob;
            _phaseKnob = phaseKnob;
        }

        public void Update()
        {
            var keyboard = Core.Input.Keyboard;
            var mouse = Core.Input.Mouse;

            BackRequested = keyboard.WasKeyJustPressed(Keys.Escape);
            SkipRequested = keyboard.WasKeyJustPressed(Keys.Space);
            ToggleDebugRequested = keyboard.WasKeyJustPressed(Keys.F2);

            // Troca de parâmetro (A/D ou setas)
            ParameterChange = 0;
            if (keyboard.WasKeyJustPressed(Keys.D) || keyboard.WasKeyJustPressed(Keys.Right))
                ParameterChange = 1;
            else if (keyboard.WasKeyJustPressed(Keys.A) || keyboard.WasKeyJustPressed(Keys.Left))
                ParameterChange = -1;

            // Ajuste do parâmetro selecionado (scroll tem prioridade sobre W/S ou setas)
            SelectedParameterDelta = 0f;
            if (mouse.ScrollWheelDelta != 0)
            {
                SelectedParameterDelta = Math.Sign(mouse.ScrollWheelDelta);
            }
            else
            {
                if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
                    SelectedParameterDelta = 1f;
                if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
                    SelectedParameterDelta = -1f;
            }

            // Botões radiais
            _amplitudeKnob.Update(mouse);
            _frequencyKnob.Update(mouse);
            _phaseKnob.Update(mouse);

            AmplitudeRotation = _amplitudeKnob.RotationDelta;
            FrequencyRotation = _frequencyKnob.RotationDelta;
            PhaseRotation = _phaseKnob.RotationDelta;
        }
    }
}
