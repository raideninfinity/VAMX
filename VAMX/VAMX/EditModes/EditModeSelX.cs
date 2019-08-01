using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace VAMX
{
    public class EditModeSelX : EditMode
    {
        public override int Mode { get { return 3; } }

        public bool selected = false;
        public bool selecting = false;
        public int sel_sx = 0;
        public int sel_sy = 0;
        public int sel_ex = 0;
        public int sel_ey = 0;

        public bool saved;
        public int savew;
        public int saveh;
        public int[] savedata;

        public int sel_w { get { return Math.Abs(sel_sx - sel_ex) + 1; } }
        public int sel_h { get { return Math.Abs(sel_sy - sel_ey) + 1; } }
        public int sel_rx { get { return Math.Min(sel_sx, sel_ex); } }
        public int sel_ry { get { return Math.Min(sel_sy, sel_ey); } }

        public override void Reset()
        {
            saved = false;
            savedata = null;
            saveh = 0;
            savew = 0;
            selected = false;
            selecting = false;
            sel_sx = 0;
            sel_sy = 0;
            sel_ex = 0;
            sel_ey = 0;
        }

        public override void PTopDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            SetTop(e.X, e.Y);
        }

        public override void PTopLeave()
        {

        }

        public override void PTopMove(MouseEventArgs e)
        {

        }

        public override void PTopUp(MouseEventArgs e)
        {

        }

        public override void PLowDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }
            if (Core.Editor.f.pictureBoxL.Image == null) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            SetLower(e.X, e.Y);
        }

        public override void PLowLeave()
        {

        }
        public override void PLowMove(MouseEventArgs e)
        {

        }
        public override void PLowUp(MouseEventArgs e)
        {

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

        //----------------------------------------------------------

        //-------------------------------------------

        #region Set Tile

        public void ShowSinglePointer(int type, int x, int y)
        {
            Core.Editor.ClearTop(); Core.Editor.ClearLowerImg();
            using (Graphics g = Graphics.FromImage(Core.Editor.TilesetSelect[type]))
            {
                Core.Editor.DrawPointer(g, x, y, 1, 1);
            }
            Core.Editor.f.pictureBoxT.Refresh();
        }

        public void ShowLowerPointer(int x, int y)
        {
            Core.Editor.ClearLowerImg();
            using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.Image))
            {
                Core.Editor.DrawPointer(g, x, y, 1, 1);
            }
            Core.Editor.f.pictureBoxL.Refresh();
        }

        public void SetTop(int x, int y)
        {
            if (Core.Editor.EditMode > 5 && Core.Editor.EditMode < 9) { SetFlag(x, y); }
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
                    SetA5(x, y);
                }
                else if (Core.Editor.f.tabControlT.SelectedIndex == 3)
                {
                    SetB(x, y);
                }
            }
        }

        public void SetLower(int mx, int my)
        {
            if (Core.Editor.f.pictureBoxL.Image == null) { return; }
            if (mx > Core.Editor.f.pictureBoxL.Image.Size.Width || my > Core.Editor.f.pictureBoxL.Image.Size.Height) { return; }
            //switch
            switch (Core.Editor.f.pictureBoxL.Image.Size.Width)
            {
                case 64: SetBottom1(mx, my); break;
                case 128: SetBottom2(mx, my); break;
                case 256: SetBottom3(mx, my); break;
            }
        }

        #endregion

        //----------------------------------------

        #region Set Top

        public void SetFlag(int mx, int my)
        {
            int mode = Core.Editor.EditMode - 6;
            int x = mx.div(32); int y = my.div(32);
            Core.Editor.CurrentGroupID = x + y * 8 + 1;
            Core.Editor.CurrentID = x + y * 8 + 1;
            int id = Core.Editor.CurrentGroupID - 1;
            ShowSinglePointer(Core.Editor.EditMode - 2, x, y);
            //Set Bottom
            Core.Editor.f.pictureBoxL.Image = new Bitmap(64, 32);
            Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(64, 32);
            using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
            {
                g.DrawImage(Core.FlagImages[mode], new Rectangle(0, 0, 32, 32),
                    new Rectangle((id % 8) * 32, id.div(8) * 32, 32, 32),
                    GraphicsUnit.Pixel);
                g.DrawImage(Core.BErase, 32, 0);
            }
            ShowLowerPointer(0, 0);
        }

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
                    g.DrawImage(Core.BErase, 32, 0);
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
                    g.DrawImage(Core.BErase, 32, 0);
                    for (int i = 0; i < 48; i++)
                    {
                        int a = ((i % 8) * 32);
                        int b = 32 + (i.div(8) * 32);
                        g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + Core.Editor.GroundOrder[i], true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
            ShowLowerPointer(0, 0);
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
                g.DrawImage(Core.BErase, 32, 0);
                for (int i = 0; i < 48; i++)
                {
                    int a = ((i % 8) * 32);
                    int b = 32 + (i.div(8) * 32);
                    g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + Core.Editor.GroundOrder[i], true), new Rectangle(a, b, 32, 32));
                    g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                }
            }
            ShowLowerPointer(0, 0);
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
                g.DrawImage(Core.BErase, 32, 0);
                for (int i = 0; i < 16; i++)
                {
                    int a = ((i % 4) * 32);
                    int b = 32 + (i.div(4) * 32);
                    g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + Core.Editor.WallOrder[i], true), new Rectangle(a, b, 32, 32));
                    g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                }
            }
            ShowLowerPointer(0, 0);
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
                    g.DrawImage(Core.BErase, 32, 0);
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
                    g.DrawImage(Core.BErase, 32, 0);
                    for (int i = 0; i < 48; i++)
                    {
                        int a = ((i % 8) * 32);
                        int b = 32 + (i.div(8) * 32);
                        g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentGroupID + Core.Editor.GroundOrder[i], true), new Rectangle(a, b, 32, 32));
                        g.DrawImage(Core.GridCache[1.0f], new Rectangle(a, b, 32, 32));
                    }
                }
            }
            ShowLowerPointer(0, 0);
        }

        public void SetA5(int mx, int my)
        {
            int x = mx.div(32); int y = my.div(32);
            int id = x + y * 8;
            Core.Editor.CurrentGroupID = Core.Editor.CurrentID = 0x05000000 | (id.div(128) << 16) | id % 128;
            ShowSinglePointer(2, x, y);
            //Set Bottom
            Core.Editor.f.pictureBoxL.Image = new Bitmap(64, 32);
            Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(64, 32);
            using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
            {
                g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentID, true), new Rectangle(0, 0, 32, 32));
                g.DrawImage(Core.BErase, 32, 0);
            }
            ShowLowerPointer(0, 0);
        }

        public void SetB(int mx, int my)
        {
            int x = mx.div(32); int y = my.div(32);
            int id = x + y * 8;
            Core.Editor.CurrentGroupID = Core.Editor.CurrentID = 0x06000000 | (id.div(256) << 16) | id % 256;
            ShowSinglePointer(3, x, y);
            //Set Bottom
            Core.Editor.f.pictureBoxL.Image = new Bitmap(64, 32);
            Core.Editor.f.pictureBoxL.BackgroundImage = new Bitmap(64, 32);
            using (Graphics g = Graphics.FromImage(Core.Editor.f.pictureBoxL.BackgroundImage))
            {
                g.DrawImage(Core.Editor.GetTile(Core.Editor.CurrentID, true), new Rectangle(0, 0, 32, 32));
                g.DrawImage(Core.BErase, 32, 0);
            }
            ShowLowerPointer(0, 0);
        }

        #endregion

        //-------------------------------------

        #region Set Bottom

        public void SetBottom1(int mx, int my)
        {
            int x = mx.div(32); int y = my.div(32);
            int id = x + y * 2;
            if (id == 0) { Core.Editor.CurrentID = Core.Editor.CurrentGroupID; }
            else { Core.Editor.CurrentID = 0; }
            ShowLowerPointer(x, y);
        }

        public void SetBottom2(int mx, int my)
        {
            int x = mx.div(32); int y = my.div(32);
            int id = x + y * 4;
            if (id > 1 && id < 4) { return; }
            if (id == 0) { Core.Editor.CurrentID = -1; }
            else if (id == 1) { Core.Editor.CurrentID = 0; }
            else
            {
                id -= 4;
                if (Core.Editor.f.pictureBoxL.Image.Size.Height == 64)
                {
                    Core.Editor.CurrentID = Core.Editor.CurrentGroupID + id;
                }
                else
                {
                    Core.Editor.CurrentID = Core.Editor.CurrentGroupID + Core.Editor.WallOrder[id];
                }
            }
            ShowLowerPointer(x, y);
        }

        public void SetBottom3(int mx, int my)
        {
            int x = mx.div(32); int y = my.div(32);
            int id = x + y * 8;
            if (id > 1 && id < 8) { return; }
            if (id == 0) { Core.Editor.CurrentID = -1; }
            else if (id == 1) { Core.Editor.CurrentID = 0; }
            else
            {
                id -= 8;
                Core.Editor.CurrentID = Core.Editor.CurrentGroupID + Core.Editor.GroundOrder[id];
            }
            ShowLowerPointer(x, y);
        }

        #endregion

        //-------------------------------------

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

        #region MapDraw

        public void MapDrawTile()
        {
            if (!selected || !Core.Editor.Hover || Core.Editor.HoverX < 0 || Core.Editor.EditMode >= 9) { return; }
            if (!InSelectedBox()) { return; }
            bool sd = Core.Editor.f.ShiftDown; bool cd = Core.Editor.f.CtrlDown;
            for (int i = 0; i < sel_w; i++)
            {
                for (int j = 0; j < sel_h; j++)
                {
                    int x = i + sel_rx; int y = j + sel_ry;
                    int tid = Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode];
                    int type = (int)((tid & 0xFF000000) >> 24);
                    if (!Core.Editor.f.ShiftDown && Core.Editor.f.CtrlDown)
                    {
                        Core.Map.Layers[x, y, Core.Editor.EditMode] = 0; continue;
                    }
                    else if (Core.Editor.f.ShiftDown && Core.Editor.f.CtrlDown)
                    {
                        Core.Map.Layers[x, y, Core.Editor.EditMode] = 0;
                        if (type >= 1 && type < 5)
                        {
                            Core.Editor.FixAutoTile(x, y, Core.Editor.BaseTile(tid));
                        }
                    }
                    if (Core.Editor.CurrentID == -1) //Autotile
                    {
                        Core.Editor.MapAutoTile(x, y, Core.Editor.CurrentGroupID);
                    }
                    else
                    {
                        Core.Map.Layers[x, y, Core.Editor.EditMode] = Core.Editor.CurrentID;
                    }
                    if (type >= 1 && type < 5 && Core.Editor.f.ShiftDown && !Core.Editor.f.CtrlDown)
                    {

                        Core.Editor.FixAutoTile(x, y, Core.Editor.BaseTile(tid));
                    }
                }
            }
        }

        public bool InSelectedBox()
        {
            if (Core.Editor.HoverX < sel_rx || Core.Editor.HoverY < sel_ry) { return false; }
            if (Core.Editor.HoverX >= sel_rx + sel_w || Core.Editor.HoverY >= sel_ry + sel_h) { return false; }
            return true;
        }

        #endregion
        //------------------------------------------------------------------------------
    }
}
