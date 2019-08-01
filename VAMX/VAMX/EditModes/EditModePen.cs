using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace VAMX
{
    public class EditModePen : EditMode
    {
        public override int Mode { get { return 1; } }

        public override void Reset()
        {

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
                MapMouseDownR();
            }
            Core.Editor.RefreshMap();
        }

        public override void MapLeave()
        {
            MouseDownL = false;
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
            Core.Editor.RefreshMap();
        }
        public override void MapUp(MouseEventArgs e)
        {
            MouseDownL = false;
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
            RightClickSelect();
        }

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

        #region Draw Tiles

        public void MapDrawTile()
        {
            if (!Core.Editor.Hover || Core.Editor.HoverX < 0) { return; }
            if (Core.Editor.EditMode > 5 && Core.Editor.EditMode < 9) { MapDrawFlag(); return; }
            //if (Core.Editor.CurrentGroupID == 0) { return; }
            //Perform Draw
            int tid = Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode];
            int type = (int)((tid & 0xFF000000) >> 24);
            int btile = Core.Editor.BaseTile(tid);
            bool shift = Core.Editor.ShiftDown;
            if (!Core.Editor.SpaceDown)
            {
                //Ctrl Erase
                if (Core.Editor.CtrlDown)
                {
                    Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode] = 0;
                    if (type >= 1 && type < 5 && shift)
                    {
                        Core.Editor.FixAutoTile(Core.Editor.HoverX, Core.Editor.HoverY, btile);
                    }
                    return;
                }

                //Normal Mapping
                if (Core.Editor.CurrentID == -1) //Autotile
                {
                    Core.Editor.MapAutoTile(Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.CurrentGroupID);
                }
                else
                {
                    Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode] = Core.Editor.CurrentID;
                }

                //Hold Shift -> Fix Autotiles
                if (type >= 1 && type < 5 && shift)
                {
                    Core.Editor.FixAutoTile(Core.Editor.HoverX, Core.Editor.HoverY, btile);
                }
            }
            else
            {
                //Paint Bucket
                int id = Core.Editor.CurrentID;
                if (Core.Editor.CtrlDown) { id = 0; }
                if (id == -1) //Autotile
                {
                    paintedtiles.Clear();
                    Core.Editor.MapAutoTile(Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.CurrentGroupID);
                    if (shift) { Core.Editor.FixAutoTile(Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.BaseTile(tid)); }
                    PaintAutoTile(Core.Editor.HoverX, Core.Editor.HoverY, tid, Core.Editor.CurrentGroupID, shift);
                }
                else //Normal tiles
                {
                    paintedtiles.Clear();
                    Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode] = id;
                    if (shift) { Core.Editor.FixAutoTile(Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.BaseTile(tid)); }
                    PaintTile(Core.Editor.HoverX, Core.Editor.HoverY, tid, id, shift);
                }
            }
        }

        List<int> paintedtiles = new List<int>();

        public void PaintAutoTile(int x, int y, int t, int id, bool fix)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Math.Abs(i) + Math.Abs(j) != 1) { continue; }
                    int ax = x + i; int ay = y + j;
                    if (!Core.Editor.InMap(ax, ay)) { continue; }
                    if (!EqualTile(ax, ay, t)) { continue; }
                    if (paintedtiles.Contains(ax * 1000 + ay)) { continue; }
                    //Perform Paint Auto Tile
                    Core.Editor.MapAutoTile(ax, ay, id);
                    if (fix) { Core.Editor.FixAutoTile(ax, ay, Core.Editor.BaseTile(t)); }
                    paintedtiles.Add(ax * 1000 + ay);
                    PaintAutoTile(ax, ay, t, id, fix);
                }
            }
        }

        public void PaintTile(int x, int y, int t, int id, bool fix)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Math.Abs(i) + Math.Abs(j) != 1) { continue; }
                    int ax = x + i; int ay = y + j;
                    if (!Core.Editor.InMap(ax, ay)) { continue; }
                    if (!EqualTile(ax, ay, t)) { continue; }
                    if (paintedtiles.Contains(ax * 1000 + ay)) { continue; }
                    //Perform Paint Single Tile
                    Core.Map.Layers[ax, ay, Core.Editor.EditMode] = id;
                    if (fix) { Core.Editor.FixAutoTile(ax, ay, Core.Editor.BaseTile(t)); }
                    paintedtiles.Add(ax * 1000 + ay);
                    PaintTile(ax, ay, t, id, fix);
                }
            }
        }

        //FLAGS

        List<int> paintedflags = new List<int>();

        public void MapDrawFlag()
        {
            //if (Core.Editor.CurrentGroupID == 0) { return; }
            int id = Core.Editor.CurrentID;
            bool ctrldown = Core.Editor.CtrlDown;
            if (!Core.Editor.SpaceDown)
            {
                if (ctrldown) { id = 0; }
                Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode] = id;
            }
            else
            {
                int t = Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode];
                if (ctrldown) { id = 0; }
                Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode] = id;
                paintedflags.Clear();
                PaintFlag(Core.Editor.HoverX, Core.Editor.HoverY, t, id);
            }
        }

        public void PaintFlag(int x, int y, int t, int id)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Math.Abs(i) + Math.Abs(j) != 1) { continue; }
                    int ax = x + i; int ay = y + j;
                    if (!Core.Editor.InMap(ax, ay)) { continue; }
                    if (!EqualTile(ax, ay, t)) { continue; }
                    if (paintedflags.Contains(ax * 1000 + ay)) { continue; }
                    //Perform Paint Flag
                    Core.Map.Layers[ax, ay, Core.Editor.EditMode] = id;
                    paintedflags.Add(ax * 1000 + ay);
                    PaintFlag(ax, ay, t, id);
                }
            }
        }

        public bool EqualTile(int x, int y, int j)
        {
            if (!Core.Editor.InMap(x, y)) { return false; }
            int i = Core.Map.Layers[x, y, Core.Editor.EditMode];
            if (Core.Editor.GetTileType(j) > 0 && Core.Editor.GetTileType(j) <= 4) { return (Core.Editor.BaseTile(i) == Core.Editor.BaseTile(j)); }
            return (i == j);
        }

        #endregion

        #region Right Click Select

        public void RightClickSelect()
        {
            if (Core.Editor.EditMode > 5 && Core.Editor.EditMode < 9) { RCFlag(); return; }
            //Perform Select
            int id = Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode];
            int type = (int)((id & 0xFF000000) >> 24);
            bool erase = false;
            if (type == 0 && Core.Editor.CurrentGroupID < 0x01000000)
            {
                switch (Core.Editor.f.tabControlT.SelectedIndex)
                {
                    case 0: SelectA1(0); break;
                    case 1: SelectA3(0); break;
                    case 2: SelectA5(0); break;
                    case 3: SelectB(0); break;
                }
                SetLower(34, 1);
                return;
            }
            else if (type == 0 && Core.Editor.CurrentGroupID >= 0x01000000)
            {
                erase = true;
                type = (int)((Core.Editor.CurrentGroupID & 0xFF000000) >> 24);
                id = Core.Editor.CurrentGroupID;
            }
            switch (type)
            {
                case 1: SelectA1(id); break;
                case 2: SelectA2(id); break;
                case 3: SelectA3(id); break;
                case 4: SelectA4(id); break;
                case 5: SelectA5(id); break;
                case 6: SelectB(id); break;
            }
            if (erase) { SetLower(34, 1); }
        }

        public void RCFlag()
        {
            int id = Core.Map.Layers[Core.Editor.HoverX, Core.Editor.HoverY, Core.Editor.EditMode];
            if (id == 0)
            {
                SetTop(1, 1);
                SetLower(34, 1);
            }
            else
            {
                SetTop(id % 8 * 32 - 1, Core.div(id, 8) * 32 + 1);
            }
        }

        public void SelectA1(int t)
        {
            if (t == 0)
            {
                Core.Editor.f.tabControlT.SelectTab(0);
                Core.Editor.f.panelT.AutoScrollPosition = new Point(0, 0);
                SetA1(1, 1);
                return;
            }
            int index = (int)((t & 0x00FF0000) >> 16);
            int id = (int)(t & 0x0000FFFF);
            int kind = Core.div(id, 100);
            int shape = id % 100;
            Core.Editor.f.tabControlT.SelectTab(0);
            Core.Editor.f.panelT.AutoScrollPosition = new Point(0, Core.div(kind, 8) * 32 + index * 64);
            SetA1(kind % 8 * 32, Core.div(kind, 8) * 32 + index * 64);
            if (Core.Editor.ShiftDown && t != 0) { SetLower(1, 1); return; }
            int tid;
            if (kind >= 10 && kind < 15)
            {
                tid = shape;
            }
            else
            {
                tid = Core.Editor.GroundOrderReverse(shape);
            }
            SetLower((tid % 8) * 32, Core.div(tid, 8) * 32 + 32);
        }

        public void SelectA2(int t)
        {
            int index = (int)((t & 0x00FF0000) >> 16);
            int id = (int)(t & 0x0000FFFF);
            int kind = Core.div(id, 100);
            int shape = id % 100;
            Core.Editor.f.tabControlT.SelectTab(0);
            Core.Editor.f.panelT.AutoScrollPosition = new Point(0, Core.div(kind, 8) * 32 + index * 128 + Core.Map.Tilesets[0].Count * 64);
            SetA2(kind % 8 * 32, Core.div(kind, 8) * 32 + index * 128 + Core.Map.Tilesets[0].Count * 64);
            if (Core.Editor.ShiftDown && t != 0) { SetLower(1, 1); return; }
            int tid = Core.Editor.GroundOrderReverse(shape);
            SetLower((tid % 8) * 32, Core.div(tid, 8) * 32 + 32);
        }

        public void SelectA3(int t)
        {
            if (t == 0)
            {
                Core.Editor.f.tabControlT.SelectTab(1);
                Core.Editor.f.panelT.AutoScrollPosition = new Point(0, 0);
                SetA3(1, 1);
                return;
            }
            int index = (int)((t & 0x00FF0000) >> 16);
            int id = (int)(t & 0x0000FFFF);
            int kind = Core.div(id, 100);
            int shape = id % 100;
            Core.Editor.f.tabControlT.SelectTab(1);
            Core.Editor.f.panelT.AutoScrollPosition = new Point(0, Core.div(kind, 8) * 32 + index * 128);
            SetA3(kind % 8 * 32, Core.div(kind, 8) * 32 + index * 128);
            if (Core.Editor.ShiftDown && t != 0) { SetLower(1, 1); return; }
            int tid = Core.Editor.WallOrderReverse(shape);
            SetLower((tid % 4) * 32, Core.div(tid, 4) * 32 + 32);
        }

        public void SelectA4(int t)
        {
            int index = (int)((t & 0x00FF0000) >> 16);
            int id = (int)(t & 0x0000FFFF);
            int kind = Core.div(id, 100);
            int shape = id % 100;
            Core.Editor.f.tabControlT.SelectTab(1);
            Core.Editor.f.panelT.AutoScrollPosition = new Point(0, Core.div(kind, 8) * 32 + index * 192 + Core.Map.Tilesets[2].Count * 128);
            SetA4(kind % 8 * 32, Core.div(kind, 8) * 32 + index * 192 + Core.Map.Tilesets[2].Count * 128);
            if (Core.Editor.ShiftDown && t != 0) { SetLower(1, 1); return; }
            int k = Core.div(kind, 8) % 2;
            if (k == 0)
            {
                int tid = Core.Editor.GroundOrderReverse(shape);
                SetLower((tid % 8) * 32, Core.div(tid, 8) * 32 + 32);
            }
            else
            {
                int tid = Core.Editor.WallOrderReverse(shape);
                SetLower((tid % 4) * 32, Core.div(tid, 4) * 32 + 32);
            }
        }

        public void SelectA5(int t)
        {
            int index = (int)((t & 0x00FF0000) >> 16);
            int tid = ((t & 0x0000FFFF));
            int id = index * 128 + tid;
            Core.Editor.f.tabControlT.SelectTab(2);
            Core.Editor.f.panelT.AutoScrollPosition = new Point(0, Core.div(id, 8) * 32);
            SetA5(id % 8 * 32, Core.div(id, 8) * 32);
        }

        public void SelectB(int t)
        {
            int type = (int)((t & 0xFF000000) >> 24);
            int index = (int)((t & 0x00FF0000) >> 16);
            int tid = ((t & 0x0000FFFF));
            int id = index * 256 + tid;
            Core.Editor.f.tabControlT.SelectTab(3);
            Core.Editor.f.panelT.AutoScrollPosition = new Point(0, Core.div(id, 8) * 32);
            SetB(id % 8 * 32, Core.div(id, 8) * 32);
        }

        #endregion


    }
}
