using System.Collections.ObjectModel;

namespace GeneticAlgorithm
{
	public interface IGeneticAlgorithmParameters<T>
	{
		/// <summary>
		/// Used in crossover to create a copy of a chromosome.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>
		/// It is up to the implementor of this interface to determine 
		/// if <c>Copy</c> should return a reference to <c>value</c> (i.e. "return value;")
		/// or a new object that contains the same values in <c>value</c>.
		/// Objects that are semantically read-only can return the reference (e.g. strings and Expressions).
		/// However, objects that are modifiable should return a different reference
		/// to avoid accidentally mutating parents (which would cause concurrency foreach issues
		/// and undesirable algorithmic characteristics - i.e. you don't want to 
		/// mutate the parents accidentally).
		/// </remarks>
		T Copy(T value);
		ReadOnlyCollection<T> Crossover(ReadOnlyCollection<Chromosome<T>> parents);
		double FitnessEvaluator(T chromosome);
		Population<T> GeneratePopulation();
		T Mutator(T chromosome);
		ReadOnlyCollection<Chromosome<T>> SelectFittestChildren(Population<T> population);
		Chromosome<T> Terminator(Population<T> population);

		int ChromosomeLength { get; }
		double CrossoverProbability { get; }
		double MutationProbability { get; }
		int NumberOfGenerations { get; }
		int NumberOfGenerationRuns { get; }
		int PopulationSize { get; }
		int TaskCount { get; }
		double WorstFitnessValue { get; }
	}
}
