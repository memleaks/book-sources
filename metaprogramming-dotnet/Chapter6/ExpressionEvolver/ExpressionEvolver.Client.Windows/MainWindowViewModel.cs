using ExpressionBaker;
using GeneticAlgorithm;
using Spackle;
using Spackle.Extensions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;

namespace ExpressionEvolver.Client.Windows
{
	public sealed class MainWindowViewModel : ObservableObject
	{
		private string acceptableFitnessValue;
		private ReadOnlyCollection<Point> baseLine;
		private string bestExpression;
		private string bestFitness;
		private ReadOnlyCollection<Point> evolvingLine;
		private string expression;
		private string generation;
		private bool isEvolveEnabled;
		private BackgroundWorker worker;

		public MainWindowViewModel()
		{
			this.IsEvolveEnabled = true;
		}

		public void Evolve()
		{
			this.IsEvolveEnabled = false;

			var baker = new Baker<Func<double, double>>(this.Expression);
			var func = baker.Bake();

			var parameters = new ExpressionEvolverGeneticAlgorithmParameters(func.Compile());
			var generationCount = 1;

			this.AcceptableFitnessValue = parameters.AcceptableAverageMeanSquareError.ToString("#.00");
			this.BaseLine = this.GetPoints(parameters.Results);

			this.worker = new BackgroundWorker();
			this.worker.WorkerReportsProgress = true;
			this.worker.RunWorkerCompleted += (s, e) =>
			{
				var args = e.Result as Population<Expression<Func<double, double>>>;
				this.PrintPopulation(args, generationCount, parameters.Results);

				parameters.Dispose();
				this.IsEvolveEnabled = true;
			};

			this.worker.ProgressChanged += (ps, pe) =>
			{
				if(generationCount % 10 == 0)
				{
					var args = pe.UserState as EventArgs<Population<Expression<Func<double, double>>>>;
					this.PrintPopulation(args.Value, generationCount, parameters.Results);
				}

				generationCount++;
			};

			this.worker.DoWork += (s, e) =>
			{
				var ga = new GeneticAlgorithm<Expression<Func<double, double>>>(parameters);

				var generationCompletedHandler = new EventHandler<EventArgs<Population<Expression<Func<double, double>>>>>(
					(gs, ge) =>
					{
						this.worker.ReportProgress(0, ge);
					});
				ga.GenerationCompleted += generationCompletedHandler;
				ga.Run();
				ga.GenerationCompleted -= generationCompletedHandler;
				e.Result = ga.Final;
			};

			this.worker.RunWorkerAsync();
		}

		private void PrintPopulation(Population<Expression<Func<double, double>>> population,
			int generationCount, ReadOnlyCollection<ExpressionEvolverResult> results)
		{
			var best = (from chromosome in population.Chromosomes
							orderby chromosome.Fitness descending
							select chromosome).Take(1).FirstOrDefault();
			this.Generation = generationCount.ToString();
			this.BestFitness = best.Fitness.ToString("#.##0");
			this.BestExpression = best.Value.ToString();
			this.EvolvingLine = this.GetBestPoints(results, best.Value.Compile());
		}

		private ReadOnlyCollection<Point> GetBestPoints(ReadOnlyCollection<ExpressionEvolverResult> results,
			Func<double, double> value)
		{
			return (from point in
						  (from result in results
							where result.Exception == null
							select new Point(result.Parameter, value(result.Parameter)))
					  where !double.IsNaN(point.Y) && !double.IsInfinity(point.Y)
					  select point).AsReadOnly();
		}

		private ReadOnlyCollection<Point> GetPoints(ReadOnlyCollection<ExpressionEvolverResult> results)
		{
			return new ReadOnlyCollection<Point>((from result in results
															  select new Point(result.Parameter, result.Result)).ToList());
		}

		public string AcceptableFitnessValue
		{
			get { return this.acceptableFitnessValue; }
			private set { this.SetAndNotify(ref this.acceptableFitnessValue, value, () => this.AcceptableFitnessValue); }
		}

		public ReadOnlyCollection<Point> BaseLine
		{
			get { return this.baseLine; }
			private set { this.SetAndNotify(ref this.baseLine, value, () => this.BaseLine); }
		}

		public string BestExpression
		{
			get { return this.bestExpression; }
			private set { this.SetAndNotify(ref this.bestExpression, value, () => this.BestExpression); }
		}

		public string BestFitness
		{
			get { return this.bestFitness; }
			private set { this.SetAndNotify(ref this.bestFitness, value, () => this.BestFitness); }
		}

		public ReadOnlyCollection<Point> EvolvingLine
		{
			get { return this.evolvingLine; }
			private set { this.SetAndNotify(ref this.evolvingLine, value, () => this.EvolvingLine); }
		}

		public string Expression
		{
			get { return this.expression; }
			set { this.SetAndNotify(ref this.expression, value, () => this.Expression); }
		}

		public string Generation
		{
			get { return this.generation; }
			private set { this.SetAndNotify(ref this.generation, value, () => this.Generation); }
		}

		public bool IsEvolveEnabled
		{
			get { return this.isEvolveEnabled; }
			set { this.SetAndNotify(ref this.isEvolveEnabled, value, () => this.IsEvolveEnabled); }
		}
	}
}
