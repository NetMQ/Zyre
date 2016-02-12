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
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.splitContainerLogging = new System.Windows.Forms.SplitContainer();
            this.rtbEventsLog = new System.Windows.Forms.RichTextBox();
            this.rtbNodeLog = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
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
            this.splitContainerMain.Panel1.Controls.Add(this.btnStop);
            this.splitContainerMain.Panel1.Controls.Add(this.btnStart);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerLogging);
            this.splitContainerMain.Size = new System.Drawing.Size(778, 578);
            this.splitContainerMain.SplitterDistance = 306;
            this.splitContainerMain.TabIndex = 0;
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
            this.splitContainerLogging.Size = new System.Drawing.Size(778, 268);
            this.splitContainerLogging.SplitterDistance = 123;
            this.splitContainerLogging.TabIndex = 0;
            // 
            // rtbEventsLog
            // 
            this.rtbEventsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEventsLog.Location = new System.Drawing.Point(0, 0);
            this.rtbEventsLog.Name = "rtbEventsLog";
            this.rtbEventsLog.Size = new System.Drawing.Size(778, 123);
            this.rtbEventsLog.TabIndex = 0;
            this.rtbEventsLog.Text = "";
            // 
            // rtbNodeLog
            // 
            this.rtbNodeLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbNodeLog.Location = new System.Drawing.Point(0, 0);
            this.rtbNodeLog.Name = "rtbNodeLog";
            this.rtbNodeLog.Size = new System.Drawing.Size(778, 141);
            this.rtbNodeLog.TabIndex = 0;
            this.rtbNodeLog.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 578);
            this.Controls.Add(this.splitContainerMain);
            this.Name = "MainForm";
            this.Text = "Zyre Peer:";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
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
    }
}

