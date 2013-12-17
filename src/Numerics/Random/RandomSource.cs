﻿// <copyright file="AbstractRandomNumberGenerator.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
// 
// Copyright (c) 2009-2013 Math.NET
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using System.Collections.Generic;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.Random
{
    /// <summary>
    /// Base class for random number generators. This class introduces a layer between <see cref="System.Random"/>
    /// and the Math.Net Numerics random number generators to provide thread safety.
    /// When used directly it use the System.Random as random number source.
    /// </summary>
    public class RandomSource : System.Random
    {
        readonly bool _threadSafe;
        readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSource"/> class using
        /// the value of <see cref="Control.ThreadSafeRandomNumberGenerators"/> to set whether
        /// the instance is thread safe or not.
        /// </summary>
        public RandomSource() : base(RandomSeed.Guid())
        {
            _threadSafe = Control.ThreadSafeRandomNumberGenerators;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSource"/> class.
        /// </summary>
        /// <param name="threadSafe">if set to <c>true</c> , the class is thread safe.</param>
        /// <remarks>Thread safe instances are two and half times slower than non-thread
        /// safe classes.</remarks>
        public RandomSource(bool threadSafe) : base(RandomSeed.Guid())
        {
            _threadSafe = threadSafe;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSource"/> class using
        /// the value of <see cref="Control.ThreadSafeRandomNumberGenerators"/> to set whether
        /// the instance is thread safe or not.
        /// </summary>
        /// <param name="systemRandomSeed">The seed value.</param>
        public RandomSource(int systemRandomSeed) : base(systemRandomSeed)
        {
            _threadSafe = Control.ThreadSafeRandomNumberGenerators;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSource"/> class.
        /// </summary>
        /// <param name="systemRandomSeed">The seed value.</param>
        /// <param name="threadSafe">if set to <c>true</c> , the class is thread safe.</param>
        /// <remarks>Thread safe instances are two and half times slower than non-thread
        /// safe classes.</remarks>
        public RandomSource(int systemRandomSeed, bool threadSafe) : base(systemRandomSeed)
        {
            _threadSafe = threadSafe;
        }

        /// <summary>
        /// Returns an array of uniformly distributed random doubles in the interval [0.0,1.0].
        /// </summary>
        /// <param name="n">The size of the array.</param>
        /// <returns>
        /// An array of uniformly distributed random doubles in the interval [0.0,1.0].
        /// </returns>
        /// <exception cref="ArgumentException">if n is not greater than 0.</exception>
        public double[] NextDoubles(int n)
        {
            if (n < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive);
            }

            var ret = new double[n];
            if (_threadSafe)
            {
                lock (_lock)
                {
                    for (var i = 0; i < ret.Length; i++)
                    {
                        ret[i] = DoSample();
                    }
                }
            }
            else
            {
                for (var i = 0; i < ret.Length; i++)
                {
                    ret[i] = DoSample();
                }
            }
            return ret;
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero and less than <see cref="F:System.Int32.MaxValue"/>.
        /// </returns>
        public override sealed int Next()
        {
            if (_threadSafe)
            {
                lock (_lock)
                {
                    return (int)(DoSample()*int.MaxValue);
                }
            }

            return (int)(DoSample()*int.MaxValue);
        }

        /// <summary>
        /// Returns a random number less then a specified maximum.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns>A 32-bit signed integer less than <paramref name="maxValue"/>.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="maxValue"/> is negative. </exception>
        public override sealed int Next(int maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMustBePositive);
            }

            if (_threadSafe)
            {
                lock (_lock)
                {
                    return (int)(DoSample()*maxValue);
                }
            }

            return (int)(DoSample()*maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to <paramref name="minValue"/> and less than <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but not <paramref name="maxValue"/>. If <paramref name="minValue"/> equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>. </exception>
        public override sealed int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMinValueGreaterThanMaxValue);
            }

            if (_threadSafe)
            {
                lock (_lock)
                {
                    return (int)(DoSample()*(maxValue - minValue)) + minValue;
                }
            }

            return (int)(DoSample()*(maxValue - minValue)) + minValue;
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception>
        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (_threadSafe)
            {
                lock (_lock)
                {
                    for (var i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = (byte)(((int)(DoSample()*int.MaxValue))%256);
                    }
                }
                return;
            }

            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(((int)(DoSample()*int.MaxValue))%256);
            }
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
        protected override double Sample()
        {
            if (_threadSafe)
            {
                lock (_lock)
                {
                    return DoSample();
                }
            }

            return DoSample();
        }

        /// <summary>
        /// Thread safe version of <seealso cref="DoSample"/> which returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number greater than or equal to 0.0, and less than 1.0</returns>
        double ThreadSafeSample()
        {
            lock (_lock)
            {
                return DoSample();
            }
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// A double-precision floating point number greater than or equal to 0.0, and less than 1.0.
        /// </returns>
        protected virtual double DoSample()
        {
            return base.Sample();
        }

        /// <summary>
        /// Returns an array of random numbers greater than or equal to 0.0 and less than 1.0.
        /// </summary>
        public static double[] Samples(int length, int systemRandomSeed)
        {
            var rnd = new System.Random(systemRandomSeed);

            var data = new double[length];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = rnd.NextDouble();
            }
            return data;
        }

        /// <summary>
        /// Returns an infinite sequence of random numbers greater than or equal to 0.0 and less than 1.0.
        /// </summary>
        public static IEnumerable<double> SampleSequence(int systemRandomSeed)
        {
            var rnd = new System.Random(systemRandomSeed);

            while (true)
            {
                yield return rnd.NextDouble();
            }
        }
    }
}
