using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace Pointillism
{
    public static class Utility
    {
        static Utility()
        {
            Random = new Random();
            SaveDirectory = Path.Combine(Path.GetTempPath(), "Pointillism");

            if (Directory.Exists(SaveDirectory))
            {
                Debugger.Break();
                Directory.Delete(SaveDirectory, true);
            }

            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
        }

        public static Random Random { get; }

        public static string SaveDirectory { get; }

        public static Canvas Canvas { get; set; }

        public static int Generation { get; set; }
    }
}
