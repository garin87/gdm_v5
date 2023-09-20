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
    }
}
