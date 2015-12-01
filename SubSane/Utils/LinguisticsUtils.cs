using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSane.Utils
{
    public static class LinguisticsUtils
    {
        public static string GetFirstLetter(string name)
        {
            name = name.ToUpper();
            string theFirstLetter = "";
            if (name.StartsWith("THE "))
                theFirstLetter = name.Substring(4, 1);
            else if (name.StartsWith("LOS "))
                theFirstLetter = name.Substring(4, 1);
            else if (name.StartsWith("LAS "))
                theFirstLetter = name.Substring(4, 1);
            else if (name.StartsWith("LES "))
                theFirstLetter = name.Substring(4, 1);
            else if (name.StartsWith("LA "))
                theFirstLetter = name.Substring(3, 1);
            else
                theFirstLetter = name.Substring(0, 1); // No leading word

            return theFirstLetter.ToUpper();
        }
    }
}
