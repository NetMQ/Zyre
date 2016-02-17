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
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.groupBoxJoinLeave = new System.Windows.Forms.GroupBox();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnJoin = new System.Windows.Forms.Button();
            this.btnLeave = new System.Windows.Forms.Button();
            this.groupBoxNotes = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBoxShout = new System.Windows.Forms.GroupBox();
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
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.splitContainerLogging = new System.Windows.Forms.SplitContainer();
            this.rtbEventsLog = new System.Windows.Forms.RichTextBox();
            this.rtbNodeLog = new System.Windows.Forms.RichTextBox();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ownGroupBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.peerGroupBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.senderNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addressDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.senderUuidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.peerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBoxEventsLog = new System.Windows.Forms.GroupBox();
            this.groupBoxNodeLog = new System.Windows.Forms.GroupBox();
            this.splitContainerTop = new System.Windows.Forms.SplitContainer();
            this.groupBoxMessages = new System.Windows.Forms.GroupBox();
            this.rtbMessages = new System.Windows.Forms.RichTextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLogging)).BeginInit();
            this.splitContainerLogging.Panel1.SuspendLayout();
            this.splitContainerLogging.Panel2.SuspendLayout();
            this.splitContainerLogging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ownGroupBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerGroupBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingSource)).BeginInit();
            this.groupBoxEventsLog.SuspendLayout();
            this.groupBoxNodeLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTop)).BeginInit();
            this.splitContainerTop.Panel1.SuspendLayout();
            this.splitContainerTop.Panel2.SuspendLayout();
            this.splitContainerTop.SuspendLayout();
            this.groupBoxMessages.SuspendLayout();
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
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerTop);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerLogging);
            this.splitContainerMain.Size = new System.Drawing.Size(1136, 662);
            this.splitContainerMain.SplitterDistance = 499;
            this.splitContainerMain.TabIndex = 0;
            // 
            // groupBoxJoinLeave
            // 
            this.groupBoxJoinLeave.Controls.Add(this.txtGroupName);
            this.groupBoxJoinLeave.Controls.Add(this.label1);
            this.groupBoxJoinLeave.Controls.Add(this.btnJoin);
            this.groupBoxJoinLeave.Controls.Add(this.btnLeave);
            this.groupBoxJoinLeave.Location = new System.Drawing.Point(534, 201);
            this.groupBoxJoinLeave.Name = "groupBoxJoinLeave";
            this.groupBoxJoinLeave.Size = new System.Drawing.Size(244, 79);
            this.groupBoxJoinLeave.TabIndex = 17;
            this.groupBoxJoinLeave.TabStop = false;
            this.groupBoxJoinLeave.Text = "Join or Leave the selected Peer Group";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Group Name:";
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
            this.groupBoxNotes.Location = new System.Drawing.Point(946, 15);
            this.groupBoxNotes.Name = "groupBoxNotes";
            this.groupBoxNotes.Size = new System.Drawing.Size(158, 265);
            this.groupBoxNotes.TabIndex = 16;
            this.groupBoxNotes.TabStop = false;
            this.groupBoxNotes.Text = "Notes";
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(3, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 62);
            this.label4.TabIndex = 14;
            this.label4.Text = "The Peer Groups list includes every peer group ever seen by this instance. The li" +
    "st can grow but it can never shrink. ";
            // 
            // groupBoxShout
            // 
            this.groupBoxShout.Controls.Add(this.btnShout);
            this.groupBoxShout.Controls.Add(this.txtShoutMessage);
            this.groupBoxShout.Controls.Add(this.label3);
            this.groupBoxShout.Location = new System.Drawing.Point(618, 286);
            this.groupBoxShout.Name = "groupBoxShout";
            this.groupBoxShout.Size = new System.Drawing.Size(525, 83);
            this.groupBoxShout.TabIndex = 14;
            this.groupBoxShout.TabStop = false;
            this.groupBoxShout.Text = "Shout to the selected Peer Group";
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
            this.groupBoxWhisper.Location = new System.Drawing.Point(96, 286);
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
            this.groupBoxGroups.Location = new System.Drawing.Point(534, 15);
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
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
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
            this.peerGroupDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.peerGroupDataGridView.Size = new System.Drawing.Size(105, 130);
            this.peerGroupDataGridView.TabIndex = 0;
            // 
            // groupBoxConnectedPeers
            // 
            this.groupBoxConnectedPeers.Controls.Add(this.peerDataGridView);
            this.groupBoxConnectedPeers.Location = new System.Drawing.Point(93, 12);
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
            this.peerDataGridView.Location = new System.Drawing.Point(3, 16);
            this.peerDataGridView.MultiSelect = false;
            this.peerDataGridView.Name = "peerDataGridView";
            this.peerDataGridView.ReadOnly = true;
            this.peerDataGridView.RowHeadersVisible = false;
            this.peerDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.peerDataGridView.Size = new System.Drawing.Size(429, 249);
            this.peerDataGridView.TabIndex = 0;
            this.peerDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.peerDataGridView_DataError);
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
            this.splitContainerLogging.Panel1.Controls.Add(this.groupBoxEventsLog);
            // 
            // splitContainerLogging.Panel2
            // 
            this.splitContainerLogging.Panel2.Controls.Add(this.groupBoxNodeLog);
            this.splitContainerLogging.Size = new System.Drawing.Size(1136, 159);
            this.splitContainerLogging.SplitterDistance = 73;
            this.splitContainerLogging.TabIndex = 0;
            // 
            // rtbEventsLog
            // 
            this.rtbEventsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEventsLog.Location = new System.Drawing.Point(3, 16);
            this.rtbEventsLog.Name = "rtbEventsLog";
            this.rtbEventsLog.Size = new System.Drawing.Size(1130, 54);
            this.rtbEventsLog.TabIndex = 0;
            this.rtbEventsLog.Text = "";
            // 
            // rtbNodeLog
            // 
            this.rtbNodeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbNodeLog.Location = new System.Drawing.Point(3, 16);
            this.rtbNodeLog.Name = "rtbNodeLog";
            this.rtbNodeLog.Size = new System.Drawing.Size(1130, 63);
            this.rtbNodeLog.TabIndex = 0;
            this.rtbNodeLog.Text = "";
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
            // groupBoxEventsLog
            // 
            this.groupBoxEventsLog.Controls.Add(this.rtbEventsLog);
            this.groupBoxEventsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxEventsLog.Location = new System.Drawing.Point(0, 0);
            this.groupBoxEventsLog.Name = "groupBoxEventsLog";
            this.groupBoxEventsLog.Size = new System.Drawing.Size(1136, 73);
            this.groupBoxEventsLog.TabIndex = 0;
            this.groupBoxEventsLog.TabStop = false;
            this.groupBoxEventsLog.Text = "Events Log";
            // 
            // groupBoxNodeLog
            // 
            this.groupBoxNodeLog.Controls.Add(this.rtbNodeLog);
            this.groupBoxNodeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxNodeLog.Location = new System.Drawing.Point(0, 0);
            this.groupBoxNodeLog.Name = "groupBoxNodeLog";
            this.groupBoxNodeLog.Size = new System.Drawing.Size(1136, 82);
            this.groupBoxNodeLog.TabIndex = 0;
            this.groupBoxNodeLog.TabStop = false;
            this.groupBoxNodeLog.Text = "Node Log";
            // 
            // splitContainerTop
            // 
            this.splitContainerTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerTop.Location = new System.Drawing.Point(0, 0);
            this.splitContainerTop.Name = "splitContainerTop";
            this.splitContainerTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerTop.Panel1
            // 
            this.splitContainerTop.Panel1.Controls.Add(this.btnStart);
            this.splitContainerTop.Panel1.Controls.Add(this.groupBoxNotes);
            this.splitContainerTop.Panel1.Controls.Add(this.groupBoxJoinLeave);
            this.splitContainerTop.Panel1.Controls.Add(this.btnStop);
            this.splitContainerTop.Panel1.Controls.Add(this.groupBoxConnectedPeers);
            this.splitContainerTop.Panel1.Controls.Add(this.groupBoxGroups);
            this.splitContainerTop.Panel1.Controls.Add(this.groupBoxShout);
            this.splitContainerTop.Panel1.Controls.Add(this.groupBoxWhisper);
            // 
            // splitContainerTop.Panel2
            // 
            this.splitContainerTop.Panel2.Controls.Add(this.groupBoxMessages);
            this.splitContainerTop.Size = new System.Drawing.Size(1136, 499);
            this.splitContainerTop.SplitterDistance = 374;
            this.splitContainerTop.TabIndex = 20;
            // 
            // groupBoxMessages
            // 
            this.groupBoxMessages.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxMessages.Controls.Add(this.rtbMessages);
            this.groupBoxMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMessages.Location = new System.Drawing.Point(0, 0);
            this.groupBoxMessages.Name = "groupBoxMessages";
            this.groupBoxMessages.Size = new System.Drawing.Size(1136, 121);
            this.groupBoxMessages.TabIndex = 0;
            this.groupBoxMessages.TabStop = false;
            this.groupBoxMessages.Text = "Messages";
            // 
            // rtbMessages
            // 
            this.rtbMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMessages.Location = new System.Drawing.Point(3, 16);
            this.rtbMessages.Name = "rtbMessages";
            this.rtbMessages.Size = new System.Drawing.Size(1130, 102);
            this.rtbMessages.TabIndex = 0;
            this.rtbMessages.Text = "";
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
            ((System.ComponentModel.ISupportInitialize)(this.peerDataGridView)).EndInit();
            this.splitContainerLogging.Panel1.ResumeLayout(false);
            this.splitContainerLogging.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLogging)).EndInit();
            this.splitContainerLogging.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ownGroupBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerGroupBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.peerBindingSource)).EndInit();
            this.groupBoxEventsLog.ResumeLayout(false);
            this.groupBoxNodeLog.ResumeLayout(false);
            this.splitContainerTop.Panel1.ResumeLayout(false);
            this.splitContainerTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTop)).EndInit();
            this.splitContainerTop.ResumeLayout(false);
            this.groupBoxMessages.ResumeLayout(false);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.SplitContainer splitContainerTop;
        private System.Windows.Forms.GroupBox groupBoxMessages;
        private System.Windows.Forms.RichTextBox rtbMessages;
        private System.Windows.Forms.GroupBox groupBoxEventsLog;
        private System.Windows.Forms.GroupBox groupBoxNodeLog;
    }
}

