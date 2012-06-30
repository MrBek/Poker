using System;

namespace Poker.Common.Conversion
{
    public static class ArrayConverters
    {
        public static void FloatToShort(float[] input,int count,short[] output)
        {
            for (var i = 0; i < count; ++i )
            {
                output[i] = (short)(input[i] * (float)short.MaxValue);
            }
        }

        public static void ShortToFloat(short[] input,int count,float[] output)
        {
            for (var i = 0; i < count; ++i)
            {
                output[i] = input[i] / (float)short.MaxValue;
            }
        }

        public static void GetBytes(short[] input,int count,byte[] output)
        {
            for (var i = 0; i < count; ++i )
            {
                var bytes = BitConverter.GetBytes(input[i]);

                output[i * 2] = bytes[0];
                output[i * 2 +1] = bytes[1];
            }
        }

        public static void GetShorts(ArraySegment<byte> input,ArraySegment<short> output )
        {
            
        }
    }
}