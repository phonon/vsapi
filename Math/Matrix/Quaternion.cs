﻿//glMatrix license:
//Copyright (c) 2013, Brandon Jones, Colin MacKenzie IV. All rights reserved.

//Redistribution and use in source and binary forms, with or without modification,
//are permitted provided that the following conditions are met:

//  * Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
//ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


using System;

namespace Vintagestory.API.MathTools
{
    public class Quaternion
    {
        ///**
        // * Creates a new identity quat
        // *
        // * @returns {quat} a new quaternion
        // */
        public static double[] Create()
        {
            double[] output = new double[4];
            output[0] = 0;
            output[1] = 0;
            output[2] = 0;
            output[3] = 1;
            return output;
        }

        ///**
        // * Sets a quaternion to represent the shortest rotation from one
        // * vector to another.
        // *
        // * Both vectors are assumed to be unit length.
        // *
        // * @param {quat} output the receiving quaternion.
        // * @param {vec3} a the initial vector
        // * @param {vec3} b the destination vector
        // * @returns {quat} output
        // */
        public static double[] RotationTo(double[] output, double[] a, double[] b)
        {
            double[] tmpvec3 = Vec3Utils.Create();
            double[] xUnitVec3 = Vec3Utils.FromValues(1, 0, 0);
            double[] yUnitVec3 = Vec3Utils.FromValues(0, 1, 0);

            //    return function(output, a, b) {
            double dot = Vec3Utils.Dot(a, b);

            double nines = 999999; // 0.999999
            nines /= 1000000;

            double epsilon = 1; // 0.000001
            epsilon /= 1000000;

            if (dot < -nines)
            {
                Vec3Utils.Cross(tmpvec3, xUnitVec3, a);
                if (Vec3Utils.Length_(tmpvec3) < epsilon)
                    Vec3Utils.Cross(tmpvec3, yUnitVec3, a);
                Vec3Utils.Normalize(tmpvec3, tmpvec3);
                Quaternion.SetAxisAngle(output, tmpvec3, GameMath.PI);
                return output;
            }
            else if (dot > nines)
            {
                output[0] = 0;
                output[1] = 0;
                output[2] = 0;
                output[3] = 1;
                return output;
            }
            else
            {
                Vec3Utils.Cross(tmpvec3, a, b);
                output[0] = tmpvec3[0];
                output[1] = tmpvec3[1];
                output[2] = tmpvec3[2];
                output[3] = 1 + dot;
                return Quaternion.Normalize(output, output);
            }
            //    };
        }

        ///**
        // * Sets the specified quaternion with values corresponding to the given
        // * axes. Each axis is a vec3 and is expected to be unit length and
        // * perpendicular to all other specified axes.
        // *
        // * @param {vec3} view  the vector representing the viewing direction
        // * @param {vec3} right the vector representing the local "right" direction
        // * @param {vec3} up    the vector representing the local "up" direction
        // * @returns {quat} output
        // */
        public static double[] SetAxes(double[] output, double[] view, double[] right, double[] up)
        {
            double[] matr = Mat3d.Create();

            //    return function(output, view, right, up) {
            matr[0] = right[0];
            matr[3] = right[1];
            matr[6] = right[2];

            matr[1] = up[0];
            matr[4] = up[1];
            matr[7] = up[2];

            matr[2] = view[0];
            matr[5] = view[1];
            matr[8] = view[2];

            return Quaternion.Normalize(output, Quaternion.FromMat3(output, matr));
            //    };
        }

        ///**
        // * Creates a new quat initialized with values from an existing quaternion
        // *
        // * @param {quat} a quaternion to clone
        // * @returns {quat} a new quaternion
        // * @function
        // */
        public static double[] CloneIt(double[] a)
        {
            return Vec4.CloneIt(a);
        }

        ///**
        // * Creates a new quat initialized with the given values
        // *
        // * @param {Number} x X component
        // * @param {Number} y Y component
        // * @param {Number} z Z component
        // * @param {Number} w W component
        // * @returns {quat} a new quaternion
        // * @function
        // */
        public static double[] FromValues(double x, double y, double z, double w)
        {
            return Vec4.FromValues(x, y, z, w);
        }

        ///**
        // * Copy the values from one quat to another
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {quat} a the source quaternion
        // * @returns {quat} output
        // * @function
        // */
        public static double[] Copy(double[] output, double[] a)
        {
            return Vec4.Copy(output, a);
        }

        ///**
        // * Set the components of a quat to the given values
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {Number} x X component
        // * @param {Number} y Y component
        // * @param {Number} z Z component
        // * @param {Number} w W component
        // * @returns {quat} output
        // * @function
        // */
        public static double[] Set(double[] output, double x, double y, double z, double w)
        {
            return Vec4.Set(output, x, y, z, w);
        }

        ///**
        // * Set a quat to the identity quaternion
        // *
        // * @param {quat} output the receiving quaternion
        // * @returns {quat} output
        // */
        public static double[] Identity_(double[] output)
        {
            output[0] = 0;
            output[1] = 0;
            output[2] = 0;
            output[3] = 1;
            return output;
        }

        ///**
        // * Sets a quat from the given angle and rotation axis,
        // * then returns it.
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {vec3} axis the axis around which to rotate
        // * @param {Number} rad the angle in radians
        // * @returns {quat} output
        // **/
        public static double[] SetAxisAngle(double[] output, double[] axis, double rad)
        {
            rad = rad / 2;
            double s = GameMath.Sin(rad);
            output[0] = s * axis[0];
            output[1] = s * axis[1];
            output[2] = s * axis[2];
            output[3] = GameMath.Cos(rad);
            return output;
        }

        ///**
        // * Adds two quat's
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {quat} a the first operand
        // * @param {quat} b the second operand
        // * @returns {quat} output
        // * @function
        // */
        //quat.add = vec4.add;
        public static double[] Add(double[] output, double[] a, double[] b)
        {
            return Vec4.Add(output, a, b);
        }

        ///**
        // * Multiplies two quat's
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {quat} a the first operand
        // * @param {quat} b the second operand
        // * @returns {quat} output
        // */
        public static double[] Multiply(double[] output, double[] a, double[] b)
        {
            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double bx = b[0]; double by = b[1]; double bz = b[2]; double bw = b[3];

            output[0] = ax * bw + aw * bx + ay * bz - az * by;
            output[1] = ay * bw + aw * by + az * bx - ax * bz;
            output[2] = az * bw + aw * bz + ax * by - ay * bx;
            output[3] = aw * bw - ax * bx - ay * by - az * bz;
            return output;
        }

        ///**
        // * Alias for {@link quat.multiply}
        // * @function
        // */
        public static double[] Mul(double[] output, double[] a, double[] b)
        {
            return Multiply(output, a, b);
        }

        ///**
        // * Scales a quat by a scalar number
        // *
        // * @param {quat} output the receiving vector
        // * @param {quat} a the vector to scale
        // * @param {Number} b amount to scale the vector by
        // * @returns {quat} output
        // * @function
        // */
        //quat.scale = vec4.scale;
        public static double[] Scale(double[] output, double[] a, double b)
        {
            return Vec4.Scale(output, a, b);
        }

        ///**
        // * Rotates a quaternion by the given angle aboutput the X axis
        // *
        // * @param {quat} output quat receiving operation result
        // * @param {quat} a quat to rotate
        // * @param {number} rad angle (in radians) to rotate
        // * @returns {quat} output
        // */
        public static double[] RotateX(double[] output, double[] a, double rad)
        {
            rad /= 2;

            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double bx = GameMath.Sin(rad); double bw = GameMath.Cos(rad);

            output[0] = ax * bw + aw * bx;
            output[1] = ay * bw + az * bx;
            output[2] = az * bw - ay * bx;
            output[3] = aw * bw - ax * bx;
            return output;
        }

        ///**
        // * Rotates a quaternion by the given angle aboutput the Y axis
        // *
        // * @param {quat} output quat receiving operation result
        // * @param {quat} a quat to rotate
        // * @param {number} rad angle (in radians) to rotate
        // * @returns {quat} output
        // */
        public static double[] RotateY(double[] output, double[] a, double rad)
        {
            rad /= 2;

            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double by = GameMath.Sin(rad); double bw = GameMath.Cos(rad);

            output[0] = ax * bw - az * by;
            output[1] = ay * bw + aw * by;
            output[2] = az * bw + ax * by;
            output[3] = aw * bw - ay * by;
            return output;
        }

        ///**
        // * Rotates a quaternion by the given angle aboutput the Z axis
        // *
        // * @param {quat} output quat receiving operation result
        // * @param {quat} a quat to rotate
        // * @param {number} rad angle (in radians) to rotate
        // * @returns {quat} output
        // */
        public static double[] RotateZ(double[] output, double[] a, double rad)
        {
            rad /= 2;

            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double bz = GameMath.Sin(rad); double bw = GameMath.Cos(rad);

            output[0] = ax * bw + ay * bz;
            output[1] = ay * bw - ax * bz;
            output[2] = az * bw + aw * bz;
            output[3] = aw * bw - az * bz;
            return output;
        }

        ///**
        // * Calculates the W component of a quat from the X, Y, and Z components.
        // * Assumes that quaternion is 1 unit in length.
        // * Any existing W component will be ignored.
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {quat} a quat to calculate W component of
        // * @returns {quat} output
        // */
        public static double[] CalculateW(double[] output, double[] a)
        {
            double x = a[0]; double y = a[1]; double z = a[2];

            output[0] = x;
            output[1] = y;
            output[2] = z;
            double one = 1;
            output[3] = -GameMath.Sqrt(Math.Abs(one - x * x - y * y - z * z));
            return output;
        }

        ///**
        // * Calculates the dot product of two quat's
        // *
        // * @param {quat} a the first operand
        // * @param {quat} b the second operand
        // * @returns {Number} dot product of a and b
        // * @function
        // */
        public static double Dot(double[] a, double[] b)
        {
            return Vec4.Dot(a, b);
        }

        ///**
        // * Performs a linear interpolation between two quat's
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {quat} a the first operand
        // * @param {quat} b the second operand
        // * @param {Number} t interpolation amount between the two inputs
        // * @returns {quat} output
        // * @function
        // */
        public static double[] Lerp(double[] output, double[] a, double[] b, double t)
        {
            return Vec4.Lerp(output, a, b, t);
        }

        ///**
        // * Performs a spherical linear interpolation between two quat
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {quat} a the first operand
        // * @param {quat} b the second operand
        // * @param {Number} t interpolation amount between the two inputs
        // * @returns {quat} output
        // */
        //quat.slerp = function (output, a, b, t) {
        public static double[] Slerp(double[] output, double[] a, double[] b, double t)
        {
            //    // benchmarks:
            //    //    http://jsperf.com/quaternion-slerp-implementations

            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double bx = b[0]; double by = b[1]; double bz = b[2]; double bw = b[3];

            double omega; double cosom; double sinom; double scale0; double scale1;

            // calc cosine
            cosom = ax * bx + ay * by + az * bz + aw * bw;
            // adjust signs (if necessary)
            if (cosom < 0)
            {
                cosom = -cosom;
                bx = -bx;
                by = -by;
                bz = -bz;
                bw = -bw;
            }
            double one = 1;
            double epsilon = one / 1000000;
            // calculate coefficients
            if ((one - cosom) > epsilon)
            {
                // standard case (slerp)
                omega = GameMath.Acos(cosom);
                sinom = GameMath.Sin(omega);
                scale0 = GameMath.Sin((one - t) * omega) / sinom;
                scale1 = GameMath.Sin(t * omega) / sinom;
            }
            else
            {
                // "from" and "to" quaternions are very close 
                //  ... so we can do a linear interpolation
                scale0 = one - t;
                scale1 = t;
            }
            // calculate final values
            output[0] = scale0 * ax + scale1 * bx;
            output[1] = scale0 * ay + scale1 * by;
            output[2] = scale0 * az + scale1 * bz;
            output[3] = scale0 * aw + scale1 * bw;

            return output;
        }

        ///**
        // * Calculates the inverse of a quat
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {quat} a quat to calculate inverse of
        // * @returns {quat} output
        // */
        public double[] Invert(double[] output, double[] a)
        {
            double a0 = a[0]; double a1 = a[1]; double a2 = a[2]; double a3 = a[3];
            double dot = a0 * a0 + a1 * a1 + a2 * a2 + a3 * a3;
            double one = 1;
            double invDot = (dot != 0) ? one / dot : 0;

            // TODO: Would be faster to return [0,0,0,0] immediately if dot == 0

            output[0] = -a0 * invDot;
            output[1] = -a1 * invDot;
            output[2] = -a2 * invDot;
            output[3] = a3 * invDot;
            return output;
        }

        ///**
        // * Calculates the conjugate of a quat
        // * If the quaternion is normalized, this function is faster than quat.inverse and produces the same result.
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {quat} a quat to calculate conjugate of
        // * @returns {quat} output
        // */
        public double[] Conjugate(double[] output, double[] a)
        {
            output[0] = -a[0];
            output[1] = -a[1];
            output[2] = -a[2];
            output[3] = a[3];
            return output;
        }

        ///**
        // * Calculates the length of a quat
        // *
        // * @param {quat} a vector to calculate length of
        // * @returns {Number} length of a
        // * @function
        // */
        //quat.length = vec4.length;
        public static double Length_(double[] a)
        {
            return Vec4.Length_(a);
        }

        ///**
        // * Alias for {@link quat.length}
        // * @function
        // */
        public static double Len(double[] a)
        {
            return Length_(a);
        }

        ///**
        // * Calculates the squared length of a quat
        // *
        // * @param {quat} a vector to calculate squared length of
        // * @returns {Number} squared length of a
        // * @function
        // */
        public static double SquaredLength(double[] a)
        {
            return Vec4.SquaredLength(a);
        }

        ///**
        // * Alias for {@link quat.squaredLength}
        // * @function
        // */
        public static double SqrLen(double[] a)
        {
            return SquaredLength(a);
        }

        ///**
        // * Normalize a quat
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {quat} a quaternion to normalize
        // * @returns {quat} output
        // * @function
        // */
        public static double[] Normalize(double[] output, double[] a)
        {
            return Vec4.Normalize(output, a);
        }

        ///**
        // * Creates a quaternion from the given 3x3 rotation matrix.
        // *
        // * NOTE: The resultant quaternion is not normalized, so you should be sure
        // * to renormalize the quaternion yourself where necessary.
        // *
        // * @param {quat} output the receiving quaternion
        // * @param {mat3} m rotation matrix
        // * @returns {quat} output
        // * @function
        // */
        public static double[] FromMat3(double[] output, double[] m)
        {
            // Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
            // article "Quaternion Calculus and Fast Animation".
            double fTrace = m[0] + m[4] + m[8];
            double fRoot;

            double zero = 0;
            double one = 1;
            double half = one / 2;
            if (fTrace > zero)
            {
                // |w| > 1/2, may as well choose w > 1/2
                fRoot = GameMath.Sqrt(fTrace + one);  // 2w
                output[3] = half * fRoot;
                fRoot = half / fRoot;  // 1/(4w)
                output[0] = (m[7] - m[5]) * fRoot;
                output[1] = (m[2] - m[6]) * fRoot;
                output[2] = (m[3] - m[1]) * fRoot;
            }
            else
            {
                // |w| <= 1/2
                int i = 0;
                if (m[4] > m[0])
                    i = 1;
                if (m[8] > m[i * 3 + i])
                    i = 2;
                int j = (i + 1) % 3;
                int k = (i + 2) % 3;

                fRoot = GameMath.Sqrt(m[i * 3 + i] - m[j * 3 + j] - m[k * 3 + k] + one);
                output[i] = half * fRoot;
                fRoot = half / fRoot;
                output[3] = (m[k * 3 + j] - m[j * 3 + k]) * fRoot;
                output[j] = (m[j * 3 + i] + m[i * 3 + j]) * fRoot;
                output[k] = (m[k * 3 + i] + m[i * 3 + k]) * fRoot;
            }

            return output;
        }
    }


    class Vec4
    {
        
        ///**
        // * Creates a new, empty vec4
        // *
        // * @returns {vec4} a new 4D vector
        // */
        public static double[] Create()
        {
            double[] output = new double[4];
            output[0] = 0;
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            return output;
        }

        ///**
        // * Creates a new vec4 initialized with values from an existing vector
        // *
        // * @param {vec4} a vector to clone
        // * @returns {vec4} a new 4D vector
        // */
        public static double[] CloneIt(double[] a)
        {
            double[] output = new double[4];
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[3];
            return output;
        }

        ///**
        // * Creates a new vec4 initialized with the given values
        // *
        // * @param {Number} x X component
        // * @param {Number} y Y component
        // * @param {Number} z Z component
        // * @param {Number} w W component
        // * @returns {vec4} a new 4D vector
        // */
        public static double[] FromValues(double x, double y, double z, double w)
        {
            double[] output = new double[4];
            output[0] = x;
            output[1] = y;
            output[2] = z;
            output[3] = w;
            return output;
        }

        ///**
        // * Copy the values from one vec4 to another
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the source vector
        // * @returns {vec4} output
        // */
        public static double[] Copy(double[] output, double[] a)
        {
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[3];
            return output;
        }

        ///**
        // * Set the components of a vec4 to the given values
        // *
        // * @param {vec4} output the receiving vector
        // * @param {Number} x X component
        // * @param {Number} y Y component
        // * @param {Number} z Z component
        // * @param {Number} w W component
        // * @returns {vec4} output
        // */
        public static double[] Set(double[] output, double x, double y, double z, double w)
        {
            output[0] = x;
            output[1] = y;
            output[2] = z;
            output[3] = w;
            return output;
        }

        ///**
        // * Adds two vec4's
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @returns {vec4} output
        // */
        public static double[] Add(double[] output, double[] a, double[] b)
        {
            output[0] = a[0] + b[0];
            output[1] = a[1] + b[1];
            output[2] = a[2] + b[2];
            output[3] = a[3] + b[3];
            return output;
        }

        ///**
        // * Subtracts vector b from vector a
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @returns {vec4} output
        // */
        public static double[] Subtract(double[] output, double[] a, double[] b)
        {
            output[0] = a[0] - b[0];
            output[1] = a[1] - b[1];
            output[2] = a[2] - b[2];
            output[3] = a[3] - b[3];
            return output;
        }

        ///**
        // * Alias for {@link vec4.subtract}
        // * @function
        // */
        public static double[] Sub(double[] output, double[] a, double[] b)
        {
            return Subtract(output, a, b);
        }

        ///**
        // * Multiplies two vec4's
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @returns {vec4} output
        // */
        public static double[] Multiply(double[] output, double[] a, double[] b)
        {
            output[0] = a[0] * b[0];
            output[1] = a[1] * b[1];
            output[2] = a[2] * b[2];
            output[3] = a[3] * b[3];
            return output;
        }

        ///**
        // * Alias for {@link vec4.multiply}
        // * @function
        // */
        //vec4.mul = vec4.multiply;
        public static double[] Mul(double[] output, double[] a, double[] b)
        {
            return Multiply(output, a, b);
        }

        ///**
        // * Divides two vec4's
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @returns {vec4} output
        // */
        public static double[] Divide(double[] output, double[] a, double[] b)
        {
            output[0] = a[0] / b[0];
            output[1] = a[1] / b[1];
            output[2] = a[2] / b[2];
            output[3] = a[3] / b[3];
            return output;
        }

        ///**
        // * Alias for {@link vec4.divide}
        // * @function
        // */
        //vec4.div = vec4.divide;
        public static double[] Div(double[] output, double[] a, double[] b)
        {
            return Divide(output, a, b);
        }

        ///**
        // * Returns the minimum of two vec4's
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @returns {vec4} output
        // */
        public static double[] Min(double[] output, double[] a, double[] b)
        {
            output[0] = Math.Min(a[0], b[0]);
            output[1] = Math.Min(a[1], b[1]);
            output[2] = Math.Min(a[2], b[2]);
            output[3] = Math.Min(a[3], b[3]);
            return output;
        }

        ///**
        // * Returns the maximum of two vec4's
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @returns {vec4} output
        // */
        public static double[] Max(double[] output, double[] a, double[] b)
        {
            output[0] = Math.Max(a[0], b[0]);
            output[1] = Math.Max(a[1], b[1]);
            output[2] = Math.Max(a[2], b[2]);
            output[3] = Math.Max(a[3], b[3]);
            return output;
        }

        ///**
        // * Scales a vec4 by a scalar number
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the vector to scale
        // * @param {Number} b amount to scale the vector by
        // * @returns {vec4} output
        // */
        public static double[] Scale(double[] output, double[] a, double b)
        {
            output[0] = a[0] * b;
            output[1] = a[1] * b;
            output[2] = a[2] * b;
            output[3] = a[3] * b;
            return output;
        }

        ///**
        // * Adds two vec4's after scaling the second operand by a scalar value
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @param {Number} scale the amount to scale b by before adding
        // * @returns {vec4} output
        // */
        public static double[] ScaleAndAdd(double[] output, double[] a, double[] b, double scale)
        {
            output[0] = a[0] + (b[0] * scale);
            output[1] = a[1] + (b[1] * scale);
            output[2] = a[2] + (b[2] * scale);
            output[3] = a[3] + (b[3] * scale);
            return output;
        }

        ///**
        // * Calculates the euclidian distance between two vec4's
        // *
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @returns {Number} distance between a and b
        // */
        public static double Distance(double[] a, double[] b)
        {
            double x = b[0] - a[0];
            double y = b[1] - a[1];
            double z = b[2] - a[2];
            double w = b[3] - a[3];
            return GameMath.Sqrt(x * x + y * y + z * z + w * w);
        }

        ///**
        // * Alias for {@link vec4.distance}
        // * @function
        // */
        //vec4.dist = vec4.distance;
        public static double Dist(double[] a, double[] b)
        {
            return Distance(a, b);
        }

        ///**
        // * Calculates the squared euclidian distance between two vec4's
        // *
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @returns {Number} squared distance between a and b
        // */
        public static double SquaredDistance(double[] a, double[] b)
        {
            double x = b[0] - a[0];
            double y = b[1] - a[1];
            double z = b[2] - a[2];
            double w = b[3] - a[3];
            return x * x + y * y + z * z + w * w;
        }

        ///**
        // * Alias for {@link vec4.squaredDistance}
        // * @function
        // */
        public static double SqrDist(double[] a, double[] b)
        {
            return SquaredDistance(a, b);
        }
        ///**
        // * Calculates the length of a vec4
        // *
        // * @param {vec4} a vector to calculate length of
        // * @returns {Number} length of a
        // */
        public static double Length_(double[] a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            double w = a[3];
            return GameMath.Sqrt(x * x + y * y + z * z + w * w);
        }

        ///**
        // * Alias for {@link vec4.length}
        // * @function
        // */
        public static double Len(double[] a)
        {
            return Length_(a);
        }

        ///**
        // * Calculates the squared length of a vec4
        // *
        // * @param {vec4} a vector to calculate squared length of
        // * @returns {Number} squared length of a
        // */
        public static double SquaredLength(double[] a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            double w = a[3];
            return x * x + y * y + z * z + w * w;
        }

        ///**
        // * Alias for {@link vec4.squaredLength}
        // * @function
        // */
        //vec4.sqrLen = vec4.squaredLength;
        public static double SqrLen(double[] a)
        {
            return SquaredLength(a);
        }

        ///**
        // * Negates the components of a vec4
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a vector to negate
        // * @returns {vec4} output
        // */
        public static double[] Negate(double[] output, double[] a)
        {
            output[0] = -a[0];
            output[1] = -a[1];
            output[2] = -a[2];
            output[3] = -a[3];
            return output;
        }

        ///**
        // * Normalize a vec4
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a vector to normalize
        // * @returns {vec4} output
        // */
        public static double[] Normalize(double[] output, double[] a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            double w = a[3];
            double len = x * x + y * y + z * z + w * w;
            if (len > 0)
            {
                double one = 1;
                len = one / GameMath.Sqrt(len);
                output[0] = a[0] * len;
                output[1] = a[1] * len;
                output[2] = a[2] * len;
                output[3] = a[3] * len;
            }
            return output;
        }

        ///**
        // * Calculates the dot product of two vec4's
        // *
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @returns {Number} dot product of a and b
        // */
        public static double Dot(double[] a, double[] b)
        {
            return a[0] * b[0] + a[1] * b[1] + a[2] * b[2] + a[3] * b[3];
        }

        ///**
        // * Performs a linear interpolation between two vec4's
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the first operand
        // * @param {vec4} b the second operand
        // * @param {Number} t interpolation amount between the two inputs
        // * @returns {vec4} output
        // */
        public static double[] Lerp(double[] output, double[] a, double[] b, double t)
        {
            double ax = a[0];
            double ay = a[1];
            double az = a[2];
            double aw = a[3];
            output[0] = ax + t * (b[0] - ax);
            output[1] = ay + t * (b[1] - ay);
            output[2] = az + t * (b[2] - az);
            output[3] = aw + t * (b[3] - aw);
            return output;
        }
        

        ///**
        // * Transforms the vec4 with a mat4.
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the vector to transform
        // * @param {mat4} m matrix to transform with
        // * @returns {vec4} output
        // */
        public static double[] TransformMat4(double[] output, double[] a, double[] m)
        {
            double x = a[0]; double y = a[1]; double z = a[2]; double w = a[3];
            output[0] = m[0] * x + m[4] * y + m[8] * z + m[12] * w;
            output[1] = m[1] * x + m[5] * y + m[9] * z + m[13] * w;
            output[2] = m[2] * x + m[6] * y + m[10] * z + m[14] * w;
            output[3] = m[3] * x + m[7] * y + m[11] * z + m[15] * w;
            return output;
        }

        ///**
        // * Transforms the vec4 with a quat
        // *
        // * @param {vec4} output the receiving vector
        // * @param {vec4} a the vector to transform
        // * @param {quat} q quaternion to transform with
        // * @returns {vec4} output
        // */
        public static double[] transformQuat(double[] output, double[] a, double[] q)
        {
            double x = a[0]; double y = a[1]; double z = a[2];
            double qx = q[0]; double qy = q[1]; double qz = q[2]; double qw = q[3];

            // calculate quat * vec
            double ix = qw * x + qy * z - qz * y;
            double iy = qw * y + qz * x - qx * z;
            double iz = qw * z + qx * y - qy * x;
            double iw = -qx * x - qy * y - qz * z;

            // calculate result * inverse quat
            output[0] = ix * qw + iw * -qx + iy * -qz - iz * -qy;
            output[1] = iy * qw + iw * -qy + iz * -qx - ix * -qz;
            output[2] = iz * qw + iw * -qz + ix * -qy - iy * -qx;
            return output;
        }
        
    }



    /// <summary>
    /// Don't use this class unless you need it to interoperate with Mat4d
    /// </summary>
    public class Vec3Utils
    {
        /// Creates a new, empty vec3
        /// Returns {vec3} a new 3D vector.
        public static double[] Create()
        {
            double[] output = new double[3];
            output[0] = 0;
            output[1] = 0;
            output[2] = 0;
            return output;
        }

        /// Creates a new vec3 initialized with values from an existing vector
        /// Returns {vec3} a new 3D vector
        public static double[] CloneIt(
            /// a vector to clone
            double[] a)
        {
            double[] output = new double[3];
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            return output;
        }

        /// Creates a new vec3 initialized with the given values
        /// Returns {vec3} a new 3D vector
        public static double[] FromValues(
            /// X component
            double x,
            /// Y component
            double y,
            /// Z component
            double z)
        {
            double[] output = new double[3];
            output[0] = x;
            output[1] = y;
            output[2] = z;
            return output;
        }

        /// Copy the values from one vec3 to another
        ///@returns {vec3} out
        public static double[] Copy(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the source vector
            double[] a)
        {
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            return output;
        }

        ///Set the components of a vec3 to the given values
        ///@returns {vec3} out
        public static double[] Set(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {Number} x X component
            double x,
            ////@param {Number} y Y component
            double y,
            ////@param {Number} z Z component
            double z)
        {
            output[0] = x;
            output[1] = y;
            output[2] = z;
            return output;
        }

        ///Adds two vec3's
        ///@returns {vec3} out
        public static double[] Add(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            output[0] = a[0] + b[0];
            output[1] = a[1] + b[1];
            output[2] = a[2] + b[2];
            return output;
        }

        ///Subtracts vector b from vector a
        ///@returns {vec3} out
        public static double[] Substract(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            output[0] = a[0] - b[0];
            output[1] = a[1] - b[1];
            output[2] = a[2] - b[2];
            return output;
        }

        ///Alias for {@link vec3.subtract}
        ///@function
        //vec3.sub = vec3.subtract;
        public static double[] Sub(double[] output, double[] a, double[] b)
        {
            return Substract(output, a, b);
        }

        ///Multiplies two vec3's
        ///@returns {vec3} out
        public static double[] Multiply(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            output[0] = a[0] * b[0];
            output[1] = a[1] * b[1];
            output[2] = a[2] * b[2];
            return output;
        }

        ///Alias for {@link vec3.multiply}
        public static double[] Mul(double[] output, double[] a, double[] b)
        {
            return Multiply(output, a, b);
        }

        ///Divides two vec3's
        ///@returns {vec3} out
        public static double[] Divide(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            output[0] = a[0] / b[0];
            output[1] = a[1] / b[1];
            output[2] = a[2] / b[2];
            return output;
        }

        ///Alias for {@link vec3.divide}
        public static double[] Div(double[] output, double[] a, double[] b)
        {
            return Divide(output, a, b);
        }

        ///Returns the minimum of two vec3's
        ///@returns {vec3} out
        public static double[] Min(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            output[0] = Math.Min(a[0], b[0]);
            output[1] = Math.Min(a[1], b[1]);
            output[2] = Math.Min(a[2], b[2]);
            return output;
        }

        ///Returns the maximum of two vec3's
        ///@returns {vec3} out
        public static double[] Max(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            output[0] = Math.Max(a[0], b[0]);
            output[1] = Math.Max(a[1], b[1]);
            output[2] = Math.Max(a[2], b[2]);
            return output;
        }

        ///Scales a vec3 by a scalar number
        ///@returns {vec3} out
        public static double[] Scale(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the vector to scale
            double[] a,
            ////@param {Number} b amount to scale the vector by
            double b)
        {
            output[0] = a[0] * b;
            output[1] = a[1] * b;
            output[2] = a[2] * b;
            return output;
        }

        ///Adds two vec3's after scaling the second operand by a scalar value
        ///@returns {vec3} out
        public static double[] ScaleAndAdd(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b,
            ////@param {Number} scale the amount to scale b by before adding
            double scale)
        {
            output[0] = a[0] + (b[0] * scale);
            output[1] = a[1] + (b[1] * scale);
            output[2] = a[2] + (b[2] * scale);
            return output;
        }

        ///Calculates the euclidian distance between two vec3's
        ///@returns {Number} distance between a and b
        public static double Distance(
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            double x = b[0] - a[0];
            double y = b[1] - a[1];
            double z = b[2] - a[2];
            return GameMath.Sqrt(x * x + y * y + z * z);
        }

        ///Alias for {@link vec3.distance}
        public static double Dist(double[] a, double[] b)
        {
            return Distance(a, b);
        }

        ///Calculates the squared euclidian distance between two vec3's
        ///@returns {Number} squared distance between a and b
        public static double SquaredDistance(
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            double x = b[0] - a[0];
            double y = b[1] - a[1];
            double z = b[2] - a[2];
            return x * x + y * y + z * z;
        }

        ///Alias for {@link vec3.squaredDistance}
        ///@function
        //vec3.sqrDist = vec3.squaredDistance;
        public static double SqrDist(double[] a, double[] b)
        {
            return SquaredDistance(a, b);
        }

        ///Calculates the length of a vec3
        ///@returns {Number} length of a
        public static double Length_(
            ////@param {vec3} a vector to calculate length of
            double[] a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            return GameMath.Sqrt(x * x + y * y + z * z);
        }

        ///Alias for {@link vec3.length}
        public static double Len(double[] a)
        {
            return Length_(a);
        }

        ///Calculates the squared length of a vec3
        ///@returns {Number} squared length of a
        public static double SquaredLength(
            ////@param {vec3} a vector to calculate squared length of
            double[] a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            return x * x + y * y + z * z;
        }

        ///Alias for {@link vec3.squaredLength}
        public static double SqrLen(double[] a)
        {
            return SquaredLength(a);
        }

        ///Negates the components of a vec3
        ///@returns {vec3} out
        public static double[] Negate(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a vector to negate
            double[] a)
        {
            output[0] = 0 - a[0];
            output[1] = 0 - a[1];
            output[2] = 0 - a[2];
            return output;
        }

        ///Normalize a vec3
        ///@returns {vec3} out
        public static double[] Normalize(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a vector to normalize
            double[] a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            double len = x * x + y * y + z * z;
            if (len > 0)
            {
                //TODO: evaluate use of glm_invsqrt here?
                double one = 1;
                len = one / GameMath.Sqrt(len);
                output[0] = a[0] * len;
                output[1] = a[1] * len;
                output[2] = a[2] * len;
            }
            return output;
        }

        ///Calculates the dot product of two vec3's
        ///@returns {Number} dot product of a and b
        public static double Dot(
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
        }

        ///Computes the cross product of two vec3's
        ///@returns {vec3} out
        public static double[] Cross(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b)
        {
            double ax = a[0];
            double ay = a[1];
            double az = a[2];
            double bx = b[0];
            double by = b[1];
            double bz = b[2];

            output[0] = ay * bz - az * by;
            output[1] = az * bx - ax * bz;
            output[2] = ax * by - ay * bx;

            return output;
        }

        ///Performs a linear interpolation between two vec3's
        ///@returns {vec3} out
        public static double[] Lerp(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the first operand
            double[] a,
            ////@param {vec3} b the second operand
            double[] b,
            ////@param {Number} t interpolation amount between the two inputs
            double t)
        {
            double ax = a[0];
            double ay = a[1];
            double az = a[2];
            output[0] = ax + t * (b[0] - ax);
            output[1] = ay + t * (b[1] - ay);
            output[2] = az + t * (b[2] - az);
            return output;
        }
        

        ////Transforms the vec3 with a mat4.
        ////4th vector component is implicitly '1'
        ////@returns {vec3} out
        public static double[] TransformMat4(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the vector to transform
            double[] a,
            ////@param {mat4} m matrix to transform with
            double[] m)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            output[0] = m[0] * x + m[4] * y + m[8] * z + m[12];
            output[1] = m[1] * x + m[5] * y + m[9] * z + m[13];
            output[2] = m[2] * x + m[6] * y + m[10] * z + m[14];
            return output;
        }

        ///Transforms the vec3 with a mat3.
        ///@returns {vec3} out
        public static double[] TransformMat3(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the vector to transform
            double[] a,
            ////@param {mat4} m the 3x3 matrix to transform with
            double[] m)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            output[0] = x * m[0] + y * m[3] + z * m[6];
            output[1] = x * m[1] + y * m[4] + z * m[7];
            output[2] = x * m[2] + y * m[5] + z * m[8];
            return output;
        }

        ///Transforms the vec3 with a quat
        ///@returns {vec3} out
        //    // benchmarks: http://jsperf.com/quaternion-transform-vec3-implementations
        public static double[] TransformQuat(
            ////@param {vec3} out the receiving vector
            double[] output,
            ////@param {vec3} a the vector to transform
            double[] a,
            ////@param {quat} q quaternion to transform with
            double[] q)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];

            double qx = q[0];
            double qy = q[1];
            double qz = q[2];
            double qw = q[3];

            // calculate quat * vec
            double ix = qw * x + qy * z - qz * y;
            double iy = qw * y + qz * x - qx * z;
            double iz = qw * z + qx * y - qy * x;
            double iw = (0 - qx) * x - qy * y - qz * z;

            // calculate result * inverse quat
            output[0] = ix * qw + iw * (0 - qx) + iy * (0 - qz) - iz * (0 - qy);
            output[1] = iy * qw + iw * (0 - qy) + iz * (0 - qx) - ix * (0 - qz);
            output[2] = iz * qw + iw * (0 - qz) + ix * (0 - qy) - iy * (0 - qx);
            return output;
        }
        
    }

}
