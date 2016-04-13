namespace Ch3
{
  using System;
  public static class enhanced_generated_max
  {
    public static System.Int32 of(System.Int32 left, System.Int32 right)
    {
      return left.CompareTo(right) < 0 ? right : left;
    }
    public static System.Single of(System.Single left, System.Single right)
    {
      return (left < right) ? right : left;
    }
    public static System.Double of(System.Double left, System.Double right)
    {
      return (left < right) ? right : left;
    }
    public static System.String of(System.String left, System.String right)
    {
      return left.CompareTo(right) < 0 ? right : left;
    }
  }
}
