﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Used for transformations applied to a block or item model
    /// </summary>
    public class ModelTransform
    {
        /// <summary>
        /// Offsetting
        /// </summary>
        public Vec3f Translation;
        /// <summary>
        /// Rotation in degrees
        /// </summary>
        public Vec3f Rotation;
        /// <summary>
        /// Uniform Scaling
        /// </summary>
        public float Scale { set { ScaleXYZ.Set(value, value, value); } }
        /// <summary>
        /// Rotation/Scaling Origin
        /// </summary>
        public Vec3f Origin = new Vec3f(0.5f, 0.5f, 0.5f);
        /// <summary>
        /// Whether to rotate in gui item preview or to rotate when dropped
        /// </summary>
        public bool Rotate = true;

        /// <summary>
        /// Scaling per axis
        /// </summary>
        public Vec3f ScaleXYZ = new Vec3f(1, 1, 1);

        /// <summary>
        /// Scale = 1, No Translation, Rotation by -45 deg in Y-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform BlockDefaultGui()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(),
                Rotation = new Vec3f(-22.6f, -45 - 0.3f, 0),
                Scale = 1f
            };
        }

        /// <summary>
        /// Scale = 1, No Translation, Rotation by -45 deg in Y-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform BlockDefault()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(),
                Rotation = new Vec3f(0, -45, 0),
                Scale = 1f
            };
        }

        /// <summary>
        /// Scale = 1, No Translation, Rotation by -45 deg in Y-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform BlockDefaultTp()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(-2.1f, -1.8f, -1.5f),
                Rotation = new Vec3f(0, -45, -25),
                Scale = 0.3f
            };
        }


        /// <summary>
        /// Scale = 1, No Translation, No Rotation
        /// </summary>
        /// <returns></returns>
        public static ModelTransform ItemDefaultGui()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(3, 1, 0),
                Rotation = new Vec3f(0, 0, 0),
                Origin = new Vec3f(0.6f, 0.6f, 0),
                Scale = 1f,
                Rotate = false
            };
        }

        /// <summary>
        /// Scale = 1, No Translation, Rotation by 180 deg in X-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform ItemDefault()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(0.05f, 0, 0),
                Rotation = new Vec3f(180, 90, -30),
                Scale = 1f
            };
        }

        /// <summary>
        /// Scale = 1, No Translation, Rotation by 180 deg in X-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform ItemDefaultTp()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(-1.5f, -1.6f, -1.4f),
                Rotation = new Vec3f(0, -62, 18),
                Scale = 0.33f
            };
        }


        /// <summary>
        /// Makes sure that Translation and Rotation is not null
        /// </summary>
        public void EnsureDefaultValues()
        {
            if (Translation == null) Translation = new Vec3f();
            if (Rotation == null) Rotation = new Vec3f();
        }

        public ModelTransform Clone()
        {
            return new ModelTransform()
            {
                Rotate = Rotate,
                Rotation = Rotation?.Clone(),
                Translation = Translation?.Clone(),
                ScaleXYZ = ScaleXYZ.Clone(),
                Origin = Origin?.Clone()
            };
        }

        public void Clear()
        {
            Rotation.Set(0, 0, 0);
            Translation.Set(0, 0, 0);
            Origin.Set(0, 0, 0);
            Scale = 1f;
        }
    }
}
