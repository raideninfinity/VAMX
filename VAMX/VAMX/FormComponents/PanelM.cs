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
    public class PanelM : Panel
    {
        public PanelM()
        {
            this.DoubleBuffered = this.ResizeRedraw = true;
            AutoScroll = true;
        }

        bool enabled;
        public void EnablePanel() { enabled = true; }

        #region Base

        public int posx = 0;
        public int posy = 0;
        public int numx = 0;
        public int numy = 0;
        public int start_tilex = 0;
        public int start_tiley = 0;
        public int offset_x = 0;
        public int offset_y = 0;

        public void PaintMap()
        {
            this.AutoScrollMinSize = new Size(GetMapW(), GetMapH());
            DisplayMap();
        }

        public void DisplayMap()
        {
            this.Refresh();
        }

        public int Zoom(int i)
        {
            return Core.Editor.Zoom(i);
        }

        public int GetMapW()
        {
            return Zoom(Core.Map.Width * 32);
        }

        public int GetMapH()
        {
            return Zoom(Core.Map.Height * 32);
        }

        public int Tile(int i, int j, int k)
        {
            return Core.Map.Layers[i, j, k];
        }

        public void CalculatePos()
        {
            posx = -this.AutoScrollPosition.X;
            posy = -this.AutoScrollPosition.Y;
            if (this.Width >= GetMapW()) { numx = Core.Map.Width; start_tilex = 0; offset_x = 0; }
            else
            {
                numx = (Convert.ToInt32(Math.Ceiling(this.Width / (Core.Zoom * 32)))) + 1;
                if (numx >= Core.Map.Width) { numx = Core.Map.Width; }
                start_tilex = Convert.ToInt32(Math.Floor(posx / (Core.Zoom * 32)));
                offset_x = Zoom(start_tilex * 32) - posx;
            }
            if (this.Height >= GetMapH()) { numy = Core.Map.Height; start_tiley = 0; offset_y = 0; }
            else
            {
                numy = (Convert.ToInt32(Math.Ceiling(this.Height / (Core.Zoom * 32)))) + 1;
                if (numy >= Core.Map.Height) { numx = Core.Map.Height; }
                start_tiley = Convert.ToInt32(Math.Floor(posy / (Core.Zoom * 32)));
                offset_y = Zoom(start_tiley * 32) - posy;
            }
        }

        public void ClearBack(Graphics g)
        {
            g.FillRectangle(Brushes.DimGray, new Rectangle(offset_x, offset_y, Zoom(numx * 32), Zoom(numy * 32)));
        }

        #endregion

        #region Draw

        public bool OptDisplayLayer(int i)
        {
            if (Core.Editor.EditMode == i) { return true; }
            switch (i)
            {
                case 0: return !Core.Editor.f.tsbH1.Checked;
                case 1: return !Core.Editor.f.tsbH2.Checked;
                case 2: return !Core.Editor.f.tsbH3.Checked;
                case 3: return !Core.Editor.f.tsbH4.Checked;
                case 4: return !Core.Editor.f.tsbH5.Checked;
                case 5: return !Core.Editor.f.tsbH6.Checked;
            }
            return false;
        }

        public void FixBorder(Graphics g)
        {
            if (this.Width > GetMapW())
            {
                int x = GetMapW(); int y = 0; int w = this.Width; int h = this.Height;
                g.FillRectangle(Brushes.DarkGray, x, y, w, h);
            }
            if (this.Height > GetMapH())
            {
                int x = 0; int y = GetMapH(); int w = this.Width; int h = this.Height;
                g.FillRectangle(Brushes.DarkGray, x, y, w, h);
            }
        }

        public void DrawMap(Graphics g)
        {
            //Display Layers
            for (int i = 0; i < 6; i++)
            {
                if (OptDisplayLayer(i)) { DrawLayer(g, i); }
            }
            //Display Flags
            if (Core.Editor.EditMode > 5 & Core.Editor.EditMode < 9)
            {
                DrawFlag(g, Core.Editor.EditMode);
            }
        }

        public bool TileOpacity(int i)
        {
            int mode = Core.Editor.EditMode;
            if (mode >= 9) { return true; }
            else if (mode > 5 && mode < 9) { return false; }
            else { return (mode == i); }
        }

        public bool ObjectOpacity(int i, int s)
        {
            return TileOpacity(i) && ObjectOpacity(s);
        }

        const int B_TILE = 0x06000000;

        public void DrawLayer(Graphics g, int l)
        {
            for (int i = 0; i < numx; i++)
            {
                for (int j = 0; j < numy; j++)
                {
                    if (start_tilex + i >= Core.Map.Width || start_tiley + j >= Core.Map.Height) { continue; }
                    Core.Editor.DrawTile(g, Tile(start_tilex + i, start_tiley + j, l),
                        offset_x + Zoom(i * 32), offset_y + Zoom(j * 32), TileOpacity(l));
                }
            }
            //Draw Object
            List<UObject> l_objects = Core.Map.GetCurrentLayerObjects(l);
            foreach (UObject obj in l_objects)
            {
                if (!CheckObjInRange(obj)) { continue; }
                if (!ShowObject(obj.SubLayer)){ continue; }
                for (int i = 0; i < obj.SpanX; i++)
                {
                    for (int j = 0; j < obj.SpanY; j++)
                    {
                        if ((i + 1) * 32 <= obj.CropLeft || 
                            (j + 1) * 32 <= obj.CropTop ||
                            i * 32 >= obj.SpanX * 32 - obj.CropRight || 
                            j  * 32 >= obj.SpanY * 32 - obj.CropBottom) { continue; }

                        int tile = GetObjTile(obj.TileIndex, obj.TileID, i, j);
                        int x = obj.X + i * 32 - obj.CropLeft - (start_tilex * 32 - offset_x);
                        int y = obj.Y + j * 32 - obj.CropTop - (start_tiley * 32 - offset_y);

                        var dest = new Rectangle(x, y, 32, 32);
                        var src = new Rectangle(0, 0, 32, 32);

                        if (i * 32 < obj.CropLeft)
                        {
                            int k = obj.CropLeft - i * 32;
                            dest.Width -= k; src.Width -= k; dest.X += k; src.X += k;
                        }
                        if (j * 32 < obj.CropTop)
                        {
                            int k = obj.CropTop - j * 32;
                            dest.Height -= k; src.Height -= k; dest.Y += k; src.Y += k;
                        }
                        if ((i + 1) * 32 > obj.SpanX * 32 - obj.CropRight)
                        {
                            int k = (i + 1) * 32 - (obj.SpanX * 32 - obj.CropRight);
                            dest.Width -= k; src.Width -= k;
                        }
                        if ((j + 1) * 32 > obj.SpanY * 32 - obj.CropBottom)
                        {
                            int k = (j + 1) * 32 - (obj.SpanY * 32 - obj.CropBottom);
                            dest.Height -= k; src.Height -= k;
                        }

                        Core.Editor.DrawTileEX(g, tile, dest, src, ObjectOpacity(l,obj.SubLayer));
                    }
                }
            }
        }

        public bool ShowObject(int s)
        {
            switch (s)
            {
                case 0: return !Core.Editor.f.tsbSH1.Checked;
                case 1: return !Core.Editor.f.tsbSH2.Checked;
                case 2: return !Core.Editor.f.tsbSH3.Checked;
                case 3: return !Core.Editor.f.tsbSH4.Checked;
                case 4: return !Core.Editor.f.tsbSH5.Checked;
                case 5: return !Core.Editor.f.tsbSH6.Checked;
            }
            return true;
        }

        public bool ObjectOpacity(int s)
        {
            switch (s)
            {
                case 0: return Core.Editor.f.tsbS1.Checked;
                case 1: return Core.Editor.f.tsbS2.Checked;
                case 2: return Core.Editor.f.tsbS3.Checked;
                case 3: return Core.Editor.f.tsbS4.Checked;
                case 4: return Core.Editor.f.tsbS5.Checked;
                case 5: return Core.Editor.f.tsbS6.Checked;
            }
            return true;
        }

        public bool CheckObjInRange(UObject obj)
        {
            int x = obj.X; int y = obj.Y;
            int w = obj.SpanX * 32 - obj.CropLeft - obj.CropRight; int h = obj.SpanY * 32 - obj.CropTop - obj.CropBottom;
            return !(x + w < (start_tilex * 32 - offset_x) || y + h < (start_tiley * 32 - offset_y) ||
                x > (start_tilex + numx) * 32 - offset_x || y > (start_tiley + numy) * 32 - offset_y);
        }

        public int GetObjTile(int tile_index, int tile_id, int x, int y)
        {
            tile_id += (x + y * 8);
            while (tile_id > 255)
            {
                tile_id -= 256;
                tile_index++;
            }
            return (B_TILE | (tile_index << 16) | tile_id);
        }

        public void DrawFlag(Graphics g, int t)
        {
            for (int i = 0; i < numx; i++)
            {
                for (int j = 0; j < numy; j++)
                {
                    if (start_tilex + i >= Core.Map.Width || start_tiley + j >= Core.Map.Height) { continue; }
                    Core.Editor.DrawFlagTile(g, t - 6, Tile(start_tilex + i, start_tiley + j, t),
                        offset_x + Zoom(i * 32), offset_y + Zoom(j * 32));
                }
            }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!enabled) { base.OnPaint(e); return; }
            //Preparations
            CalculatePos();
            var g = e.Graphics;
            ClearBack(g);
            //Draw
            DrawMap(g);
            DrawGrid(g);
            DrawTileIndic(g);
            DrawObjectIndic(g);
            DrawSelector(g);
            DrawHover(g);
            //Finishing
            FixBorder(g);
            base.OnPaint(e);
        }

        #region Draw Hover

        public void DrawHover(Graphics g)
        {
            if (Core.Editor.DrawMode == 3) { return; }
            if (Core.Editor.Hover && Core.Editor.EditMode < 9)
            {
                switch (Core.Editor.DrawMode)
                {
                    case 1: ED1_DrawHover(g); break;
                    case 2: ED2_DrawHover(g); break;
                    case 4: ED4_DrawHover(g); break; 
                }
            }
        }

        public void ED1_DrawHover(Graphics g)
        {
            int w = Zoom(32);
            int h = Zoom(32);
            int x = (Core.Editor.HoverX - start_tilex) * w + offset_x;
            int y = (Core.Editor.HoverY - start_tiley) * h + offset_y;
            g.FillRectangle(Brushes.Red, x, y, w, 2);
            g.FillRectangle(Brushes.Red, x, y, 2, h);
            g.FillRectangle(Brushes.Red, x, y + h - 2, w, 2);
            g.FillRectangle(Brushes.Red, x + w - 2, y, 2, h);
        }

        public void ED2_DrawHover(Graphics g)
        {
            if (!Core.Editor.Selected) { return; }
            int w = Zoom(32) * Core.Editor.SelectWidth;
            int h = Zoom(32) * Core.Editor.SelectHeight; ;
            int x = (Core.Editor.HoverX - start_tilex) * Zoom(32) + offset_x;
            int y = (Core.Editor.HoverY - start_tiley) * Zoom(32) + offset_y;
            DrawPointerS(g, x, y, w, h);
        }

        public void ED4_DrawHover(Graphics g)
        {
            if (!Core.Editor.Selected) { return; }
            int w = Zoom(32) * Core.Editor.SelectWidth;
            int h = Zoom(32) * Core.Editor.SelectHeight;
            int x = Core.Editor.HoverMX - offset_x - start_tilex * 32;
            int y = Core.Editor.HoverMY - offset_y - start_tiley * 32;
            DrawPointerS(g, x, y, w, h);
        }

        public void DrawPointerS(Graphics g, int ax, int ay, int aw, int ah)
        {

            g.FillRectangle(Brushes.Black, ax, ay, aw, 4);
            g.FillRectangle(Brushes.Black, ax, ay, 4, ah);
            g.FillRectangle(Brushes.Black, ax, ay + ah - 4, aw, 4);
            g.FillRectangle(Brushes.Black, ax + aw - 4, ay, 4, ah);

            g.FillRectangle(Brushes.White, ax + 1, ay + 1, aw - 2, 2);
            g.FillRectangle(Brushes.White, ax + 1, ay + 1, 2, ah - 2);
            g.FillRectangle(Brushes.White, ax + 1, ay + ah - 3, aw - 2, 2);
            g.FillRectangle(Brushes.White, ax + aw - 3, ay + 1, 2, ah - 2);

        }

        #endregion

        #region DrawSelector

        public void DrawSelector(Graphics g)
        {
            if (Core.Editor.EditMode < 9)
            {
                if (Core.Editor.Selecting && Core.Editor.DrawMode == 2)
                {
                    ED2_DrawSelector(g);
                }
                if (Core.Editor.Selecting && Core.Editor.DrawMode == 3)
                {
                    ED3_DrawSelector(g);
                }
                if (Core.Editor.Selected && Core.Editor.DrawMode == 3)
                {
                    ED3_DrawSelected(g);
                }
            }
            if (Core.Editor.Selecting && Core.Editor.DrawMode == 4)
            {
                ED4_DrawSelector(g);
            }
        }

        public void ED2_DrawSelector(Graphics g)
        {
            int w = Zoom(32) * Core.Editor.SelectWidth;
            int h = Zoom(32) * Core.Editor.SelectHeight;
            int ax = Math.Min(Core.Editor.SelectStartX, Core.Editor.SelectEndX);
            int ay = Math.Min(Core.Editor.SelectStartY, Core.Editor.SelectEndY);
            int x = (ax - start_tilex) * Zoom(32) + offset_x;
            int y = (ay - start_tiley) * Zoom(32) + offset_y;
            DrawPointerR(g, x, y, w, h);
        }

        public void ED3_DrawSelector(Graphics g)
        {
            int w = Zoom(32) * Core.Editor.SelectWidth;
            int h = Zoom(32) * Core.Editor.SelectHeight;
            int x = (Core.Editor.SelectStartX - start_tilex) * Zoom(32) + offset_x;
            int y = (Core.Editor.SelectStartY - start_tiley) * Zoom(32) + offset_y;
            DrawPointerR(g, x, y, w, h);
        }

        public void ED4_DrawSelector(Graphics g)
        {
            int w = Core.Editor.SelectWidth;
            int h = Core.Editor.SelectHeight;
            int x = (Core.Editor.SelectStartX - start_tilex * 32) + offset_x;
            int y = (Core.Editor.SelectStartY - start_tiley * 32) + offset_y;
            DrawPointerSR(g, x, y, w, h);
        }

        public void ED3_DrawSelected(Graphics g)
        {
            int w = Zoom(32) * Core.Editor.SelectWidth;
            int h = Zoom(32) * Core.Editor.SelectHeight;
            int x = (Core.Editor.SelectStartX - start_tilex) * Zoom(32) + offset_x;
            int y = (Core.Editor.SelectStartY - start_tiley) * Zoom(32) + offset_y;
            DrawPointerS(g, x, y, w, h);
        }

        public void DrawPointerR(Graphics g, int ax, int ay, int aw, int ah)
        {

            g.FillRectangle(Brushes.Black, ax, ay, aw, 4);
            g.FillRectangle(Brushes.Black, ax, ay, 4, ah);
            g.FillRectangle(Brushes.Black, ax, ay + ah - 4, aw, 4);
            g.FillRectangle(Brushes.Black, ax + aw - 4, ay, 4, ah);

            g.FillRectangle(Brushes.Red, ax + 1, ay + 1, aw - 2, 2);
            g.FillRectangle(Brushes.Red, ax + 1, ay + 1, 2, ah - 2);
            g.FillRectangle(Brushes.Red, ax + 1, ay + ah - 3, aw - 2, 2);
            g.FillRectangle(Brushes.Red, ax + aw - 3, ay + 1, 2, ah - 2);

        }

        public void DrawPointerSR(Graphics g, int ax, int ay, int aw, int ah)
        {

            g.FillRectangle(Brushes.Red, ax, ay, aw, 1);
            g.FillRectangle(Brushes.Red, ax, ay, 1, ah);
            g.FillRectangle(Brushes.Red, ax, ay + ah - 1, aw, 1);
            g.FillRectangle(Brushes.Red, ax + aw - 1, ay, 1, ah);
        }

        #endregion

        #region Grid/Indic

        public void DrawGrid(Graphics g)
        {
            if (Core.Editor.f.tsbGrid.Checked)
            {
                for (int i = 0; i < numx; i++)
                {
                    for (int j = 0; j < numy; j++)
                    {
                        g.DrawImage(Core.GridCache[Core.Zoom], offset_x + Zoom(i * 32),
                            offset_y + Zoom(j * 32));
                    }
                }
            }
        }

        public void DrawTileIndic(Graphics g)
        {
            if (Core.Editor.f.tsbIndic.Checked && Core.Editor.DrawMode < 4)
            {
                for (int k = 0; k < 6; k++)
                {
                    for (int i = 0; i < numx; i++)
                    {
                        for (int j = 0; j < numy; j++)
                        {
                            if (start_tilex + i >= Core.Map.Width || start_tiley + j >= Core.Map.Height) { continue; }
                            if (k == Core.Editor.EditMode)
                            {
                                if (Tile(start_tilex + i, start_tiley + j, k) > 0)
                                {
                                    g.DrawImage(Core.IndicCache[Core.Zoom], offset_x + Zoom(i * 32),
                                        offset_y + Zoom(j * 32));
                                }
                            }
                        }
                    }
                }
            }
        }

        Font arial = new Font("Arial", 8);


        public void DrawObjectIndic(Graphics g)
        {
            if (Core.Editor.DrawMode == 4)
            {
                bool sel_only = false;
                if (!Core.Editor.f.tsbIndic.Checked) { sel_only = true; }
                if (sel_only && (((EditModeUnl)Core.Editor.CurrentMode).SelectedObjects.Count == 0)) { return; }

                foreach (UObject obj in Core.Map.Objects)
                {
                    if (!CheckObjInRange(obj)) { continue; }

                    bool sel_exist = ((EditModeUnl)Core.Editor.CurrentMode).SelectedObjects.Exists(o => o.ID == obj.ID);

                    if (sel_only)
                    {
                        if (!sel_exist) { continue; }
                    }
                    else
                    {
                        if (!sel_exist && (!ShowObject(obj.SubLayer) || !ObjectOpacity(obj.SubLayer))){ continue; }
                    }

                    int x = obj.X - (start_tilex * 32 - offset_x); int y = obj.Y - (start_tiley * 32 - offset_y);
                    int w = obj.SpanX * 32 - obj.CropLeft - obj.CropRight; int h = obj.SpanY * 32 - obj.CropTop - obj.CropBottom;

                    Brush b = Brushes.White;
                    if (sel_exist)
                    {
                        b = Brushes.Red;
                    }
                    g.FillRectangle(b, x, y, w, 1);
                    g.FillRectangle(b, x, y, 1, h);
                    g.FillRectangle(b, x, y + h - 1, w, 1);
                    g.FillRectangle(b, x + w - 1, y, 1, h);
                    g.DrawString(obj.ID.ToString(), arial, b, new Point(x + 2, y + 2));
                    g.DrawString("S" + (obj.SubLayer + 1).ToString(), arial, b, new Point(x + w - 20, y + h - 13));
                }
            }
        }

        #endregion

        public void RefreshMap()
        {
            this.AutoScrollMinSize = new Size(GetMapW(), GetMapH());
            this.Refresh();
        }

    }
}
