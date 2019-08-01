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
    public partial class TilesetForm : Form
    {
        public TilesetForm(int t = 0, string n = "")
        {
            InitializeComponent();
            comboBox1.SelectedIndex = t;
            textBox1.Text = n;
        }

        bool _success = false;
        int _type;
        string _name;
        public bool success { get { return _success; } }
        public int type { get { return _type; } }
        public string name { get { return _name; } }

        private void button1_Click(object sender, EventArgs e)
        {
            _type = comboBox1.SelectedIndex;
            _name = textBox1.Text;
            _success = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
