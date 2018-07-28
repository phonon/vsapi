﻿using System;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public abstract class AiTaskBase : IAiTask
    {
        public Random rand;
        public EntityAgent entity;
        public IWorldAccessor world;
        public AnimationMetaData animMeta;

        protected float priority;
        protected float priorityForCancel;
        protected int slot;
        protected int mincooldown;
        protected int maxcooldown;
        protected string sound;
        protected float soundRange;

        protected string whenInEmotionState;

        protected long cooldownUntilMs;


        public AiTaskBase(EntityAgent entity)
        {
            this.entity = entity;
            this.world = entity.World;
            rand = new Random((int)entity.Entityid);
        }

        public virtual void LoadConfig(JsonObject taskConfig, JsonObject aiConfig)
        {
            this.priority = taskConfig["priority"].AsFloat();
            this.priorityForCancel = taskConfig["priorityForCancel"].AsFloat(priority);
            this.slot = (int)taskConfig["slot"]?.AsInt(0);
            this.mincooldown = (int)taskConfig["mincooldown"]?.AsInt(0);
            this.maxcooldown = (int)taskConfig["maxcooldown"]?.AsInt(100);

            if (taskConfig["animation"].Exists)
            {
                animMeta = new AnimationMetaData()
                {
                    Code = taskConfig["animation"].AsString()?.ToLowerInvariant(),
                    Animation = taskConfig["animation"].AsString()?.ToLowerInvariant(),
                    AnimationSpeed = taskConfig["animationSpeed"].AsFloat(1f)
                }.Init();
            }

            this.whenInEmotionState = taskConfig["whenInEmotionState"].AsString();

            if (taskConfig["sound"] != null)
            {
                sound = taskConfig["sound"].AsString();
                soundRange = taskConfig["soundRange"].AsFloat(16);
            }

            cooldownUntilMs = entity.World.ElapsedMilliseconds + mincooldown + entity.World.Rand.Next(maxcooldown - mincooldown);
        }

        public virtual int Slot
        {
            get { return slot; }
        }

        public virtual float Priority
        {
            get { return priority; }
        }

        public virtual float PriorityForCancel
        {
            get { return priorityForCancel; }
        }


        public abstract bool ShouldExecute();

        public virtual void StartExecute()
        {
            if (animMeta != null)
            {
                entity.StartAnimation(animMeta);
            }

            if (sound != null)
            {
                entity.World.PlaySoundAt(new AssetLocation("sounds/"+sound), entity.ServerPos.X, entity.ServerPos.Y, entity.ServerPos.Z, null, true, soundRange);
            }
        }

        public virtual bool ContinueExecute(float dt)
        {
            return true;
        }

        public virtual void FinishExecute(bool cancelled)
        {
            cooldownUntilMs = entity.World.ElapsedMilliseconds + mincooldown + entity.World.Rand.Next(maxcooldown - mincooldown);

            if (animMeta != null)
            {
                entity.StopAnimation(animMeta.Code);
            }
        }


        public virtual void OnStateChanged(EnumEntityState beforeState)
        {
            // Reset timer because otherwise the tasks will always be executed upon entering active state
            if (entity.State == EnumEntityState.Active)
            {
                cooldownUntilMs = entity.World.ElapsedMilliseconds + mincooldown + entity.World.Rand.Next(maxcooldown - mincooldown);
            }
        }

        public virtual bool Notify(string key, object data)
        {
            return false;
        }
    }
}
