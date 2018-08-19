namespace SelectorForm
{
    partial class RegressBuyForm
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
            this.mainGrid = new System.Windows.Forms.DataGridView();
            this.subGrid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.mainGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.subGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // mainGrid
            // 
            this.mainGrid.AllowUserToAddRows = false;
            this.mainGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mainGrid.Location = new System.Drawing.Point(0, 0);
            this.mainGrid.Name = "mainGrid";
            this.mainGrid.ReadOnly = true;
            this.mainGrid.RowHeadersWidth = 50;
            this.mainGrid.RowTemplate.Height = 23;
            this.mainGrid.Size = new System.Drawing.Size(1024, 438);
            this.mainGrid.TabIndex = 0;
            this.mainGrid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.mainGrid_RowsAdded);
            this.mainGrid.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.mainGrid_RowsRemoved);
            // 
            // subGrid
            // 
            this.subGrid.AllowUserToAddRows = false;
            this.subGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.subGrid.Location = new System.Drawing.Point(0, 445);
            this.subGrid.Name = "subGrid";
            this.subGrid.ReadOnly = true;
            this.subGrid.RowHeadersWidth = 50;
            this.subGrid.RowTemplate.Height = 23;
            this.subGrid.Size = new System.Drawing.Size(1024, 150);
            this.subGrid.TabIndex = 1;
            this.subGrid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.subGrid_RowsAdded);
            this.subGrid.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.subGrid_RowsRemoved);
            // 
            // RegressBuyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 585);
            this.Controls.Add(this.subGrid);
            this.Controls.Add(this.mainGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RegressBuyForm";
            this.Text = "RegressTabPage";
            ((System.ComponentModel.ISupportInitialize)(this.mainGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.subGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView mainGrid;
        private System.Windows.Forms.DataGridView subGrid;
    }
}