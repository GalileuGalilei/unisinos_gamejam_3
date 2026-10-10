using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace game.Audio
{
    public class AudioManager
    {
        private SoundEffect _ambientSound;
        private SoundEffectInstance _ambientInstance;
        private SoundEffect _staticSound;
        private SoundEffectInstance _staticInstance;
        private SoundEffect _clickSound;

        private const float MIN_VOLUME = 0.15f; 
        private const float MAX_VOLUME = 0.85f;
        private const float MAX_STATIC_VOLUME = 0.5f;
        private const float MIN_STATIC_VOLUME = 0.0f;
        private const float LERP_SPEED_UP = 4.0f;     
        private const float FADE_OUT_SPEED = 1.8f;

        private float _currentVolume = MIN_VOLUME;
        private float _currentStaticVolume = MAX_STATIC_VOLUME;
        private bool _isPostMatchFading = false;
        private readonly Random _random = new Random();

        public float CurrentVolume => _currentVolume;
        public float CurrentStaticVolume => _currentStaticVolume;

        public void LoadContent(ContentManager content)
        {
            _ambientSound = content.Load<SoundEffect>("audio/ambient_loop");
            _ambientInstance = _ambientSound.CreateInstance();
            _ambientInstance.IsLooped = true;
            _ambientInstance.Volume = MIN_VOLUME;
            _ambientInstance.Play();

            _staticSound = content.Load<SoundEffect>("audio/static_loop");
            _staticInstance = _staticSound.CreateInstance();
            _staticInstance.IsLooped = true;
            _staticInstance.Volume = MAX_STATIC_VOLUME;
            _staticInstance.Play();

            _clickSound = content.Load<SoundEffect>("audio/click_sfx");
        }

        public void Update(GameTime gameTime, float matchProgress, bool isUserInteracting)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isPostMatchFading)
            {
                _currentVolume = MathHelper.Lerp(_currentVolume, MIN_VOLUME, FADE_OUT_SPEED * dt);
                
                _currentStaticVolume = MathHelper.Lerp(_currentStaticVolume, MAX_STATIC_VOLUME, FADE_OUT_SPEED * dt);

                if (Math.Abs(_currentVolume - MIN_VOLUME) < 0.02f || isUserInteracting)
                {
                    _isPostMatchFading = false;
                }
            }
            else
            {
                float targetVolume = MathHelper.Lerp(MIN_VOLUME, MAX_VOLUME, matchProgress);
                float targetStaticVolume = MathHelper.Lerp(MAX_STATIC_VOLUME, MIN_STATIC_VOLUME, matchProgress);

                float speed = (_currentVolume < targetVolume) ? LERP_SPEED_UP : FADE_OUT_SPEED;
                _currentVolume = MathHelper.Lerp(_currentVolume, targetVolume, speed * dt);

                // Estática: silencia rápido quando o jogador acerta e volta suave se errar
                float staticSpeed = (_currentStaticVolume > targetStaticVolume) ? LERP_SPEED_UP : FADE_OUT_SPEED;
                _currentStaticVolume = MathHelper.Lerp(_currentStaticVolume, targetStaticVolume, staticSpeed * dt);
            }

            if (_ambientInstance != null)
                _ambientInstance.Volume = MathHelper.Clamp(_currentVolume, 0f, 1f);

            if (_staticInstance != null)
                _staticInstance.Volume = MathHelper.Clamp(_currentStaticVolume, 0f, 1f);
        }

        public void PlayLevelCompleteJuice()
        {
            _isPostMatchFading = true;

            if (_clickSound != null)
            {
                float randomPitch = (float)(_random.NextDouble() * 0.12 - 0.06);
                _clickSound.Play(0.5f, randomPitch, 0.0f);
            }
        }

        public void Stop()
        {
            _ambientInstance?.Stop();
            _staticInstance?.Stop();
        }

        public void UnloadContent()
        {
            _ambientInstance?.Stop();
            _ambientInstance?.Dispose();
            _staticInstance?.Stop();
            _staticInstance?.Dispose();
        }
    }
}
