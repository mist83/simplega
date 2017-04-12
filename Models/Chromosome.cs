using System.Windows.Media;

namespace Pointillism.Models
{
    public class Chromosome
    {
        public Chromosome(int x, int y, int blockSize)
        {
            X = x * blockSize;
            Y = y * blockSize;

            Radius = blockSize;

            var gray = (byte)Utility.Random.Next(256);
            Color = Color.FromRgb(gray, gray, gray);
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Radius { get; set; }

        public Color Color { get; set; }

        public override string ToString()
        {
            return $"{X},{Y}/{Radius}: {Color}";
        }
    }
}
