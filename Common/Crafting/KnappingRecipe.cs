﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public class KnappingRecipe : SingleLayerVoxelRecipe<KnappingRecipe>, ByteSerializable
    {
        /// <summary>
        /// Creates a deep copy
        /// </summary>
        /// <returns></returns>
        public override KnappingRecipe Clone()
        {
            KnappingRecipe recipe = new KnappingRecipe();

            recipe.Pattern = (string[])Pattern.Clone();
            recipe.Ingredient = Ingredient.Clone();
            recipe.Output = Output.Clone();
            recipe.Name = Name;

            return recipe;
        }

    }
}
