using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSane.ConsoleForms
{
    public static class Utils
    {
        public static void UOut(ConsoleColor foregroundColor, string message, ConsoleColor backgroundColor = ConsoleColor.Black, params object[] formatParams)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Out.WriteLine(message, formatParams);
            Console.ResetColor();
        }
    }
}
