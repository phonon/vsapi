﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace Vintagestory.API.Client
{
    public class RainMusicTrack : IMusicTrack
    {
        public bool IsActive
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float Priority
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void FadeOut(float seconds, Common.Action<ILoadedSound> onFadedOut)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IAssetManager assetManager, IClientWorldAccessor world)
        {
            throw new NotImplementedException();
        }


        public bool ShouldPlay(TrackedPlayerProperties props, IMusicEngine musicEngine)
        {
            
            return false;
        }

        public void BeginPlay(TrackedPlayerProperties props, IMusicEngine musicEngine)
        {
            
        }

        public bool ContinuePlay(float dt, TrackedPlayerProperties props, IMusicEngine musicEngine)
        {
            // Track should be started
            /*if (shouldStart)
            {
                string filename = part.Files[rand.Next(part.Files.Length)];
                part.Sound = musicEngine.StartTrack(
                    filename, 
                    part.CurrentVolume(world, props) + VolumeRandomization - 2 * rand.Next() * VolumeRandomization,
                    PitchRandomization - 2 * rand.Next() * PitchRandomization
                );
                part.startedMs = world.ElapsedMilliseconds;
                quantityPlaying++;
            }

            if (isPlaying && shouldPlay && RuntimeRandomizationInterval != null && world.ElapsedMilliseconds - part.startedMs > RuntimeRandomizationInterval * 1000)
            {
                part.Sound.SetPitch(part.Sound.Params.Pitch + PitchRandomization - 2 * rand.Next() * PitchRandomization);
                part.Sound.SetVolume(part.Sound.Params.Volume + VolumeRandomization - 2 * rand.Next() * VolumeRandomization);
            }*/
            return false;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void UpdateVolume()
        {
            throw new NotImplementedException();
        }

        public void FadeOut(float seconds, Common.Action onFadedOut = null)
        {
            throw new NotImplementedException();
        }
    }
}
