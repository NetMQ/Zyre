using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamplePeer
{
    public partial class MainForm : Form
    {
        private readonly string _name;

        public MainForm(string name)
        {
            _name = name;
            InitializeComponent();
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

        }



    }
}
