using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace VAMX
{
    public class EditModeRec : EditMode
    {
        public override int Mode { get { return 2; } }

        public bool selected;
        public bool selecting;
        public bool selector_on;
        public int sel_sx = 0;
        public int sel_sy = 0;
        public int sel_ex = 0;
        public int sel_ey = 0;
        public int[] sel_data;
        public int current_group = 0;

        public int sel_w { get { return Math.Abs(sel_sx - sel_ex) + 1; } }
        public int sel_h { get { return Math.Abs(sel_sy - sel_ey) + 1; } }

        public override void Reset()
        {
            current_group = 0;
            selecting = false;
            sel_ex = 0;
            sel_ey = 0;
            sel_sx = 0;
            sel_sy = 0;
            selected = false;
            selector_on = false;
            sel_data = null;
        }

        public override void PTopDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            TopDown(e.X, e.Y);
        }

        public override void PTopLeave()
        {
            if (Core.Editor.EditMode >= 9) { return; }
            TopUp(); 
        }

        public override void PTopMove(MouseEventArgs e)
        {
            if (Core.Editor.EditMode >= 9) { return; }
            TopMove(e.X, e.Y);
        }

        public override void PTopUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            TopUp();
        }

        public override void PLowDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }
            if (Core.Editor.f.pictureBoxL.Image == null) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            BtmDown(e.X, e.Y);
        }

        public override void PLowLeave()
        {
            if (Core.Editor.f.pictureBoxL.Image == null) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            BtmUp();
        }
        public override void PLowMove(MouseEventArgs e)
        {
            if (Core.Editor.f.pictureBoxL.Image == null) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            BtmMove(e.X, e.Y);
        }
        public override void PLowUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }
            if (Core.Editor.f.pictureBoxL.Image == null) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            BtmUp();
        }

        bool MouseDownL = false;
        bool MouseDownR = false;

        public override void MapDown(MouseEventArgs e)
        {
            Core.Editor.MapHover(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                Core.Editor.SetUndo();
                MouseDownL = true;
                MapMouseDownL();
            }
            else
            {
                MouseDownR = true;
                MapMouseDownR();
            }
            Core.Editor.RefreshMap();
        }

        public override void MapLeave()
        {
            MouseDownL = false;
            MouseDownR = false;
            if (Core.Editor.EditMode >= 9) { return; }
            MapRUp();
            Core.Editor.CancelHover();
            Core.Editor.RefreshMap();
        }

        public override void MapMove(MouseEventArgs e)
        {
            Core.Editor.MapHover(e.X, e.Y);
            if (MouseDownL)
            {
                MapMouseDownL();
            }
            else if (MouseDownR)
            {
                if (Core.Editor.EditMode >= 9) { return; }
                MapRMove();
            }
            Core.Editor.RefreshMap();
        }
        public override void MapUp(MouseEventArgs e)
        {
            MouseDownL = false;
            MouseDownR = false;
            if (Core.Editor.EditMode >= 9) { return; }
            MapRUp();
            Core.Editor.RefreshMap();
        }

        public override void MapMouseDownL()
        {
            if (Core.Editor.EditMode >= 9) { return; }
            MapDrawTile();
        }
        public override void MapMouseDownR()
        {
            if (Core.Editor.EditMode >= 9) { return; }
            MapRDown();
        }

        //------------------------------------------------------------------

        public void ShowSinglePointer(int type, int x, int y)
        {
            Core.Editor.ClearTop(); Core.Editor.ClearLowerImg();
            using (Graphics g = Graphics.FromImage(Core.Editor.TilesetSelect[type]))
            {
                Core.Editor.DrawPointer(g, x, y, 1, 1);
            }
            Core.Editor.f.pictureBoxT.Refresh();
        }

        #region Controls

        public void TopDown(int x, int y)
        {
            if (Core.Editor.EditMode > 5 && Core.Editor.EditMode < 9) { FlagDown(x, y); }
            else if (Core.Editor.EditMode >= 9) { return; }
            else
            {
                if (Core.Editor.f.tabControlT.SelectedIndex == 0)
                {
                    if (y > Core.Map.Tilesets[0].Count * 64)
                    {
                        SetA2(x, y);
                    }
                    else
                    {
                        SetA1(x, y);
                    }
                }
                else if (Core.Editor.f.tabControlT.SelectedIndex == 1)
                {
                    if (y > Core.Map.Tilesets[2].Count * 128)
                    {
                        SetA4(x, y);
                    }
                    else
                    {
                        SetA3(x, y);
                    }
                }
                else if (Core.Editor.f.tabControlT.SelectedIndex == 2)
                {
                    A5Down(x, y);
                }
                else if (Core.Editor.f.tabControlT.SelectedIndex == 3)
                {
                    BDown(x, y);
                }
            }
        }

        public void TopMove(int x, int y)
        {
            if (Core.Editor.EditMode > 5 && Core.Editor.EditMode < 9) { FlagMove(x, y); }
            else if (Core.Editor.EditMode >= 9) { return; }
            else
            {
                if (Core.Editor.f.tabControlT.SelectedIndex == 2)
                {
                    A5Move(x, y);
                }
                else if (Core.Editor.f.tabControlT.SelectedIndex == 3)
                {
                    BMove(x, y);
                }
            }
        }

        public void TopUp()
        {
            if (Core.Editor.EditMode > 5 && Core.Editor.EditMode < 9) { FlagUp(); }
            else if (Core.Editor.EditMode >= 9) { return; }
            else
            {
                if (Core.Editor.f.tabControlT.SelectedIndex == 2)
                {
                    A5Up();
                }
                else if (Core.Editor.f.tabControlT.SelectedIndex == 3)
                {
                    BUp();
                }
            }
        }

        #endregion

        #region Autotiles A1~A4

        public void SetA1(int mx, int my)
        {
            int x = Core.div(mx, 32); int y = Core.div(my, 32);
            int id = x + y * 8;
            int shape = id % 16;
            current_group = 0x01000000 | (Core.div(id, 16) << 16) | (shape * 100);
            ShowSinglePointer(0, x, y);
            if (shape >= 10)
            {
                Core.Editor.f.pictureBoxL.Image = new Bitmap(128, 32);
                Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(128, 32);
                using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
                {
                    g.Clear(Color.Transparent);
                    for (int i = 0; i < 4; i++)
                    {
                        int a = ((i % 4) * 32);
                        int b = (Core.div(i, 4) * 32);
                        g.DrawImage(Core.Editor.GetTile(current_group + i, true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
            else
            {
                Core.Editor.f.pictureBoxL.Image = new Bitmap(256, 192);
                Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(256, 192);
                using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
                {
                    for (int i = 0; i < 48; i++)
                    {
                        int a = ((i % 8) * 32);
                        int b = (Core.div(i, 8) * 32);
                        g.DrawImage(Core.Editor.GetTile(current_group + Core.Editor.GroundOrder[i], true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
        }

        public void SetA2(int mx, int my)
        {
            int x = Core.div(mx, 32); int y = Core.div(my - Core.Map.Tilesets[0].Count * 64, 32);
            int id = x + y * 8;
            current_group = 0x02000000 | (Core.div(id, 32) << 16) | ((id % 32) * 100);
            ShowSinglePointer(0, x, y + Core.Map.Tilesets[0].Count * 2);
            Core.Editor.f.pictureBoxL.Image = new Bitmap(256, 192);
            Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(256, 192);
            using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
            {
                g.Clear(Color.Transparent);
                for (int i = 0; i < 48; i++)
                {
                    int a = ((i % 8) * 32);
                    int b = (Core.div(i, 8) * 32);
                    g.DrawImage(Core.Editor.GetTile(current_group + Core.Editor.GroundOrder[i], true), new Rectangle(a, b, 32, 32));
                    g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                }
            }
        }

        public void SetA3(int mx, int my)
        {
            int x = Core.div(mx, 32); int y = Core.div(my, 32);
            int id = x + y * 8;
            current_group = 0x03000000 | (Core.div(id, 32) << 16) | ((id % 32) * 100);
            ShowSinglePointer(1, x, y);
            //Set Bottom
            Core.Editor.f.pictureBoxL.Image = new Bitmap(128, 128);
            Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(128, 128);
            using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
            {
                g.Clear(Color.Transparent);
                for (int i = 0; i < 16; i++)
                {
                    int a = ((i % 4) * 32);
                    int b = (Core.div(i, 4) * 32);
                    g.DrawImage(Core.Editor.GetTile(current_group + Core.Editor.WallOrder[i], true), new Rectangle(a, b, 32, 32));
                    g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                }
            }
        }

        public void SetA4(int mx, int my)
        {
            int x = Core.div(mx, 32); int y = Core.div(my - Core.Map.Tilesets[2].Count * 128, 32);
            int id = x + y * 8;
            current_group = 0x04000000 | (Core.div(id, 48) << 16) | ((id % 48) * 100);
            ShowSinglePointer(1, x, y + Core.Map.Tilesets[2].Count * 4);
            int k = Core.div(id, 8) % 2;
            if (k == 1)
            {
                Core.Editor.f.pictureBoxL.Image = new Bitmap(128, 128);
                Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(128, 128);
                using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
                {
                    g.Clear(Color.Transparent);
                    for (int i = 0; i < 16; i++)
                    {
                        int a = ((i % 4) * 32);
                        int b = (Core.div(i, 4) * 32);
                        g.DrawImage(Core.Editor.GetTile(current_group + Core.Editor.WallOrder[i], true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
            else
            {
                Core.Editor.f.pictureBoxL.Image = new Bitmap(256, 192);
                Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(256, 192);
                using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
                {
                    for (int i = 0; i < 48; i++)
                    {
                        int a = ((i % 8) * 32);
                        int b = (Core.div(i, 8) * 32);
                        g.DrawImage(Core.Editor.GetTile(current_group + Core.Editor.GroundOrder[i], true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
        }

        #endregion

        #region Autotile Selector

        public void DrawBtmSelector()
        {
            int x = Math.Min(sel_sx, sel_ex);
            int y = Math.Min(sel_sy, sel_ey);
            int w = sel_w; int h = sel_h;
            Core.Editor.ClearImg((Bitmap)Core.Editor.f.pictureBoxL.Image);
            using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.Image))
            {
                if (selector_on)
                {
                    Core.Editor.DrawPointerR(g, x, y, w, h);
                }
                else
                {
                    Core.Editor.DrawPointerG(g, x, y, w, h);
                }
            }
            Core.Editor.f.pictureBoxL.Refresh();
        }

        public void BtmDown(int mx, int my)
        {
            if (Core.Editor.f.pictureBoxL.Image == null) { return; }
            if (mx >= Core.Editor.f.pictureBoxL.Image.Size.Width || my >= Core.Editor.f.pictureBoxL.Image.Size.Height || mx < 0 || my < 0) { return; }
            int x = Core.div(mx, 32); int y = Core.div(my, 32);
            sel_sx = x;
            sel_sy = y;
            sel_ex = x;
            sel_ey = y;
            selector_on = true;
            DrawBtmSelector();
        }

        public void BtmMove(int mx, int my)
        {
            if (Core.Editor.f.pictureBoxL.Image == null) { return; }
            if (selector_on)
            {
                if (mx >= Core.Editor.f.pictureBoxL.Image.Size.Width || my >= Core.Editor.f.pictureBoxL.Image.Size.Height || mx < 0 || my < 0) { return; }
                int x = Core.div(mx, 32); int y = Core.div(my, 32);
                sel_ex = x;
                sel_ey = y;
                DrawBtmSelector();
            }
        }

        public void BtmUp()
        {
            if (Core.Editor.f.pictureBoxL.Image == null) { return; }
            if (selector_on)
            {
                selector_on = false;
                selected = true;
                DrawBtmSelector();
                //Load data into table
                int x = Math.Min(sel_sx, sel_ex);
                int y = Math.Min(sel_sy, sel_ey);
                int w = sel_w;
                int h = sel_h;
                int div; int[] t;
                if (Core.Editor.f.pictureBoxL.Image.Size.Height == 32) { div = 4; t = Core.Editor.WaterfallOrder; }
                else if (Core.Editor.f.pictureBoxL.Image.Size.Height == 128) { div = 4; t = Core.Editor.WallOrder; }
                else { div = 8; t = Core.Editor.GroundOrder; }
                int g = current_group;
                int[] data = new int[w * h];
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        data[i + j * w] = g + t[(x + i) + (y + j) * div];
                    }
                }
                sel_data = data;
            }
        }

        #endregion

        #region A5/B/Flag Tiles

        public void DrawTopSelector(int i)
        {
            int x = Math.Min(sel_sx, sel_ex);
            int y = Math.Min(sel_sy, sel_ey);
            int w = sel_w; int h = sel_h;
            Core.Editor.ClearImg(Core.Editor.TilesetSelect[i]);
            using (Graphics g = Graphics.FromImage(Core.Editor.TilesetSelect[i]))
            {
                if (selector_on)
                {
                    Core.Editor.DrawPointerR(g, x, y, w, h);
                }
                else
                {
                    Core.Editor.DrawPointerG(g, x, y, w, h);
                }
            }
            Core.Editor.f.pictureBoxT.Refresh();
        }

        //A5

        public void A5Down(int mx, int my)
        {
            Core.Editor.ClearTop();
            Core.Editor.ClearLower();
            if (mx >= Core.Editor.TilesetSelect[2].Width || my >= Core.Editor.TilesetSelect[2].Height || mx < 0 || my < 0) { return; }
            int x = Core.div(mx, 32); int y = Core.div(my, 32);
            sel_sx = x;
            sel_sy = y;
            sel_ex = x;
            sel_ey = y;
            selector_on = true;
            DrawTopSelector(2);
        }

        public void A5Move(int mx, int my)
        {
            if (Core.Editor.TilesetSelect[2] == null) { return; }
            if (selector_on)
            {
                if (mx >= Core.Editor.TilesetSelect[2].Width || my >= Core.Editor.TilesetSelect[2].Height || mx < 0 || my < 0) { return; }
                int x = Core.div(mx, 32); int y = Core.div(my, 32);
                sel_ex = x;
                sel_ey = y;
                DrawTopSelector(2);
            }
        }

        public void A5Up()
        {
            if (Core.Editor.TilesetSelect[2] == null) { return; }
            if (selector_on)
            {
                selector_on = false;
                selected = true;
                DrawTopSelector(2);
                //Load Data
                int x = Math.Min(sel_sx, sel_ex);
                int y = Math.Min(sel_sy, sel_ey);
                int w = sel_w;
                int h = sel_h;
                int[] data = new int[w * h];
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        int id = (x + i) + (y + j) * 8;
                        data[i + j * w] = 0x05000000 | (Core.div(id, 128) << 16) | id % 128;
                    }
                }
                sel_data = data;
            }
        }

        //B

        public void BDown(int mx, int my)
        {
            Core.Editor.ClearTop();
            Core.Editor.ClearLower();
            if (mx >= Core.Editor.TilesetSelect[3].Width || my >= Core.Editor.TilesetSelect[3].Height || mx < 0 || my < 0) { return; }
            int x = Core.div(mx, 32); int y = Core.div(my, 32);
            sel_sx = x;
            sel_sy = y;
            sel_ex = x;
            sel_ey = y;
            selector_on = true;
            DrawTopSelector(3);
        }

        public void BMove(int mx, int my)
        {
            if (Core.Editor.TilesetSelect[3] == null) { return; }
            if (selector_on)
            {
                if (mx >= Core.Editor.TilesetSelect[3].Width || my >= Core.Editor.TilesetSelect[3].Height || mx < 0 || my < 0) { return; }
                int x = Core.div(mx, 32); int y = Core.div(my, 32);
                sel_ex = x;
                sel_ey = y;
                DrawTopSelector(3);
            }
        }

        public void BUp()
        {
            if (Core.Editor.TilesetSelect[3] == null) { return; }
            if (selector_on)
            {
                selector_on = false;
                selected = true;
                DrawTopSelector(3);
                //Load Data
                int x = Math.Min(sel_sx, sel_ex);
                int y = Math.Min(sel_sy, sel_ey);
                int w = sel_w;
                int h = sel_h;
                int[] data = new int[w * h];
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        int id = (x + i) + (y + j) * 8;
                        data[i + j * w] = 0x06000000 | (Core.div(id, 256) << 16) | id % 256;
                    }
                }
                sel_data = data;
            }
        }

        //Flags
        public void FlagDown(int mx, int my)
        {
            Core.Editor.ClearTop();
            Core.Editor.ClearLower();
            int f = Core.Editor.EditMode - 2;
            if (mx >= Core.Editor.TilesetSelect[f].Width || my >= Core.Editor.TilesetSelect[f].Height || mx < 0 || my < 0) { return; }
            int x = Core.div(mx, 32); int y = Core.div(my, 32);
            sel_sx = x;
            sel_sy = y;
            sel_ex = x;
            sel_ey = y;
            selector_on = true;
            DrawTopSelector(f);
        }

        public void FlagMove(int mx, int my)
        {
            if (selector_on)
            {
                int f = Core.Editor.EditMode - 2;
                if (mx >= Core.Editor.TilesetSelect[f].Width || my >= Core.Editor.TilesetSelect[f].Height || mx < 0 || my < 0) { return; }
                int x = Core.div(mx, 32); int y = Core.div(my, 32);
                sel_ex = x;
                sel_ey = y;
                DrawTopSelector(f);
            }
        }

        public void FlagUp()
        {
            if (selector_on)
            {
                int f = Core.Editor.EditMode - 2;
                selector_on = false;
                selected = true;
                DrawTopSelector(f);
                //Load Data
                int x = Math.Min(sel_sx, sel_ex);
                int y = Math.Min(sel_sy, sel_ey);
                int w = sel_w;
                int h = sel_h;
                int[] data = new int[w * h];
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        int id = (x + i) + (y + j) * 8;
                        data[i + j * w] = id + 1;
                    }
                }
                sel_data = data;
            }
        }

        #endregion

        //================================================================================

        #region Right Click Select

        public void MapRDown()
        {
            selected = false;
            sel_sx = Core.Editor.HoverX;
            sel_sy = Core.Editor.HoverY;
            sel_ex = Core.Editor.HoverX;
            sel_ey = Core.Editor.HoverY;
            selecting = true;
            Core.Editor.f.panelM1.PaintMap();
        }

        public void MapRMove()
        {
            if (!Core.Editor.Hover || Core.Editor.HoverX < 0) { return; }
            sel_ex = Core.Editor.HoverX;
            sel_ey = Core.Editor.HoverY;
        }

        public void MapRUp()
        {
            if (selecting)
            {
                selecting = false;
                selected = true;
                Core.Editor.f.panelM1.PaintMap();
                Core.Editor.ClearTop();
                Core.Editor.f.pictureBoxT.Refresh();
                Core.Editor.ClearLower();
                Core.Editor.f.pictureBoxL.Refresh();
                //Load_Data
                int x = Math.Min(sel_sx, sel_ex);
                int y = Math.Min(sel_sy, sel_ey);
                int w = sel_w;
                int h = sel_h;
                int[] data = new int[w * h];
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        data[i + j * w] = GetMapData(x + i, y + j);
                    }
                }
                sel_data = data;
            }

        }

        public int GetMapData(int x, int y)
        {
            int tid = Core.Map.Layers[x, y, Core.Editor.EditMode];
            if (Core.Editor.f.CtrlDown && !Core.Editor.f.ShiftDown) { Core.Map.Layers[x, y, Core.Editor.EditMode] = 0; }
            else if (Core.Editor.f.CtrlDown && Core.Editor.f.ShiftDown)
            {
                Core.Map.Layers[x, y, Core.Editor.EditMode] = 0; Core.Editor.FixAutoTile(x, y, Core.Editor.BaseTile(tid));
            }
            else if (!Core.Editor.f.CtrlDown && Core.Editor.f.ShiftDown)
            {
                int type = (int)((tid & 0xFF000000) >> 24);
                if (type <= 4 && type > 0) { tid = Core.Editor.BaseTile(tid) + 99; }
            }
            return tid;
        }

        #endregion

        #region Draw Tile

        public void MapDrawTile()
        {
            if (!Core.Editor.Hover || !selected || Core.Editor.HoverX < 0) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            else
            {
                bool s = Core.Editor.f.ShiftDown; bool c = Core.Editor.f.CtrlDown;
                int w = sel_w; int h = sel_h;
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        int x = Core.Editor.HoverX + i; int y = Core.Editor.HoverY + j;
                        int t = sel_data[i + j * w];
                        int g = Core.Editor.BaseTile(t);
                        if (!Core.Editor.InMap(x, y)) { continue; }
                        if (c && !s)
                        {
                            Core.Map.Layers[x, y, Core.Editor.EditMode] = 0;
                        }
                        else if (s && !c)
                        {
                            MapDrawTile(x, y, t);
                            Core.Editor.FixAutoTile(x, y, g);
                        }
                        else if (s && c)
                        {
                            Core.Map.Layers[x, y, Core.Editor.EditMode] = 0;
                            Core.Editor.FixAutoTile(x, y, g);
                        }
                        else { MapDrawTile(x, y, t); }
                    }
                }
            }
        }

        public void MapDrawTile(int x, int y, int t)
        {
            if ((t & 0x0000FFFF) % 100 == 99) { Core.Editor.MapAutoTile(x, y, Core.Editor.BaseTile(t)); }
            else
            {
                Core.Map.Layers[x, y, Core.Editor.EditMode] = t;
            }
        }

        #endregion

        //==========================================================================

    }
}
