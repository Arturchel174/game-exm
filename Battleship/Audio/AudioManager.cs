using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace Battleship.Audio
{
    /// <summary>
    /// An audio manager class that handles sounds.
    /// </summary>
    public class AudioManager : GameComponent
    {

        #region Fields

        // The listener describes the ear which is hearing 3D sounds.
        // This is usually set to match the camera.
        public AudioListener Listener
        {
            get { return listener; }
        }

        AudioListener listener = new AudioListener();


        // The emitter describes an entity which is making a 3D sound.
        AudioEmitter emitter = new AudioEmitter();

        //Store all the sound effects that are available to be played.
        Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();

        // Keep track of all the 3D sounds that are currently playing.
        //List<ActiveSound> activeSounds = new List<ActiveSound>();

        #endregion

        #region Methods
        
        public AudioManager(Game game) : base(game)
        { }

        /// <summary>
        /// Initializes the audio manager.
        /// </summary>
        public override void Initialize()
        {            
            soundEffects.Add("hit", Game.Content.Load<SoundEffect>("hit"));
            soundEffects.Add("sink", Game.Content.Load<SoundEffect>("sink"));
            //soundEffects.Add("unhit", Game.Content.Load<SoundEffect>("unhit"));

            base.Initialize();
        }
        
        // <summary>
        /// Triggers a new 3D sound.
        /// </summary>
        public SoundEffectInstance Play3DSound(string soundName, bool isLooped, IAudioEmitter emitter)
        {
            ActiveSound activeSound = new ActiveSound();

            // Fill in the instance and emitter fields.
            activeSound.Instance = soundEffects[soundName].CreateInstance();
            activeSound.Instance.IsLooped = isLooped;

            activeSound.Emitter = emitter;

            activeSound.Instance.Play();

            // Remember that this sound is now active.
            //activeSounds.Add(activeSound);

            return activeSound.Instance;

        }

        /// <summary>
        /// Unloads the sound effect data.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    foreach (SoundEffect soundEffect in soundEffects.Values)
                    {
                        soundEffect.Dispose();
                    }

                    soundEffects.Clear();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Internal helper class for keeping track of an active 3D sound,
        /// and remembering which emitter object it is attached to.
        /// </summary>
        private class ActiveSound
        {
            public SoundEffectInstance Instance;
            public IAudioEmitter Emitter;
        }

        #endregion
    }
}
