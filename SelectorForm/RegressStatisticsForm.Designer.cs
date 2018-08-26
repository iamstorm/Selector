namespace SelectorForm
{
    partial class RegressStatisticsForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart_ = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.straListView_ = new System.Windows.Forms.ListView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.historyView = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.writeAssetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.chart_)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart_
            // 
            chartArea1.AxisX.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.None;
            chartArea1.AxisX.LabelStyle.Interval = 1D;
            chartArea1.Name = "ChartArea1";
            this.chart_.ChartAreas.Add(chartArea1);
            this.chart_.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart_.Legends.Add(legend1);
            this.chart_.Location = new System.Drawing.Point(0, 0);
            this.chart_.Name = "chart_";
            this.chart_.Size = new System.Drawing.Size(767, 360);
            this.chart_.TabIndex = 0;
            this.chart_.Text = "test";
            // 
            // straListView_
            // 
            this.straListView_.Dock = System.Windows.Forms.DockStyle.Fill;
            this.straListView_.Location = new System.Drawing.Point(0, 25);
            this.straListView_.Name = "straListView_";
            this.straListView_.Size = new System.Drawing.Size(767, 35);
            this.straListView_.TabIndex = 1;
            this.straListView_.UseCompatibleStateImageBehavior = false;
            this.straListView_.SelectedIndexChanged += new System.EventHandler(this.straListView__SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.historyView);
            this.splitContainer1.Panel1.Controls.Add(this.straListView_);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chart_);
            this.splitContainer1.Size = new System.Drawing.Size(767, 424);
            this.splitContainer1.SplitterDistance = 60;
            this.splitContainer1.TabIndex = 2;
            // 
            // historyView
            // 
            this.historyView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyView.Location = new System.Drawing.Point(0, 25);
            this.historyView.Name = "historyView";
            this.historyView.Size = new System.Drawing.Size(767, 35);
            this.historyView.TabIndex = 2;
            this.historyView.UseCompatibleStateImageBehavior = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.writeAssetToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(767, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // writeAssetToolStripMenuItem
            // 
            this.writeAssetToolStripMenuItem.Name = "writeAssetToolStripMenuItem";
            this.writeAssetToolStripMenuItem.Size = new System.Drawing.Size(82, 21);
            this.writeAssetToolStripMenuItem.Text = "WriteAsset";
            this.writeAssetToolStripMenuItem.Click += new System.EventHandler(this.writeAssetToolStripMenuItem_Click);
            // 
            // RegressStatisticsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 424);
            this.Controls.Add(this.splitContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RegressStatisticsForm";
            this.Text = "RegressStatisticsForm";
            ((System.ComponentModel.ISupportInitialize)(this.chart_)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart_;
        private System.Windows.Forms.ListView straListView_;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView historyView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem writeAssetToolStripMenuItem;

    }
}