using Spackle;
using Spackle.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
	public sealed class GeneticAlgorithm<T>
	{
		public event EventHandler<EventArgs<Population<T>>> GenerationRunCompleted;
		public event EventHandler<EventArgs<Population<T>>> GenerationCompleted;

		public GeneticAlgorithm(IGeneticAlgorithmParameters<T> parameters)
		{
			parameters.CheckParameterForNull("parameters");
			this.Parameters = parameters;
		}

		private IEnumerable<Chromosome<T>> DoCrossover(Population<T> generationPopulation,
			SecureRandom random, int childCount)
		{
			const int ParentCount = 2;
			var RouletteSelection = (uint)Math.Ceiling((double)generationPopulation.Chromosomes.Count * 0.05);

			var crossovers = new ConcurrentBag<Chromosome<T>>();

			var tasks = new Task[this.Parameters.TaskCount];

			for(var c = 0; c < this.Parameters.TaskCount; c++)
			{
				tasks[c] = Task.Factory.StartNew(() =>
				{
					do
					{
						var parents = new List<Chromosome<T>>();
						var indexes = random.GetInt32Values(RouletteSelection,
							new Range<int>(0, generationPopulation.Chromosomes.Count), ValueGeneration.UniqueValuesOnly);

						for(var i = 0; i < ParentCount; i++)
						{
							var parent = (from chromosome in
												  (from index in indexes
													select generationPopulation.Chromosomes[index])
											  orderby chromosome.Fitness descending
											  select chromosome).Take(1).First();

							parents.Add(parent);
						}

						var children = random.NextDouble() < this.Parameters.CrossoverProbability ?
							this.Parameters.Crossover(parents.AsReadOnly()) :
							new List<T>(from parent in parents
											select this.Parameters.Copy(parent.Value)).AsReadOnly();

						foreach(var child in children)
						{
							var mutatedChild = this.Parameters.Mutator(child);

							if(mutatedChild != null)
							{
								crossovers.Add(new Chromosome<T>(
									mutatedChild, this.Parameters.FitnessEvaluator(mutatedChild)));
							}
						}
					} while(crossovers.Count < childCount);
				});
			}

			Task.WaitAll(tasks);
			return crossovers.AsEnumerable();
		}

		public void Run()
		{
			var population = this.Parameters.GeneratePopulation();

			var solution = this.Parameters.Terminator(population);
			var runCount = 0;

			while(solution == null && runCount < this.Parameters.NumberOfGenerationRuns)
			{
				population = this.RunGeneration(population);

				if(this.GenerationRunCompleted != null)
				{
					this.GenerationRunCompleted(this, new EventArgs<Population<T>>(population));
				}

				solution = this.Parameters.Terminator(population);
				runCount++;
			}

			this.WasOptimalSolutionFound = solution != null;
			this.Final = population;
		}

		private Population<T> RunGeneration(Population<T> population)
		{
			Population<T> generationPopulation = population;

			using(var random = new SecureRandom())
			{
				for(var i = 0; i < this.Parameters.NumberOfGenerations; i++)
				{
					var chromosomes = new List<Chromosome<T>>(
						from fitChild in this.Parameters.SelectFittestChildren(generationPopulation)
						select fitChild);

					var childCount = (int)Math.Ceiling(
						(double)(generationPopulation.Chromosomes.Count - chromosomes.Count));

					chromosomes.AddRange(this.DoCrossover(generationPopulation, random, childCount));

					generationPopulation = new Population<T>(new List<Chromosome<T>>(
						chromosomes.Take(generationPopulation.Chromosomes.Count)));

					if(this.GenerationCompleted != null)
					{
						this.GenerationCompleted(this, new EventArgs<Population<T>>(generationPopulation));
					}

					if(this.Parameters.Terminator(generationPopulation) != null)
					{
						break;
					}
				}
			}

			return generationPopulation;
		}

		public Population<T> Final { get; private set; }
		public IGeneticAlgorithmParameters<T> Parameters { get; private set; }
		public bool WasOptimalSolutionFound { get; private set; }
	}
}
