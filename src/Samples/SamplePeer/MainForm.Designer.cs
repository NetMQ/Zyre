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
            this.groupBoxJoinLeave = new System.Windows.Forms.GroupBox();
            this.btnJoin = new System.Windows.Forms.Button();
            this.btnLeave = new System.Windows.Forms.Button();
            this.groupBoxNotes = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxShout = new System.Windows.Forms.GroupBox();
            this.comboBoxPeerGroupNames = new System.Windows.Forms.ComboBox();
            this.btnShout = new System.Windows.Forms.Button();
            this.txtShoutMessage = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBoxWhisper = new System.Windows.Forms.GroupBox();
            this.txtWhisperMessage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnWhisper = new System.Windows.Forms.Button();
            this.groupBoxGroups = new System.Windows.Forms.GroupBox();
            this.groupBoxOwnGroups = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.groupBoxPeerGroups = new System.Windows.Forms.GroupBox();
            this.peerGroupDataGridView = new System.Windows.Forms.DataGridView();
            this.groupBoxConnectedPeers = new System.Windows.Forms.GroupBox();
            this.peerDataGridView = new System.Windows.Forms.DataGridView();
            this.peerBindingNavigator = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.splitContainerLogging = new System.Windows.Forms.SplitContainer();
            this.rtbEventsLog = new System.Windows.Forms.RichTextBox();
            this.rtbNodeLog = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownGroupBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.peerGroupBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.senderNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.senderUuidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.peerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.lblMessageReceived = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.groupBoxJoinLeave.SuspendLayout();
            this.groupBoxNotes.SuspendLayout();
            this.groupBoxShout.SuspendLayout();
            this.groupBoxWhisper.SuspendLayout();
            this.groupBoxGroups.SuspendLayout();
            this.groupBoxOwnGroups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBoxPeerGroups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.peerGroupDataGridView)).BeginInit();
            this.groupBoxConnectedPeers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.peerDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingNavigator)).BeginInit();
            this.peerBindingNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLogging)).BeginInit();
            this.splitContainerLogging.Panel1.SuspendLayout();
            this.splitContainerLogging.Panel2.SuspendLayout();
            this.splitContainerLogging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ownGroupBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerGroupBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingSource)).BeginInit();
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
            this.splitContainerMain.Panel1.Controls.Add(this.lblMessageReceived);
            this.splitContainerMain.Panel1.Controls.Add(this.label6);
            this.splitContainerMain.Panel1.Controls.Add(this.groupBoxJoinLeave);
            this.splitContainerMain.Panel1.Controls.Add(this.groupBoxNotes);
            this.splitContainerMain.Panel1.Controls.Add(this.groupBoxShout);
            this.splitContainerMain.Panel1.Controls.Add(this.groupBoxWhisper);
            this.splitContainerMain.Panel1.Controls.Add(this.groupBoxGroups);
            this.splitContainerMain.Panel1.Controls.Add(this.groupBoxConnectedPeers);
            this.splitContainerMain.Panel1.Controls.Add(this.btnStop);
            this.splitContainerMain.Panel1.Controls.Add(this.btnStart);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerLogging);
            this.splitContainerMain.Size = new System.Drawing.Size(1136, 662);
            this.splitContainerMain.SplitterDistance = 408;
            this.splitContainerMain.TabIndex = 0;
            // 
            // groupBoxJoinLeave
            // 
            this.groupBoxJoinLeave.Controls.Add(this.txtGroupName);
            this.groupBoxJoinLeave.Controls.Add(this.label1);
            this.groupBoxJoinLeave.Controls.Add(this.btnJoin);
            this.groupBoxJoinLeave.Controls.Add(this.btnLeave);
            this.groupBoxJoinLeave.Location = new System.Drawing.Point(546, 201);
            this.groupBoxJoinLeave.Name = "groupBoxJoinLeave";
            this.groupBoxJoinLeave.Size = new System.Drawing.Size(244, 79);
            this.groupBoxJoinLeave.TabIndex = 17;
            this.groupBoxJoinLeave.TabStop = false;
            this.groupBoxJoinLeave.Text = "Join or Leave the selected Peer Group";
            // 
            // btnJoin
            // 
            this.btnJoin.Location = new System.Drawing.Point(6, 19);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(75, 23);
            this.btnJoin.TabIndex = 6;
            this.btnJoin.Text = "Join";
            this.btnJoin.UseVisualStyleBackColor = true;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
            // 
            // btnLeave
            // 
            this.btnLeave.Location = new System.Drawing.Point(6, 50);
            this.btnLeave.Name = "btnLeave";
            this.btnLeave.Size = new System.Drawing.Size(75, 23);
            this.btnLeave.TabIndex = 9;
            this.btnLeave.Text = "Leave";
            this.btnLeave.UseVisualStyleBackColor = true;
            this.btnLeave.Click += new System.EventHandler(this.btnLeave_Click);
            // 
            // groupBoxNotes
            // 
            this.groupBoxNotes.BackColor = System.Drawing.SystemColors.Info;
            this.groupBoxNotes.Controls.Add(this.label4);
            this.groupBoxNotes.Controls.Add(this.label5);
            this.groupBoxNotes.Location = new System.Drawing.Point(806, 12);
            this.groupBoxNotes.Name = "groupBoxNotes";
            this.groupBoxNotes.Size = new System.Drawing.Size(318, 265);
            this.groupBoxNotes.TabIndex = 16;
            this.groupBoxNotes.TabStop = false;
            this.groupBoxNotes.Text = "Notes";
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(3, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(312, 40);
            this.label4.TabIndex = 14;
            this.label4.Text = "Peer Groups are all groups ever known. The list can grow but it can never shrink." +
    " ";
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Location = new System.Drawing.Point(3, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(312, 40);
            this.label5.TabIndex = 15;
            this.label5.Text = "Start/Stop is currently broken. Once stopped you can never restart.";
            // 
            // groupBoxShout
            // 
            this.groupBoxShout.Controls.Add(this.comboBoxPeerGroupNames);
            this.groupBoxShout.Controls.Add(this.btnShout);
            this.groupBoxShout.Controls.Add(this.txtShoutMessage);
            this.groupBoxShout.Controls.Add(this.label3);
            this.groupBoxShout.Location = new System.Drawing.Point(546, 289);
            this.groupBoxShout.Name = "groupBoxShout";
            this.groupBoxShout.Size = new System.Drawing.Size(525, 83);
            this.groupBoxShout.TabIndex = 14;
            this.groupBoxShout.TabStop = false;
            this.groupBoxShout.Text = "Shout to the selected Peer Group";
            // 
            // comboBoxPeerGroupNames
            // 
            this.comboBoxPeerGroupNames.FormattingEnabled = true;
            this.comboBoxPeerGroupNames.Location = new System.Drawing.Point(6, 18);
            this.comboBoxPeerGroupNames.Name = "comboBoxPeerGroupNames";
            this.comboBoxPeerGroupNames.Size = new System.Drawing.Size(75, 21);
            this.comboBoxPeerGroupNames.TabIndex = 14;
            // 
            // btnShout
            // 
            this.btnShout.Location = new System.Drawing.Point(6, 45);
            this.btnShout.Name = "btnShout";
            this.btnShout.Size = new System.Drawing.Size(75, 23);
            this.btnShout.TabIndex = 14;
            this.btnShout.Text = "Shout";
            this.btnShout.UseVisualStyleBackColor = true;
            this.btnShout.Click += new System.EventHandler(this.btnShout_Click);
            // 
            // txtShoutMessage
            // 
            this.txtShoutMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShoutMessage.Location = new System.Drawing.Point(96, 48);
            this.txtShoutMessage.Name = "txtShoutMessage";
            this.txtShoutMessage.Size = new System.Drawing.Size(417, 20);
            this.txtShoutMessage.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(93, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Shout Message:";
            // 
            // groupBoxWhisper
            // 
            this.groupBoxWhisper.Controls.Add(this.txtWhisperMessage);
            this.groupBoxWhisper.Controls.Add(this.label2);
            this.groupBoxWhisper.Controls.Add(this.btnWhisper);
            this.groupBoxWhisper.Location = new System.Drawing.Point(24, 286);
            this.groupBoxWhisper.Name = "groupBoxWhisper";
            this.groupBoxWhisper.Size = new System.Drawing.Size(516, 83);
            this.groupBoxWhisper.TabIndex = 10;
            this.groupBoxWhisper.TabStop = false;
            this.groupBoxWhisper.Text = "Whisper to the selected Connected Peer";
            // 
            // txtWhisperMessage
            // 
            this.txtWhisperMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWhisperMessage.Location = new System.Drawing.Point(96, 48);
            this.txtWhisperMessage.Name = "txtWhisperMessage";
            this.txtWhisperMessage.Size = new System.Drawing.Size(408, 20);
            this.txtWhisperMessage.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Whisper Message:";
            // 
            // btnWhisper
            // 
            this.btnWhisper.Location = new System.Drawing.Point(6, 46);
            this.btnWhisper.Name = "btnWhisper";
            this.btnWhisper.Size = new System.Drawing.Size(75, 23);
            this.btnWhisper.TabIndex = 10;
            this.btnWhisper.Text = "Whisper";
            this.btnWhisper.UseVisualStyleBackColor = true;
            this.btnWhisper.Click += new System.EventHandler(this.btnWhisper_Click);
            // 
            // groupBoxGroups
            // 
            this.groupBoxGroups.Controls.Add(this.groupBoxOwnGroups);
            this.groupBoxGroups.Controls.Add(this.groupBoxPeerGroups);
            this.groupBoxGroups.Location = new System.Drawing.Point(546, 12);
            this.groupBoxGroups.Name = "groupBoxGroups";
            this.groupBoxGroups.Size = new System.Drawing.Size(244, 183);
            this.groupBoxGroups.TabIndex = 9;
            this.groupBoxGroups.TabStop = false;
            this.groupBoxGroups.Text = "Groups";
            // 
            // groupBoxOwnGroups
            // 
            this.groupBoxOwnGroups.Controls.Add(this.dataGridView1);
            this.groupBoxOwnGroups.Location = new System.Drawing.Point(123, 24);
            this.groupBoxOwnGroups.Name = "groupBoxOwnGroups";
            this.groupBoxOwnGroups.Size = new System.Drawing.Size(111, 149);
            this.groupBoxOwnGroups.TabIndex = 5;
            this.groupBoxOwnGroups.TabStop = false;
            this.groupBoxOwnGroups.Text = "My Own Groups";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2});
            this.dataGridView1.DataSource = this.ownGroupBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 16);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(105, 130);
            this.dataGridView1.TabIndex = 0;
            // 
            // groupBoxPeerGroups
            // 
            this.groupBoxPeerGroups.Controls.Add(this.peerGroupDataGridView);
            this.groupBoxPeerGroups.Location = new System.Drawing.Point(6, 23);
            this.groupBoxPeerGroups.Name = "groupBoxPeerGroups";
            this.groupBoxPeerGroups.Size = new System.Drawing.Size(111, 149);
            this.groupBoxPeerGroups.TabIndex = 4;
            this.groupBoxPeerGroups.TabStop = false;
            this.groupBoxPeerGroups.Text = "Peer Groups";
            // 
            // peerGroupDataGridView
            // 
            this.peerGroupDataGridView.AllowUserToAddRows = false;
            this.peerGroupDataGridView.AllowUserToDeleteRows = false;
            this.peerGroupDataGridView.AutoGenerateColumns = false;
            this.peerGroupDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.peerGroupDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.peerGroupDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            this.peerGroupDataGridView.DataSource = this.peerGroupBindingSource;
            this.peerGroupDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.peerGroupDataGridView.Location = new System.Drawing.Point(3, 16);
            this.peerGroupDataGridView.MultiSelect = false;
            this.peerGroupDataGridView.Name = "peerGroupDataGridView";
            this.peerGroupDataGridView.ReadOnly = true;
            this.peerGroupDataGridView.RowHeadersVisible = false;
            this.peerGroupDataGridView.Size = new System.Drawing.Size(105, 130);
            this.peerGroupDataGridView.TabIndex = 0;
            // 
            // groupBoxConnectedPeers
            // 
            this.groupBoxConnectedPeers.Controls.Add(this.peerDataGridView);
            this.groupBoxConnectedPeers.Controls.Add(this.peerBindingNavigator);
            this.groupBoxConnectedPeers.Location = new System.Drawing.Point(105, 12);
            this.groupBoxConnectedPeers.Name = "groupBoxConnectedPeers";
            this.groupBoxConnectedPeers.Size = new System.Drawing.Size(435, 268);
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
            this.addressDataGridViewTextBoxColumn,
            this.senderUuidDataGridViewTextBoxColumn});
            this.peerDataGridView.DataSource = this.peerBindingSource;
            this.peerDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.peerDataGridView.Location = new System.Drawing.Point(3, 41);
            this.peerDataGridView.MultiSelect = false;
            this.peerDataGridView.Name = "peerDataGridView";
            this.peerDataGridView.ReadOnly = true;
            this.peerDataGridView.RowHeadersVisible = false;
            this.peerDataGridView.Size = new System.Drawing.Size(429, 224);
            this.peerDataGridView.TabIndex = 0;
            this.peerDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.peerDataGridView_DataError);
            // 
            // peerBindingNavigator
            // 
            this.peerBindingNavigator.AddNewItem = null;
            this.peerBindingNavigator.BindingSource = this.peerBindingSource;
            this.peerBindingNavigator.CountItem = this.bindingNavigatorCountItem;
            this.peerBindingNavigator.DeleteItem = null;
            this.peerBindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem});
            this.peerBindingNavigator.Location = new System.Drawing.Point(3, 16);
            this.peerBindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.peerBindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.peerBindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.peerBindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.peerBindingNavigator.Name = "peerBindingNavigator";
            this.peerBindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
            this.peerBindingNavigator.Size = new System.Drawing.Size(429, 25);
            this.peerBindingNavigator.TabIndex = 1;
            this.peerBindingNavigator.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
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
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(24, 45);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(24, 12);
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
            this.splitContainerLogging.Size = new System.Drawing.Size(1136, 250);
            this.splitContainerLogging.SplitterDistance = 115;
            this.splitContainerLogging.TabIndex = 0;
            // 
            // rtbEventsLog
            // 
            this.rtbEventsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEventsLog.Location = new System.Drawing.Point(0, 0);
            this.rtbEventsLog.Name = "rtbEventsLog";
            this.rtbEventsLog.Size = new System.Drawing.Size(1136, 115);
            this.rtbEventsLog.TabIndex = 0;
            this.rtbEventsLog.Text = "";
            // 
            // rtbNodeLog
            // 
            this.rtbNodeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbNodeLog.Location = new System.Drawing.Point(0, 0);
            this.rtbNodeLog.Name = "rtbNodeLog";
            this.rtbNodeLog.Size = new System.Drawing.Size(1136, 131);
            this.rtbNodeLog.TabIndex = 0;
            this.rtbNodeLog.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Group Name:";
            // 
            // txtGroupName
            // 
            this.txtGroupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGroupName.Location = new System.Drawing.Point(90, 52);
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.Size = new System.Drawing.Size(144, 20);
            this.txtGroupName.TabIndex = 13;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "GroupName";
            this.dataGridViewTextBoxColumn2.HeaderText = "GroupName";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // ownGroupBindingSource
            // 
            this.ownGroupBindingSource.DataSource = typeof(SamplePeer.Group);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "GroupName";
            this.dataGridViewTextBoxColumn1.HeaderText = "GroupName";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // peerGroupBindingSource
            // 
            this.peerGroupBindingSource.DataSource = typeof(SamplePeer.Group);
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
            // addressDataGridViewTextBoxColumn
            // 
            this.addressDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.addressDataGridViewTextBoxColumn.DataPropertyName = "Address";
            this.addressDataGridViewTextBoxColumn.HeaderText = "Address";
            this.addressDataGridViewTextBoxColumn.Name = "addressDataGridViewTextBoxColumn";
            this.addressDataGridViewTextBoxColumn.ReadOnly = true;
            this.addressDataGridViewTextBoxColumn.Width = 70;
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 382);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Message Received:";
            // 
            // lblMessageReceived
            // 
            this.lblMessageReceived.AutoSize = true;
            this.lblMessageReceived.Location = new System.Drawing.Point(129, 382);
            this.lblMessageReceived.Name = "lblMessageReceived";
            this.lblMessageReceived.Size = new System.Drawing.Size(106, 13);
            this.lblMessageReceived.TabIndex = 19;
            this.lblMessageReceived.Text = "lblMessageReceived";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 662);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "MainForm";
            this.Text = "Zyre Peer:";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel1.PerformLayout();
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.groupBoxJoinLeave.ResumeLayout(false);
            this.groupBoxJoinLeave.PerformLayout();
            this.groupBoxNotes.ResumeLayout(false);
            this.groupBoxShout.ResumeLayout(false);
            this.groupBoxShout.PerformLayout();
            this.groupBoxWhisper.ResumeLayout(false);
            this.groupBoxWhisper.PerformLayout();
            this.groupBoxGroups.ResumeLayout(false);
            this.groupBoxOwnGroups.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBoxPeerGroups.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.peerGroupDataGridView)).EndInit();
            this.groupBoxConnectedPeers.ResumeLayout(false);
            this.groupBoxConnectedPeers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.peerDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingNavigator)).EndInit();
            this.peerBindingNavigator.ResumeLayout(false);
            this.peerBindingNavigator.PerformLayout();
            this.splitContainerLogging.Panel1.ResumeLayout(false);
            this.splitContainerLogging.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLogging)).EndInit();
            this.splitContainerLogging.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ownGroupBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerGroupBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingSource)).EndInit();
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
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.GroupBox groupBoxPeerGroups;
        private System.Windows.Forms.GroupBox groupBoxGroups;
        private System.Windows.Forms.Button btnLeave;
        private System.Windows.Forms.GroupBox groupBoxWhisper;
        private System.Windows.Forms.TextBox txtWhisperMessage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnWhisper;
        private System.Windows.Forms.GroupBox groupBoxShout;
        private System.Windows.Forms.TextBox txtShoutMessage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnShout;
        private System.Windows.Forms.GroupBox groupBoxNotes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn addressDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn senderUuidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn senderNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.GroupBox groupBoxOwnGroups;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.BindingSource ownGroupBindingSource;
        private System.Windows.Forms.DataGridView peerGroupDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.BindingSource peerGroupBindingSource;
        private System.Windows.Forms.GroupBox groupBoxJoinLeave;
        private System.Windows.Forms.ComboBox comboBoxPeerGroupNames;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.Label lblMessageReceived;
        private System.Windows.Forms.Label label6;
    }
}

