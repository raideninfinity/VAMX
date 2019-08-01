using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VAMX
{
    public class ListViewX : ListView
    {

        public ListViewX()
        {
            DoubleBuffered = true;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (Core.Editor.DrawMode == 4 && Core.Editor.f.tabControlT.SelectedIndex == 1)
            {
                var a = (EditModeUnl)Core.Editor.CurrentMode;
                if (a.moved)
                {
                    a.RedrawListX();
                    a.moved = false;
                }
            }
            bool p = false;
            if (keyData == Keys.Delete)
            {
                Core.Editor.LVOControlKeys(5); p = true;
            }
            else if ((((int)keyData) & (int)Keys.Right) == 38)
            {
                if (keyData == Keys.Up) { Core.Editor.LVOControlKeys(1); p = true; }
                else if (keyData == (Keys.Up | Keys.Shift)) { Core.Editor.LVOControlKeys(6); p = true; }
                else if (keyData == (Keys.Up | Keys.Control)) { Core.Editor.LVOControlKeys(10); p = true; }
            }
            else if ((((int)keyData) & (int)Keys.Up) == 32)
            {
                if (keyData == Keys.Down) { Core.Editor.LVOControlKeys(2); p = true; }
                else if (keyData == (Keys.Down | Keys.Shift)) { Core.Editor.LVOControlKeys(7); p = true; }
                else if (keyData == (Keys.Down | Keys.Control)) { Core.Editor.LVOControlKeys(11); p = true; }
            }
            else if ((((int)keyData) & (int)Keys.Up) == 36)
            {
                if (keyData == Keys.Left) { Core.Editor.LVOControlKeys(3); p = true; }
                else if (keyData == (Keys.Left | Keys.Shift)) { Core.Editor.LVOControlKeys(8); p = true; }
                else if (keyData == (Keys.Left | Keys.Control)) { Core.Editor.LVOControlKeys(12); p = true; }
            }
            else if ((((int)keyData) & (int)Keys.Right) == 39)
            {
                if (keyData == Keys.Right) { Core.Editor.LVOControlKeys(4); p = true; }
                else if (keyData == (Keys.Right | Keys.Shift)) { Core.Editor.LVOControlKeys(9); p = true; }
                else if (keyData == (Keys.Right | Keys.Control)) { Core.Editor.LVOControlKeys(13); p = true; }
            }
            else if (keyData == Keys.Delete)
            {
                Core.Editor.LVOControlKeys(5); p = true;
            }
            if (p)
            {
                Core.Editor.RefreshMap();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
