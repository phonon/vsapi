﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Represent a dynamically composed track made out of individual small pieces of music mixed together defined by specific rules
    /// Requirements:
    /// - Start/Stop Multiple Trackpieces
    /// - Set their volumne dynamically
    /// - Decide which Trackpieces to play
    /// - Allow individual rules per Trackpiece
    /// Specific examples:
    /// - Play Thunder ambient only if thunderlevel > 10
    ///   - Thunder ambient volume based on thunderlevel (between 0.3 and 1.1?)
    /// - Play Aquatic Drone only when y < 60
    /// - Play Deep Drone only when y < 50
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class CaveMusicTrack : IMusicTrack
    {
        Random rand = new Random();

        [JsonProperty]
        MusicTrackPart[] Parts = null;

        MusicTrackPart[] PartsShuffled;

        int MaxSimultaenousTracks = 3;
        float SimultaenousTrackChance = 0.01f;
        float Priority = 2f;

        long activeUntilMs;
        long cooldownUntilMs;
        IWorldAccessor world;
        List<string> activeTracks = new List<string>();


        public string Name { get {
                string active = "";
                for (int i = 0; i < Parts.Length; i++)
                {
                    if (Parts[i].IsPlaying)
                    {
                        if (active.Length > 0)
                        {
                            active += ", ";
                        }
                        active += Parts[i].NowPlayingFile.GetName();
                    }
                }
                return "Cave Mix ("+active+")";
        } }

        /// <summary>
        /// When playing cave sounds, play between 4-10 minutes each time
        /// </summary>
        double SessionPlayTime
        {
            get { return 4 * 60 + 6 * 60 * rand.NextDouble(); }
        }

        public bool IsActive
        {
            get
            {
                foreach (MusicTrackPart part in Parts)
                {
                    if (part.IsPlaying || part.Loading) return true;
                }
                return false;
            }
        }

        float IMusicTrack.Priority
        {
            get { return Priority; }
        }

        public void Initialize(IAssetManager assetManager, IClientWorldAccessor world)
        {
            this.world = world;

            PartsShuffled = new MusicTrackPart[Parts.Length];

            for (int i = 0; i < Parts.Length; i++)
            {
                Parts[i].ExpandFiles(assetManager);
                PartsShuffled[i] = Parts[i];
            }
        }

        public bool ShouldPlay(TrackedPlayerProperties props, IMusicEngine musicEngine)
        {
            if (props.sunSlight > 3) return false;
            if (world.ElapsedMilliseconds < cooldownUntilMs) return false;

            return true;
        }


        public void BeginPlay(TrackedPlayerProperties props, IMusicEngine musicEngine)
        {
            activeUntilMs = world.ElapsedMilliseconds + (int)(SessionPlayTime * 1000);
        }


        public bool ContinuePlay(float dt, TrackedPlayerProperties props, IMusicEngine musicEngine)
        {
            if (props.sunSlight > 3)
            {
                FadeOut(3);
                return false;
            }

            if (activeUntilMs > 0 && world.ElapsedMilliseconds >= activeUntilMs)
            {
                // Ok, time to stop. We play the current tracks until the end and stop
                bool active = IsActive;
                if (!active)
                {
                    activeUntilMs = 0;
                    foreach (MusicTrackPart part in Parts)
                    {
                        part.Sound?.Dispose();
                    }
                }
                return active;
            }

            int quantityActive = 0;
            for (int i = 0; i < Parts.Length; i++)
            {
                quantityActive += (Parts[i].IsPlaying || Parts[i].Loading) ? 1: 0;
            }

            int beforePlaying = quantityActive;

            GameMath.Shuffle(rand, PartsShuffled);

            for (int i = 0; i < PartsShuffled.Length; i++)
            {
                MusicTrackPart part = PartsShuffled[i];
                bool isPlaying = part.IsPlaying;
                bool shouldPlay = part.Applicable(world, props);

                // Part has recently ended
                if (!isPlaying && part.Sound != null)
                {
                    part.Sound.Dispose();
                    part.Sound = null;
                    continue;
                }

                // Part should be stopped
                if (isPlaying && !shouldPlay)
                {
                    if (!part.Sound.IsFadingOut)
                    {
                        part.Sound.FadeOut(3, (sound) => { part.Sound.Dispose(); part.Sound = null; });
                    }
                    continue;
                }


                bool shouldStart =
                    !isPlaying && 
                    shouldPlay && 
                    !part.Loading &&
                    quantityActive < MaxSimultaenousTracks && 
                    (quantityActive == 0 || rand.NextDouble() < SimultaenousTrackChance)
                ;

                if (shouldStart)
                {
                    AssetLocation location = part.Files[rand.Next(part.Files.Length)];
                    part.NowPlayingFile = location;
                    part.Loading = true;
                    musicEngine.StartTrack(location, (sound) => { part.Sound = sound; part.Loading = false; });

                    part.StartedMs = world.ElapsedMilliseconds;
                    quantityActive++;
                }
            }

            return true;
        }

      
        public void FadeOut(float seconds, Common.Action onFadedOut = null)
        {
            bool wasInterupted = false;

            foreach (MusicTrackPart part in Parts)
            {
                if (part.IsPlaying)
                {
                    part.Sound.FadeOut(seconds, (sound) => {
                        sound.Dispose();
                        part.Sound = null;
                        onFadedOut?.Invoke();
                    });

                    wasInterupted = true;
                }
            }

            // When naturally stopped, give the player a break from the cave sounds (3-10 minutes)
            if (!wasInterupted)
            {
                cooldownUntilMs = world.ElapsedMilliseconds + (long)(1000 * (3*60 + rand.NextDouble() * 7*60));
            }
        }
        
        public void UpdateVolume()
        {
            foreach (MusicTrackPart part in Parts)
            {
                if (part.IsPlaying) part.Sound.SetVolume();
            }
                
        }
    }



}
