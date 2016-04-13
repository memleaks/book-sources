using System;
using System.Windows;

namespace ExpressionEvolver.Client.Windows
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
			this.DataContext = new MainWindowViewModel();
		}

		private void OnEvolveClick(object sender, RoutedEventArgs e)
		{
			(this.DataContext as MainWindowViewModel).Evolve();
		}
	}
}
