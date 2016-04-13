//#define InefficientDataSource
#define ImprovedDataSource

using System.Reflection;
using System.Collections;

class PseudoListBox
{
  public string DisplayMember { get; set; }
  public string ValueMember { get; set; }

#if ImprovedDataSource
  public IEnumerable DataSource
  {
    set
    {
      IEnumerator iterator = value.GetEnumerator();
      object currentItem;
      do
      {
        if (!iterator.MoveNext())
          return;
        currentItem = iterator.Current;
      } while (currentItem == null);

      PropertyInfo displayMetadata =
        currentItem.GetType().GetProperty(DisplayMember);
      PropertyInfo valueMetadata =
        currentItem.GetType().GetProperty(ValueMember);

      do
      {
        currentItem = iterator.Current;
        string displayString =
          displayMetadata.GetValue(currentItem, null).ToString();
        // insert a listbox item here showing the displayString

        object valueObject =
          valueMetadata.GetValue(currentItem, null);
        // assign this value to the listbox item here
      } while (iterator.MoveNext());
    }
  }
#endif

#if InefficientDataSource
  public System.Collections.IEnumerable DataSource
  {
    set
    {
      foreach (object current in value)
      {
        System.Reflection.PropertyInfo displayMetadata =
          current.GetType().GetProperty(DisplayMember);
        string displayString =
          displayMetadata.GetValue(current, null).ToString();
        // insert a listbox item here showing the displayString

        System.Reflection.PropertyInfo valueMetadata =
          current.GetType().GetProperty(ValueMember);
        object valueObject =
          valueMetadata.GetValue(current, null);
        // assign this value to the listbox item here
      }
    }
  }
#endif

  class PseudoDataBinding
  {
    static void Main()
    {
    }
  }
}

