﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// The distribution of the random numbers 
    /// </summary>
    public enum EnumDistribution
    {
        /// <summary>
        /// Select completely random numbers within avg-var until avg+var
        /// </summary>
        UNIFORM = 0,
        /// <summary>
        /// Select random numbers with numbers near avg being the most commonly selected ones, following a triangle curve
        /// </summary>
        TRIANGLE = 1,
        /// <summary>
        /// Select random numbers with numbers near avg being the most commonly selected ones, following a gaussian curve
        /// </summary>
        GAUSSIAN = 2,
        /// <summary>
        /// Select random numbers with numbers near avg being the most commonly selected ones, following a narrow gaussian curve
        /// </summary>
        NARROWGAUSSIAN = 3,
        /// <summary>
        /// Select random numbers with numbers near avg being the least commonly selected ones, following an even narrower gaussian curve
        /// </summary>
        VERYNARROWGAUSSIAN = 10,
        /// <summary>
        /// Select random numbers with numbers near avg being the least commonly selected ones, following an upside down gaussian curve
        /// </summary>
        INVERSEGAUSSIAN = 4,
        /// <summary>
        /// Select random numbers with numbers near avg being the least commonly selected ones, following an upside down gaussian curve
        /// </summary>
        NARROWINVERSEGAUSSIAN = 5,
        /// <summary>
        /// Select numbers in the form of avg + var, wheras low value of var are preferred
        /// </summary>
        INVEXP = 6,
        /// <summary>
        /// Select numbers in the form of avg + var, wheras low value of var are strongly preferred
        /// </summary>
        STRONGINVEXP = 7,
        /// <summary>
        /// Select numbers in the form of avg + var, wheras low value of var are very strongly preferred
        /// </summary>
        STRONGERINVEXP = 8,
        /// <summary>
        /// Select completely random numbers within avg-var until avg+var only ONCE and then always 0
        /// </summary>
        DIRAC = 9,    
    }


    /// <summary>
    /// A more natural random number generator (nature usually doesn't grow by the exact same numbers nor does it completely randomly)
    /// </summary>
    public class NatFloat
    {
        /// <summary>
        /// Always 0
        /// </summary>
        public static NatFloat Zero { get { return new NatFloat(0, 0, EnumDistribution.UNIFORM); } }

        /// <summary>
        /// Always 1
        /// </summary>
        public static NatFloat One { get { return new NatFloat(1, 0, EnumDistribution.UNIFORM); } }

        public float offset = 0f;

        public float avg = 0f;
        public float var = 0;
        public EnumDistribution dist = EnumDistribution.UNIFORM;


        static Random random = new Random();

        

        public NatFloat(float averagevalue, float variance, EnumDistribution distribution)
        {
            this.avg = averagevalue;
            this.var = variance;
            this.dist = distribution;
        }


        public static NatFloat createInvexp(float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, EnumDistribution.INVEXP);
        }

        public static NatFloat createStrongInvexp(float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, EnumDistribution.STRONGINVEXP);
        }

        public static NatFloat createStrongerInvexp(float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, EnumDistribution.STRONGERINVEXP);
        }

        public static NatFloat createUniform(float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, EnumDistribution.UNIFORM);
        }

        public static NatFloat createGauss(float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, EnumDistribution.GAUSSIAN);
        }

        public static NatFloat createNarrowGauss(float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, EnumDistribution.NARROWGAUSSIAN);
        }

        public static NatFloat createInvGauss(float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, EnumDistribution.INVERSEGAUSSIAN);
        }

        public static NatFloat createTri(float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, EnumDistribution.TRIANGLE);
        }

        public static NatFloat createDirac(float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, EnumDistribution.DIRAC);
        }

        public static NatFloat create(EnumDistribution distribution, float averagevalue, float variance)
        {
            return new NatFloat(averagevalue, variance, distribution);
        }


        public NatFloat copyWithOffset(float value)
        {
            NatFloat copy = new NatFloat(value, value, dist);
            copy.offset += value;
            return copy;
        }


        public NatFloat addOffset(float value)
        {
            offset += value;
            return this;
        }

        public NatFloat setOffset(float offset)
        {
            this.offset = offset;
            return this;
        }


        public float nextFloat()
        {
            return nextFloat(1f, random);
        }

        public float nextFloat(float multiplier)
        {
            return nextFloat(multiplier, random);
        }

        public float nextFloat(float multiplier, Random rand)
        {
            float rnd;

            switch (dist)
            {
                case EnumDistribution.UNIFORM:
                    rnd = (float)rand.NextDouble() - 0.5f;
                    return offset + multiplier * (avg + rnd * 2 * var);

                case EnumDistribution.GAUSSIAN:
                    rnd = (float)(rand.NextDouble() + rand.NextDouble() + rand.NextDouble()) / 3;  // Random value out of a gauss curve between 0..1, with 0.5f being most common

                    // Center gauss curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + rnd * 2 * var);

                case EnumDistribution.NARROWGAUSSIAN:
                    rnd = (float)(rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble()) / 6;  // Random value out of a gauss curve between 0..1, with 0.5f being most common

                    // Center gauss curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + rnd * 2 * var);


                case EnumDistribution.VERYNARROWGAUSSIAN:
                    rnd = (float)(rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble()) / 12;  // Random value out of a gauss curve between 0..1, with 0.5f being most common

                    // Center gauss curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + rnd * 2 * var);


                case EnumDistribution.INVEXP:
                    rnd = (float)(rand.NextDouble() * rand.NextDouble());

                    return offset + multiplier * (avg + rnd * var);

                case EnumDistribution.STRONGINVEXP:
                    rnd = (float)(rand.NextDouble() * rand.NextDouble() * rand.NextDouble());

                    return offset + multiplier * (avg + rnd * var);

                case EnumDistribution.STRONGERINVEXP:
                    rnd = (float)(rand.NextDouble() * rand.NextDouble() * rand.NextDouble() * rand.NextDouble());

                    return offset + multiplier * (avg + rnd * var);


                case EnumDistribution.INVERSEGAUSSIAN:
                    rnd = (float)(rand.NextDouble() + rand.NextDouble() + rand.NextDouble()) / 3;  // Random value out of a gauss curve between 0..1, with 0.5f being most common

                    // Flip curve
                    if (rnd > 0.5f)
                    {
                        rnd -= 0.5f;
                    }
                    else
                    {
                        rnd += 0.5f;
                    }

                    // Center gauss curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + 2 * rnd * var);


                case EnumDistribution.NARROWINVERSEGAUSSIAN:
                    rnd = (float)(rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble() + rand.NextDouble()) / 6;  // Random value out of a gauss curve between 0..1, with 0.5f being most common

                    // Flip curve
                    if (rnd > 0.5f)
                    {
                        rnd -= 0.5f;
                    }
                    else
                    {
                        rnd += 0.5f;
                    }

                    // Center gauss curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + 2 * rnd * var);


                case EnumDistribution.DIRAC:
                    rnd = (float)rand.NextDouble() - 0.5f;
                    float value = offset + multiplier * (avg + rnd * 2 * var);

                    avg = 0f;
                    var = 0f;
                    return value;


                case EnumDistribution.TRIANGLE:
                    rnd = (float)(rand.NextDouble() + rand.NextDouble()) / 2;  // Random value out of a triangle curve between 0..1, with 0.5f being most common

                    // Center curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + rnd * 2 * var);

                default:
                    return 0f;
            }

        }


        public float nextFloat(float multiplier, FastRandom rand)
        {
            float rnd;

            switch (dist)
            {
                case EnumDistribution.UNIFORM:
                    rnd = rand.NextFloat() - 0.5f;
                    return offset + multiplier * (avg + rnd * 2 * var);

                case EnumDistribution.GAUSSIAN:
                    rnd = (float)(rand.NextFloat() + rand.NextFloat() + rand.NextFloat()) / 3;  // Random value out of a gauss curve between 0..1, with 0.5f being most common

                    // Center gauss curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + rnd * 2 * var);

                case EnumDistribution.NARROWGAUSSIAN:
                    rnd = (rand.NextFloat() + rand.NextFloat() + rand.NextFloat() + rand.NextFloat() + rand.NextFloat() + rand.NextFloat()) / 6;  // Random value out of a gauss curve between 0..1, with 0.5f being most common

                    // Center gauss curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + rnd * 2 * var);


                case EnumDistribution.INVEXP:
                    rnd = (rand.NextFloat() * rand.NextFloat());

                    return offset + multiplier * (avg + rnd * var);

                case EnumDistribution.STRONGINVEXP:
                    rnd = (rand.NextFloat() * rand.NextFloat() * rand.NextFloat());

                    return offset + multiplier * (avg + rnd * var);

                case EnumDistribution.STRONGERINVEXP:
                    rnd = (rand.NextFloat() * rand.NextFloat() * rand.NextFloat() * rand.NextFloat());

                    return offset + multiplier * (avg + rnd * var);


                case EnumDistribution.INVERSEGAUSSIAN:
                    rnd = (rand.NextFloat() + rand.NextFloat() + rand.NextFloat()) / 3;  // Random value out of a gauss curve between 0..1, with 0.5f being most common

                    // Flip curve
                    if (rnd > 0.5f)
                    {
                        rnd -= 0.5f;
                    }
                    else
                    {
                        rnd += 0.5f;
                    }

                    // Center gauss curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + 2 * rnd * var);


                case EnumDistribution.NARROWINVERSEGAUSSIAN:
                    rnd = (rand.NextFloat() + rand.NextFloat() + rand.NextFloat() + rand.NextFloat() + rand.NextFloat() + rand.NextFloat()) / 6;  // Random value out of a gauss curve between 0..1, with 0.5f being most common

                    // Flip curve
                    if (rnd > 0.5f)
                    {
                        rnd -= 0.5f;
                    }
                    else
                    {
                        rnd += 0.5f;
                    }

                    // Center gauss curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + 2 * rnd * var);


                case EnumDistribution.DIRAC:
                    rnd = rand.NextFloat() - 0.5f;
                    float value = offset + multiplier * (avg + rnd * 2 * var);

                    avg = 0f;
                    var = 0f;
                    return value;


                case EnumDistribution.TRIANGLE:
                    rnd = (rand.NextFloat() + rand.NextFloat()) / 2;  // Random value out of a triangle curve between 0..1, with 0.5f being most common

                    // Center curve to 0
                    rnd = rnd - 0.5f;

                    return offset + multiplier * (avg + rnd * 2 * var);

                default:
                    return 0f;
            }

        }

        internal static NatFloat createFromBytes(BinaryReader reader)
        {
            NatFloat value = NatFloat.Zero;
            value.FromBytes(reader);
            return value;
        }

        public NatFloat Clone()
        {
            return (NatFloat)MemberwiseClone();
        }

        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(offset);
            writer.Write(avg);
            writer.Write(var);
            writer.Write((byte)dist);
        }

        public void FromBytes(BinaryReader reader)
        {
            offset = reader.ReadSingle();
            avg = reader.ReadSingle();
            var = reader.ReadSingle();
            dist = (EnumDistribution)reader.ReadByte();
        }
    }
}