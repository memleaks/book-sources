using Spackle.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GeneticAlgorithm
{
	public sealed class Population<T>
	{
		public Population(IList<Chromosome<T>> chromosomes)
		{
			chromosomes.CheckParameterForNull("chromosomes");

			if(chromosomes.Count < 2)
			{
				throw new ArgumentException("At least two chromosomes must exist in the list.", "chromosomes");
			}

			this.SetFitnessValues(chromosomes);
			this.Chromosomes = chromosomes.AsReadOnly();
		}

		private void SetFitnessValues(IList<Chromosome<T>> chromosomes)
		{
			foreach(var chromosome in chromosomes)
			{
				this.FitnessSummary += chromosome.Fitness;
			}

			this.FitnessAverage = this.FitnessSummary / chromosomes.Count;
		}

		public ReadOnlyCollection<Chromosome<T>> Chromosomes { get; private set; }
		public double FitnessAverage { get; private set; }
		public double FitnessSummary { get; private set; }
	}
}
