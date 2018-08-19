﻿namespace SelectorForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainTab = new System.Windows.Forms.TabControl();
            this.TabSelect = new System.Windows.Forms.TabPage();
            this.selectGrid_ = new System.Windows.Forms.DataGridView();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.basicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newRegressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.msgText = new System.Windows.Forms.Label();
            this.startWorker = new System.ComponentModel.BackgroundWorker();
            this.selectWorker = new System.ComponentModel.BackgroundWorker();
            this.tradedayText = new System.Windows.Forms.Label();
            this.endWorker = new System.ComponentModel.BackgroundWorker();
            this.regressWorker = new System.ComponentModel.BackgroundWorker();
            this.mainTab.SuspendLayout();
            this.TabSelect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectGrid_)).BeginInit();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(this.TabSelect);
            this.mainTab.Location = new System.Drawing.Point(0, 25);
            this.mainTab.Name = "mainTab";
            this.mainTab.SelectedIndex = 0;
            this.mainTab.Size = new System.Drawing.Size(1031, 607);
            this.mainTab.TabIndex = 0;
            // 
            // TabSelect
            // 
            this.TabSelect.Controls.Add(this.selectGrid_);
            this.TabSelect.Location = new System.Drawing.Point(4, 22);
            this.TabSelect.Name = "TabSelect";
            this.TabSelect.Padding = new System.Windows.Forms.Padding(3);
            this.TabSelect.Size = new System.Drawing.Size(1023, 581);
            this.TabSelect.TabIndex = 0;
            this.TabSelect.Text = "SelectList";
            this.TabSelect.UseVisualStyleBackColor = true;
            // 
            // selectGrid_
            // 
            this.selectGrid_.AllowUserToAddRows = false;
            this.selectGrid_.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.selectGrid_.Location = new System.Drawing.Point(-3, 0);
            this.selectGrid_.Name = "selectGrid_";
            this.selectGrid_.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.selectGrid_.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.selectGrid_.RowHeadersWidth = 50;
            this.selectGrid_.RowTemplate.Height = 23;
            this.selectGrid_.Size = new System.Drawing.Size(1023, 585);
            this.selectGrid_.TabIndex = 0;
            this.selectGrid_.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.selectGrid__CellValueNeeded);
            this.selectGrid_.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.selectGrid__RowsAdded);
            this.selectGrid_.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.selectGrid__RowsRemoved);
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.basicToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.newRegressToolStripMenuItem,
            this.regressToolStripMenuItem});
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
            // newRegressToolStripMenuItem
            // 
            this.newRegressToolStripMenuItem.Name = "newRegressToolStripMenuItem";
            this.newRegressToolStripMenuItem.Size = new System.Drawing.Size(93, 21);
            this.newRegressToolStripMenuItem.Text = "NewRegress";
            this.newRegressToolStripMenuItem.Click += new System.EventHandler(this.newRegressToolStripMenuItem_Click);
            // 
            // regressToolStripMenuItem
            // 
            this.regressToolStripMenuItem.Name = "regressToolStripMenuItem";
            this.regressToolStripMenuItem.Size = new System.Drawing.Size(67, 21);
            this.regressToolStripMenuItem.Text = "Regress";
            this.regressToolStripMenuItem.Click += new System.EventHandler(this.regressToolStripMenuItem_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(1, 660);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1027, 23);
            this.progressBar.TabIndex = 2;
            // 
            // msgText
            // 
            this.msgText.AutoSize = true;
            this.msgText.Location = new System.Drawing.Point(1, 639);
            this.msgText.Name = "msgText";
            this.msgText.Size = new System.Drawing.Size(29, 12);
            this.msgText.TabIndex = 3;
            this.msgText.Text = "msg:";
            // 
            // startWorker
            // 
            this.startWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.startWorker_DoWork);
            // 
            // selectWorker
            // 
            this.selectWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.selectWorker_DoWork);
            this.selectWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.selectWorker_RunWorkerCompleted);
            // 
            // tradedayText
            // 
            this.tradedayText.AutoSize = true;
            this.tradedayText.Location = new System.Drawing.Point(949, 639);
            this.tradedayText.Name = "tradedayText";
            this.tradedayText.Size = new System.Drawing.Size(53, 12);
            this.tradedayText.TabIndex = 3;
            this.tradedayText.Text = "tradeday";
            this.tradedayText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 686);
            this.Controls.Add(this.tradedayText);
            this.Controls.Add(this.msgText);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.mainTab);
            this.Controls.Add(this.menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu;
            this.Name = "MainForm";
            this.Text = "Selector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.mainTab.ResumeLayout(false);
            this.TabSelect.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.selectGrid_)).EndInit();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl mainTab;
        private System.Windows.Forms.TabPage TabSelect;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newRegressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem basicToolStripMenuItem;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label msgText;
        private System.ComponentModel.BackgroundWorker startWorker;
        private System.Windows.Forms.DataGridView selectGrid_;
        private System.ComponentModel.BackgroundWorker selectWorker;
        private System.Windows.Forms.Label tradedayText;
        private System.ComponentModel.BackgroundWorker endWorker;
        private System.Windows.Forms.ToolStripMenuItem regressToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker regressWorker;
    }
}

