using System;

namespace RevStack.Payment
{
    public static partial class Extensions
    {
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToNearestDollar(this decimal value)
        {
            return Math.Round(value, MidpointRounding.AwayFromZero).ToString("C2");
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatAsCurrency"></param>
        /// <returns></returns>
        public static string ToNearestDollar(this decimal value, bool formatAsCurrency)
        {
            if (formatAsCurrency)
            {
                return Math.Round(value, MidpointRounding.AwayFromZero).ToString("C2");
            }
            return Math.Round(value, MidpointRounding.AwayFromZero).ToString();
        }
    }
}
