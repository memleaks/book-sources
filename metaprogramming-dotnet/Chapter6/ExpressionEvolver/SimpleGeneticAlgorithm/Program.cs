using GeneticAlgorithm;
using Spackle;
using System;
using System.Linq;

namespace SimpleGeneticAlgorithm
{
	class Program
	{
		private const double CrossoverProbability = 0.7;
		private const int GenerationCount = 200;
		private const double MutationProbability = 0.001;
		private const int RunCount = 100;

		static void Main()
		{
			Program.RunSimpleGA();
			Console.Out.WriteLine("Press return to continue...");
			Console.In.ReadLine();
		}

		private static void PrintPopulation(Population<string> population, int generationCount)
		{
			var best = (from chromosome in population.Chromosomes
							orderby chromosome.Fitness descending
							select chromosome).Take(1).FirstOrDefault();
			Console.Out.WriteLine(generationCount + "," + population.FitnessSummary + "," +
				population.FitnessAverage + "," + best.Fitness + ", " + best.Value.ToString());
		}

		private static void RunSimpleGA()
		{
			var ga = new GeneticAlgorithm<string>(new SimpleGeneticAlgorithmParameters());

			var generationCount = 1;

			var generationCompletedHandler = new EventHandler<EventArgs<Population<string>>>(
				(sender, e) =>
				{
					Program.PrintPopulation(e.Value, generationCount);
					generationCount++;
				});
			ga.GenerationRunCompleted += generationCompletedHandler;
			ga.Run();
			ga.GenerationRunCompleted -= generationCompletedHandler;
			Console.Out.WriteLine(ga.WasOptimalSolutionFound);
		}
	}
}
