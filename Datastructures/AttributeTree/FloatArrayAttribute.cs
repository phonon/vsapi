﻿using System.IO;
using System.Text;

namespace Vintagestory.API.Datastructures
{
    public class FloatArrayAttribute : ArrayAttribute<float>, IAttribute
    {
        public FloatArrayAttribute()
        {

        }

        public FloatArrayAttribute(float[] value)
        {
            this.value = value;
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                stream.Write(value[i]);
            }

        }

        public void FromBytes(BinaryReader stream)
        {
            int quantity = stream.ReadInt32();
            value = new float[quantity];
            for (int i = 0; i < quantity; i++)
            {
                value[i] = stream.ReadSingle();
            }

        }

        public int GetAttributeId()
        {
            return 12;
        }
        
    }
}
