using System;

namespace gdm5._0.Extensions
{
    public static class MathExtensions
    {

        public static double Test = 9;
        public static double ParseDouble(this string valueText)
        {
            double dd = 0;
            if (!String.IsNullOrEmpty(valueText))
            {
                double d;
                bool result = double.TryParse(valueText, out d);

                if (result)
                    dd = d;
                else
                    dd = 0;
            }
            else dd = 0;

            return dd;
        }
        public static float ParseFloat(this string valueText)
        {
            float dd = 0;
            if (!String.IsNullOrEmpty(valueText))
            {
                float d;
                bool result = float.TryParse(valueText, out d);

                if (result)
                    dd = d;
                else
                    dd = 0;
            }
            else dd = 0;

            return dd;
        }
        public static int ParseInt(this string valueText)
        {
            int dd = 0;
            if (!String.IsNullOrEmpty(valueText))
            {
                int d;
                bool result = int.TryParse(valueText, out d);

                if (result)
                    dd = d;
                else
                    dd = 0;
            }
            else dd = 0;

            return dd;
        }

        //public static float Round(this float? value, int numberOfValuesAfterDot = 3)
        //{
        //    var nonNullableValue = value ?? 0;
        //    return nonNullableValue != 0
        //        ? nonNullableValue.Round(numberOfValuesAfterDot)
        //        : nonNullableValue;
        //}
    }
}

