using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

public static class dynamic_max_methods
{
  public static T dynamic_max<T>(T left, T right)
  {
    if (left is IComparable<T>)
      return ((left as IComparable<T>).CompareTo(right) < 0)
        ? right : left;

    if (left is IComparable)
      return ((left as IComparable).CompareTo(right) < 0)
        ? right : left;

    throw new ApplicationException(String.Format(
      "Type {0} must implement one of the IComparable or " +
      "IComparable<{0}> interfaces.", typeof(T).Name));
  }

  public static T enhanced_dynamic_max<T>(T left, T right)
  {
    BindingFlags flags =
      BindingFlags.Static | BindingFlags.Public;
    Type type = typeof(T);
    MethodInfo lessThan =
      (from method in type.GetMethods(flags)
       let parameters = method.GetParameters()
       where method.Name == "op_LessThan"
             && parameters != null
             && parameters.Length == 2
             && parameters[0].ParameterType == type
             && parameters[1].ParameterType == type
       select method).FirstOrDefault();
    if (lessThan != null)
      return (bool)lessThan.Invoke(null,
        new object[] { left, right }) ? right : left;

    if (left is IComparable<T>)
      return ((left as IComparable<T>).CompareTo(right) < 0)
        ? right : left;

    if (left is IComparable)
      return ((left as IComparable).CompareTo(right) < 0)
        ? right : left;

    throw new ApplicationException(String.Format(
      "Type {0} must override the less-than operator for " +
      "its type or implement one of the IComparable or " +
      "IComparable<{0}> interfaces.", type.Name));
  }
}