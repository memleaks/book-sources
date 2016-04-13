using GeneticAlgorithm;
using Spackle;
using Spackle.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleGeneticAlgorithm
{
	public sealed class SimpleGeneticAlgorithmParameters
		: IGeneticAlgorithmParameters<string>
	{
		private const int ChromosomeLengthValue = 50;
		private const double CrossoverProbabilityValue = 0.9;
		private const int GenerationCountValue = 200;
		private const double MutationProbabilityValue = 0.01;
		private const int ParentCount = 2;
		private const int PopulationSizeValue = 200;
		private const int RunCountValue = 1000;

		public string Copy(string value)
		{
			return value;
		}

		public ReadOnlyCollection<string> Crossover(ReadOnlyCollection<Chromosome<string>> parents)
		{
			parents.CheckParameterForNull("parents");

			var crossoverPoint = 0;

			using(var random = new SecureRandom())
			{
				crossoverPoint = random.Next(this.ChromosomeLength);
			}

			return new List<string>()
				{ 
					parents[0].Value.Substring(0, crossoverPoint) + parents[1].Value.Substring(crossoverPoint),
					parents[1].Value.Substring(0, crossoverPoint) + parents[0].Value.Substring(crossoverPoint) 
				}.AsReadOnly();
		}

		public double FitnessEvaluator(string chromosome)
		{
			chromosome.CheckParameterForNull("chromosome");

			var fitness = 0d;

			foreach(var allele in chromosome)
			{
				if(allele == '1')
				{
					fitness++;
				}
			}

			return fitness;
		}

		public string Mutator(string chromosome)
		{
			chromosome.CheckParameterForNull("chromosome");

			var mutatedChromosome = new StringBuilder();

			using(var random = new SecureRandom())
			{
				foreach(char allele in chromosome)
				{
					if(random.NextDouble() < this.MutationProbability)
					{
						mutatedChromosome.Append(allele == '1' ? '0' : '1');
					}
					else
					{
						mutatedChromosome.Append(allele);
					}
				}
			}

			return mutatedChromosome.ToString();
		}

		public Population<string> GeneratePopulation()
		{
			var chromosomes = new List<Chromosome<string>>();

			using(var random = new SecureRandom())
			{
				for(var i = 0; i < this.PopulationSize; i++)
				{
					var chromosome = new StringBuilder();

					for(var j = 0; j < this.ChromosomeLength; j++)
					{
						chromosome.Append(random.Next(2).ToString(CultureInfo.CurrentCulture));
					}

					var value = chromosome.ToString();

					chromosomes.Add(new Chromosome<string>(value, this.FitnessEvaluator(value)));
				}
			}

			return new Population<string>(chromosomes);
		}

		public ReadOnlyCollection<Chromosome<string>> SelectFittestChildren(Population<string> population)
		{
			return new ReadOnlyCollection<Chromosome<string>>(new List<Chromosome<string>>());
		}

		public Chromosome<string> Terminator(Population<string> population)
		{
			population.CheckParameterForNull("population");

			return (from value in population.Chromosomes
					  where value.Fitness == this.ChromosomeLength
					  select value).FirstOrDefault();
		}

		public int ChromosomeLength
		{
			get
			{
				return SimpleGeneticAlgorithmParameters.ChromosomeLengthValue;
			}
		}

		public double CrossoverProbability
		{
			get
			{
				return SimpleGeneticAlgorithmParameters.CrossoverProbabilityValue;
			}
		}

		public double MutationProbability
		{
			get
			{
				return SimpleGeneticAlgorithmParameters.MutationProbabilityValue;
			}
		}

		public int NumberOfGenerations
		{
			get
			{
				return SimpleGeneticAlgorithmParameters.GenerationCountValue;
			}
		}

		public int NumberOfGenerationRuns
		{
			get
			{
				return SimpleGeneticAlgorithmParameters.RunCountValue;
			}
		}

		public int PopulationSize
		{
			get
			{
				return SimpleGeneticAlgorithmParameters.PopulationSizeValue;
			}
		}

		public int TaskCount
		{
			get
			{
				return Environment.ProcessorCount;
			}
		}

		public double WorstFitnessValue
		{
			get
			{
				return 0d;
			}
		}
	}
}
