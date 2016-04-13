using ExpressionBaker;
using GeneticAlgorithm;
using Spackle;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionEvolver.Client.CommandLine
{
	internal static class Program
	{
		static void Main()
		{
			//a => ((Math.Pow(a, 0.5) * a * a) + a);
			//a => (a + a) * ((a + a) / (a * a));
			//a => a * a * a;
			//a => a + a + a + Math.Pow(a, 0.5) + (a * a * a);
			//a => a - (a - Math.Pow(a,  0.5) + (a * a * Math.Pow(a, 0.5)) - a);
			//a => a - (Math.Pow(a, 0.5));
			//a => (4 - a) + (Math.Pow(a, 0.5) * 3);
			//a => (4 - a) + 3;
			//a => ((a / 4) - a) + (Math.Pow((33 * a), 0.5));
			//a => (1000 / Math.Pow(a, 0.5) * (a / 54));
			//a => (((47 + (a * a) / (3 - a) + (Math.Pow(2, a)))));
			//a => (-2 * Math.Pow(a, 3)) - (15 * Math.Pow(a, 2)) - (6 * a) + 7;
			//a => (a - 1) * Math.Pow((a - 4), 2);
			//a => (2 * Math.Pow(a, 4)) - (11 * Math.Pow(a, 3)) - (6 * Math.Pow(a, 2)) + (64 * a) + 32;

			Console.Out.WriteLine("Baking expression...");

			var baker = new Baker<Func<double, double>>(
				"a => (4 - a) + (Math.Pow(a, 0.5) * 3)");
			var func = baker.Bake();

			Console.Out.WriteLine("Target expression: " + func.ToString());

			using(var parameters = new ExpressionEvolverGeneticAlgorithmParameters(func.Compile()))
			{
				var ga = new GeneticAlgorithm<Expression<Func<double, double>>>(parameters);

				var generationCount = 1;
				var generationRunCount = 1;

				var generationRunCompletedHandler = new EventHandler<EventArgs<Population<Expression<Func<double, double>>>>>(
					(sender, e) =>
					{
						Program.PrintPopulation(e.Value, generationRunCount, "Run");
						generationRunCount++;
					});
				var generationCompletedHandler = new EventHandler<EventArgs<Population<Expression<Func<double, double>>>>>(
					(sender, e) =>
					{
						Program.PrintPopulation(e.Value, generationCount, "Generation");
						generationCount++;
					});

				ga.GenerationCompleted += generationCompletedHandler;
				ga.GenerationRunCompleted += generationRunCompletedHandler;
				ga.Run();
				ga.GenerationCompleted -= generationCompletedHandler;
				ga.GenerationRunCompleted -= generationRunCompletedHandler;
				Console.Out.WriteLine(ga.WasOptimalSolutionFound);
				var best = (from chromosome in ga.Final.Chromosomes
								orderby chromosome.Fitness descending
								select chromosome).Take(1).FirstOrDefault();
				Console.Out.WriteLine(best.Value.ToString());
			}
		}

		private static void PrintPopulation(Population<Expression<Func<double, double>>> population,
			int generationCount, string type)
		{
			var best = (from chromosome in population.Chromosomes
							orderby chromosome.Fitness descending
							select chromosome).Take(1).FirstOrDefault();
			Console.Out.WriteLine(type);
			Console.Out.WriteLine("\tGeneration Count: " + generationCount);
			Console.Out.WriteLine("\tFitness Summary: " + (double)population.FitnessSummary);
			Console.Out.WriteLine("\tFitness Average: " + (double)population.FitnessAverage);
			Console.Out.WriteLine("\tBest Fitness: " + (double)best.Fitness);
			Console.Out.WriteLine("\tBest Expression: " + best.Value.ToString());
			Console.Out.WriteLine();
		}
	}
}
