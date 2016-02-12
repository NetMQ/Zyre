using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ.Zyre;

namespace SamplePeer
{
    public partial class MainForm : Form
    {
        private readonly string _name;
        private Zyre _zyre;


        public MainForm(string name)
        {
            _name = name;
            InitializeComponent();
            var logger = new ConsoleLogger(_name);
            _zyre = new Zyre(_name, logger.ConsoleWrite);
            _zyre.EnterEvent += ZyreEnterEvent;
            _zyre.StopEvent += ZyreStopEvent;
            _zyre.ExitEvent += ZyreExitEvent;
            
        }

        private void ZyreExitEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventExit e)
        {
            MessageBox.Show($"Exited: {e.SenderName}", _name);
        }

        private void ZyreStopEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventStop e)
        {
            MessageBox.Show($"Stopped: {e.SenderName}", _name);
        }

        private void ZyreEnterEvent(object sender, NetMQ.Zyre.ZyreEvents.ZyreEventEnter e)
        {
            MessageBox.Show($"Entered: {e.SenderName}", _name);
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
            _zyre.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _zyre.Stop();
        }
    }
}
