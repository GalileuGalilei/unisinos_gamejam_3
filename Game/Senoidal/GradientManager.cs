using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

namespace game.Senoidal
{
    class GradientManager
    {
        private Texture2D _gradientTexture;
        private Random _random;

        private int MaxIndex => GradientLibrary.Gradients.Length - 1;
        private int _lastIndex = 0;

        public GradientManager()
        {            
            _random = new Random();

            _gradientTexture = new Texture2D(Core.GraphicsDevice, 1, 2);
        
            ResetTexture();
        }

        public void ResetTexture()
        {
            int gradientIndex = _random.Next(0, MaxIndex);

            if (gradientIndex == _lastIndex)
            {
                gradientIndex = (gradientIndex + 1) % MaxIndex;
            }

            _lastIndex = gradientIndex;

            _gradientTexture.SetData(GradientLibrary.GetGradient(gradientIndex));
        }

        public Texture2D GetTexture()
        {
            return _gradientTexture;
        }
    }
}