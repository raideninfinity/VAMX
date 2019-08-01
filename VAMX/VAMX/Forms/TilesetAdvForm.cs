using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VAMX
{
    public partial class TilesetAdvForm : Form
    {
        public TilesetAdvForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        bool _success = false;
        int _index = 0;

        public bool success { get { return _success; } }
        public int index { get { return _index; } }

        private void button1_Click(object sender, EventArgs e)
        {
            _success = true;
            _index = comboBox1.SelectedIndex;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
