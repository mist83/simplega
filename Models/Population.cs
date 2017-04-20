using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Pointillism.Models
{
    public class Population
    {
        public List<Individual> Individuals { get; } = new List<Individual>();

        int size;

        public Population(int size)
        {
            this.size = size;

            for (var i = 0; i < size; i++)
            {
                var chromosomeCount = 32 * 32;

                var chromosomes = new List<Chromosome>();
                for (int y = 0; y < Math.Sqrt(chromosomeCount); y++)
                {
                    for (int x = 0; x < Math.Sqrt(chromosomeCount); x++)
                    {
                        var c = new Chromosome(x, y, 32 / (int)Math.Sqrt(chromosomeCount));
                        chromosomes.Add(c);
                    }
                }

                var individual = new Individual(chromosomes.ToArray());
                Individuals.Add(individual);
            }
        }

        private Population(params Individual[] individuals)
        {
            size = individuals.Length;
            Individuals.AddRange(individuals);
        }

        public void Save()
        {
            if ("a"[0] == 'a')
                return;

            var generationDirectory = Path.Combine(Utility.SaveDirectory, $"Generation_{Utility.Generation:D6}");
            Directory.CreateDirectory(generationDirectory);

            for (var i = 0; i < Individuals.Count; i++)
            {
                var lines = new List<string>();
                foreach (var chromosome in Individuals[i].Chromosomes)
                {
                    lines.Add($"{chromosome.X},{chromosome.Y},{chromosome.Radius},{chromosome.Color}");
                }

                var individualFile = Path.Combine(generationDirectory, $"{i:D4}.csv");
                using (var writer = new StreamWriter(individualFile))
                {
                    lines.ForEach(x => writer.WriteLine(x));
                }
            }

            Process.Start(generationDirectory);
        }

        private Population Rank()
        {
            // ones closest to 0 are the most "fit"
            var scored = Individuals.OrderBy(x => Math.Abs(x.Fitness)).ToArray();
            return new Population(scored);
        }

        List<int> indexes = null;

        public string FittestFile { get; set; }

        public string Best { get; set; }
        public string Worst{ get; set; }
        public string Total { get; set; }
        public string Average { get; set; }

        public Population Breed()
        {
            if (indexes == null)
            {
                indexes = new List<int>();
                for (var i = 0; i < Individuals.Count; i++)
                {
                    for (var j = 0; j <= i; j++)
                        indexes.Add(j);
                }

                indexes.Sort();
            }

            var ranked = Rank().Individuals.ToList();



            var bitmap = new System.Drawing.Bitmap(32, 32);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            foreach (var chromosome in ranked.First().Chromosomes)
            {
                var argb = (chromosome.Color.A << 24) | (chromosome.Color.R << 16) | (chromosome.Color.G << 8) | chromosome.Color.B;

                var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(argb));
                graphics.FillRectangle(brush, chromosome.X, chromosome.Y, chromosome.Radius, chromosome.Radius);
            }

            FittestFile = Path.GetTempFileName() + ".ga.png";
            bitmap.Save(FittestFile, System.Drawing.Imaging.ImageFormat.Png);

            Best = ((int)Math.Abs(ranked[0].Fitness)).ToString("n0");
            Worst = ((int)Math.Abs(ranked[ranked.Count - 1].Fitness)).ToString("n0");
            Total = ranked.Sum(x => Math.Abs((long)x.Fitness)).ToString("n0");
            Average = (((long)ranked.Average(x => Math.Abs((long)x.Fitness)))).ToString("n0");

            Console.WriteLine($"{Utility.Generation.ToString().PadRight(6)}Best/Worst/Total/Average: {Best.PadLeft(15)}{Worst.PadLeft(15)}{Total.PadLeft(15)}{Average.PadLeft(15)}");
            var nextGeneration = new List<Individual>();
            while (nextGeneration.Count < Individuals.Count)
            {
                var twoRandoms = new[] { indexes[Utility.Random.Next(indexes.Count)], indexes[Utility.Random.Next(indexes.Count)] };

                var momIndex = (twoRandoms[0] * twoRandoms[0]) / ranked.Count;
                var popIndex = (twoRandoms[1] * twoRandoms[1]) / ranked.Count;
                var mom = ranked[momIndex];
                var pop = ranked[popIndex];

                var child = Individual.Breed(mom, pop);
                nextGeneration.Add(child);
            }

            return new Population(nextGeneration.ToArray());
        }
    }
}
