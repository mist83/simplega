using System;
using System.Collections.Generic;
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
                var individual = new Individual(1000, Enumerable.Repeat(1, 1000).Select(x => Chromosome.Random()).ToArray());
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
        }

        private Population Rank()
        {
            // ones closest to 0 are the most "fit"
            var scored = Individuals.OrderBy(x => Math.Abs(x.Fitness)).ToArray();
            return new Population(scored);
        }

        public Population Breed()
        {
            var ranked = Rank().Individuals.Take((int)(0.8 * size)).ToList(); // bottom % of population "die"

            var best = Math.Abs(ranked[0].Fitness);
            var worst = Math.Abs(ranked[ranked.Count - 1].Fitness);
            var total = ranked.Sum(x => Math.Abs((long)x.Fitness));
            var average = ((long)ranked.Average(x => Math.Abs((long)x.Fitness)));

            Console.WriteLine($"{Utility.Generation.ToString().PadRight(6)}Best/Worst/Total/Average: {best.ToString().PadLeft(15)}{worst.ToString().PadLeft(15)}{total.ToString().PadLeft(15)}{average.ToString().PadLeft(15)}");
            var nextGeneration = new List<Individual>();
            while (nextGeneration.Count < Individuals.Count)
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
