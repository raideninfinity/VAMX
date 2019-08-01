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
    public partial class MapShiftForm : Form
    {
        public MapShiftForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            numericUpDown1.Maximum = Core.Map.Width - 1;
        }

        bool _success = false;
        public bool success { get { return _success; } }

        int _type;
        public int type { get { return _type; } }
        int _amount;
        public int amount { get { return _amount; } }

        private void button1_Click(object sender, EventArgs e)
        {
            _success = true;
            _type = comboBox1.SelectedIndex;
            _amount = (int)numericUpDown1.Value;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 2)
            {
                numericUpDown1.Maximum = Core.Map.Width - 1;
            }
            else
            {
                numericUpDown1.Maximum = Core.Map.Height - 1;
            }
        }
    }
}
