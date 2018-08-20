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
            this.mainListView_ = new System.Windows.Forms.ListView();
            this.subListView_ = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // mainListView_
            // 
            this.mainListView_.Location = new System.Drawing.Point(-1, -4);
            this.mainListView_.Name = "mainListView_";
            this.mainListView_.Size = new System.Drawing.Size(1025, 443);
            this.mainListView_.TabIndex = 2;
            this.mainListView_.UseCompatibleStateImageBehavior = false;
            this.mainListView_.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.mainListView__RetrieveVirtualItem);
            // 
            // subListView_
            // 
            this.subListView_.Location = new System.Drawing.Point(-1, 446);
            this.subListView_.Name = "subListView_";
            this.subListView_.Size = new System.Drawing.Size(1025, 199);
            this.subListView_.TabIndex = 3;
            this.subListView_.UseCompatibleStateImageBehavior = false;
            // 
            // RegressSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 585);
            this.Controls.Add(this.subListView_);
            this.Controls.Add(this.mainListView_);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RegressSelectForm";
            this.Text = "RegressTabPage";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mainListView_;
        private System.Windows.Forms.ListView subListView_;

    }
}