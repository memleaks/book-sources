using GeneticAlgorithm;
using Spackle;
using Spackle.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionEvolver
{
	public sealed class ExpressionEvolverGeneticAlgorithmParameters
		: IGeneticAlgorithmParameters<Expression<Func<double, double>>>,
		IDisposable
	{
		private const double ConstantLimit = 100d;
		public const double CrossoverProbabilityValue = 0.9;
		private const int ExpressionMaximum = 10;
		public const int GenerationCountValue = 100;
		private const double InjectConstantProbabilityValue = 0.5;
		public const double MutationProbabilityValue = 0.05;
		private const double NodeCountPenalty = 0.05;
		public const int PopulationSizeValue = 2000;
		public const int ResultGenerationCount = 30;
		private const double ResultGenerationVariance = 1000d;
		public const int RunCountValue = 100;
		public const double SelectFittestChildrenPercentage = 10d;
		private const double Tolerance = 0.001d;

		public ExpressionEvolverGeneticAlgorithmParameters(Func<double, double> target)
			: this(target, new SecureRandom()) { }

		public ExpressionEvolverGeneticAlgorithmParameters(Func<double, double> target,
			SecureRandom random)
			: base()
		{
			target.CheckParameterForNull("target");
			random.CheckParameterForNull("random");

			this.Random = random;
			this.GenerateResults(target,
				ExpressionEvolverGeneticAlgorithmParameters.ResultGenerationCount,
				ExpressionEvolverGeneticAlgorithmParameters.ResultGenerationVariance);
		}

		public ExpressionEvolverGeneticAlgorithmParameters(Func<double, double> target,
			int count, double variance)
			: this(target, count, variance, new SecureRandom()) { }

		public ExpressionEvolverGeneticAlgorithmParameters(Func<double, double> target,
			int count, double variance, SecureRandom random)
			: base()
		{
			target.CheckParameterForNull("target");
			random.CheckParameterForNull("random");

			this.Random = random;
			this.GenerateResults(target, count, variance);
		}

		public ExpressionEvolverGeneticAlgorithmParameters(ReadOnlyCollection<ExpressionEvolverResult> results)
			: this(results, new SecureRandom()) { }

		public ExpressionEvolverGeneticAlgorithmParameters(ReadOnlyCollection<ExpressionEvolverResult> results,
			SecureRandom random)
			: base()
		{
			results.CheckParameterForNull("results");
			random.CheckParameterForNull("random");

			if(results.Count < 1)
			{
				throw new ArgumentException("At least one result must be in the collection.", "results");
			}

			this.Results = results;
			this.Random = random;
		}

		private void CalcuateAcceptableAverageMeanSquareError()
		{
			var resultsSummation = 0d;
			var summation = 0d;
			var errorTotal = 0d;
			var total = 0d;
			var largestValue = double.MinValue;

			foreach(var result in this.results)
			{
				if(result.Exception == null)
				{
					errorTotal += Math.Pow(
						result.Result - (
						result.Result + (result.Result * ExpressionEvolverGeneticAlgorithmParameters.Tolerance)),
						2d);
					resultsSummation += Math.Abs(result.Result);
					summation += result.Result;
					total++;
					largestValue = Math.Max(largestValue, Math.Abs(result.Result));
				}
			}

			this.AcceptableAverageMeanSquareError = -1d * (errorTotal / total);
			this.ExceptionPenaltyFactor = Math.Pow((resultsSummation / total), 2d);
			this.WorstFitnessValue = -1d * Math.Pow(largestValue * total, 2d);
			this.NodeCountPenaltyFactor = Math.Abs(this.AcceptableAverageMeanSquareError) *
				ExpressionEvolverGeneticAlgorithmParameters.NodeCountPenalty;
		}

		private void CheckForDispose()
		{
			if(this.Disposed)
			{
				throw new ObjectDisposedException(this.GetType().Name);
			}
		}

		public Expression<Func<double, double>> Copy(Expression<Func<double, double>> value)
		{
			this.CheckForDispose();
			return value;
		}

		private void GenerateResults(Func<double, double> target, int count, double variance)
		{
			if(count < 1)
			{
				throw new ArgumentException("At least one result must be created.", "count");
			}

			var targetResults = new List<ExpressionEvolverResult>();

			do
			{
				var parameter = this.Random.NextDouble() * variance *
					(this.Random.NextBoolean() ? 1d : -1d);

				try
				{
					var result = target(parameter);

					if(!double.IsNaN(result) && !double.IsInfinity(result))
					{
						targetResults.Add(new ExpressionEvolverResult(parameter, result));
					}
				}
				catch(ArithmeticException e)
				{
					targetResults.Add(new ExpressionEvolverResult(parameter, e));
				}
			} while(targetResults.Count < count);

			this.Results = targetResults.AsReadOnly();
		}

		public ReadOnlyCollection<Expression<Func<double, double>>> Crossover(
			ReadOnlyCollection<Chromosome<Expression<Func<double, double>>>> parents)
		{
			this.CheckForDispose();

			parents.CheckParameterForNull("parents");

			if(parents.Count < 2)
			{
				throw new ArgumentException("At least 2 parents need to be in the collection.", "parents");
			}

			var children = new List<Expression<Func<double, double>>>();

			var first = parents[0];
			var second = parents[1];

			var firstCount = first.Value.Body.GetNodeCount();
			var secondCount = second.Value.Body.GetNodeCount();

			var firstCrossoverPoint = this.Random.Next(firstCount);
			var secondCrossoverPoint = this.Random.Next(secondCount);

			var firstExpression = first.Value.Body.GetNode(firstCrossoverPoint);
			var secondExpression = second.Value.Body.GetNode(secondCrossoverPoint);

			var newFirst = first.Value.Body.Replace(first.Value.Parameters, firstExpression, secondExpression).Compress();
			var newSecond = second.Value.Body.Replace(second.Value.Parameters, secondExpression, firstExpression).Compress();

			if(newFirst.IsValid())
			{
				children.Add(Expression.Lambda<Func<double, double>>(newFirst,
					first.Value.Parameters));
			}

			if(newSecond.IsValid())
			{
				children.Add(Expression.Lambda<Func<double, double>>(newSecond,
					second.Value.Parameters));
			}

			return children.AsReadOnly();
		}

		public void Dispose()
		{
			this.Random.Dispose();
			this.Disposed = true;
		}

		public double FitnessEvaluator(Expression<Func<double, double>> chromosome)
		{
			this.CheckForDispose();

			chromosome.CheckParameterForNull("chromosome");

			var fitness = 0d;
			var phenotype = chromosome.Compile();
			var exceptionCount = 0;

			foreach(var result in this.Results)
			{
				try
				{
					var calculation = phenotype(result.Parameter);

					if(result.Exception == null)
					{
						if((double.IsNaN(result.Result) && !double.IsNaN(calculation)) ||
							(!double.IsNaN(result.Result) && double.IsNaN(calculation)) ||
							(double.IsInfinity(result.Result) && !double.IsInfinity(calculation)) ||
							(!double.IsInfinity(result.Result) && double.IsInfinity(calculation)))
						{
							exceptionCount++;
						}
						else
						{
							fitness += Math.Pow(Math.Abs(calculation - result.Result), 2d);
						}
					}
				}
				catch(ArithmeticException)
				{
					if(result.Exception == null)
					{
						exceptionCount++;
					}
				}
			}

			if(exceptionCount > 0)
			{
				fitness += (exceptionCount * this.ExceptionPenaltyFactor);
			}

			//var nodeCount = chromosome.GetNodeCount();

			//if(nodeCount > ExpressionEvolverGeneticAlgorithmParameters.ExpressionMaximum)
			//{
			//   fitness += Math.Pow((double)(nodeCount -
			//      ExpressionEvolverGeneticAlgorithmParameters.ExpressionMaximum), 10d);
			//}

			fitness += chromosome.GetNodeCount() * this.NodeCountPenaltyFactor;

			if(double.IsNaN(fitness) || double.IsInfinity(fitness))
			{
				fitness = this.WorstFitnessValue;
			}
			else
			{
				fitness = -1d * ((fitness / (double)this.Results.Count));
			}

			return fitness;
		}

		public Expression<Func<double, double>> Mutator(Expression<Func<double, double>> chromosome)
		{
			this.CheckForDispose();

			chromosome.CheckParameterForNull("chromosome");

			Expression<Func<double, double>> mutated = null;

			if(this.Random.NextDouble() < this.MutationProbability)
			{
				var chromosomeBody = chromosome.Body;
				var nodeCount = chromosomeBody.GetNodeCount();
				var selectedNodeIndex = this.Random.Next(nodeCount);
				var selectedNode = chromosomeBody.GetNode(selectedNodeIndex);
				var selectedNodeCount = selectedNode.GetNodeCount();
				var mutatedNode = new RandomExpressionGenerator(this.Random.Next(selectedNodeCount),
					ExpressionEvolverGeneticAlgorithmParameters.InjectConstantProbabilityValue,
					ExpressionEvolverGeneticAlgorithmParameters.ConstantLimit,
					chromosome.Parameters[0], this.Random).Body;

				chromosomeBody = chromosomeBody.Replace(null, selectedNode, mutatedNode).Compress();

				mutated = Expression.Lambda<Func<double, double>>(chromosomeBody,
					(from param in chromosome.Parameters
					 select param));
			}
			else
			{
				mutated = chromosome;
			}

			return mutated.IsValid() ? mutated : null;
		}

		public Population<Expression<Func<double, double>>> GeneratePopulation()
		{
			this.CheckForDispose();

			var chromosomes = new List<Chromosome<Expression<Func<double, double>>>>();

			while(chromosomes.Count < ExpressionEvolverGeneticAlgorithmParameters.PopulationSizeValue)
			{
				var parameter = Expression.Parameter(typeof(double), "a");
				var maximumExpressions = this.Random.Next(
					ExpressionEvolverGeneticAlgorithmParameters.ExpressionMaximum);

				var body = new RandomExpressionGenerator(maximumExpressions,
					ExpressionEvolverGeneticAlgorithmParameters.InjectConstantProbabilityValue,
					ExpressionEvolverGeneticAlgorithmParameters.ConstantLimit,
					parameter, this.Random).Body.Compress();

				if(body.IsValid())
				{
					var value = Expression.Lambda<Func<double, double>>(body, parameter);
					chromosomes.Add(new Chromosome<Expression<Func<double, double>>>(
						value, this.FitnessEvaluator(value)));
				}
			}

			return new Population<Expression<Func<double, double>>>(chromosomes);
		}

		public ReadOnlyCollection<Chromosome<Expression<Func<double, double>>>> SelectFittestChildren(
			Population<Expression<Func<double, double>>> population)
		{
			this.CheckForDispose();

			population.CheckParameterForNull("population");

			return new ReadOnlyCollection<Chromosome<Expression<Func<double, double>>>>(
				new List<Chromosome<Expression<Func<double, double>>>>((
					from value in population.Chromosomes
					orderby value.Fitness descending
					select value).Take(
						(int)((double)population.Chromosomes.Count /
							ExpressionEvolverGeneticAlgorithmParameters.SelectFittestChildrenPercentage))));
		}

		public Chromosome<Expression<Func<double, double>>> Terminator(
			Population<Expression<Func<double, double>>> population)
		{
			this.CheckForDispose();

			population.CheckParameterForNull("population");

			return (from value in population.Chromosomes
					  where value.Fitness >= this.AcceptableAverageMeanSquareError
					  orderby value.Fitness descending
					  select value).FirstOrDefault();
		}

		public double AcceptableAverageMeanSquareError { get; private set; }

		public int ChromosomeLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public double CrossoverProbability
		{
			get
			{
				this.CheckForDispose();
				return ExpressionEvolverGeneticAlgorithmParameters.CrossoverProbabilityValue;
			}
		}

		public double ExceptionPenaltyFactor { get; private set; }

		public double MutationProbability
		{
			get
			{
				this.CheckForDispose();
				return ExpressionEvolverGeneticAlgorithmParameters.MutationProbabilityValue;
			}
		}

		public int NumberOfGenerations
		{
			get
			{
				this.CheckForDispose();
				return ExpressionEvolverGeneticAlgorithmParameters.GenerationCountValue;
			}
		}

		public int NumberOfGenerationRuns
		{
			get
			{
				this.CheckForDispose();
				return ExpressionEvolverGeneticAlgorithmParameters.RunCountValue;
			}
		}

		public double NodeCountPenaltyFactor { get; private set; }

		public int PopulationSize
		{
			get
			{
				this.CheckForDispose();
				return ExpressionEvolverGeneticAlgorithmParameters.PopulationSizeValue;
			}
		}

		private ReadOnlyCollection<ExpressionEvolverResult> results;
		public ReadOnlyCollection<ExpressionEvolverResult> Results
		{
			get
			{
				this.CheckForDispose();
				return this.results;
			}
			private set
			{
				this.results = value;
				this.CalcuateAcceptableAverageMeanSquareError();
			}
		}

		public int TaskCount
		{
			get
			{
				this.CheckForDispose();
				return Environment.ProcessorCount;
			}
		}

		private bool Disposed { get; set; }

		private SecureRandom random;
		public SecureRandom Random
		{
			get
			{
				this.CheckForDispose();
				return this.random;
			}
			private set
			{
				this.random = value;
			}
		}

		public double WorstFitnessValue { get; private set; }
	}
}