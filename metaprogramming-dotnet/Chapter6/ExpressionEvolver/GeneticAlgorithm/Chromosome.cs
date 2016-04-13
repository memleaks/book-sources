using Spackle;
using System;
using System.Globalization;

namespace GeneticAlgorithm
{
	public class Chromosome<T> 
	{
		public Chromosome(T value, double fitness)
			: base()
		{
			this.Value = value;
			this.Fitness = fitness;
		}

		public double Fitness
		{
			get;
			private set;
		}

		public T Value
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0}, Fitness = {1}", 
				this.Value.ToString(), 
				this.Fitness.ToString(CultureInfo.CurrentCulture));
		}
	}
}
