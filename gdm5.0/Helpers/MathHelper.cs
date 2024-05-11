using gdm5._0.DTO;
using gdm5._0.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Helpers
{
    public class MathHelper
    {
        public static double ParseDouble(string valueText)
        {
            if (String.IsNullOrEmpty(valueText))
                return 0;

            if (double.TryParse(valueText, out double result))
                return result;

            return 0;
        }

        public static int ParseInt(string valueText)
        {
            if (String.IsNullOrEmpty(valueText))
                return 0;

            if (int.TryParse(valueText, out int result))
                return result;

            return 0;
        }

        public static void CallParseDouble(string valueText, out Double dd)
        {
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
        }

        public static DateTime DateTimeNowWithOffset()
        {
            return DateTime.UtcNow.AddHours(3);
        }
        
    }
}
