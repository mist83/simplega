using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Pointillism.Models
{
    public class Individual
    {
        public List<Chromosome> Chromosomes { get; } = new List<Chromosome>();

        public static Color[,] target = new Color[32, 32];

        static Individual()
        {
            Bitmap nb = new Bitmap(32,32);
            var g = Graphics.FromImage(nb);

            var comparison = new LockBitmap(new Bitmap(@"Z:\Users\Michael\Documents\Visual Studio 2015\Projects\Pointillism\Pointillism\images\TestImage.png"));
            comparison.LockBits();
            //for (var y = 0; y < comparison.Height; y++)
            //{
            //    for (var x = 0; x < comparison.Width; x++)
            //    {
            //        var pixel = comparison.GetPixel(x, y);
            //        target[y, x] = pixel;
            //    }
            //}

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
            Chromosomes.AddRange(chromosomes);
        }

        public double Fitness { get; private set; }

        public System.Drawing.Color[,] GrayScales { get; set; }

        public static Individual Breed(Individual mom, Individual pop)
        {
            var child = new Individual(Enumerable.Repeat(default(Chromosome), mom.Chromosomes.Count).ToArray());

            //var mutations = Enumerable.Repeat(1, mom.Chromosomes.Count).Select(x => Utility.Random.Next(10000) < 15).ToArray();
            var mutations = Enumerable.Repeat(1, mom.Chromosomes.Count).Select(x => Utility.Random.Next(1000) < 7).ToArray();
            var trues = mutations.Count(x => x);
            for (var i = 0; i < mom.Chromosomes.Count; i++)
            {
                // 10% of chromosomes get mutated
                if (mutations[i])
                {
                    //child.Chromosomes[i] = mom.Chromosomes[i];
                    //child.Chromosomes[i].Color = new Chromosome(-1, -1, -1).Color;
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
            //int[,,] health = new int[256, 256, 3];

            //var stream = (MemoryStream)Utility.Canvas.Tag;
            //var b = new Bitmap(stream);

            //var fitness = 0.0;

            //var lb = new LockBitmap(b);
            //lb.LockBits();
            //for (var y = 0; y < b.Height; y++)
            //{
            //    for (var x = 0; x < b.Width; x++)
            //    {
            //        var pixel = lb.GetPixel(x, y);

            //        var dist = ColorDistanceGray(target[y, x], pixel);
            //        fitness += Math.Pow(dist, 2); // bigger differences have more weight
            //    }
            //}

            var fitness = 0.0;

            for (var y = 0; y <= GrayScales.GetUpperBound(0); y++)
            {
                for (var x = 0; x <= GrayScales.GetUpperBound(1); x++)
                {
                    var pixel = GrayScales[y, x];

                    var dist = ColorDistanceGray(target[y, x], pixel);
                    if(dist < 0)
                    {

                    }else
                    {

                    }
                    fitness += Math.Pow(dist, 2); // bigger differences have more weight
                }
            }

            Fitness = fitness;
        }

        private static Dictionary<string, double> cache = new Dictionary<string, double>();

        public static Color GetGrayColor(Color c)
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

        double ColourDistance(Color e1, Color e2)
        {
            long rmean = (e1.R + e2.R) / 2;
            long r = e1.R - e2.R;
            long g = e1.G - e2.G;
            long b = e1.B - e2.B;

            return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
        }

        public override string ToString()
        {
            return $"Fitness: {Fitness}";
        }
    }
}
