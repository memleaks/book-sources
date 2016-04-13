using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

class Demo
{
  public static bool operator <(Demo left, Demo right)
  {
      return false;
  }
  public static bool operator >(Demo left, Demo right)
  {
      return false;
  }
  static void Main()
  {
    string className = "DynamicCar";
    bool generateCtor = true;
    var properties = new List<Tuple<Type, string, bool>>()
      {
        Tuple.Create(typeof(string), "Make", true),
        Tuple.Create(typeof(string), "Model", true),
        Tuple.Create(typeof(int), "Year", true),
        Tuple.Create(typeof(int), "MPG", false)
      };

    Chapter3Intro.GenerateDataClass(className, properties, generateCtor);
    Console.WriteLine("{0}", dynamic_max_methods.dynamic_max(new Test(1), new Test(2)));
    //Console.WriteLine("{0}", generic_max(new Test(7), new Test(11)));
    Console.WriteLine("{0}", dynamic_max_methods.dynamic_max(1, 2));
    Console.WriteLine("{0}", generic_max(3.0, 4.0));
    Console.WriteLine("{0}", generic_max(3.0d, 4.0d));
    Console.WriteLine("{0}", generic_max(true, false));
    Console.WriteLine("{0}", generic_max(5L, 6L));
    Console.WriteLine("{0}", generic_max("cat", "can"));
    Console.ReadLine();
  }

  static T generic_max<T>(T left, T right) where T : IComparable, IComparable<T>
  {
      return (left.CompareTo(right) < 0) ? right : left;
  }
}

class Test : IComparable<Test> //, IComparable
{
    private int val;

    public Test(int val)
    {
      this.val = val;
    }

    public override string ToString()
    {
      return val.ToString();
    }

    public static bool operator <(Test L, Test R)
    {
      if ((object)L == null || (object)R == null)
        return false;
      return L.val < R.val;
    }

    public static bool operator >=(Test L, Test R)
    {
      return !(L.val < R.val);
    }

    public static bool operator >(Test L, Test R)
    {
      if ((object)L == null || (object)R == null)
        return false;
      return L.val > R.val;
    }

    public static bool operator <=(Test L, Test R)
    {
      return !(L.val > R.val);
    }

    public static bool operator ==(Test L, Test R)
    {
      if ((object)L == null || (object)R == null)
        return false;
      return L.val == R.val;
    }
    
    public static bool operator !=(Test L, Test R)
    {
      return !(L.val == R.val);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        return val == (obj as Test).val;
    }

    public override int GetHashCode()
    {
        return val;
    }

    public int CompareTo(object obj)
    {
        if (obj == null)
          return 0;
        return val.CompareTo((obj as Test).val);
    }

    public int CompareTo(Test other)
    {
        if ((object)other == null)
            return 0;
        return val.CompareTo(other.val);
    }
}
