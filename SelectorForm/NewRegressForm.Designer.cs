namespace SelectorForm
{
    partial class NewRegressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewRegressForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.dateRangeComboBox_ = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.name_ = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.slnComboBox_ = new System.Windows.Forms.ComboBox();
            this.radioRaw = new System.Windows.Forms.RadioButton();
            this.radioAsset = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(242, 199);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOk.Location = new System.Drawing.Point(141, 199);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 70);
            this.label4.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "DateRange:";
            // 
            // dateRangeComboBox_
            // 
            this.dateRangeComboBox_.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dateRangeComboBox_.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dateRangeComboBox_.FormattingEnabled = true;
            this.dateRangeComboBox_.Location = new System.Drawing.Point(124, 70);
            this.dateRangeComboBox_.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            this.dateRangeComboBox_.Name = "dateRangeComboBox_";
            this.dateRangeComboBox_.Size = new System.Drawing.Size(277, 20);
            this.dateRangeComboBox_.TabIndex = 1;
            this.dateRangeComboBox_.SelectedIndexChanged += new System.EventHandler(this.dateRangeComboBox__SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(74, 128);
            this.label3.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Name:";
            // 
            // name_
            // 
            this.name_.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.name_.Location = new System.Drawing.Point(124, 125);
            this.name_.Name = "name_";
            this.name_.Size = new System.Drawing.Size(277, 21);
            this.name_.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Solution:";
            // 
            // slnComboBox_
            // 
            this.slnComboBox_.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.slnComboBox_.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.slnComboBox_.FormattingEnabled = true;
            this.slnComboBox_.Location = new System.Drawing.Point(124, 21);
            this.slnComboBox_.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            this.slnComboBox_.Name = "slnComboBox_";
            this.slnComboBox_.Size = new System.Drawing.Size(277, 20);
            this.slnComboBox_.TabIndex = 1;
            this.slnComboBox_.SelectedIndexChanged += new System.EventHandler(this.slnComboBox__SelectedIndexChanged);
            // 
            // radioRaw
            // 
            this.radioRaw.AutoSize = true;
            this.radioRaw.Location = new System.Drawing.Point(124, 167);
            this.radioRaw.Name = "radioRaw";
            this.radioRaw.Size = new System.Drawing.Size(65, 16);
            this.radioRaw.TabIndex = 8;
            this.radioRaw.TabStop = true;
            this.radioRaw.Text = "RawMode";
            this.radioRaw.UseVisualStyleBackColor = true;
            // 
            // radioAsset
            // 
            this.radioAsset.AutoSize = true;
            this.radioAsset.Location = new System.Drawing.Point(259, 167);
            this.radioAsset.Name = "radioAsset";
            this.radioAsset.Size = new System.Drawing.Size(77, 16);
            this.radioAsset.TabIndex = 8;
            this.radioAsset.TabStop = true;
            this.radioAsset.Text = "AssetMode";
            this.radioAsset.UseVisualStyleBackColor = true;
            // 
            // NewRegressForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(451, 264);
            this.Controls.Add(this.radioAsset);
            this.Controls.Add(this.radioRaw);
            this.Controls.Add(this.name_);
            this.Controls.Add(this.dateRangeComboBox_);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.slnComboBox_);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "NewRegressForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NewRegressForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewRegressForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox dateRangeComboBox_;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox name_;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox slnComboBox_;
        private System.Windows.Forms.RadioButton radioRaw;
        private System.Windows.Forms.RadioButton radioAsset;
    }
}