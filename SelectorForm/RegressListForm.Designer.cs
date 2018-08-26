namespace SelectorForm
{
    partial class RegressListForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegressListForm));
            this.regressGrid = new System.Windows.Forms.DataGridView();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.regressGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // regressGrid
            // 
            this.regressGrid.AllowUserToAddRows = false;
            this.regressGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.regressGrid.Location = new System.Drawing.Point(-2, 0);
            this.regressGrid.Name = "regressGrid";
            this.regressGrid.ReadOnly = true;
            this.regressGrid.RowTemplate.Height = 23;
            this.regressGrid.Size = new System.Drawing.Size(572, 289);
            this.regressGrid.TabIndex = 0;
            this.regressGrid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.regressGrid_RowsAdded);
            this.regressGrid.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.regressGrid_RowsRemoved);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(12, 295);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(156, 295);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(322, 295);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(75, 23);
            this.btnView.TabIndex = 1;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(483, 295);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // RegressListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 324);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.regressGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RegressListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RegressListForm";
            ((System.ComponentModel.ISupportInitialize)(this.regressGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView regressGrid;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnExit;
    }
}