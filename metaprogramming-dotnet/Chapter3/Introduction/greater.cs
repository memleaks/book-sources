using System;
public static class greater
{
  public static Object of(Object left, Object right)
  {
    throw new ApplicationException(
      "Type Object must implement one of the " +
      "IComparable or IComparable<Object> interfaces.");
  }
  public static Boolean of(Boolean left, Boolean right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static Byte of(Byte left, Byte right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static Char of(Char left, Char right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static Decimal of(Decimal left, Decimal right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static Double of(Double left, Double right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static Single of(Single left, Single right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static Int32 of(Int32 left, Int32 right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static Int64 of(Int64 left, Int64 right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static SByte of(SByte left, SByte right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static Int16 of(Int16 left, Int16 right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static String of(String left, String right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static UInt32 of(UInt32 left, UInt32 right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static UInt64 of(UInt64 left, UInt64 right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
  public static UInt16 of(UInt16 left, UInt16 right)
  {
    return left.CompareTo(right) < 0 ? right : left;
  }
}
