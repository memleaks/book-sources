namespace ECommerceExample
{
  partial class RuleEditor
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.tbRule = new System.Windows.Forms.TextBox();
      this.btnApply = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // tbRule
      // 
      this.tbRule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbRule.BackColor = System.Drawing.Color.Black;
      this.tbRule.ForeColor = System.Drawing.Color.White;
      this.tbRule.Location = new System.Drawing.Point(0, 0);
      this.tbRule.Multiline = true;
      this.tbRule.Name = "tbRule";
      this.tbRule.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.tbRule.Size = new System.Drawing.Size(521, 423);
      this.tbRule.TabIndex = 0;
      // 
      // btnApply
      // 
      this.btnApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.btnApply.Location = new System.Drawing.Point(156, 431);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(173, 41);
      this.btnApply.TabIndex = 1;
      this.btnApply.Text = "Apply Now";
      this.btnApply.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btnApply.UseVisualStyleBackColor = true;
      this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
      // 
      // RuleEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(521, 484);
      this.ControlBox = false;
      this.Controls.Add(this.btnApply);
      this.Controls.Add(this.tbRule);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(6);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "RuleEditor";
      this.Text = "Rule Editor";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox tbRule;
    private System.Windows.Forms.Button btnApply;
  }
}