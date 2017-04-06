using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pointillism.Models
{
    public class Population : List<Individual>
    {
        int size;

        public Population(int size)
        {
            this.size = size;

            for (var i = 0; i < size; i++)
            {
                var individual = new Individual(1000, Enumerable.Repeat(1, 1000).Select(x => Chromosome.Random()).ToArray());
                Add(individual);
            }
        }

        private Population(params Individual[] individuals)
        {
            size = individuals.Length;
            AddRange(individuals);
        }

        public void Save()
        {
            var generationDirectory = Path.Combine(Utility.SaveDirectory, $"Generation_{Utility.Generation:D6}");
            Directory.CreateDirectory(generationDirectory);

            for (var i = 0; i < Count; i++)
            {
                var lines = new List<string>();
                foreach (var chromosome in this[i].Chromosomes)
                {
                    lines.Add($"{chromosome.X},{chromosome.Y},{chromosome.Radius},{chromosome.Color}");
                }

                var individualFile = Path.Combine(generationDirectory, $"{i:D4}.csv");
                using (var writer = new StreamWriter(individualFile))
                {
                    lines.ForEach(x => writer.WriteLine(x));
                }
            }
        }

        private Population Rank()
        {
            // ones closest to 0 are the most "fit"
            var scored = this.OrderBy(x => Math.Abs(x.Fitness)).ToArray();
            return new Population(scored);
        }

        public Population Breed()
        {
            var ranked = Rank().Take((int)(0.8 * size)).ToList(); // bottom % of population "die"
            Console.WriteLine($"{Utility.Generation.ToString().PadRight(6)}Best/Worst/Total/Average: {Math.Abs(ranked[0].Fitness).ToString().PadLeft(15)}{Math.Abs(ranked[ranked.Count - 1].Fitness).ToString().PadLeft(15)}{ranked.Sum(x => Math.Abs((long)x.Fitness)).ToString().PadLeft(15)}{((long)ranked.Average(x => Math.Abs((long)x.Fitness))).ToString().PadLeft(15)}");
            var nextGeneration = new List<Individual>();
            while (nextGeneration.Count < Count)
            {
                var mom = ranked[Utility.Random.Next(ranked.Count)];
                var pop = ranked[Utility.Random.Next(ranked.Count)];

                var child = Individual.Breed(mom, pop);
                nextGeneration.Add(child);
            }

            return new Population(nextGeneration.ToArray());
        }
    }
}
