using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ExpressionEvolver.Client.Windows
{
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void SetAndNotify<T>(ref T field, T value, Expression<Func<T>> property)
		{
			if(!object.ReferenceEquals(field, value))
			{
				field = value;
				this.OnPropertyChanged(property);
			}
		}

		protected virtual void OnPropertyChanged<T>(Expression<Func<T>> changedProperty)
		{
			var propertyChangedEvent = this.PropertyChanged;

			if(propertyChangedEvent != null)
			{
				var name = ((MemberExpression)changedProperty.Body).Member.Name;
				propertyChangedEvent(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
