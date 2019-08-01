using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VAMX
{
    public partial class Editor
    {
        EditMode current_mode;
        public EditMode CurrentMode { get { return current_mode; } }
        public int DrawMode { get { return current_mode.Mode; } }
        public List<EditMode> draw_modes = new List<EditMode>();


        public void InitializeDrawMode()
        {
            draw_modes.Clear();
            draw_modes.Add(new EditModePen());
            draw_modes.Add(new EditModeRec());
            draw_modes.Add(new EditModeSel());
            draw_modes.Add(new EditModeUnl());
            current_mode = draw_modes[0];
        }

        int prev_u1_index = 0;

        public void ClearDrawMode()
        {
            ClearTop();
            ClearLower();
            f.pictureBoxL.Refresh();
            f.pictureBoxT.Refresh();

            CurrentGroupID = 0;
            CurrentID = 0;

            if (DrawMode == 4)
            {
                f.tscU1.Enabled = true;
                f.tsbS1.Enabled = true;
                f.tsbS2.Enabled = true;
                f.tsbS3.Enabled = true;
                f.tsbS4.Enabled = true;
                f.tsbS5.Enabled = true;
                f.tsbS6.Enabled = true;
                f.tscU1.SelectedIndex = prev_u1_index;
            }
            else
            {
                f.tscU1.Enabled = false;
                f.tsbS1.Enabled = false;
                f.tsbS2.Enabled = false;
                f.tsbS3.Enabled = false;
                f.tsbS4.Enabled = false;
                f.tsbS5.Enabled = false;
                f.tsbS6.Enabled = false;
                if (f.tscU1.SelectedIndex != -1)
                {
                    prev_u1_index = f.tscU1.SelectedIndex;
                }
                f.tscU1.SelectedIndex = -1;
            }
            SetModeU();
            CurrentMode.Reset();
        }

        public void SetDMButtons()
        {
            if (DrawMode == 1) { f.tsbEM1.Checked = true; } else { f.tsbEM1.Checked = false; }
            if (DrawMode == 2) { f.tsbEM2.Checked = true; } else { f.tsbEM2.Checked = false; }
            if (DrawMode == 3) { f.tsbEM3.Checked = true; } else { f.tsbEM3.Checked = false; }
            if (DrawMode == 4) { f.tsbEM4.Checked = true; } else { f.tsbEM4.Checked = false; }
        }

        public void SetModeU()
        {
            if (DrawMode == 4)
            {
                if (EditMode >= 6) { f.tsbLA.PerformClick(); }
                f.tsbZoomIn.Enabled = false;
                f.tsbZoomOut.Enabled = false;
                Core.SetZoom(1.0f);
                f.tLabelZoom.Text = "显示大小: " + Convert.ToInt32(Core.Zoom * 100).ToString() +
                    "% (" + Convert.ToInt32(32 * Core.Zoom).ToString() + "像素) ";
                f.tsbPass.Enabled = false;
                f.tsbRegions.Enabled = false;
                f.tsbFlags.Enabled = false;
                SetTopTabpagesX();
            }
            else
            {
                f.tsbZoomIn.Enabled = true;
                f.tsbZoomOut.Enabled = true;
                f.tsbPass.Enabled = true;
                f.tsbRegions.Enabled = true;
                f.tsbFlags.Enabled = true;
                SetTopTabpagesX();
            }
        }

        public void ChangeDrawMode(int dm)
        {
            if (dm == DrawMode) { return; }
            current_mode = draw_modes[dm - 1];
            SetDMButtons();
            SetXCV();
            ClearDrawMode();
        }

        //XCV
        public void PerformCut()
        {
            if (DrawMode == 3)
            {
                var ed = (EditModeSel)CurrentMode;
                ed.PerformCut();
            }
            else if (DrawMode == 4)
            {
                var ed = (EditModeUnl)CurrentMode;
                ed.PerformCut();
            }
        }

        public void PerformCopy()
        {
            if (DrawMode == 3)
            {
                var ed = (EditModeSel)CurrentMode;
                ed.PerformCopy();
            }
            else if (DrawMode == 4)
            {
                var ed = (EditModeUnl)CurrentMode;
                ed.PerformCopy();
            }
        }

        public void PerformPaste()
        {
            if (DrawMode == 3)
            {
                var ed = (EditModeSel)CurrentMode;
                ed.PerformPaste();
            }
            else if (DrawMode == 4)
            {
                var ed = (EditModeUnl)CurrentMode;
                ed.PerformPaste();
            }
        }

        #region Base

        bool hover;
        int hover_x;
        int hover_y;
        int hover_mx;
        int hover_my;
        int current_id;
        int current_group_id;

        public void MapHover(int mx, int my)
        {
            if (mx <= 0 || my <= 0 || mx >= f.panelM1.GetMapW() || my >= f.panelM1.GetMapH())
            { hover_x = -1; hover_y = -1; hover_mx = -1; hover_my = -1; hover = false; f.tLabelMouse.Text = "-"; return; }
            int stx = f.panelM1.start_tilex;
            int sty = f.panelM1.start_tiley;
            int ax = Convert.ToInt32(Math.Floor((mx - f.panelM1.offset_x) / (Core.Zoom * 32)));
            int ay = Convert.ToInt32(Math.Floor((my - f.panelM1.offset_y) / (Core.Zoom * 32)));
            hover_x = stx + ax;
            hover_y = sty + ay;
            hover_mx = stx * 32 + mx - f.panelM1.offset_x;
            hover_my = sty * 32 + my - f.panelM1.offset_y;
            hover = true;
            if (Core.Zoom == 1.0f)
            {
                f.tLabelMouse.Text = "(" + hover_x.ToString() + "," + hover_y.ToString() + ")" +
                    " [" + hover_mx.ToString() + "," + hover_my.ToString() + "]" ;
            }
            else
            {
                f.tLabelMouse.Text = "(" + hover_x.ToString() + "," + hover_y.ToString() + ")";
            }
        }

        public void CancelHover()
        {
            hover = false;
            hover_x = -1;
            hover_y = -1;
            hover_mx = -1;
            hover_my = -1;
            f.tLabelMouse.Text = "-";
        }

        public void DrawPointer(Graphics g, int x, int y, int w, int h)
        {
            int ax = x * 32; int ay = y * 32; int aw = w * 32; int ah = h * 32;

            g.FillRectangle(Brushes.Black, ax, ay, aw, 4);
            g.FillRectangle(Brushes.Black, ax, ay, 4, ah);
            g.FillRectangle(Brushes.Black, ax, ay + ah - 4, aw, 4);
            g.FillRectangle(Brushes.Black, ax + aw - 4, ay, 4, ah);

            g.FillRectangle(Brushes.White, ax + 1, ay + 1, aw - 2, 2);
            g.FillRectangle(Brushes.White, ax + 1, ay + 1, 2, ah - 2);
            g.FillRectangle(Brushes.White, ax + 1, ay + ah - 3, aw - 2, 2);
            g.FillRectangle(Brushes.White, ax + aw - 3, ay + 1, 2, ah - 2);

        }

        public void DrawPointerR(Graphics g, int x, int y, int w, int h)
        {
            int ax = x * 32; int ay = y * 32; int aw = w * 32; int ah = h * 32;

            g.FillRectangle(Brushes.Black, ax, ay, aw, 4);
            g.FillRectangle(Brushes.Black, ax, ay, 4, ah);
            g.FillRectangle(Brushes.Black, ax, ay + ah - 4, aw, 4);
            g.FillRectangle(Brushes.Black, ax + aw - 4, ay, 4, ah);

            g.FillRectangle(Brushes.Red, ax + 1, ay + 1, aw - 2, 2);
            g.FillRectangle(Brushes.Red, ax + 1, ay + 1, 2, ah - 2);
            g.FillRectangle(Brushes.Red, ax + 1, ay + ah - 3, aw - 2, 2);
            g.FillRectangle(Brushes.Red, ax + aw - 3, ay + 1, 2, ah - 2);

        }

        public void DrawPointerG(Graphics g, int x, int y, int w, int h)
        {
            int ax = x * 32; int ay = y * 32; int aw = w * 32; int ah = h * 32;

            g.FillRectangle(Brushes.Black, ax, ay, aw, 4);
            g.FillRectangle(Brushes.Black, ax, ay, 4, ah);
            g.FillRectangle(Brushes.Black, ax, ay + ah - 4, aw, 4);
            g.FillRectangle(Brushes.Black, ax + aw - 4, ay, 4, ah);

            g.FillRectangle(Brushes.Lime, ax + 1, ay + 1, aw - 2, 2);
            g.FillRectangle(Brushes.Lime, ax + 1, ay + 1, 2, ah - 2);
            g.FillRectangle(Brushes.Lime, ax + 1, ay + ah - 3, aw - 2, 2);
            g.FillRectangle(Brushes.Lime, ax + aw - 3, ay + 1, 2, ah - 2);

        }

        public void ClearLower()
        {
            f.pictureBoxL.Image = null;
            f.pictureBoxL.BackgroundImage = null;
            f.pictureBoxL.Refresh();
        }

        public void ClearLowerImg()
        {
            if (f.pictureBoxL.Image == null) { return; }
            ClearImg((Bitmap)f.pictureBoxL.Image);
        }

        public void ClearImg(Bitmap image)
        {
            if (image == null) { return; }
            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.Transparent);
            }
        }

        public void ClearTop()
        {
            foreach (Bitmap b in tileset_sel)
            {
                ClearImg(b);
            }
        }

        #endregion
        
        public bool ShiftDown { get { return f.ShiftDown; } }
        public bool SpaceDown { get { return f.SpaceDown; } }
        public bool CtrlDown { get { return f.CtrlDown; } }

        //Getters

        public bool Hover { get { return hover; } }
        public int HoverX { get { return hover_x; } }
        public int HoverY { get { return hover_y; } }
        public int HoverMX { get { return hover_mx; } }
        public int HoverMY { get { return hover_my; } }
        public int CurrentID { get { return current_id; } set { current_id = value; UpdateTileLabel(); } }
        public int CurrentGroupID { get { return current_group_id; } set { current_group_id = value; UpdateTileLabel(); } }

        public bool Selected
        {
            get
            {
                if (DrawMode == 2)
                {
                    return ((EditModeRec)CurrentMode).selected;
                }
                else if (DrawMode == 3)
                {
                    return ((EditModeSel)CurrentMode).selected;
                }
                else if (DrawMode == 4)
                {
                    return ((EditModeUnl)CurrentMode).selected;
                }
                return false;
            }
        }
        public bool Selecting
        {
            get
            {
                if (DrawMode == 2)
                {
                    return ((EditModeRec)CurrentMode).selecting;
                }
                else if (DrawMode == 3)
                {
                    return ((EditModeSel)CurrentMode).selecting;
                }
                else if (DrawMode == 4)
                {
                    return ((EditModeUnl)CurrentMode).selecting;
                }
                return false;
            }
        }
        public int SelectWidth
        {
            get
            {
                if (DrawMode == 2)
                {
                    return ((EditModeRec)CurrentMode).sel_w;
                }
                else if (DrawMode == 3)
                {
                    return ((EditModeSel)CurrentMode).sel_w;
                }
                else if (DrawMode == 4)
                {
                    return ((EditModeUnl)CurrentMode).sel_w;
                }
                return 0;
            }
        }
        public int SelectHeight
        {
            get
            {
                if (DrawMode == 2)
                {
                    return ((EditModeRec)CurrentMode).sel_h;
                }
                else if (DrawMode == 3)
                {
                    return ((EditModeSel)CurrentMode).sel_h;
                }
                else if (DrawMode == 4)
                {
                    return ((EditModeUnl)CurrentMode).sel_h;
                }
                return 0;
            }
        }
        public int SelectStartX
        {
            get
            {
                if (DrawMode == 2)
                {
                    return ((EditModeRec)CurrentMode).sel_sx;
                }
                else if (DrawMode == 3)
                {
                    return ((EditModeSel)CurrentMode).sel_rx;
                }
                else if (DrawMode == 4)
                {
                    return ((EditModeUnl)CurrentMode).sel_rx;
                }
                return 0;
            }
        }
        public int SelectStartY
        {
            get
            {
                if (DrawMode == 2)
                {
                    return ((EditModeRec)CurrentMode).sel_sy;
                }
                else if (DrawMode == 3)
                {
                    return ((EditModeSel)CurrentMode).sel_ry;
                }
                else if (DrawMode == 4)
                {
                    return ((EditModeUnl)CurrentMode).sel_ry;
                }
                return 0;
            }
        }
        public int SelectEndX
        {
            get
            {
                if (DrawMode == 2)
                {
                    return ((EditModeRec)CurrentMode).sel_ex;
                }
                else if (DrawMode == 4)
                {
                    return ((EditModeUnl)CurrentMode).sel_ex;
                }
                return 0;
            }
        }
        public int SelectEndY
        {
            get
            {
                if (DrawMode == 2)
                {
                    return ((EditModeRec)CurrentMode).sel_ey;
                }
                else if (DrawMode == 4)
                {
                    return ((EditModeUnl)CurrentMode).sel_ey;
                }
                return 0;
            }
        }

        private string GetTileTypeName(int i)
        {
            switch (i)
            {
                case 0: return "A1";
                case 1: return "A2";
                case 2: return "A3";
                case 3: return "A4";
                case 4: return "A5";
                case 5: return "B";
                default: return "";
            }
        }

        void UpdateTileLabel()
        {
            if (DrawMode == 2 || DrawMode == 4){ f.tLabelTile.Text = "-"; return; }
            if (EditMode == 6) { f.tLabelTile.Text = "通行度: " + current_id.ToString(); }
            else if (EditMode == 7) { f.tLabelTile.Text = "特殊标志: " + current_id.ToString(); }
            else if (EditMode == 8) { f.tLabelTile.Text = "区域: " + current_id.ToString(); }
            else
            {
                if (current_id == 0) { f.tLabelTile.Text = "-"; return; }
                int t = current_id; string str = "";
                if (current_id == -1) { t = current_group_id; }
                int type = (int)((t & 0xFF000000) >> 24);
                if (type == 0) { f.tLabelTile.Text = "-"; return; }
                int index = (int)((t & 0x00FF0000) >> 16);
                int tid = ((t & 0x0000FFFF));
                str += "类型: " + GetTileTypeName(type - 1);
                str += " 序号: " + index.ToString();
                str += " ID: " + tid.ToString();
                if (current_id == -1) { str += " (自动绘制)"; }
                f.tLabelTile.Text = str;
            }
        }

        public void LVOSelChange()
        {
            if (DrawMode != 4) { return; }
            var ed = (EditModeUnl)CurrentMode;
            ed.LVOSelChange();
        }
    }
}
