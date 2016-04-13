using System;
using System.Reflection;
using System.Diagnostics;

class Product
{
  public string ProductName { get; set; }
  public int ProductID { get; set; }
  public decimal Price { get; set; }

  public object GetPropertyValue(string propName)
  {
    if (propName == null) return null;
    PropertyInfo info = this.GetType().GetProperty(propName, BindingFlags.IgnoreCase);
    return (info == null) ? null : info.GetValue(this, null);
  }
}

class ReflectionDemo
{
  static void Main(string[] args)
  {
    if (args == null || args.Length == 0)
    {
      Console.WriteLine("Usage: ReflectionDemo " +
        "<property name> [<property name> ...]");
      return;
    }

    Product prod = new Product() 
    {
      ProductName = "Weird Al CD",
      ProductID = 5,
      Price = 12.95m
    };

    foreach (string arg in args)
    {
      object result = prod.GetPropertyValue(arg);
      if (result == null) continue;
      Console.WriteLine("{0} = {1}", arg, result);
    }

    if (Debugger.IsAttached)
      Console.ReadLine();
  }
}
