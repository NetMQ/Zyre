namespace SamplePeer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.groupBoxConnectedPeers = new System.Windows.Forms.GroupBox();
            this.peerDataGridView = new System.Windows.Forms.DataGridView();
            this.senderNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.senderUuidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.peerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.peerBindingNavigator = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.peerBindingNavigatorSaveItem = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.splitContainerLogging = new System.Windows.Forms.SplitContainer();
            this.rtbEventsLog = new System.Windows.Forms.RichTextBox();
            this.rtbNodeLog = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.groupBoxConnectedPeers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.peerDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingNavigator)).BeginInit();
            this.peerBindingNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLogging)).BeginInit();
            this.splitContainerLogging.Panel1.SuspendLayout();
            this.splitContainerLogging.Panel2.SuspendLayout();
            this.splitContainerLogging.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.AutoScroll = true;
            this.splitContainerMain.Panel1.Controls.Add(this.groupBoxConnectedPeers);
            this.splitContainerMain.Panel1.Controls.Add(this.btnStop);
            this.splitContainerMain.Panel1.Controls.Add(this.btnStart);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerLogging);
            this.splitContainerMain.Size = new System.Drawing.Size(1136, 578);
            this.splitContainerMain.SplitterDistance = 226;
            this.splitContainerMain.TabIndex = 0;
            // 
            // groupBoxConnectedPeers
            // 
            this.groupBoxConnectedPeers.Controls.Add(this.peerDataGridView);
            this.groupBoxConnectedPeers.Controls.Add(this.peerBindingNavigator);
            this.groupBoxConnectedPeers.Location = new System.Drawing.Point(105, 12);
            this.groupBoxConnectedPeers.Name = "groupBoxConnectedPeers";
            this.groupBoxConnectedPeers.Size = new System.Drawing.Size(210, 182);
            this.groupBoxConnectedPeers.TabIndex = 2;
            this.groupBoxConnectedPeers.TabStop = false;
            this.groupBoxConnectedPeers.Text = "Connected Peers";
            // 
            // peerDataGridView
            // 
            this.peerDataGridView.AllowUserToAddRows = false;
            this.peerDataGridView.AllowUserToDeleteRows = false;
            this.peerDataGridView.AutoGenerateColumns = false;
            this.peerDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.peerDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.peerDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.peerDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.senderNameDataGridViewTextBoxColumn,
            this.senderUuidDataGridViewTextBoxColumn});
            this.peerDataGridView.DataSource = this.peerBindingSource;
            this.peerDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.peerDataGridView.Location = new System.Drawing.Point(3, 41);
            this.peerDataGridView.Name = "peerDataGridView";
            this.peerDataGridView.ReadOnly = true;
            this.peerDataGridView.RowHeadersVisible = false;
            this.peerDataGridView.Size = new System.Drawing.Size(204, 138);
            this.peerDataGridView.TabIndex = 0;
            this.peerDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.peerDataGridView_DataError);
            // 
            // senderNameDataGridViewTextBoxColumn
            // 
            this.senderNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.senderNameDataGridViewTextBoxColumn.DataPropertyName = "SenderName";
            this.senderNameDataGridViewTextBoxColumn.HeaderText = "SenderName";
            this.senderNameDataGridViewTextBoxColumn.Name = "senderNameDataGridViewTextBoxColumn";
            this.senderNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.senderNameDataGridViewTextBoxColumn.Width = 94;
            // 
            // senderUuidDataGridViewTextBoxColumn
            // 
            this.senderUuidDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.senderUuidDataGridViewTextBoxColumn.DataPropertyName = "SenderUuid";
            this.senderUuidDataGridViewTextBoxColumn.HeaderText = "SenderUuid";
            this.senderUuidDataGridViewTextBoxColumn.Name = "senderUuidDataGridViewTextBoxColumn";
            this.senderUuidDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // peerBindingSource
            // 
            this.peerBindingSource.DataSource = typeof(SamplePeer.Peer);
            // 
            // peerBindingNavigator
            // 
            this.peerBindingNavigator.AddNewItem = this.bindingNavigatorAddNewItem;
            this.peerBindingNavigator.BindingSource = this.peerBindingSource;
            this.peerBindingNavigator.CountItem = this.bindingNavigatorCountItem;
            this.peerBindingNavigator.DeleteItem = this.bindingNavigatorDeleteItem;
            this.peerBindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.peerBindingNavigatorSaveItem});
            this.peerBindingNavigator.Location = new System.Drawing.Point(3, 16);
            this.peerBindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.peerBindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.peerBindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.peerBindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.peerBindingNavigator.Name = "peerBindingNavigator";
            this.peerBindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.peerBindingNavigator.Size = new System.Drawing.Size(204, 25);
            this.peerBindingNavigator.TabIndex = 1;
            this.peerBindingNavigator.Text = "bindingNavigator1";
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 20);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 20);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 20);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // peerBindingNavigatorSaveItem
            // 
            this.peerBindingNavigatorSaveItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.peerBindingNavigatorSaveItem.Enabled = false;
            this.peerBindingNavigatorSaveItem.Image = ((System.Drawing.Image)(resources.GetObject("peerBindingNavigatorSaveItem.Image")));
            this.peerBindingNavigatorSaveItem.Name = "peerBindingNavigatorSaveItem";
            this.peerBindingNavigatorSaveItem.Size = new System.Drawing.Size(23, 20);
            this.peerBindingNavigatorSaveItem.Text = "Save Data";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(12, 41);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // splitContainerLogging
            // 
            this.splitContainerLogging.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLogging.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLogging.Name = "splitContainerLogging";
            this.splitContainerLogging.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerLogging.Panel1
            // 
            this.splitContainerLogging.Panel1.Controls.Add(this.rtbEventsLog);
            // 
            // splitContainerLogging.Panel2
            // 
            this.splitContainerLogging.Panel2.Controls.Add(this.rtbNodeLog);
            this.splitContainerLogging.Size = new System.Drawing.Size(1136, 348);
            this.splitContainerLogging.SplitterDistance = 89;
            this.splitContainerLogging.TabIndex = 0;
            // 
            // rtbEventsLog
            // 
            this.rtbEventsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEventsLog.Location = new System.Drawing.Point(0, 0);
            this.rtbEventsLog.Name = "rtbEventsLog";
            this.rtbEventsLog.Size = new System.Drawing.Size(1136, 89);
            this.rtbEventsLog.TabIndex = 0;
            this.rtbEventsLog.Text = "";
            // 
            // rtbNodeLog
            // 
            this.rtbNodeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbNodeLog.Location = new System.Drawing.Point(0, 0);
            this.rtbNodeLog.Name = "rtbNodeLog";
            this.rtbNodeLog.Size = new System.Drawing.Size(1136, 255);
            this.rtbNodeLog.TabIndex = 0;
            this.rtbNodeLog.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 578);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "MainForm";
            this.Text = "Zyre Peer:";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.groupBoxConnectedPeers.ResumeLayout(false);
            this.groupBoxConnectedPeers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.peerDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingNavigator)).EndInit();
            this.peerBindingNavigator.ResumeLayout(false);
            this.peerBindingNavigator.PerformLayout();
            this.splitContainerLogging.Panel1.ResumeLayout(false);
            this.splitContainerLogging.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLogging)).EndInit();
            this.splitContainerLogging.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.SplitContainer splitContainerLogging;
        private System.Windows.Forms.RichTextBox rtbEventsLog;
        private System.Windows.Forms.RichTextBox rtbNodeLog;
        private System.Windows.Forms.GroupBox groupBoxConnectedPeers;
        private System.Windows.Forms.DataGridView peerDataGridView;
        private System.Windows.Forms.BindingSource peerBindingSource;
        private System.Windows.Forms.BindingNavigator peerBindingNavigator;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton peerBindingNavigatorSaveItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn senderNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn senderUuidDataGridViewTextBoxColumn;
    }
}

