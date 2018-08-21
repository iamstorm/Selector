namespace SelectorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menu = new System.Windows.Forms.MenuStrip();
            this.basicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regressLisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startWorker = new System.ComponentModel.BackgroundWorker();
            this.selectWorker = new System.ComponentModel.BackgroundWorker();
            this.endWorker = new System.ComponentModel.BackgroundWorker();
            this.regressWorker = new System.ComponentModel.BackgroundWorker();
            this.msgText = new System.Windows.Forms.Label();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.mainTab = new System.Windows.Forms.TabControl();
            this.TabSelect = new System.Windows.Forms.TabPage();
            this.selectListView_ = new System.Windows.Forms.ListView();
            this.statusStrip_ = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar_ = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1_ = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2_ = new System.Windows.Forms.ToolStripStatusLabel();
            this.menu.SuspendLayout();
            this.mainLayout.SuspendLayout();
            this.mainTab.SuspendLayout();
            this.TabSelect.SuspendLayout();
            this.statusStrip_.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.regressToolStripMenuItem,
            this.regressLisToolStripMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(1028, 25);
            this.menu.TabIndex = 1;
            this.menu.Text = "menu";
            // 
            // basicToolStripMenuItem
            // 
            this.basicToolStripMenuItem.Name = "basicToolStripMenuItem";
            this.basicToolStripMenuItem.Size = new System.Drawing.Size(50, 21);
            this.basicToolStripMenuItem.Text = "Basic";
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(54, 21);
            this.selectToolStripMenuItem.Text = "Select";
            this.selectToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // regressToolStripMenuItem
            // 
            this.regressToolStripMenuItem.Name = "regressToolStripMenuItem";
            this.regressToolStripMenuItem.Size = new System.Drawing.Size(67, 21);
            this.regressToolStripMenuItem.Text = "Regress";
            this.regressToolStripMenuItem.Click += new System.EventHandler(this.regressToolStripMenuItem_Click);
            // 
            // regressLisToolStripMenuItem
            // 
            this.regressLisToolStripMenuItem.Name = "regressLisToolStripMenuItem";
            this.regressLisToolStripMenuItem.Size = new System.Drawing.Size(86, 21);
            this.regressLisToolStripMenuItem.Text = "RegressList";
            this.regressLisToolStripMenuItem.Click += new System.EventHandler(this.regressLisToolStripMenuItem_Click);
            // 
            // startWorker
            // 
            this.startWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.startWorker_DoWork);
            this.startWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.startWorker_RunWorkerCompleted);
            // 
            // selectWorker
            // 
            this.selectWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.selectWorker_DoWork);
            this.selectWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.selectWorker_RunWorkerCompleted);
            // 
            // endWorker
            // 
            this.endWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.endWorker_DoWork);
            // 
            // regressWorker
            // 
            this.regressWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.regressWorker_DoWork);
            this.regressWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.regressWorker_RunWorkerCompleted);
            // 
            // msgText
            // 
            this.msgText.AutoSize = true;
            this.msgText.Location = new System.Drawing.Point(3, 621);
            this.msgText.Name = "msgText";
            this.msgText.Size = new System.Drawing.Size(29, 12);
            this.msgText.TabIndex = 3;
            this.msgText.Text = "msg:";
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.msgText, 0, 1);
            this.mainLayout.Controls.Add(this.mainTab, 0, 0);
            this.mainLayout.Controls.Add(this.statusStrip_, 0, 2);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 25);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainLayout.Size = new System.Drawing.Size(1028, 661);
            this.mainLayout.TabIndex = 4;
            // 
            // mainTab
            // 
            this.mainTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTab.Controls.Add(this.TabSelect);
            this.mainTab.Location = new System.Drawing.Point(3, 3);
            this.mainTab.Name = "mainTab";
            this.mainTab.SelectedIndex = 0;
            this.mainTab.Size = new System.Drawing.Size(1022, 615);
            this.mainTab.TabIndex = 0;
            // 
            // TabSelect
            // 
            this.TabSelect.Controls.Add(this.selectListView_);
            this.TabSelect.Location = new System.Drawing.Point(4, 22);
            this.TabSelect.Name = "TabSelect";
            this.TabSelect.Padding = new System.Windows.Forms.Padding(3);
            this.TabSelect.Size = new System.Drawing.Size(1014, 589);
            this.TabSelect.TabIndex = 0;
            this.TabSelect.Text = "SelectList";
            this.TabSelect.UseVisualStyleBackColor = true;
            // 
            // selectListView_
            // 
            this.selectListView_.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectListView_.Location = new System.Drawing.Point(3, 3);
            this.selectListView_.Name = "selectListView_";
            this.selectListView_.Size = new System.Drawing.Size(1008, 583);
            this.selectListView_.TabIndex = 0;
            this.selectListView_.UseCompatibleStateImageBehavior = false;
            this.selectListView_.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.selectListView__ColumnClick);
            this.selectListView_.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.selectList__RetrieveVirtualItem);
            // 
            // statusStrip_
            // 
            this.statusStrip_.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar_,
            this.toolStripStatusLabel1_,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2_});
            this.statusStrip_.Location = new System.Drawing.Point(0, 641);
            this.statusStrip_.Name = "statusStrip_";
            this.statusStrip_.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip_.Size = new System.Drawing.Size(1028, 20);
            this.statusStrip_.TabIndex = 4;
            this.statusStrip_.Text = "statusStrip1";
            // 
            // toolStripProgressBar_
            // 
            this.toolStripProgressBar_.Name = "toolStripProgressBar_";
            this.toolStripProgressBar_.Size = new System.Drawing.Size(100, 14);
            // 
            // toolStripStatusLabel1_
            // 
            this.toolStripStatusLabel1_.Name = "toolStripStatusLabel1_";
            this.toolStripStatusLabel1_.Size = new System.Drawing.Size(131, 15);
            this.toolStripStatusLabel1_.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(649, 15);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // toolStripStatusLabel2_
            // 
            this.toolStripStatusLabel2_.Name = "toolStripStatusLabel2_";
            this.toolStripStatusLabel2_.Size = new System.Drawing.Size(131, 15);
            this.toolStripStatusLabel2_.Text = "toolStripStatusLabel2";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 686);
            this.Controls.Add(this.mainLayout);
            this.Controls.Add(this.menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Selector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout();
            this.mainTab.ResumeLayout(false);
            this.TabSelect.ResumeLayout(false);
            this.statusStrip_.ResumeLayout(false);
            this.statusStrip_.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem basicToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker startWorker;
        private System.ComponentModel.BackgroundWorker selectWorker;
        private System.ComponentModel.BackgroundWorker endWorker;
        private System.Windows.Forms.ToolStripMenuItem regressToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker regressWorker;
        private System.Windows.Forms.ToolStripMenuItem regressLisToolStripMenuItem;
        private System.Windows.Forms.Label msgText;
        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.TabControl mainTab;
        private System.Windows.Forms.TabPage TabSelect;
        private System.Windows.Forms.ListView selectListView_;
        private System.Windows.Forms.StatusStrip statusStrip_;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar_;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1_;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2_;
    }
}

