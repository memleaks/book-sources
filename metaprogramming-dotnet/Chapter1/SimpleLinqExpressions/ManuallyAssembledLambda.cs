using System;
using System.Linq.Expressions;

class ManuallyAssembledLambda
{
  static Func<int, int, bool> CompileLambda()
  {
    ParameterExpression Left =
        Expression.Parameter(typeof(int), "Left");
    ParameterExpression Right =
        Expression.Parameter(typeof(int), "Right");

    Expression<Func<int, int, bool>> GreaterThanExpr =
        Expression.Lambda<Func<int, int, bool>>
        (
            Expression.GreaterThan(Left, Right),
            Left, Right
        );

    return GreaterThanExpr.Compile();
  }

  //static void Main()
  //{
  //  int L = 7, R = 11;
  //  Console.WriteLine("{0} > {1} is {2}", L, R,
  //    CompileLambda()(L, R));
  //  Console.ReadLine();
  //}
  static void Main()
  {
    string op = ">=";
    var integerPredicate =
      DynamicPredicate.Generate<int>(op).Compile();
    var floatPredicate =
      DynamicPredicate.Generate<float>(op).Compile();

    int iA = 12, iB = 4;
    Console.WriteLine("{0} {1} {2} : {3}", iA, op, iB,
        integerPredicate(iA, iB));

    float fA = 867.0f, fB = 867.0f;
    Console.WriteLine("{0} {1} {2} : {3}", fA, op, fB,
        floatPredicate(fA, fB));

    Console.WriteLine("{0} {1} {2} : {3}", fA, ">", fB,
        DynamicPredicate.Generate<float>(">").Compile()(fA, fB));

    Console.ReadLine();
  }
}