using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ.Zyre;

namespace SamplePeer
{
    public partial class MainForm : Form
    {
        private readonly string _name;
        private readonly Zyre _zyre;
        
        public MainForm(string name)
        {
            _name = name;
            InitializeComponent();
            btnStop.Enabled = false;
            _zyre = new Zyre(_name, NodeLogger);
            _zyre.EnterEvent += ZyreEnterEvent;
            _zyre.StopEvent += ZyreStopEvent;
            _zyre.ExitEvent += ZyreExitEvent;
            _zyre.EvasiveEvent += ZyreEvasiveEvent;
            _zyre.JoinEvent += ZyreJoinEvent;
            _zyre.LeaveEvent += ZyreLeaveEvent;
            _zyre.WhisperEvent += ZyreWhisperEvent;
            _zyre.ShoutEvent += ZyreShoutEvent;
        }

        private void ZyreShoutEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventShout e)
        {
            EventsLogger($"Shout: {e.SenderName} {e.SenderUuid.ToShortString6()} with {e.Content.FrameCount} message frames shouted to group:{e.GroupName}");
        }

        private void ZyreWhisperEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventWhisper e)
        {
            EventsLogger($"Whisper: {e.SenderName} {e.SenderUuid.ToShortString6()} with {e.Content.FrameCount} message frames");
        }

        private void ZyreJoinEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventJoin e)
        {
            EventsLogger($"Join: {e.SenderName} {e.SenderUuid.ToShortString6()} Group:{e.GroupName}");
        }

        private void ZyreLeaveEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventLeave e)
        {
            EventsLogger($"Leave: {e.SenderName} {e.SenderUuid.ToShortString6()} Group:{e.GroupName}");
        }

        private void ZyreEvasiveEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventEvasive e)
        {
            EventsLogger($"Evasive: {e.SenderName} {e.SenderUuid.ToShortString6()}");
        }

        private void ZyreExitEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventExit e)
        {
            EventsLogger($"Exited: {e.SenderName} {e.SenderUuid.ToShortString6()}");
        }

        private void ZyreStopEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventStop e)
        {
            EventsLogger($"Stopped: {e.SenderName} {e.SenderUuid.ToShortString6()}");
        }

        private void ZyreEnterEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventEnter e)
        {
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_name))
            {
                Text = $"Zyre Peer: {_name}";
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
        }
        
        public void NodeLogger(string str)
        {
            var msg = $"{DateTime.Now.ToString("h:mm:ss.fff")} ({_name}) {str}";
            Logger(rtbNodeLog, msg);
        }

        public void EventsLogger(string str)
        {
            var msg = $"{DateTime.Now.ToString("h:mm:ss.fff")} ({_name}) {str}";
            Logger(rtbEventsLog, msg);
        }

        private void Logger(RichTextBox rtb, string msg)
        {
            if (rtb.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => Logger(rtb, msg)));
            }
            else
            {
                rtb.Text = msg + Environment.NewLine + rtb.Text;
                if (rtb.TextLength > 100000)
                {
                    rtb.Text = rtb.Text.Substring(0, 50000);
                }
            }
        }
    }
}
