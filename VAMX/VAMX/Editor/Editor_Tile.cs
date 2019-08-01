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

        public void ResetTileCache()
        {
            tilecache.Clear();
            tilecache_o.Clear();
        }

        public Dictionary<int, Bitmap> tilecache = new Dictionary<int, Bitmap>();
        public Dictionary<int, Bitmap> tilecache_o = new Dictionary<int, Bitmap>();

        public int anim = 0;

        public void DrawTile(Graphics g, int id, int x, int y, bool opaque)
        {
            if (id == 0) { return; }
            Bitmap b;
            GetTileBitmap(out b, id, opaque);
            g.DrawImage(b, new Rectangle(x, y, Zoom(32), Zoom(32)), new Rectangle(0, 0, 32, 32), GraphicsUnit.Pixel);
        }

        public void DrawTileEX(Graphics g, int id, Rectangle dest, Rectangle src, bool opaque)
        {
            if (id == 0) { return; }
            Bitmap b;
            GetTileBitmap(out b, id, opaque);
            g.DrawImage(b,dest, src,  GraphicsUnit.Pixel);
        }

        public void GetTileBitmap(out Bitmap b, int id, bool opaque)
        {
            if (opaque)
            {
                if (!tilecache.ContainsKey(id))
                {
                    tilecache[id] = GetTile(id, opaque);
                }
                b = tilecache[id];
            }
            else
            {
                if (!tilecache_o.ContainsKey(id))
                {
                    tilecache_o[id] = GetTile(id, opaque);
                }
                b = tilecache_o[id];
            }
        }

        public int Zoom(int i)
        {
            return Convert.ToInt32(Core.Zoom * i);
        }

        public void TileInfo(int t, out int type, out int index, out int id)
        {
            type = (int)((t & 0xFF000000) >> 24);
            index = (int)((t & 0x00FF0000) >> 16);
            id = ((t & 0x0000FFFF));
        }

        public int TileID(int type, int index, int id)
        {
            return (type << 24) | (index << 16) | id;
        }

        public Bitmap GetTile(int t, bool opaque)
        {
            Bitmap b = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(b))
            {
                int type, index, id;
                TileInfo(t, out type, out index, out id);
                switch (type)
                {
                    case 1: GetTileA1(g, index, id, opaque); break;
                    case 2: GetTileA2(g, index, id, opaque); break;
                    case 3: GetTileA3(g, index, id, opaque); break;
                    case 4: GetTileA4(g, index, id, opaque); break;
                    case 5: GetTileA5(g, index, id, opaque); break;
                    case 6: GetTileB(g, index, id, opaque); break;
                }
            }
            return b;
        }

        private int GetA1PosX(int type)
        {
            if (type < 4) { return 0; }
            if (type < 8) { return 8; }
            if (type < 12) { return 6; }
            return 14;
        }

        public int GetTileType(int t)
        {
            return (int)((t & 0xFF000000) >> 24);
        }

        public int GetTileIndex(int t)
        {
            return ((t & 0x00FF0000) >> 16);
        }

        public int GetTileId(int t)
        {
            return (t & 0x0000FFFF);
        }

        #region GetTile

        public void GetTileA1(Graphics g, int index, int id, bool opaque)
        {
            if (index >= Core.Map.Tilesets[0].Count) { return; }
            Bitmap a;
            if (opaque) { a = ts_pic[Core.Map.Tilesets[0][index]]; }
            else { a = ts_pic_o[Core.Map.Tilesets[0][index]]; }
            int type = id.div(100);
            int shape = id % 100;
            //-----------------------------------------------
            //Water
            if (type < 10)
            {
                if (shape < 0 || shape > 48) { return; };
                for (int i = 0; i < 4; i++)
                {
                    int dx = GetA1PosX(type) * 32;
                    dx += (type < 8) ? anim * 2 * 32 : 0;
                    int dy = type % 4 * 3 * 32;
                    int px = GetAutoTileData(0, shape, i, 0) * 16;
                    int py = GetAutoTileData(0, shape, i, 1) * 16;
                    Rectangle r = new Rectangle(dx + px, dy + py, 16, 16);
                    Bitmap cl = a.Clone(r, a.PixelFormat);
                    g.DrawImage(cl, (i % 2) * 16, i.div(2) * 16);
                }
            }
            //Waterfall
            else
            {
                if (shape < 0 || shape > 3) { return; };
                for (int i = 0; i < 4; i++)
                {
                    int dx = GetA1PosX(type) * 32;
                    int dy = type % 4 * 3 * 32;
                    dy += anim * 32;
                    int px = GetAutoTileData(2, shape, i, 0) * 16;
                    int py = GetAutoTileData(2, shape, i, 1) * 16;
                    Rectangle r = new Rectangle(dx + px, dy + py, 16, 16);
                    Bitmap cl = a.Clone(r, a.PixelFormat);
                    g.DrawImage(cl, (i % 2) * 16, i.div(2) * 16);
                }
            }
        }

        public void GetTileA2(Graphics g, int index, int id, bool opaque)
        {
            if (index >= Core.Map.Tilesets[1].Count) { return; }
            Bitmap a;
            if (opaque) { a = ts_pic[Core.Map.Tilesets[1][index]]; }
            else { a = ts_pic_o[Core.Map.Tilesets[1][index]]; }
            int type = id.div(100);
            int shape = id % 100;
            if (shape < 0 || shape > 48) { return; };
            //-----------------------------------------------
            for (int i = 0; i < 4; i++)
            {
                int dx = (type % 8) * 2 * 32;
                int dy = type.div(8) * 3 * 32;
                int px = GetAutoTileData(0, shape, i, 0) * 16;
                int py = GetAutoTileData(0, shape, i, 1) * 16;
                Rectangle r = new Rectangle(dx + px, dy + py, 16, 16);
                Bitmap cl = a.Clone(r, a.PixelFormat);
                g.DrawImage(cl, (i % 2) * 16, i.div(2) * 16);
            }
        }

        public void GetTileA3(Graphics g, int index, int id, bool opaque)
        {
            if (index >= Core.Map.Tilesets[2].Count) { return; }
            Bitmap a;
            if (opaque) { a = ts_pic[Core.Map.Tilesets[2][index]]; }
            else { a = ts_pic_o[Core.Map.Tilesets[2][index]]; }
            int type = id.div(100);
            int shape = id % 100;
            if (shape < 0 || shape > 15) { return; }
            for (int i = 0; i < 4; i++)
            {
                int dx = (type % 8) * 2 * 32;
                int dy = type.div(8) * 2 * 32;
                int px = GetAutoTileData(1, shape, i, 0) * 16;
                int py = GetAutoTileData(1, shape, i, 1) * 16;
                Rectangle r = new Rectangle(dx + px, dy + py, 16, 16);
                Bitmap cl = a.Clone(r, a.PixelFormat);
                g.DrawImage(cl, (i % 2) * 16, i.div(2) * 16);
            }
        }

        public void GetTileA4(Graphics g, int index, int id, bool opaque)
        {
            if (index >= Core.Map.Tilesets[3].Count) { return; }
            Bitmap a;
            if (opaque) { a = ts_pic[Core.Map.Tilesets[3][index]]; }
            else { a = ts_pic_o[Core.Map.Tilesets[3][index]]; }
            int type = id.div(100);
            int shape = id % 100;
            int mode = type.div(8) % 2;
            if (mode == 0)
            {
                if (shape < 0 || shape > 48) { return; };
                for (int i = 0; i < 4; i++)
                {
                    int dx = (type % 8) * 2 * 32;
                    int dy = (type.div(8) * 3 - (type.div(8) / 2)) * 32;
                    int px = GetAutoTileData(0, shape, i, 0) * 16;
                    int py = GetAutoTileData(0, shape, i, 1) * 16;
                    Rectangle r = new Rectangle(dx + px, dy + py, 16, 16);
                    Bitmap cl = a.Clone(r, a.PixelFormat);
                    g.DrawImage(cl, (i % 2) * 16, i.div(2) * 16);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (shape < 0 || shape > 15) { return; };
                    int dx = (type % 8) * 2 * 32;
                    int dy = (type.div(8) + (type.div(8) * 2) - ((type.div(8) - 1) / 2)) * 32;
                    int px = GetAutoTileData(1, shape, i, 0) * 16;
                    int py = GetAutoTileData(1, shape, i, 1) * 16;
                    Rectangle r = new Rectangle(dx + px, dy + py, 16, 16);
                    Bitmap cl = a.Clone(r, a.PixelFormat);
                    g.DrawImage(cl, (i % 2) * 16, i.div(2) * 16);
                }
            }
        }

        public void GetTileA5(Graphics g, int index, int id, bool opaque)
        {
            if (index >= Core.Map.Tilesets[4].Count) { return; }
            Bitmap a;
            if (opaque) { a = ts_pic[Core.Map.Tilesets[4][index]]; }
            else { a = ts_pic_o[Core.Map.Tilesets[4][index]]; }
            int i = id % 8;
            int j = id.div(8);
            Rectangle r = new Rectangle(i * 32, j * 32, 32, 32);
            Bitmap cl = a.Clone(r, a.PixelFormat);
            g.DrawImage(cl, 0, 0);
        }

        public void GetTileB(Graphics g, int index, int id, bool opaque)
        {
            if (index >= Core.Map.Tilesets[5].Count) { return; }
            Bitmap a;
            if (opaque) { a = ts_pic[Core.Map.Tilesets[5][index]]; }
            else { a = ts_pic_o[Core.Map.Tilesets[5][index]]; }
            int i = id < 128 ? id % 8 : 8 + id % 8;
            int j = id >= 128 ? (id - 128).div(8) : id.div(8);
            Rectangle r = new Rectangle(i * 32, j * 32, 32, 32);
            Bitmap cl = a.Clone(r, a.PixelFormat);
            g.DrawImage(cl, 0, 0);
        }


        #endregion

        #region GetFlagTile

        public void DrawFlagTile(Graphics g, int type, int id, int x, int y)
        {
            Bitmap b = Core.FlagImages[type];
            if (id < 1 || id > (b.Height / 32) * 8) { return; }
            id -= 1;
            g.DrawImage(b, new Rectangle(x, y, Zoom(32), Zoom(32)),
                new Rectangle((id % 8) * 32, id.div(8) * 32, 32, 32), GraphicsUnit.Pixel);
        }

        #endregion

        //Autotiles
        #region AutoTileData
        int[][][] FLOOR_AUTOTILE_TABLE = { 
        //1
       new int[][] { new int[]{2,4}, new int[]{1,4},new int[]{2,3},new int[]{1,3} },
       new int[][] { new int[]{2,0}, new int[]{1,4},new int[]{2,3},new int[]{1,3} },
       new int[][] { new int[]{2,4}, new int[]{3,0},new int[]{2,3},new int[]{1,3} },
       new int[][] { new int[]{2,0}, new int[]{3,0},new int[]{2,3},new int[]{1,3} },
        //2
       new int[][] { new int[]{2,4}, new int[]{1,4},new int[]{2,3},new int[]{3,1} },
       new int[][] { new int[]{2,0}, new int[]{1,4},new int[]{2,3},new int[]{3,1} },
       new int[][] { new int[]{2,4}, new int[]{3,0},new int[]{2,3},new int[]{3,1} },
       new int[][] { new int[]{2,0}, new int[]{3,0},new int[]{2,3},new int[]{3,1} }, 
       //3
       new int[][] { new int[]{2,4}, new int[]{1,4},new int[]{2,1},new int[]{1,3} },
       new int[][] { new int[]{2,0}, new int[]{1,4},new int[]{2,1},new int[]{1,3} },
       new int[][] { new int[]{2,4}, new int[]{3,0},new int[]{2,1},new int[]{1,3} },
       new int[][] { new int[]{2,0}, new int[]{3,0},new int[]{2,1},new int[]{1,3} }, 
       //4
       new int[][] { new int[]{2,4}, new int[]{1,4},new int[]{2,1},new int[]{3,1} },
       new int[][] { new int[]{2,0}, new int[]{1,4},new int[]{2,1},new int[]{3,1} },
       new int[][] { new int[]{2,4}, new int[]{3,0},new int[]{2,1},new int[]{3,1} },
       new int[][] { new int[]{2,0}, new int[]{3,0},new int[]{2,1},new int[]{3,1} }, 
       //5
       new int[][] { new int[]{0,4}, new int[]{1,4},new int[]{0,3},new int[]{1,3} },
       new int[][] { new int[]{0,4}, new int[]{3,0},new int[]{0,3},new int[]{1,3} },
       new int[][] { new int[]{0,4}, new int[]{1,4},new int[]{0,3},new int[]{3,1} },
       new int[][] { new int[]{0,4}, new int[]{3,0},new int[]{0,3},new int[]{3,1} },
        //6
       new int[][] { new int[]{2,2}, new int[]{1,2},new int[]{2,3},new int[]{1,3} },
       new int[][] { new int[]{2,2}, new int[]{1,2},new int[]{2,3},new int[]{3,1} },
       new int[][] { new int[]{2,2}, new int[]{1,2},new int[]{2,1},new int[]{1,3} },
       new int[][] { new int[]{2,2}, new int[]{1,2},new int[]{2,1},new int[]{3,1} }, 
       //7
       new int[][] { new int[]{2,4}, new int[]{3,4},new int[]{2,3},new int[]{3,3} },
       new int[][] { new int[]{2,4}, new int[]{3,4},new int[]{2,1},new int[]{3,3} },
       new int[][] { new int[]{2,0}, new int[]{3,4},new int[]{2,3},new int[]{3,3} },
       new int[][] { new int[]{2,0}, new int[]{3,4},new int[]{2,1},new int[]{3,3} },
        //8
       new int[][] { new int[]{2,4}, new int[]{1,4},new int[]{2,5},new int[]{1,5} },
       new int[][] { new int[]{2,0}, new int[]{1,4},new int[]{2,5},new int[]{1,5} },
       new int[][] { new int[]{2,4}, new int[]{3,0},new int[]{2,5},new int[]{1,5} },
       new int[][] { new int[]{2,0}, new int[]{3,0},new int[]{2,5},new int[]{1,5} }, 
       //9
       new int[][] { new int[]{0,4}, new int[]{3,4},new int[]{0,3},new int[]{3,3} },
       new int[][] { new int[]{2,2}, new int[]{1,2},new int[]{2,5},new int[]{1,5} },
       new int[][] { new int[]{0,2}, new int[]{1,2},new int[]{0,3},new int[]{1,3} },
       new int[][] { new int[]{0,2}, new int[]{1,2},new int[]{0,3},new int[]{3,1} }, 
       //10
       new int[][] { new int[]{2,2}, new int[]{3,2},new int[]{2,3},new int[]{3,3} },
       new int[][] { new int[]{2,2}, new int[]{3,2},new int[]{2,1},new int[]{3,3} },
       new int[][] { new int[]{2,4}, new int[]{3,4},new int[]{2,5},new int[]{3,5} },
       new int[][] { new int[]{2,0}, new int[]{3,4},new int[]{2,5},new int[]{3,5} }, 
       //11
       new int[][] { new int[]{0,4}, new int[]{1,4},new int[]{0,5},new int[]{1,5} },
       new int[][] { new int[]{0,4}, new int[]{3,0},new int[]{0,5},new int[]{1,5} },
       new int[][] { new int[]{0,2}, new int[]{3,2},new int[]{0,3},new int[]{3,3} },
       new int[][] { new int[]{0,2}, new int[]{1,2},new int[]{0,5},new int[]{1,5} },
       //12
       new int[][] { new int[]{0,4}, new int[]{3,4},new int[]{0,5},new int[]{3,5} },
       new int[][] { new int[]{2,2}, new int[]{3,2},new int[]{2,5},new int[]{3,5} },
       new int[][] { new int[]{0,2}, new int[]{3,2},new int[]{0,5},new int[]{3,5} },
       new int[][] { new int[]{0,0}, new int[]{1,0},new int[]{0,1},new int[]{1,1} },
       };

        int[][][] WATERFALL_AUTOTILE_TABLE = {
       new int[][] { new int[]{2,0}, new int[]{1,0},new int[]{2,1},new int[]{1,1} },
       new int[][] { new int[]{0,0}, new int[]{1,0},new int[]{0,1},new int[]{1,1} },
       new int[][] { new int[]{2,0}, new int[]{3,0},new int[]{2,1},new int[]{3,1} },
       new int[][] { new int[]{0,0}, new int[]{3,0},new int[]{0,1},new int[]{3,1} },
       };

        int[][][] WALL_AUTOTILE_TABLE = {
       new int[][] { new int[]{2,2}, new int[]{1,2},new int[]{2,1},new int[]{1,1} },
       new int[][] { new int[]{0,2}, new int[]{1,2},new int[]{0,1},new int[]{1,1} },
       new int[][] { new int[]{2,0}, new int[]{1,0},new int[]{2,1},new int[]{1,1} },
       new int[][] { new int[]{0,0}, new int[]{1,0},new int[]{0,1},new int[]{1,1} },

       new int[][] { new int[]{2,2}, new int[]{3,2},new int[]{2,1},new int[]{3,1} },
       new int[][] { new int[]{0,2}, new int[]{3,2},new int[]{0,1},new int[]{3,1} },
       new int[][] { new int[]{2,0}, new int[]{3,0},new int[]{2,1},new int[]{3,1} },
       new int[][] { new int[]{0,0}, new int[]{3,0},new int[]{0,1},new int[]{3,1} },

       new int[][] { new int[]{2,2}, new int[]{1,2},new int[]{2,3},new int[]{1,3} },
       new int[][] { new int[]{0,2}, new int[]{1,2},new int[]{0,3},new int[]{1,3} },
       new int[][] { new int[]{2,0}, new int[]{1,0},new int[]{2,3},new int[]{1,3} },
       new int[][] { new int[]{0,0}, new int[]{1,0},new int[]{0,3},new int[]{1,3} },

       new int[][] { new int[]{2,2}, new int[]{3,2},new int[]{2,3},new int[]{3,3} },
       new int[][] { new int[]{0,2}, new int[]{3,2},new int[]{0,3},new int[]{3,3} },
       new int[][] { new int[]{2,0}, new int[]{3,0},new int[]{2,3},new int[]{3,3} },
       new int[][] { new int[]{0,0}, new int[]{3,0},new int[]{0,3},new int[]{3,3} },
       };

        private int GetAutoTileData(int type, int shape, int x, int y)
        {
            try
            {
                switch (type)
                {
                    case 0:
                        if (shape > 48) { return 0; }
                        return FLOOR_AUTOTILE_TABLE[shape][x][y];
                    case 1:
                        if (shape > 16) { return 0; }
                        return WALL_AUTOTILE_TABLE[shape][x][y];
                    case 2:
                        if (shape > 4) { return 0; }
                        return WATERFALL_AUTOTILE_TABLE[shape][x][y];
                }
            }
            catch (Exception) { return 0; }
            return 0;
        }
        #endregion

    }
}















