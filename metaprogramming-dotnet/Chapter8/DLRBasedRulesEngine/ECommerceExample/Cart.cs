#define PASS_BY_VALUE
#define USE_SAFE_ENGINE

using System;
using System.Linq;
using DevJourney.Scripting;
using System.Collections.Generic;

namespace DevJourney.Commerce
{
  public delegate void CartChanged();

#if PASS_BY_VALUE
  [Serializable]
  public class Product :
#else
  public class Product : MarshalByRefObject,
#endif
    IComparable<Product>, IEquatable<Product>
  {
    private string _name;
    public string Name
    {
      get { return _name ?? String.Empty; }
      set { _name = value; }
    }

    private string _sku;
    public string SKU
    {
      get { return _sku ?? String.Empty; }
      set { _sku = value; }
    }

    private float _price;
    public float Price
    {
      get { return _price; }
      set
      {
        if (value <= 0.0f)
          throw new ArgumentOutOfRangeException(
            "Price", String.Format("The Price " +
            "must be non-negative. The supplied " +
            "value of {0} is invalid.", value));
        _price = value;
      }
    }

    private string _category;
    public string Category
    {
      get { return _category ?? String.Empty; }
      set { _category = value; }
    }

    public Product() { }

    public Product(string name, string sku,
      float price, string category)
    {
      Name = name;
      SKU = sku;
      Price = price;
      Category = category;
    }

    public int CompareTo(Product other)
    {
      return Name.CompareTo(other.Name);
    }

    public override bool Equals(object obj)
    {
      var other = obj as Product;
      if ((object)other == null)
        return false;
      return Equals(other);
    }

    public override int GetHashCode()
    {
      return (SKU == null) ? 0 : SKU.GetHashCode();
    }

    public static bool operator ==(
      Product left, Product right)
    {
      if ((object)left == null)
        return false;
      return left.Equals(right);
    }

    public static bool operator !=(
      Product left, Product right)
    {
      return !(left == right);
    }

    public bool Equals(Product other)
    {
      return
        (object)other == null
        || SKU == null
        || other.SKU == null
          ? false
          : SKU.Equals(other.SKU);
    }

    public override string ToString()
    {
      return String.Format("{0} @ {1} [{2}]",
          Name, Price, Category);
    }
  }

#if PASS_BY_VALUE
  [Serializable]
  public class Cart
#else
  public class Cart : MarshalByRefObject
#endif
  {
    public event CartChanged OnCartChanged;

    private readonly List<LineItem>
      _lineItems = new List<LineItem>();

    private readonly
      Dictionary<string, object> _ruleParameters;

    [NonSerialized]
#if USE_SAFE_ENGINE
    private readonly RuleEngine _ruleEngine;
#else
    private readonly UnsafeRuleEngine _ruleEngine;
#endif

    private readonly int _ruleHandle;

    private readonly IsolationMode _appDomainMode;
    private readonly IsolationMode _runtimeMode;
    private readonly IsolationMode _scopeMode;

    public Cart(IRule discountRule,
      IsolationMode appDomainMode = IsolationMode.Shared,
      IsolationMode runtimeMode = IsolationMode.Shared,
      IsolationMode scopeMode = IsolationMode.Shared)
    {
      if (discountRule == null)
        throw new ArgumentNullException(
          "discountRule");
      _appDomainMode = appDomainMode;
      _runtimeMode = runtimeMode;
      _scopeMode = scopeMode;

#if PASS_BY_VALUE
      if (_appDomainMode == IsolationMode.Private)
        discountRule.ExpectedReturnValueNames =
          new[] { "cart" };
#endif

#if USE_SAFE_ENGINE
      _ruleEngine = new RuleEngine(_appDomainMode);
      _ruleHandle = _ruleEngine.InsertRule(
        discountRule, _runtimeMode);
#else
      _ruleEngine = new UnsafeRuleEngine();
      _ruleHandle = _ruleEngine.InsertRule(
        discountRule);
#endif
      _ruleParameters =
        new Dictionary<string, object>
        {
          {
            "cart",
            this
          }
        };
    }

    public IRule DiscountRule
    {
      set
      {
        if (value == null)
          throw new ArgumentNullException(
            "DiscountRule");
#if USE_SAFE_ENGINE
        _ruleEngine.UpdateRule(_ruleHandle,
          rule: value, runtimeMode: _runtimeMode);
#else
        _ruleEngine.UpdateRule(_ruleHandle,
          rule: value);
#endif
        Recalculate();
      }
    }

#if PASS_BY_VALUE
    private bool CompareModifiedLineItem(
      LineItem original, LineItem modified, int ndx)
    {
      if (modified == null)
        throw new ApplicationException(
          String.Format("After recalculating " +
            "the cart value, line item {0} was " +
            "null. The discount script must not " +
            "remove line items.", ndx));
      if (modified.Product == null)
        throw new ApplicationException(
          String.Format("After recalculating " +
            "the cart value, the product on " +
            "line {0} was null. The discount " +
            "script must not modify the " +
            "products.", ndx));
      if (!original.Product.Equals(modified.Product))
        throw new ApplicationException(
          String.Format("After recalculating " +
            "the cart value, the product on " +
            "line {0} was different from the " +
            "original. The discount script " +
            "must not modify the products.", ndx));
      if (original.Quantity != modified.Quantity)
        throw new ApplicationException(
          String.Format("After recalculating " +
            "the cart value, the quantity on " +
            "line {0} was different from the " +
            "original. The discount script " +
            "must not modify quantities.", ndx));
      return (original.Discount != modified.Discount);
    }

    private void UpdateFromModifiedCart(
      Cart modifiedCart)
    {
      if (modifiedCart == null)
        throw new ApplicationException(
          "The modified cart was not returned " +
          "from the discount script as expected.");
      if (LineItems.Length !=
          modifiedCart.LineItems.Length)
        throw new ApplicationException(
          String.Format("After recalculating the " +
            "cart value, {0} line items were " +
            "expected but {1} items were found. " +
            "The discount script must not add " +
            "or remove line items.", LineItems.Length,
            modifiedCart.LineItems.Length));
      for (int ndx = 0; ndx < LineItems.Length; ndx++)
      {
        LineItem original = LineItems[ndx];
        LineItem modified =
          modifiedCart.LineItems[ndx];
        if (CompareModifiedLineItem(
          original, modified, ndx))
        {
          original.Discount = modified.Discount;
        }
      }
    }
#endif // PASS_BY_VALUE

    private void Recalculate()
    {
      IDictionary<string, dynamic> returnValues;

#if USE_SAFE_ENGINE
      returnValues =
        _ruleEngine.Execute(_ruleHandle,
          _ruleParameters, _scopeMode);
#else
      returnValues =
        _ruleEngine.Execute(_ruleHandle,
          _ruleParameters);
#endif

#if PASS_BY_VALUE
      if (_appDomainMode == IsolationMode.Private)
      {
        Cart modifiedCart =
          returnValues["cart"] as Cart;
        UpdateFromModifiedCart(modifiedCart);
      }
#endif
      if (OnCartChanged != null)
        OnCartChanged();
    }

    public LineItem[] LineItems
    {
      get { return _lineItems.ToArray(); }
    }

    public long ItemCount
    {
      get
      {
        return _lineItems.Sum(
          i => i.Quantity);
      }
    }

    public long ItemCountByCategory(
      string categoryName)
    {
      var query = from item in _lineItems
                  where item.Product
                    .Category == categoryName
                  select item;
      return query.Sum(i => i.Quantity);
    }

    public float CartValue
    {
      get
      {
        return _lineItems.Sum(
          i => i.Quantity
            * i.Product.Price);
      }
    }

    public float DiscountValue
    {
      get
      {
        return _lineItems.Sum(
          i => i.Discount
            * i.Quantity
            * i.Product.Price);
      }
    }

    public float TotalCost
    {
      get
      {
        return _lineItems.Sum(
          i => i.Quantity
            * i.Product.Price
            * (1.0f - i.Discount));
      }
    }

    public void AddProduct(
      Product product, uint quantity)
    {
      if (quantity == 0)
        return;
      if (product == null)
        throw new ArgumentNullException(
          "product");

      var ndx = _lineItems.FindIndex(
          li => li.Product.Equals(product));
      if (ndx >= 0)
      {
        _lineItems[ndx].Quantity
          += quantity;
      }
      else
      {
        _lineItems.Add(
          new LineItem
          {
            Discount = 0.0f,
            Product = product,
            Quantity = quantity
          });
      }
      Recalculate();
    }

    public void AdjustQuantity(
      Product product, int adjustment)
    {
      if (adjustment == 0)
        return;
      if (product == null)
        throw new ArgumentNullException(
          "product");

      var ndx = _lineItems.FindIndex(
          li => li.Product.Equals(product));
      if (ndx >= 0)
      {
        int newQty =
          (int)_lineItems[ndx].Quantity
          + adjustment;
        if (newQty > 0)
          _lineItems[ndx].Quantity = (uint)newQty;
        else
          _lineItems.RemoveAt(ndx);
        Recalculate();
      }
    }

    public void RemoveLineItemByIndex(int ndx)
    {
      if (ndx < 0 || ndx >= _lineItems.Count)
        throw new ArgumentOutOfRangeException(
          "ndx");
      _lineItems.RemoveAt(ndx);
      Recalculate();
    }

#if PASS_BY_VALUE
    [Serializable]
    public class LineItem
#else
    public class LineItem : MarshalByRefObject
#endif
    {
      const float MAX_DISCOUNT = 0.50f;

      public Product Product { get; set; }
      public uint Quantity { get; set; }

      private float _discount;
      public float Discount
      {
        get { return _discount; }
        set
        {
          if (value < 0.0f || value > MAX_DISCOUNT)
            throw new ArgumentOutOfRangeException(
              "Discount", String.Format(
              "The Discount must be non-negative and " +
              "may not exceed {0}. The supplied value " +
              "of {1} is invalid.", MAX_DISCOUNT));
          _discount = value;
        }
      }

      public override string ToString()
      {
        return String.Format(
          "[- {2}%] ({0}) {1}", Quantity,
          Product, Discount * 100);
      }
    }
  }
}
