using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Pointillism.Models
{
    public class Individual
    {
        public Chromosome[] Chromosomes { get; private set; }

        public static Color[,] target = new Color[32, 32];

        static Individual()
        {
            Bitmap nb = new Bitmap(32, 32);
            var g = Graphics.FromImage(nb);

            var landscape = System.IO.Path.GetTempFileName() + ".png";
            using (var stream = new FileStream(landscape, FileMode.Create))
            {
                typeof(MainWindow).Assembly.GetManifestResourceStream("Pointillism.images.Landscape.png").CopyTo(stream);
            }

            var comparison = new Bitmap(landscape);
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    var grayTotal = 0.0;

                    for (var y0 = y * 8; y0 < y * 8 + 8; y0++)
                    {
                        for (var x0 = x * 8; x0 < x * 8 + 8; x0++)
                        {
                            var pixel = comparison.GetPixel(x0, y0);
                            var gray = GetGrayColor(pixel).R;
                            grayTotal += gray;
                        }
                    }

                    var grayAverage = (int)(grayTotal / 64.0);
                    target[y, x] = Color.FromArgb(255, grayAverage, grayAverage, grayAverage);
                    g.FillRectangle(new SolidBrush(target[y, x]), x, y, 1, 1);
                }
            }

            var temp = Path.GetTempFileName();
            nb.Save(temp, System.Drawing.Imaging.ImageFormat.Png);
            Utility.Original = temp;
        }

        public Individual(Chromosome[] chromosomes)
        {
            Chromosomes=chromosomes;
        }

        public double Fitness { get; private set; }

        public System.Drawing.Color[,] GrayScales { get; set; }

        public static Individual Breed(Individual mom, Individual pop)
        {
            var child = new Individual(Enumerable.Repeat(default(Chromosome), mom.Chromosomes.Length).ToArray());

            // mom and dad have same # of chromosomes, I could have just as easily used dad.Chromosomes.Count
            for (var i = 0; i < mom.Chromosomes.Length; i++)
            {
                // 0.2% of chromosomes get mutated (random chromosome)
                if (Utility.Random.Next(1000) < 2)
                {
                    child.Chromosomes[i] = new Chromosome(mom.Chromosomes[i].X, mom.Chromosomes[i].Y, 1);
                    continue;
                }

                // The rest are either from mom or pop, randomly
                var momOrPop = Utility.Random.Next(2);
                child.Chromosomes[i] = momOrPop == 0 ? mom.Chromosomes[i] : pop.Chromosomes[i];
            }

            return child;
        }

        public void Score()
        {
            var fitness = 0.0;

            for (var y = 0; y <= GrayScales.GetUpperBound(0); y++)
            {
                for (var x = 0; x <= GrayScales.GetUpperBound(1); x++)
                {
                    var pixel = GrayScales[y, x];

                    var dist = ColorDistanceGray(target[y, x], pixel);

                    // square it so that bigger differences have more weight, positive diff and negative diff are treated the same
                    fitness += Math.Pow(dist, 2);
                }
            }

            Fitness = fitness;
        }

        #region Convert a color to grayscale

        private static Color GetGrayColor(Color c)
        {
            int gray = (int)(.2989 * c.R + .5870 * c.G + .1140 * c.B);
            return Color.FromArgb(c.A, gray, gray, gray);
        }

        double ColorDistanceGray(Color c1, Color c2)
        {
            var gc1 = GetGrayColor(c1);
            var gc2 = GetGrayColor(c2);

            return gc1.R - gc2.R;
        }

        #endregion

        public override string ToString()
        {
            return $"Fitness: {Fitness}";
        }
    }
}
