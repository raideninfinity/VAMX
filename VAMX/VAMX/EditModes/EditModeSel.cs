using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace VAMX
{
    public class EditModeSel : EditMode
    {
        public override int Mode { get { return 3; } }
        public bool selected;
        public bool bselected;
        public bool selecting;
        public bool selector_on;
        public int sel_sx = 0;
        public int sel_sy = 0;
        public int sel_ex = 0;
        public int sel_ey = 0;
        public int bsel_sx = 0;
        public int bsel_sy = 0;
        public int bsel_ex = 0;
        public int bsel_ey = 0;

        public bool saved;
        public int savew;
        public int saveh;
        public int[] savedata;

        public int[] sel_data;
        public int current_group = 0;

        public int sel_w { get { return Math.Abs(sel_sx - sel_ex) + 1; } }
        public int sel_h { get { return Math.Abs(sel_sy - sel_ey) + 1; } }
        public int sel_rx { get { return Math.Min(sel_sx, sel_ex); } }
        public int sel_ry { get { return Math.Min(sel_sy, sel_ey); } }

        public int bsel_w { get { return Math.Abs(bsel_sx - bsel_ex) + 1; } }
        public int bsel_h { get { return Math.Abs(bsel_sy - bsel_ey) + 1; } }
        public int bsel_rx { get { return Math.Min(bsel_sx, bsel_ex); } }
        public int bsel_ry { get { return Math.Min(bsel_sy, bsel_ey); } }

        public override void Reset()
        {
            saved = false;
            savedata = null;
            saveh = 0;
            savew = 0;
            selected = false;
            current_group = 0;
            selecting = false;
            sel_sx = 0;
            sel_sy = 0;
            sel_ex = 0;
            sel_ey = 0;
            bsel_sx = 0;
            bsel_sy = 0;
            bsel_ex = 0;
            bsel_ey = 0;
            selector_on = false;
            sel_data = null;
        }

        //Events
        //Top

        #region Top Events

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

        #endregion

        //Low

        #region Low Events

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

        #endregion

        //Map

        #region Map Controls

        bool MouseDownL = false;
        bool MouseDownR = false;

        //Map

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

        public void PerformCut()
        {
            OperationX();
        }

        public void PerformCopy()
        {
            OperationC();
        }

        public void PerformPaste()
        {
            OperationV();
        }

        #endregion

        //----------------------------------------------------------------------
        //Top Selector

        #region Top Selector

        public void ShowSinglePointer(int type, int x, int y)
        {
            Core.Editor.ClearTop(); Core.Editor.ClearLowerImg();
            using (Graphics g = Graphics.FromImage(Core.Editor.TilesetSelect[type]))
            {
                Core.Editor.DrawPointer(g, x, y, 1, 1);
            }
            Core.Editor.f.pictureBoxT.Refresh();
        }

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

        #region A5/B/Flag Tiles

        public void DrawTopSelector(int i)
        {
            int x = Math.Min(bsel_sx, bsel_ex);
            int y = Math.Min(bsel_sy, bsel_ey);
            int w = bsel_w; int h = bsel_h;
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
            bsel_sx = x;
            bsel_sy = y;
            bsel_ex = x;
            bsel_ey = y;
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
                bsel_ex = x;
                bsel_ey = y;
                DrawTopSelector(2);
            }
        }

        public void A5Up()
        {
            if (Core.Editor.TilesetSelect[2] == null) { return; }
            if (selector_on)
            {
                selector_on = false;
                DrawTopSelector(2);
                //Load Data
                int x = Math.Min(bsel_sx, bsel_ex);
                int y = Math.Min(bsel_sy, bsel_ey);
                int w = bsel_w;
                int h = bsel_h;
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
            bsel_sx = x;
            bsel_sy = y;
            bsel_ex = x;
            bsel_ey = y;
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
                bsel_ex = x;
                bsel_ey = y;
                DrawTopSelector(3);
            }
        }

        public void BUp()
        {
            if (Core.Editor.TilesetSelect[3] == null) { return; }
            if (selector_on)
            {
                selector_on = false;
                DrawTopSelector(3);
                //Load Data
                int x = Math.Min(bsel_sx, bsel_ex);
                int y = Math.Min(bsel_sy, bsel_ey);
                int w = bsel_w;
                int h = bsel_h;
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
            bsel_sx = x;
            bsel_sy = y;
            bsel_ex = x;
            bsel_ey = y;
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
                bsel_ex = x;
                bsel_ey = y;
                DrawTopSelector(f);
            }
        }

        public void FlagUp()
        {
            if (selector_on)
            {
                int f = Core.Editor.EditMode - 2;
                selector_on = false;
                DrawTopSelector(f);
                //Load Data
                int x = Math.Min(bsel_sx, bsel_ex);
                int y = Math.Min(bsel_sy, bsel_ey);
                int w = bsel_w;
                int h = bsel_h;
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

        #region Set Autotiles

        public void SetA1(int mx, int my)
        {
            int x = mx.div(32); int y = my.div(32);
            int id = x + y * 8;
            int shape = id % 16;
            Core.Editor.CurrentGroupID = 0x01000000 | (id.div(16) << 16) | (shape * 100);
            Core.Editor.CurrentID = -1;
            ShowSinglePointer(0, x, y);
            //Set Bottom
            if (shape >= 10)
            {
                Core.Editor.f.pictureBoxL.Image = new Bitmap(128, 64);
                Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(128, 64);
                using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(Core.BAuto, 0, 0);
                    for (int i = 0; i < 4; i++)
                    {
                        int a = ((i % 4) * 32);
                        int b = 32 + (i.div(4) * 32);
                        g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + i, true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
            else
            {
                Core.Editor.f.pictureBoxL.Image = new Bitmap(256, 224);
                Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(256, 224);
                using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
                {
                    g.DrawImage(Core.BAuto, 0, 0);
                    for (int i = 0; i < 48; i++)
                    {
                        int a = ((i % 8) * 32);
                        int b = 32 + (i.div(8) * 32);
                        g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + Core.Editor.GroundOrder[i], true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
            SetAuto();
        }

        public void SetA2(int mx, int my)
        {
            int x = mx.div(32); int y = (my - Core.Map.Tilesets[0].Count * 64).div(32);
            int id = x + y * 8;
            Core.Editor.CurrentGroupID = 0x02000000 | (id.div(32) << 16) | ((id % 32) * 100);
            Core.Editor.CurrentID = -1;
            ShowSinglePointer(0, x, y + Core.Map.Tilesets[0].Count * 2);
            //Set Bottom
            Core.Editor.f.pictureBoxL.Image = new Bitmap(256, 224);
            Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(256, 224);
            using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(Core.BAuto, 0, 0);
                for (int i = 0; i < 48; i++)
                {
                    int a = ((i % 8) * 32);
                    int b = 32 + (i.div(8) * 32);
                    g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + Core.Editor.GroundOrder[i], true), new Rectangle(a, b, 32, 32));
                    g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                }
            }
            SetAuto();
        }

        public void SetA3(int mx, int my)
        {
            int x = mx.div(32); int y = my.div(32);
            int id = x + y * 8;
            Core.Editor.CurrentGroupID = 0x03000000 | (id.div(32) << 16) | ((id % 32) * 100);
            Core.Editor.CurrentID = -1;
            ShowSinglePointer(1, x, y);
            //Set Bottom
            Core.Editor.f.pictureBoxL.Image = new Bitmap(128, 160);
            Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(128, 160);
            using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(Core.BAuto, 0, 0);
                for (int i = 0; i < 16; i++)
                {
                    int a = ((i % 4) * 32);
                    int b = 32 + (i.div(4) * 32);
                    g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + Core.Editor.WallOrder[i], true), new Rectangle(a, b, 32, 32));
                    g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                }
            }
            SetAuto();
        }

        public void SetA4(int mx, int my)
        {
            int x = mx.div(32); int y = (my - Core.Map.Tilesets[2].Count * 128).div(32);
            int id = x + y * 8;
            Core.Editor.CurrentGroupID = 0x04000000 | (id.div(48) << 16) | ((id % 48) * 100);
            Core.Editor.CurrentID = -1;
            ShowSinglePointer(1, x, y + Core.Map.Tilesets[2].Count * 4);
            //Set Bottom
            int k = id.div(8) % 2;
            if (k == 1)
            {
                Core.Editor.f.pictureBoxL.Image = new Bitmap(128, 160);
                Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(128, 160);
                using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
                {
                    g.Clear(Color.Transparent);
                    g.DrawImage(Core.BAuto, 0, 0);
                    for (int i = 0; i < 16; i++)
                    {
                        int a = ((i % 4) * 32);
                        int b = 32 + (i.div(4) * 32);
                        g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + Core.Editor.WallOrder[i], true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
            else
            {
                Core.Editor.f.pictureBoxL.Image = new Bitmap(256, 224);
                Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(256, 224);
                using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
                {
                    g.DrawImage(Core.BAuto, 0, 0);
                    for (int i = 0; i < 48; i++)
                    {
                        int a = ((i % 8) * 32);
                        int b = 32 + (i.div(8) * 32);
                        g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + Core.Editor.GroundOrder[i], true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
            SetAuto();
        }

        public void SetAuto()
        {
            bsel_sx = 0;
            bsel_sy = 0;
            bsel_ex = 0;
            bsel_ey = 0;
            sel_data = new int[] { Core.Editor.CurrentGroupID + 99};
            DrawBtmSelector();
        }

        #endregion

        //----------------------------------------------------------------------
        //Bottom Selector

        #region Bottom Selector

        public void DrawBtmSelector()
        {
            int x = Math.Min(bsel_sx, bsel_ex);
            int y = Math.Min(bsel_sy, bsel_ey);
            int w = bsel_w; int h = bsel_h;
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
            if (y == 0 && x > 0) { return; }
            bsel_sx = x;
            bsel_sy = y;
            bsel_ex = x;
            bsel_ey = y;
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
                if (y > 0)
                {
                    if (bsel_sy == 0) { return; }
                }
                else
                {
                    if (bsel_sy > 0) { return; }
                }
                bsel_ex = x;
                bsel_ey = y;
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
                if (bsel_sy == 0) { SetAuto(); return; }
                DrawBtmSelector();
                //Load data into table
                int x = Math.Min(bsel_sx, bsel_ex);
                int y = Math.Min(bsel_sy, bsel_ey) - 1;
                int w = bsel_w;
                int h = bsel_h;
                int div; int[] t;
                if (Core.Editor.f.pictureBoxL.Image.Size.Height == 32) { div = 4; t = Core.Editor.WaterfallOrder; }
                else if (Core.Editor.f.pictureBoxL.Image.Size.Height == 128) { div = 4; t = Core.Editor.WallOrder; }
                else { div = 8; t = Core.Editor.GroundOrder; }
                int g = Core.Editor.CurrentGroupID;
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

        //----------------------------------------------------------------------
        //Map Operations

        #region Map Select

        public void MapRDown()
        {
            selected = false;
            sel_sx = Core.Editor.HoverX;
            sel_sy = Core.Editor.HoverY;
            sel_ex = Core.Editor.HoverX;
            sel_ey = Core.Editor.HoverY;
            selecting = true;
            Core.Editor.RefreshMap();
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
                if (Core.Editor.f.ShiftDown || Core.Editor.f.CtrlDown)
                {
                    selecting = false; selected = false;
                    sel_ex = 0; sel_ey = 0;
                    sel_sx = 0; sel_sy = 0;
                }
                else
                {
                    selecting = false;
                    selected = true;
                }
                Core.Editor.RefreshMap();
            }
        }

        #endregion

        #region Cut/Copy/Paste Operations

        public void OperationX()
        {
            if (!selected) { return; }
            Core.Editor.SetUndo();
            Copy();
            for (int i = 0; i < savew; i++)
            {
                for (int j = 0; j < saveh; j++)
                {
                    if (!Core.Editor.InMap(i + sel_rx, j + sel_ry)) { continue; }
                    Core.Map.Layers[i + sel_rx, j + sel_ry, Core.Editor.EditMode] = 0;
                }
            }
            Core.Editor.RefreshMap();
        }

        public void OperationC()
        {
            if (!selected) { return; }
            Copy();
        }

        public void Copy()
        {
            saved = true;
            savew = sel_w; saveh = sel_h;
            int[] data = new int[savew * saveh];
            for (int i = 0; i < savew; i++)
            {
                for (int j = 0; j < saveh; j++)
                {
                    data[i + j * savew] = Core.Map.Layers[i + sel_rx, j + sel_ry, Core.Editor.EditMode];
                }
            }
            savedata = data;
        }

        public void OperationV()
        {
            if (!(saved && selected)) { return; }
            Core.Editor.SetUndo();
            int x = sel_rx; int y = sel_ry;
            bool c = Core.Editor.f.ShiftDown;
            for (int i = 0; i < savew; i++)
            {
                for (int j = 0; j < saveh; j++)
                {
                    if (!Core.Editor.InMap(i + x, j + y)) { continue; }
                    if (c && !InBox(i + x, j + y)) { continue; }
                    Core.Map.Layers[i + x, j + y, Core.Editor.EditMode] = savedata[i + j * savew];
                }
            }
            Core.Editor.RefreshMap();
        }

        public bool InBox(int x, int y)
        {
            int rx = sel_rx;
            int ry = sel_ry;
            int rh = sel_h;
            int rw = sel_w;
            if (x >= rx + rw) { return false; }
            if (y >= ry + rh) { return false; }
            return true;
        }

        #endregion

        //----------------------------------------------------------------------
        //Map Drawing

        public bool InSelectedBox(int x, int y)
        {
            if (x < sel_rx || y < sel_ry) { return false; }
            if (x >= sel_rx + sel_w || y >= sel_ry + sel_h) { return false; }
            return true;
        }

        public void MapDrawTile()
        {
            if (!selected || !Core.Editor.Hover || Core.Editor.HoverX < 0 || Core.Editor.EditMode >= 9) { return; }
            if (!InSelectedBox(Core.Editor.HoverX, Core.Editor.HoverY)) { return; }
            if (bsel_w == 0 || bsel_h == 0) { return; }
            bool s = Core.Editor.f.ShiftDown; bool c = Core.Editor.f.CtrlDown;
            int w = sel_w; int h = sel_h;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    //----------------------------
                    int x = sel_rx + i;
                    int y = sel_ry + j;
                    int t = sel_data[i % bsel_w + (j % bsel_h * bsel_w)];
                    int g = Core.Editor.BaseTile(t);
                    //----------------------------
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

        public void MapDrawTile(int x, int y, int t)
        {
            if ((t & 0x0000FFFF) % 100 == 99) { Core.Editor.MapAutoTile(x, y, Core.Editor.BaseTile(t)); }
            else
            {
                Core.Map.Layers[x, y, Core.Editor.EditMode] = t;
            }
        }

        //----------------------------------------------------------------------
    }
}
