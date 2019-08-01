using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace VAMX
{
    public partial class Editor
    {
        int edit_mode = -1;
        int prev_edit_mode = -1;
        public int EditMode { get { return edit_mode; } }
        
        public void SetTTab(int i)
        {
            if (i == edit_mode) { return; }
            SetRTButtons(i);
            prev_edit_mode = edit_mode;
            edit_mode = i;
            SetTopTabpages();
            SetXCV();
            if (DrawMode == 4) { ((EditModeUnl)CurrentMode).ChangeDisplayLayer(); }
            if (i >= 6) { ClearDrawMode(); }
            RefreshMap();
        }

        public void SetTabModeU(int i)
        {
            if (DrawMode != 4) { return; }
            if (i == 1)
            {
                var a = (EditModeUnl)CurrentMode;
                a.ResetSelector();
                a.RedrawList();
            }
            else
            {
                DeselectLVO();
            }
        }


        public void RedrawLVOX()
        {
            if (DrawMode != 4) { return; }
            ((EditModeUnl)CurrentMode).RedrawListX();
        }

        public void DeselectLVO()
        {
            foreach (ListViewItem lvi in f.listViewO.SelectedItems)
            {
                lvi.Selected = false;
                lvi.Focused = false;
            }
        }

        public void SetRTButtons(int i)
        {
            if (i != 9) { f.tsbLA.Checked = false; } else { f.tsbLA.Checked = true; }
            if (i != 0) { f.tsbL1.Checked = false; } else { f.tsbL1.Checked = true; }
            if (i != 1) { f.tsbL2.Checked = false; } else { f.tsbL2.Checked = true; }
            if (i != 2) { f.tsbL3.Checked = false; } else { f.tsbL3.Checked = true; }
            if (i != 3) { f.tsbL4.Checked = false; } else { f.tsbL4.Checked = true; }
            if (i != 4) { f.tsbL5.Checked = false; } else { f.tsbL5.Checked = true; }
            if (i != 5) { f.tsbL6.Checked = false; } else { f.tsbL6.Checked = true; }
            if (i != 6) { f.tsbPass.Checked = false; } else { f.tsbPass.Checked = true; }
            if (i != 7) { f.tsbFlags.Checked = false; } else { f.tsbFlags.Checked = true; }
            if (i != 8) { f.tsbRegions.Checked = false; } else { f.tsbRegions.Checked = true; }
        }

        public void SetTopTabpages()
        {
            if (DrawMode == 4)
            {
                SetTileEditTopU();
                return;
            }
            if (edit_mode >= 0 && edit_mode < 6 || edit_mode == 9)
            {
                if (prev_edit_mode >= 0 && prev_edit_mode < 6 || prev_edit_mode == 9) { return; }
                SetTileEditTop();
            }
            else if (edit_mode >= 6 && edit_mode < 9)
            {
                SetFlagEdit(edit_mode - 6);
            }
        }

        public void SetTopTabpagesX()
        {
            if (DrawMode == 4)
            {
                SetTileEditTopU();
                return;
            }
            if (edit_mode >= 0 && edit_mode < 6 || edit_mode == 9)
            {
                SetTileEditTop();
            }
            else if (edit_mode >= 6 && edit_mode < 9)
            {
                SetFlagEdit(edit_mode - 6);
            }
        }

        public void EjectTPanel()
        {
            foreach (TabPage t in f.tabControlT.TabPages)
            {
                if (t.Controls.Contains(f.panelT)) { t.Controls.Clear(); }
            }
        }

        public void SetTileEditTop()
        {
            f.tabControlT.TabPages.Clear();
            TabPage t = new TabPage("A1/2"); t.BackColor = Color.LightGray;
            f.tabControlT.TabPages.Add(t);
            SetTileTab(t);
            t = new TabPage("A3/4"); t.BackColor = Color.LightGray;
            f.tabControlT.TabPages.Add(t);
            t = new TabPage("A5"); t.BackColor = Color.LightGray;
            f.tabControlT.TabPages.Add(t);
            t = new TabPage("B"); t.BackColor = Color.LightGray;
            f.tabControlT.TabPages.Add(t);
            f.tabControlT.SelectTab(0);
        }

        public void SetTileEditTopU()
        {
            if (f.tabControlT.TabCount == 2) { return; } 
            f.tabControlT.TabPages.Clear();
            var t = new TabPage("B"); t.BackColor = Color.LightGray;
            t.Controls.Add(f.panelT);
            f.tabControlT.TabPages.Add(t);
            f.tabControlT.TabPages.Add(f.tabPageO);
            f.pictureBoxT.Image = tileset_sel[3];
            f.pictureBoxT.BackgroundImage = tileset_display[3];
            f.tabControlT.Refresh();
        }

        public void SetFlagEdit(int id)
        {
            string str = "";
            switch (id)
            {
                case 0: str = "通行度"; break;
                case 1: str = "特殊标志"; break;
                case 2: str = "区域"; break;
            }
            f.tabControlT.TabPages.Clear();
            TabPage t = new TabPage(str); t.BackColor = Color.LightGray;
            f.tabControlT.TabPages.Add(t);
            f.tabControlT.SelectTab(0);
            SetTileTab(t);
        }

        public void SetTileTab(TabPage t)
        {
            if (edit_mode >= 10) { return; }
            if (t == null) { return; }
            if (t.Controls.Contains(f.panelT)) { return; }
            t.Controls.Add(f.panelT);
            RedrawTilePanel();
        }

        public void RedrawTilePanel()
        {
            if (DrawMode == 4)
            {
                f.pictureBoxT.Image = tileset_sel[3];
                f.pictureBoxT.BackgroundImage = tileset_display[3];
                return;
            }
            if (edit_mode >= 0 && edit_mode < 6 || edit_mode == 9)
            {
                f.pictureBoxT.Image = tileset_sel[f.tabControlT.SelectedIndex];
                f.pictureBoxT.BackgroundImage = tileset_display[f.tabControlT.SelectedIndex];
            }
            else if (edit_mode >= 6 && edit_mode < 9)
            {
                f.pictureBoxT.Image = tileset_sel[edit_mode - 2];
                f.pictureBoxT.BackgroundImage = tileset_display[edit_mode - 2];
            }
            else if (edit_mode >= 10)
            {
                return;
            }
        }

        public void SetXCV()
        {
            bool xcv = false;
            if (edit_mode == 9) { xcv = false; }
            else if (DrawMode >= 3) { xcv = true; }
            if (xcv)
            {
                f.tsbCut.Enabled = true;
                f.tsbCopy.Enabled = true;
                f.tsbPaste.Enabled = true;
            }
            else
            {
                f.tsbCut.Enabled = false;
                f.tsbCopy.Enabled = false;
                f.tsbPaste.Enabled = false;
            }
        }

        public void SwitchToLayer(int i)
        {
            switch (i)
            {
                case 0: f.tsbL1.PerformClick(); return;
                case 1: f.tsbL2.PerformClick(); return;
                case 2: f.tsbL3.PerformClick(); return;
                case 3: f.tsbL4.PerformClick(); return;
                case 4: f.tsbL5.PerformClick(); return;
                case 5: f.tsbL6.PerformClick(); return;
            }
        }

    }
}
