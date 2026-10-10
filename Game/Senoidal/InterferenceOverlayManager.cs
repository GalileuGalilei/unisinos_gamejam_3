using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace game.Senoidal
{
    public class InterferenceOverlayManager
    {
        private InterferenceOverlay _targetOverlay;
        private InterferenceOverlay _playerOverlay;

        public float TargetOpacity 
        { 
            get => _targetOverlay.Opacity; 
            set => _targetOverlay.Opacity = value; 
        }
        
        public Color TargetBaseColor 
        { 
            get => _targetOverlay.BaseColor; 
            set => _targetOverlay.BaseColor = value; 
        }

        public float PlayerOpacity 
        { 
            get => _playerOverlay.Opacity; 
            set => _playerOverlay.Opacity = value; 
        }
        
        public Color PlayerBaseColor 
        { 
            get => _playerOverlay.BaseColor; 
            set => _playerOverlay.BaseColor = value; 
        }

        public float PlayerErrorLevel 
        { 
            get => _playerOverlay.ErrorLevel; 
            set => _playerOverlay.ErrorLevel = value; 
        }

        public InterferenceOverlayManager(GraphicsDevice graphicsDevice, Effect baseShader)
        {
            _targetOverlay = new InterferenceOverlay(graphicsDevice, baseShader);
            _playerOverlay = new InterferenceOverlay(graphicsDevice, baseShader);
        }

        public void UpdateWaves(Wave targetWave, Wave playerWave)
        {
            _targetOverlay.TargetWave = targetWave;
            _playerOverlay.TargetWave = playerWave;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _targetOverlay.Draw(spriteBatch);
            _playerOverlay.Draw(spriteBatch);
        }
    }
}