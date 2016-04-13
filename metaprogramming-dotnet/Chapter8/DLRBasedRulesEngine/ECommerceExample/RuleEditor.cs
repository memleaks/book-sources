using System;
using System.Windows.Forms;
using DevJourney.Scripting;

namespace ECommerceExample
{
  public partial class RuleEditor : Form
  {
    private IRule _rule;

    public RuleEditor(IRule rule)
    {
      if (rule == null)
        throw new ArgumentNullException("rule");

      InitializeComponent();
      _rule = rule;
      tbRule.Text = _rule.Body;
      Text = String.Format(
        "Edit Rule '{0}'", _rule.Name);
    }

    public delegate void RuleChanged(IRule rule);
    public event RuleChanged OnRuleChanged;

    private void FireRuleChangedEvent()
    {
      if (OnRuleChanged == null)
        return;
      OnRuleChanged(_rule);
    }

    private void btnApply_Click(
      object sender, EventArgs e)
    {
      if (_rule == null)
        return;
      _rule.Body = tbRule.Text;
      FireRuleChangedEvent();
    }
  }
}