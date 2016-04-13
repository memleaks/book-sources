using System;
using System.Linq.Expressions;

class DynamicPredicate
{
  public static Expression<Func<T, T, bool>> Generate<T>(string op)
  {
    ParameterExpression x = Expression.Parameter(typeof(T), "x");
    ParameterExpression y = Expression.Parameter(typeof(T), "y");
    return Expression.Lambda<Func<T, T, bool>>
      (
        (op.Equals(">")) ? Expression.GreaterThan(x, y) :
          (op.Equals("<")) ? Expression.LessThan(x, y) :
          (op.Equals(">=")) ? Expression.GreaterThanOrEqual(x, y) :
          (op.Equals("<=")) ? Expression.LessThanOrEqual(x, y) :
          (op.Equals("!=")) ? Expression.NotEqual(x, y) :
          Expression.Equal(x, y),
        x, y
      );
  }
}