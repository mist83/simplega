using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Pointillism.Models
{
    public class Chromosome
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }
        public Color Color { get; set; }
        public double Opacity { get; set; }

        public static Chromosome Random() // TODO: Maybe only make one thing change at a time?
        {
            var colors = new[] { Colors.Cyan, Colors.Magenta, Colors.Yellow, Colors.Black };
            //var colors = new[] { Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0), Color.FromRgb(0, 0, 255) };

            var chromosome = new Chromosome
            {
                X = Utility.Random.Next((int)Utility.Canvas.ActualWidth),
                Y = Utility.Random.Next((int)Utility.Canvas.ActualHeight),
                //Radius = 20,
                Radius = Utility.Random.Next(3, 21),
                //Color = colors[Utility.Random.Next(colors.Length)],
                Color = Color.FromRgb((byte)Utility.Random.Next(256), (byte)Utility.Random.Next(256), (byte)Utility.Random.Next(256)),
                //Opacity = Utility.Random.Next(100) / 100.0
                Opacity = 1,
            };

            return chromosome;
        }

        public override string ToString()
        {
            return $"{X},{Y}/{Radius}: {Color}/{Opacity}";
        }
    }
}
