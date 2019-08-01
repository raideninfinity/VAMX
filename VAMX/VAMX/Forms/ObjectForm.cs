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
    public partial class ObjectForm : Form
    {
        public ObjectForm(UObject obj)
        {
            InitializeComponent();
            _obj = obj;
            SetForm();
        }

        void SetForm()
        {
            this.Text = "元件编辑 (ID: " + obj.ID.ToString() + ")"; 
            comboBox1.SelectedIndex = obj.Layer;
            comboBox2.SelectedIndex = obj.SubLayer;
            numericUpDown1.Value = obj.X;
            numericUpDown2.Value = obj.Y;
            numericUpDown3.Value = obj.Z;
            numericUpDown4.Value = obj.TileIndex;
            numericUpDown5.Value = obj.TileID;
            numericUpDown6.Value = obj.SpanX;
            numericUpDown7.Value = obj.SpanY;
            numericUpDown8.Value = obj.CropTop;
            numericUpDown9.Value = obj.CropBottom;
            numericUpDown10.Value = obj.CropLeft;
            numericUpDown11.Value = obj.CropRight;
        }

        bool _success;
        public bool success { get { return _success; } }

        UObject _obj;
        public UObject obj { get { return _obj; } }

        private void button1_Click(object sender, EventArgs e)
        {
            _obj.Layer = comboBox1.SelectedIndex;
            _obj.SubLayer = comboBox2.SelectedIndex;
            _obj.X = (int)numericUpDown1.Value;
            _obj.Y = (int)numericUpDown2.Value;
            _obj.Z = (int)numericUpDown3.Value;
            _obj.TileIndex = (int)numericUpDown4.Value;
            _obj.TileID = (int)numericUpDown5.Value;
            _obj.SpanX = (int)numericUpDown6.Value;
            _obj.SpanY = (int)numericUpDown7.Value;
            //Process Crop
            _obj.CropBottom = (int)numericUpDown9.Value;
            _obj.CropRight = (int)numericUpDown11.Value;
            int ct = (int)numericUpDown8.Value;
            int cl = (int)numericUpDown10.Value;
            if (ct != _obj.CropTop)
            {
                if (ct > obj.CropTop) { obj.Y += ct - obj.CropTop; }
                else { obj.Y -= obj.CropTop - ct; }
            }
            if (cl != _obj.CropLeft)
            {
                if (cl > obj.CropLeft) { obj.X += cl - obj.CropLeft; }
                else { obj.X -= obj.CropLeft - cl; }
            }
            _obj.CropTop = ct;
            _obj.CropLeft = cl;
            _success = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool _lock;

        //SpanX
        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            _lock = true;
            if (numericUpDown10.Value + numericUpDown11.Value >= numericUpDown6.Value * 32 - 4)
            {
                numericUpDown10.Value = numericUpDown6.Value * 32 - 4 - numericUpDown11.Value;
            }
            if (numericUpDown10.Value + numericUpDown11.Value >= numericUpDown6.Value * 32 - 4)
            {
                numericUpDown11.Value = numericUpDown6.Value * 32 - 4 - numericUpDown10.Value;
            }
            _lock = false;
        }

        //SpanY
        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            _lock = true;
            if (numericUpDown8.Value + numericUpDown9.Value >= numericUpDown7.Value * 32 - 4)
            {
                numericUpDown8.Value = numericUpDown7.Value * 32 - 4 - numericUpDown9.Value;
            }
            if (numericUpDown8.Value + numericUpDown9.Value >= numericUpDown7.Value * 32 - 4)
            {
                numericUpDown9.Value = numericUpDown7.Value * 32 - 4 - numericUpDown8.Value;
            }
            _lock = false;
        }

        //Up
        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            if (_lock) { return; }
            if (numericUpDown8.Value + numericUpDown9.Value >= numericUpDown7.Value * 32 - 4)
            {
                _lock = true;
                numericUpDown8.Value = numericUpDown7.Value * 32 - 4 - numericUpDown9.Value;
                _lock = false;
            }
        }

        //Down
        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            if (_lock) { return; }
            if (numericUpDown8.Value + numericUpDown9.Value >= numericUpDown7.Value * 32 - 4)
            {
                _lock = true;
                numericUpDown9.Value = numericUpDown7.Value * 32 - 4 - numericUpDown8.Value;
                _lock = false;
            }
        }

        //Left
        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            if (_lock) { return; }
            if (numericUpDown10.Value + numericUpDown11.Value >= numericUpDown6.Value * 32 - 4)
            {
                _lock = true;
                numericUpDown10.Value = numericUpDown6.Value * 32 - 4 - numericUpDown11.Value;
                _lock = false;
            }
        }

        //Right
        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            if (_lock) { return; }
            if (numericUpDown10.Value + numericUpDown11.Value >= numericUpDown6.Value * 32 - 4)
            {
                _lock = true;
                numericUpDown11.Value = numericUpDown6.Value * 32 - 4 - numericUpDown10.Value;
                _lock = false;
            }
        }

    }
}
