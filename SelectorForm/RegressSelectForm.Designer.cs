namespace SelectorForm
{
    partial class RegressSelectForm
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
            this.subListView_ = new System.Windows.Forms.ListView();
            this.mainListView_ = new System.Windows.Forms.ListView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // subListView_
            // 
            this.subListView_.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subListView_.Location = new System.Drawing.Point(0, 0);
            this.subListView_.Name = "subListView_";
            this.subListView_.Size = new System.Drawing.Size(1020, 205);
            this.subListView_.TabIndex = 3;
            this.subListView_.UseCompatibleStateImageBehavior = false;
            this.subListView_.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.subListView__ColumnClick);
            this.subListView_.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.subListView__RetrieveVirtualItem);
            // 
            // mainListView_
            // 
            this.mainListView_.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainListView_.Location = new System.Drawing.Point(0, 0);
            this.mainListView_.Name = "mainListView_";
            this.mainListView_.Size = new System.Drawing.Size(1020, 370);
            this.mainListView_.TabIndex = 2;
            this.mainListView_.UseCompatibleStateImageBehavior = false;
            this.mainListView_.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.mainListView__ColumnClick);
            this.mainListView_.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.mainListView__ItemSelectionChanged);
            this.mainListView_.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.mainListView__RetrieveVirtualItem);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.mainListView_);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.subListView_);
            this.splitContainer1.Size = new System.Drawing.Size(1020, 579);
            this.splitContainer1.SplitterDistance = 370;
            this.splitContainer1.TabIndex = 4;
            // 
            // RegressSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 585);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RegressSelectForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Text = "RegressTabPage";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView subListView_;
        private System.Windows.Forms.ListView mainListView_;
        private System.Windows.Forms.SplitContainer splitContainer1;


    }
}