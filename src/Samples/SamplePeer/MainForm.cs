using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ.Zyre;
using NetMQ.Zyre.ZyreEvents;

namespace SamplePeer
{
    public partial class MainForm : Form
    {
        private readonly string _name;
        private readonly Zyre _zyre;
        private readonly Dictionary<Guid, Peer> _connectedPeers;
        private readonly Guid _uuid;
        private string _endpoint;

        public MainForm(string name)
        {
            InitializeComponent();
            btnStop.Enabled = false;
            _connectedPeers = new Dictionary<Guid, Peer>();
            _zyre = new Zyre(_name, NodeLogger);
            if (!string.IsNullOrEmpty(name))
            {
                _zyre.SetName(name);
            }
            _name = _zyre.Name();
            _uuid = _zyre.Uuid();
            _endpoint = _zyre.EndPoint(); // every time we start, we bind our RouterSocket to a new port
            _zyre.EnterEvent += ZyreEnterEvent;
            _zyre.StopEvent += ZyreStopEvent;
            _zyre.ExitEvent += ZyreExitEvent;
            _zyre.EvasiveEvent += ZyreEvasiveEvent;
            _zyre.JoinEvent += ZyreJoinEvent;
            _zyre.LeaveEvent += ZyreLeaveEvent;
            _zyre.WhisperEvent += ZyreWhisperEvent;
            _zyre.ShoutEvent += ZyreShoutEvent;
        }

        private void ZyreShoutEvent(object sender, ZyreEventShout e)
        {
            EventsLogger($"Shout: {e.SenderName} {e.SenderUuid.ToShortString6()} with {e.Content.FrameCount} message frames shouted to group:{e.GroupName}");
        }

        private void ZyreWhisperEvent(object sender, ZyreEventWhisper e)
        {
            EventsLogger($"Whisper: {e.SenderName} {e.SenderUuid.ToShortString6()} with {e.Content.FrameCount} message frames");
        }

        private void ZyreJoinEvent(object sender, ZyreEventJoin e)
        {
            EventsLogger($"Join: {e.SenderName} {e.SenderUuid.ToShortString6()} Group:{e.GroupName}");
        }

        private void ZyreLeaveEvent(object sender, ZyreEventLeave e)
        {
            EventsLogger($"Leave: {e.SenderName} {e.SenderUuid.ToShortString6()} Group:{e.GroupName}");
        }

        private void ZyreEvasiveEvent(object sender, ZyreEventEvasive e)
        {
            EventsLogger($"Evasive: {e.SenderName} {e.SenderUuid.ToShortString6()}");
        }

        private void ZyreExitEvent(object sender, ZyreEventExit e)
        {
            _connectedPeers.Remove(e.SenderUuid);
            PeerBindingSourceResetBindings();
            EventsLogger($"Exited: {e.SenderName} {e.SenderUuid.ToShortString6()}");
        }

        private void ZyreStopEvent(object sender, ZyreEventStop e)
        {
            _connectedPeers.Remove(e.SenderUuid);
            PeerBindingSourceResetBindings();
            EventsLogger($"Stopped: {e.SenderName} {e.SenderUuid.ToShortString6()}");
        }

        private void ZyreEnterEvent(object sender, ZyreEventEnter e)
        {
            var peer = new Peer(e.SenderName, e.SenderUuid);
            _connectedPeers.Add(e.SenderUuid, peer);
            PeerBindingSourceResetBindings();
            EventsLogger($"Entered: {e.SenderName} {e.SenderUuid.ToShortString6()} at {e.Address} with {e.Headers.Count} headers");
            if (e.Headers.Count > 0)
            {
                var sb = new StringBuilder($"Headers: " + Environment.NewLine);
                foreach (var header in e.Headers)
                {
                    sb.AppendLine(header.Key + "|" + header.Value);
                }
                EventsLogger(sb.ToString());
            }
        }

        private void PeerBindingSourceResetBindings()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(PeerBindingSourceResetBindings));
            }
            else
            {
                peerBindingSource.DataSource = _connectedPeers.Values.ToList();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DisplayTitle();
        }

        private void DisplayTitle()
        {
            Text = $"Zyre Node: {_name} {_uuid}";
            if (!string.IsNullOrEmpty(_endpoint))
            {
                Text += $" -- listening at {_endpoint}";
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _zyre.Dispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            _zyre.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            _zyre.Stop();
            _endpoint = null;  // every time we start, we bind our RouterSocket to a new port
            DisplayTitle();
            _connectedPeers.Clear();
            PeerBindingSourceResetBindings();
        }
        
        public void NodeLogger(string str)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var msg = $"Thd:{threadId} {DateTime.Now.ToString("h:mm:ss.fff")} ({_name}) {str}";
            Logger(rtbNodeLog, msg);
        }

        public void EventsLogger(string str)
        {
            var msg = $"{DateTime.Now.ToString("h:mm:ss.fff")} ({_name}) {str}";
            Logger(rtbEventsLog, msg);
        }

        /// <summary>
        /// Put a message at the TOP of the richTextBox
        /// </summary>
        /// <param name="rtb">the richTextBox</param>
        /// <param name="msg">the message</param>
        private void Logger(RichTextBox rtb, string msg)
        {
            if (rtb.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => Logger(rtb, msg)));
            }
            else
            {
                const int longestText = 1000000;
                rtb.Text = msg + Environment.NewLine + rtb.Text;
                if (rtb.TextLength > longestText)
                {
                    rtb.Text = rtb.Text.Substring(0, longestText / 2);
                }
            }
        }

        private void peerDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Utility.HandleDataGridViewError(sender, e);
        }
    }
}
