using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pointillism.Models
{
    public class Individual
    {
        public List<Chromosome> Chromosomes { get; } = new List<Chromosome>();

        private static int[,,] target = new int[256, 256, 3];

        static Individual()
        {
            var comparison = new LockBitmap(new System.Drawing.Bitmap(@"Z:\Users\Michael\Documents\Visual Studio 2015\Projects\Pointillism\Pointillism\images\TestImage.png"));
            comparison.LockBits();
            for (var x = 0; x < comparison.Width; x++)
            {
                for (var y = 0; y < comparison.Height; y++)
                {
                    var pixel = comparison.GetPixel(x, y);
                    //target[x, y] = pixel.ToArgb();
                    target[x, y, 0] = pixel.R;
                    target[x, y, 1] = pixel.G;
                    target[x, y, 2] = pixel.B;
                }
            }
        }

        public Individual(int size, Chromosome[] chromosomes)
        {
            Chromosomes.AddRange(chromosomes);
        }

        public int Fitness { get; private set; }

        public static Individual Breed(Individual mom, Individual pop)
        {
            var child = new Individual(1000, Enumerable.Repeat(new Chromosome(), 1000).ToArray());

            var mutations = Enumerable.Repeat(1, mom.Chromosomes.Count).Select(x => Utility.Random.Next(100) < 5).ToArray();
            var trues = mutations.Count(x => x);
            for (var i = 0; i < mom.Chromosomes.Count; i++)
            {
                // 10% of chromosomes get mutated
                if (mutations[i])
                {
                    child.Chromosomes[i] = Chromosome.Random();
                    continue;
                }

                // The rest are either from mom or dad, randomly
                var momOrPop = Utility.Random.Next(2);
                child.Chromosomes[i] = momOrPop == 0 ? mom.Chromosomes[i] : pop.Chromosomes[i];
            }

            return child;
        }

        public void Score()
        {
            int[,,] health = new int[256, 256, 3];

            var canvas = Utility.Canvas;

            int Height = (int)canvas.ActualHeight;
            int Width = (int)canvas.ActualWidth;

            RenderTargetBitmap bmp = new RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32);

            canvas.Measure(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight));
            canvas.Arrange(new Rect(new Size((int)canvas.ActualWidth, (int)canvas.ActualHeight)));
            bmp.Render(canvas);

            BitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bmp));

            var stm = new MemoryStream();
            encoder.Save(stm);

            var b = new System.Drawing.Bitmap(stm);

            var lb = new LockBitmap(b);
            lb.LockBits();
            for (var y = 0; y < b.Height; y++)
            {
                for (var x = 0; x < b.Width; x++)
                {
                    var pixel = lb.GetPixel(x, y);
                    //health[x, y] = pixel.ToArgb();
                    health[x, y, 0] = pixel.R;
                    health[x, y, 1] = pixel.G;
                    health[x, y, 2] = pixel.B;
                }
            }

            var fitness = 0;

            for (var x = 0; x < 256; x++)
            {
                for (var y = 0; y < 256; y++)
                {
                    var targetR = target[x, y, 0];
                    var targetG = target[x, y, 1];
                    var targetB = target[x, y, 1];

                    var healthR = health[x, y, 0];
                    var healthG = health[x, y, 1];
                    var healthB = health[x, y, 1];

                    var diffR = Math.Abs(targetR - healthR);
                    var diffG = Math.Abs(targetG - healthG);
                    var diffB = Math.Abs(targetB - healthB);

                    var diff = diffR + diffG + diffB;
                    fitness += (diff);
                }
            }

            Fitness = fitness;
        }

        public override string ToString()
        {
            return $"Fitness: {Fitness}";
        }
    }
}
