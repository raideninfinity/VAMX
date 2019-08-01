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
    public partial class MainForm : Form
    {
        public MainForm(LoadForm lf)
        {
            InitializeComponent();
            Core.InitEditor(this);
            Core.Editor.PerformInitialize();
            load_form = lf;
        }

        private LoadForm load_form;
        public LoadForm LoadForm { get { return load_form; } }

        bool closing;
        public void FormClose() { closing = true; }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Core.Editor.InitializeDrawMode();
            Core.Editor.PerformLoad();
            tscLVOSortMode.SelectedIndex = 0;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closing)
            {
                Core.Editor.PerformClose();
                e.Cancel = true;
            }
        }

        #region Main Toolstrip

        private void tsbSave_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformSave();
        }

        private void tsbExit_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformClose();
        }

        private void tsbCut_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformCut();
        }

        private void tsbCopy_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformCopy();
        }

        private void tsbPaste_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformPaste();
        }

        private void tsbUndo_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformUndo();
        }

        private void tsbRedo_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformRedo();
        }

        private void tsbZoomIn_Click(object sender, EventArgs e)
        {
            if (Core.Zoom == 1.0f) { return; }
            else if (Core.Zoom == 1.0f / 2.0f) { Core.SetZoom(1.0f); }
            else if (Core.Zoom == 1.0f / 4.0f) { Core.SetZoom(1.0f / 2.0f); }
            tLabelZoom.Text = "显示大小: " + Convert.ToInt32(Core.Zoom * 100).ToString() +
                "% (" + Convert.ToInt32(32 * Core.Zoom).ToString() + "像素) ";
            Core.Editor.RefreshMap();
        }

        private void tsbZoomOut_Click(object sender, EventArgs e)
        {
            if (Core.Zoom == 1.0f / 4.0f) { return; }
            else if (Core.Zoom == 1.0f) { Core.SetZoom(1.0f / 2.0f); }
            else if (Core.Zoom == 1.0f / 2.0f) { Core.SetZoom(1.0f / 4.0f); }
            tLabelZoom.Text = "显示大小: " + Convert.ToInt32(Core.Zoom * 100).ToString() +
                "% (" + Convert.ToInt32(32 * Core.Zoom).ToString() + "像素) ";
            Core.Editor.RefreshMap();
        }

        private void tsbGrid_Click(object sender, EventArgs e)
        {
            Core.Editor.RefreshMap();
        }

        private void tsbIndic_Click(object sender, EventArgs e)
        {
            Core.Editor.RefreshMap();
        }

        private void tsbSettings_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformSettings();
        }

        private void tsbMapShift_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformMapShift();
        }

        private void tsbOutput_Click(object sender, EventArgs e)
        {
            Core.Editor.PerformOutput();
        }

        #endregion

        #region Right Toolstrip

        private void tsbLA_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(9);
        }

        private void tsbL6_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(5);
        }

        private void tsbL5_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(4);
        }

        private void tsbL4_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(3);
        }

        private void tsbL3_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(2);
        }

        private void tsbL2_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(1);
        }

        private void tsbL1_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(0);
        }

        private void tsbPass_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(6);
        }

        private void tsbFlags_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(7);
        }

        private void tsbRegions_Click(object sender, EventArgs e)
        {
            Core.Editor.SetTTab(8);
        }

        #endregion

        #region Left Toolstrip

        private void tsbH6_Click(object sender, EventArgs e)
        {
            Core.Editor.RefreshMap();
        }

        private void tsbH5_Click(object sender, EventArgs e)
        {
            Core.Editor.RefreshMap();
        }

        private void tsbH4_Click(object sender, EventArgs e)
        {
            Core.Editor.RefreshMap();
        }

        private void tsbH3_Click(object sender, EventArgs e)
        {
            Core.Editor.RefreshMap();
        }

        private void tsbH2_Click(object sender, EventArgs e)
        {
            Core.Editor.RefreshMap();
        }

        private void tsbH1_Click(object sender, EventArgs e)
        {
            Core.Editor.RefreshMap();
        }

        private void tsbEM1_Click(object sender, EventArgs e)
        {
            Core.Editor.ChangeDrawMode(1);
        }

        private void tsbEM2_Click(object sender, EventArgs e)
        {
            Core.Editor.ChangeDrawMode(2);
        }

        private void tsbEM3_Click(object sender, EventArgs e)
        {
            Core.Editor.ChangeDrawMode(3);
        }

        private void tsbEM4_Click(object sender, EventArgs e)
        {
            Core.Editor.ChangeDrawMode(4);
        }

        #endregion

        #region Map Edit
        private void tabControlT_Selected(object sender, TabControlEventArgs e)
        {
            Core.Editor.SetTileTab(e.TabPage);
        }

        private void panelM1_MouseDown(object sender, MouseEventArgs e)
        {
            Core.Editor.CurrentMode.MapDown(e);
        }

        private void panelM1_MouseLeave(object sender, EventArgs e)
        {
            Core.Editor.CurrentMode.MapLeave();
        }

        private void panelM1_MouseMove(object sender, MouseEventArgs e)
        {
            Core.Editor.CurrentMode.MapMove(e);
        }

        private void panelM1_MouseUp(object sender, MouseEventArgs e)
        {
            Core.Editor.CurrentMode.MapUp(e);
        }

        #endregion

        #region Tileset Edit

        private void pictureBoxT_MouseDown(object sender, MouseEventArgs e)
        {
            Core.Editor.CurrentMode.PTopDown(e);
        }

        private void pictureBoxT_MouseLeave(object sender, EventArgs e)
        {
            Core.Editor.CurrentMode.PTopLeave();
        }

        private void pictureBoxT_MouseMove(object sender, MouseEventArgs e)
        {
            Core.Editor.CurrentMode.PTopMove(e);
        }

        private void pictureBoxT_MouseUp(object sender, MouseEventArgs e)
        {
            Core.Editor.CurrentMode.PTopUp(e);
        }

        private void pictureBoxL_MouseDown(object sender, MouseEventArgs e)
        {
            Core.Editor.CurrentMode.PLowDown(e);
        }

        private void pictureBoxL_MouseLeave(object sender, EventArgs e)
        {
            Core.Editor.CurrentMode.PLowLeave();
        }

        private void pictureBoxL_MouseMove(object sender, MouseEventArgs e)
        {
            Core.Editor.CurrentMode.PLowMove(e);
        }

        private void pictureBoxL_MouseUp(object sender, MouseEventArgs e)
        {
            Core.Editor.CurrentMode.PLowUp(e);
        }

        #endregion

        public bool ShiftDown;
        public bool CtrlDown;
        public bool SpaceDown;

        public List<Keys> keys = new List<Keys>();

        private void MainForm_Leave(object sender, EventArgs e)
        {
            ShiftDown = false;
            CtrlDown = false;
            SpaceDown = false;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) { ShiftDown = false; }
            if (e.KeyCode == Keys.ControlKey) { CtrlDown = false; }
            if (e.KeyCode == Keys.Space) { SpaceDown = false; }
            if (keys.Contains(e.KeyCode)) { keys.Remove(e.KeyCode); }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) { ShiftDown = true; }
            if (e.KeyCode == Keys.ControlKey) { CtrlDown = true; }
            if (e.KeyCode == Keys.Space) { SpaceDown = true; }
            //Prevent repeating
            if (keys.Contains(e.KeyCode)) { return; }
            keys.Add(e.KeyCode);
            //Shortcut Keys
            Core.Editor.PerformShortcut(e.KeyCode);
        }

        private void tabControlT_SelectedIndexChanged(object sender, EventArgs e)
        {
            Core.Editor.SetTabModeU(tabControlT.SelectedIndex);
        }

        private void listViewO_SelectedIndexChanged(object sender, EventArgs e)
        {
            Core.Editor.LVOSelChange();
        }

        private void tsbLVORefresh_Click(object sender, EventArgs e)
        {
            if (Core.Editor.DrawMode != 4){ return; }
            var a = (EditModeUnl)Core.Editor.CurrentMode;
            a.RedrawList();
        }

        private void tscLVOSortMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            tsbLVORefresh.PerformClick();
        }

        private void listViewO_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Core.Editor.DrawMode != 4) { return; }
            Console.WriteLine("xx");
            var a = (EditModeUnl)Core.Editor.CurrentMode;
            a.LVODoubleClick();

        }

        private void tscU1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = tscU1.SelectedIndex;
            switch (i)
            {
                case 0: if (tsbSH1.Checked) { tsbSH1.Checked = false; } if (!tsbS1.Checked) { tsbS1.Checked = true; } break;
                case 1: if (tsbSH2.Checked) { tsbSH2.Checked = false; } { tsbS2.Checked = true; } break;
                case 2: if (tsbSH3.Checked) { tsbSH3.Checked = false; } { tsbS3.Checked = true; } break;
                case 3: if (tsbSH4.Checked) { tsbSH4.Checked = false; } { tsbS4.Checked = true; } break;
                case 4: if (tsbSH5.Checked) { tsbSH5.Checked = false; } { tsbS5.Checked = true; } break;
                case 5: if (tsbSH6.Checked) { tsbSH6.Checked = false; } { tsbS6.Checked = true; } break;
            }
            HideSButtons(i);
        }

        private void HideSButtons(int i)
        {
            tsbS1.Enabled = true;
            tsbS2.Enabled = true;
            tsbS3.Enabled = true;
            tsbS4.Enabled = true;
            tsbS5.Enabled = true;
            tsbS6.Enabled = true;
            tsbSH1.Enabled = true;
            tsbSH2.Enabled = true;
            tsbSH3.Enabled = true;
            tsbSH4.Enabled = true;
            tsbSH5.Enabled = true;
            tsbSH6.Enabled = true;
            switch (i)
            {
                case 0: tsbSH1.Enabled = false; tsbS1.Enabled = false; break;
                case 1: tsbSH2.Enabled = false; tsbS2.Enabled = false; break;
                case 2: tsbSH3.Enabled = false; tsbS3.Enabled = false; break;
                case 3: tsbSH4.Enabled = false; tsbS4.Enabled = false; break;
                case 4: tsbSH5.Enabled = false; tsbS5.Enabled = false; break;
                case 5: tsbSH6.Enabled = false; tsbS6.Enabled = false; break;
            }
        }

        private void tsbSH1_Click(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbSH2_Click(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbSH3_Click(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbSH4_Click(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbSH5_Click(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbSH6_Click(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbLVOLayer_Click(object sender, EventArgs e)
        {
            tsbLVORefresh.PerformClick();
        }

        private void tsbLVOSLayer_Click(object sender, EventArgs e)
        {
            tsbLVORefresh.PerformClick();
        }

        private void tsbS1_CheckedChanged(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbS2_CheckedChanged(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbS3_CheckedChanged(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbS4_CheckedChanged(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbS5_CheckedChanged(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbS6_CheckedChanged(object sender, EventArgs e)
        {
            Core.Editor.RedrawLVOX();
            Core.Editor.RefreshMap();
        }

        private void tsbS3_Click(object sender, EventArgs e)
        {

        }
    }
}
