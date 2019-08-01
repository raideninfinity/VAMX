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
    public partial class OutputForm : Form
    {
        public OutputForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex <= 1)
            {
                checkBox1.Enabled = true;
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                checkBox5.Enabled = true;
                checkBox6.Enabled = true;
            }
            else
            {
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                checkBox6.Enabled = false;
            }
        }

        private bool _success = false;
        public bool success { get { return _success; } }

        int _mode = 0;
        public int mode { get { return _mode; } }

        int _frame = 0;
        public int frame { get { return _frame; } }

        bool[] _out_layer = { false, false, false, false, false, false };
        public bool[] out_layer { get { return _out_layer; } }

        bool[] _out_type = { false, false };
        public bool[] out_type { get { return _out_type; } }

        double _zoom = 1.0;
        public double zoom { get { return _zoom; } }

        private bool CheckLayer()
        {
            return (!(!checkBox1.Checked && !checkBox2.Checked && !checkBox3.Checked && !checkBox4.Checked
                && !checkBox5.Checked && !checkBox6.Checked));
        }

        //Output
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex <= 1)
            {
                if (!CheckLayer()) { MessageBox.Show("请选择至少一个要输出的图层!"); return; }
            }
            if (!(checkBox7.Checked || checkBox8.Checked)) { MessageBox.Show("请选择至少一个绘制类型!"); return; }
            _mode = comboBox1.SelectedIndex;
            _frame = comboBox3.SelectedIndex;
            _out_layer[0] = checkBox1.Checked;
            _out_layer[1] = checkBox2.Checked;
            _out_layer[2] = checkBox3.Checked;
            _out_layer[3] = checkBox4.Checked;
            _out_layer[4] = checkBox5.Checked;
            _out_layer[5] = checkBox6.Checked;
            switch (comboBox2.SelectedIndex)
            {
                case 0: _zoom = 1.0; break;
                case 1: _zoom = 1.0 / 2.0; break;
                case 2: _zoom = 1.0 / 4.0; break;
            }
            _out_type[0] = checkBox7.Checked;
            _out_type[1] = checkBox8.Checked;
            _success = true;
            this.Close();
        }


    }
}
