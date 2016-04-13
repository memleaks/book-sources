using System;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using DevJourney.Scripting;
using DevJourney.Commerce;

namespace ECommerceExample
{
  public partial class MainForm : Form
  {
    private Cart _cart;
    private IRule _rule;

    public MainForm()
    {
      InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      string fileName = "ApplyDiscounts.py";
      string scriptPath = Path.GetDirectoryName(
        Application.ExecutablePath) + "\\" + fileName;
      if (!File.Exists(scriptPath))
      {
        MessageBox.Show(this, String.Format(
          "The script file '{0}' cannot be found.",
          fileName), "Error");
        Close();
      }
      _rule = new RuleFromFile(scriptPath);
      _cart = new Cart(_rule,
        appDomainMode: IsolationMode.Private,
        runtimeMode: IsolationMode.Shared,
        scopeMode: IsolationMode.Shared);
      _cart.OnCartChanged += OnCartValueChanged;
    }

    private void OnCartValueChanged()
    {
      lbLineItems.DataSource = null;
      lbLineItems.DataSource = _cart.LineItems;
      lblItemCount.Text = _cart.ItemCount.ToString();
      lblClothingItemCount.Text =
        _cart.ItemCountByCategory("Clothing").ToString();
      lblGadgetItemCount.Text =
        _cart.ItemCountByCategory("Gadgets").ToString();
      lblToyItems.Text =
        _cart.ItemCountByCategory("Toys").ToString();
      lblCartValue.Text = _cart.CartValue.ToString("C");
      lblCost.Text = _cart.TotalCost.ToString("C");
      lblDiscountTotal.Text =
        _cart.DiscountValue.ToString("C");
    }

    private void OnRemoveLineItem(
      object sender, EventArgs e)
    {
      if (lbLineItems.SelectedIndex < 0)
        return;
      _cart.RemoveLineItemByIndex(
        lbLineItems.SelectedIndex);
    }

    private void OnAddProducts(
      object sender, EventArgs e)
    {
      var parts = cbSampleProducts
        .SelectedItem.ToString().Split('/');
      var np = new Product(parts[0].Trim(),
        parts[1].Trim(), Single.Parse(parts[2]),
        parts[3].Trim());
      _cart.AddProduct(np, Convert.ToUInt32(
        nudNewProductQuantity.Value));
    }

    private void OnRemoveProducts(
      object sender, EventArgs e)
    {
      var parts = cbSampleProducts
        .SelectedItem.ToString().Split('/');
      var ep = new Product(parts[0].Trim(),
        parts[1].Trim(), Single.Parse(parts[2]),
        parts[3].Trim());
      _cart.AdjustQuantity(ep, -Convert.ToInt32(
        nudNewProductQuantity.Value));
    }

    private RuleEditor _ruleEditor = null;
    private void OnEditRules(object sender, EventArgs e)
    {
      if (_ruleEditor == null)
      {
        _ruleEditor = new RuleEditor(_rule);
        _ruleEditor.OnRuleChanged += WhenRulesAreEdited;
      }

      if (_ruleEditor.Visible)
      {
        _ruleEditor.Hide();
        btnEditRules.Text = "Edit the Rules";
      }
      else
      {
        _ruleEditor.Show();
        btnEditRules.Text = "Hide the Rules";
      }
    }

    void WhenRulesAreEdited(IRule rule)
    {
      _cart.DiscountRule = rule;
    }
  }
}