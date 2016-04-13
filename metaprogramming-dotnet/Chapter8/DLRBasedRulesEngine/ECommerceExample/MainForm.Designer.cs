namespace ECommerceExample
{
  partial class MainForm
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
      this.label1 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.nudNewProductQuantity = new System.Windows.Forms.NumericUpDown();
      this.lbLineItems = new System.Windows.Forms.ListBox();
      this.cbSampleProducts = new System.Windows.Forms.ComboBox();
      this.btnRemoveSelectedLineItem = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.lblItemCount = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.lblCartValue = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.lblDiscountTotal = new System.Windows.Forms.Label();
      this.lblCost = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.btnAddProducts = new System.Windows.Forms.Button();
      this.btnRemoveProducts = new System.Windows.Forms.Button();
      this.btnEditRules = new System.Windows.Forms.Button();
      this.label7 = new System.Windows.Forms.Label();
      this.lblClothingItemCount = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.lblGadgetItemCount = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.lblToyItems = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.nudNewProductQuantity)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(96, 29);
      this.label1.TabIndex = 0;
      this.label1.Text = "Product";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(645, 60);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(49, 29);
      this.label5.TabIndex = 10;
      this.label5.Text = "Qty";
      // 
      // nudNewProductQuantity
      // 
      this.nudNewProductQuantity.Location = new System.Drawing.Point(700, 58);
      this.nudNewProductQuantity.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.nudNewProductQuantity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.nudNewProductQuantity.Name = "nudNewProductQuantity";
      this.nudNewProductQuantity.Size = new System.Drawing.Size(76, 34);
      this.nudNewProductQuantity.TabIndex = 12;
      this.nudNewProductQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.nudNewProductQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // lbLineItems
      // 
      this.lbLineItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.lbLineItems.FormattingEnabled = true;
      this.lbLineItems.IntegralHeight = false;
      this.lbLineItems.ItemHeight = 29;
      this.lbLineItems.Location = new System.Drawing.Point(12, 117);
      this.lbLineItems.Name = "lbLineItems";
      this.lbLineItems.Size = new System.Drawing.Size(843, 227);
      this.lbLineItems.TabIndex = 14;
      // 
      // cbSampleProducts
      // 
      this.cbSampleProducts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cbSampleProducts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbSampleProducts.FormattingEnabled = true;
      this.cbSampleProducts.Items.AddRange(new object[] {
            "all your base are belong to us T-Shirt / C0012197 / 17.99 / Clothing",
            "Airzooka Air Gun / T21178 / 12.99 / Toys",
            "Batman Cuff Links / C930121 / 39.99 / Clothing",
            "Blade Runner Style LED Umbrella / G600011 / 24.99 / Gadgets",
            "Digital Measuring Tape / G544660 / 24.99 / Gadgets",
            "Electronic Spy Camera Shirt / C138923 / 39.99 / Clothing",
            "High-powered Green Laser / G559093 / 99.99 / Gadgets",
            "Micro Sonic Grenade / T21180 / 7.99 / Toys",
            "Nerf Stampede Automatic Heavy Blaster / T10131 / 69.99 / Toys",
            "NO. T-Shirt / C209876 / 15.99 / Clothing",
            "Personal Soundtrack Shirt / C457723 / 19.99 / Clothing",
            "Pizza Pi Cutter / G345196 / 12.99 / Gadgets",
            "Star Theater Pro Home Planetarium / G289890 / 169.99 / Toys",
            "Sudo T-Shirt / C111312 / 16.99 / Clothing",
            "Sun and Moon Jars / G300206 / 34.99 / Gadgets",
            "Tech Support Babydoll T-Shirt / C692110 / 17.99 / Clothing",
            "USB Rocket Launcher / T22215 / 24.99 / Toys"});
      this.cbSampleProducts.Location = new System.Drawing.Point(114, 12);
      this.cbSampleProducts.Name = "cbSampleProducts";
      this.cbSampleProducts.Size = new System.Drawing.Size(741, 37);
      this.cbSampleProducts.TabIndex = 15;
      // 
      // btnRemoveSelectedLineItem
      // 
      this.btnRemoveSelectedLineItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnRemoveSelectedLineItem.Location = new System.Drawing.Point(12, 361);
      this.btnRemoveSelectedLineItem.Name = "btnRemoveSelectedLineItem";
      this.btnRemoveSelectedLineItem.Size = new System.Drawing.Size(251, 45);
      this.btnRemoveSelectedLineItem.TabIndex = 16;
      this.btnRemoveSelectedLineItem.Text = "Remove Selected";
      this.btnRemoveSelectedLineItem.UseVisualStyleBackColor = true;
      this.btnRemoveSelectedLineItem.Click += new System.EventHandler(this.OnRemoveLineItem);
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(359, 444);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(132, 29);
      this.label2.TabIndex = 17;
      this.label2.Text = "Total items";
      // 
      // lblItemCount
      // 
      this.lblItemCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblItemCount.Location = new System.Drawing.Point(497, 443);
      this.lblItemCount.Name = "lblItemCount";
      this.lblItemCount.Size = new System.Drawing.Size(61, 29);
      this.lblItemCount.TabIndex = 18;
      this.lblItemCount.Text = "0";
      this.lblItemCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label3
      // 
      this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(601, 372);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(120, 29);
      this.label3.TabIndex = 19;
      this.label3.Text = "Cart value";
      // 
      // lblCartValue
      // 
      this.lblCartValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblCartValue.Location = new System.Drawing.Point(727, 372);
      this.lblCartValue.Name = "lblCartValue";
      this.lblCartValue.Size = new System.Drawing.Size(97, 29);
      this.lblCartValue.TabIndex = 20;
      this.lblCartValue.Text = "$0.00";
      this.lblCartValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(603, 401);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(118, 29);
      this.label4.TabIndex = 21;
      this.label4.Text = "Discounts";
      // 
      // lblDiscountTotal
      // 
      this.lblDiscountTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblDiscountTotal.Location = new System.Drawing.Point(727, 401);
      this.lblDiscountTotal.Name = "lblDiscountTotal";
      this.lblDiscountTotal.Size = new System.Drawing.Size(97, 29);
      this.lblDiscountTotal.TabIndex = 22;
      this.lblDiscountTotal.Text = "$0.00";
      this.lblDiscountTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lblCost
      // 
      this.lblCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblCost.Location = new System.Drawing.Point(727, 430);
      this.lblCost.Name = "lblCost";
      this.lblCost.Size = new System.Drawing.Size(97, 29);
      this.lblCost.TabIndex = 23;
      this.lblCost.Text = "$0.00";
      this.lblCost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label6
      // 
      this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(598, 430);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(118, 29);
      this.label6.TabIndex = 24;
      this.label6.Text = "Total cost";
      // 
      // btnAddProducts
      // 
      this.btnAddProducts.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAddProducts.Location = new System.Drawing.Point(782, 56);
      this.btnAddProducts.Name = "btnAddProducts";
      this.btnAddProducts.Size = new System.Drawing.Size(36, 36);
      this.btnAddProducts.TabIndex = 25;
      this.btnAddProducts.Text = "+";
      this.btnAddProducts.UseVisualStyleBackColor = true;
      this.btnAddProducts.Click += new System.EventHandler(this.OnAddProducts);
      // 
      // btnRemoveProducts
      // 
      this.btnRemoveProducts.Font = new System.Drawing.Font("Courier New", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnRemoveProducts.Location = new System.Drawing.Point(824, 56);
      this.btnRemoveProducts.Name = "btnRemoveProducts";
      this.btnRemoveProducts.Size = new System.Drawing.Size(36, 36);
      this.btnRemoveProducts.TabIndex = 26;
      this.btnRemoveProducts.Text = "-";
      this.btnRemoveProducts.UseVisualStyleBackColor = true;
      this.btnRemoveProducts.Click += new System.EventHandler(this.OnRemoveProducts);
      // 
      // btnEditRules
      // 
      this.btnEditRules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnEditRules.Location = new System.Drawing.Point(12, 426);
      this.btnEditRules.Name = "btnEditRules";
      this.btnEditRules.Size = new System.Drawing.Size(251, 45);
      this.btnEditRules.TabIndex = 27;
      this.btnEditRules.Text = "Edit the Rules";
      this.btnEditRules.UseVisualStyleBackColor = true;
      this.btnEditRules.Click += new System.EventHandler(this.OnEditRules);
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(7, 85);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(268, 29);
      this.label7.TabIndex = 28;
      this.label7.Text = "Shopping Cart Contents";
      // 
      // lblClothingItemCount
      // 
      this.lblClothingItemCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblClothingItemCount.Location = new System.Drawing.Point(497, 356);
      this.lblClothingItemCount.Name = "lblClothingItemCount";
      this.lblClothingItemCount.Size = new System.Drawing.Size(61, 29);
      this.lblClothingItemCount.TabIndex = 30;
      this.lblClothingItemCount.Text = "0";
      this.lblClothingItemCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label9
      // 
      this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(325, 356);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(166, 29);
      this.label9.TabIndex = 29;
      this.label9.Text = "Clothing items";
      // 
      // lblGadgetItemCount
      // 
      this.lblGadgetItemCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblGadgetItemCount.Location = new System.Drawing.Point(497, 385);
      this.lblGadgetItemCount.Name = "lblGadgetItemCount";
      this.lblGadgetItemCount.Size = new System.Drawing.Size(61, 29);
      this.lblGadgetItemCount.TabIndex = 32;
      this.lblGadgetItemCount.Text = "0";
      this.lblGadgetItemCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label10
      // 
      this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(335, 385);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(156, 29);
      this.label10.TabIndex = 31;
      this.label10.Text = "Gadget items";
      // 
      // lblToyItems
      // 
      this.lblToyItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lblToyItems.Location = new System.Drawing.Point(497, 414);
      this.lblToyItems.Name = "lblToyItems";
      this.lblToyItems.Size = new System.Drawing.Size(61, 29);
      this.lblToyItems.TabIndex = 34;
      this.lblToyItems.Text = "0";
      this.lblToyItems.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label12
      // 
      this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(373, 414);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(118, 29);
      this.label12.TabIndex = 33;
      this.label12.Text = "Toy items";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(867, 483);
      this.Controls.Add(this.lblToyItems);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.lblGadgetItemCount);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.lblClothingItemCount);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.btnEditRules);
      this.Controls.Add(this.btnRemoveProducts);
      this.Controls.Add(this.btnAddProducts);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.lblCost);
      this.Controls.Add(this.lblDiscountTotal);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.lblCartValue);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.lblItemCount);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.btnRemoveSelectedLineItem);
      this.Controls.Add(this.cbSampleProducts);
      this.Controls.Add(this.lbLineItems);
      this.Controls.Add(this.nudNewProductQuantity);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label1);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(6);
      this.Name = "MainForm";
      this.Text = "RuleEngine Test Harness - Discount Shopping Cart Items";
      this.Load += new System.EventHandler(this.MainForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.nudNewProductQuantity)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.NumericUpDown nudNewProductQuantity;
    private System.Windows.Forms.ListBox lbLineItems;
    private System.Windows.Forms.ComboBox cbSampleProducts;
    private System.Windows.Forms.Button btnRemoveSelectedLineItem;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label lblItemCount;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label lblCartValue;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label lblDiscountTotal;
    private System.Windows.Forms.Label lblCost;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Button btnAddProducts;
    private System.Windows.Forms.Button btnRemoveProducts;
    private System.Windows.Forms.Button btnEditRules;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label lblClothingItemCount;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label lblGadgetItemCount;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label lblToyItems;
    private System.Windows.Forms.Label label12;
  }
}