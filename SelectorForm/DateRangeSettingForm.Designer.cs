namespace SelectorForm
{
    partial class DateRangeSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DateRangeSettingForm));
            this.dateRangeGrid_ = new System.Windows.Forms.DataGridView();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.nameListBox_ = new System.Windows.Forms.ListBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btRemove = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dateRangeGrid_)).BeginInit();
            this.SuspendLayout();
            // 
            // dateRangeGrid_
            // 
            this.dateRangeGrid_.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dateRangeGrid_.Location = new System.Drawing.Point(217, -2);
            this.dateRangeGrid_.Name = "dateRangeGrid_";
            this.dateRangeGrid_.RowTemplate.Height = 23;
            this.dateRangeGrid_.Size = new System.Drawing.Size(384, 376);
            this.dateRangeGrid_.TabIndex = 0;
            this.dateRangeGrid_.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dateRangeGrid__CellClick);
            this.dateRangeGrid_.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dateRangeGrid__ColumnWidthChanged);
            this.dateRangeGrid_.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dateRangeGrid__Scroll);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(1, 398);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(508, 398);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // nameListBox_
            // 
            this.nameListBox_.FormattingEnabled = true;
            this.nameListBox_.ItemHeight = 12;
            this.nameListBox_.Location = new System.Drawing.Point(1, -2);
            this.nameListBox_.Name = "nameListBox_";
            this.nameListBox_.Size = new System.Drawing.Size(210, 376);
            this.nameListBox_.TabIndex = 2;
            this.nameListBox_.SelectedIndexChanged += new System.EventHandler(this.nameListBox__SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(136, 398);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(217, 398);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btRemove
            // 
            this.btRemove.Location = new System.Drawing.Point(298, 398);
            this.btRemove.Name = "btRemove";
            this.btRemove.Size = new System.Drawing.Size(75, 23);
            this.btRemove.TabIndex = 1;
            this.btRemove.Text = "Remove";
            this.btRemove.UseVisualStyleBackColor = true;
            this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
            // 
            // DateRangeSettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(595, 446);
            this.Controls.Add(this.nameListBox_);
            this.Controls.Add(this.btRemove);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dateRangeGrid_);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DateRangeSettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DateRangeSettingForm";
            ((System.ComponentModel.ISupportInitialize)(this.dateRangeGrid_)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dateRangeGrid_;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ListBox nameListBox_;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btRemove;
    }
}