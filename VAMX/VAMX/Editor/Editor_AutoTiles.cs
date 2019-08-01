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
        #region Autotile Mapping Prepare

        int[] waterfall_order = { 0, 1, 2, 3 };
        int[] wall_order = { 15, 3, 2, 6, 7, 1, 0, 4, 5, 9, 8, 12, 13, 11, 10, 14 };
        int[] ground_order = { 34, 20, 36, 35, 23, 37, 17, 26, 16, 0, 24, 19,  46, 27, 18, 25,
                                            40, 28, 38, 41, 31, 39, 22, 21, 11,  3,  7, 42,  1,  2, 29, 30,
                                             9, 15,  6, 32,  8,  4,  5, 10, 13, 12, 14, 44, 43, 33, 45 ,47};

        public int[] WaterfallOrder { get { return waterfall_order; } }
        public int[] WallOrder { get { return wall_order; } }
        public int[] GroundOrder { get { return ground_order; } }

        public int WallOrderReverse(int id)
        {
            for (int i = 0; i < 16; i++)
            {
                if (wall_order[i] == id)
                {
                    return i;
                }
            }
            return 0;
        }

        public int GroundOrderReverse(int id)
        {
            for (int i = 0; i < 48; i++)
            {
                if (ground_order[i] == id)
                {
                    return i;
                }
            }
            return 0;
        }

        //-------------------------------------------------------------
        int[] tilecode = { 1, 2, 4, 8, 0, 16, 32, 64, 128 };
        int[] tiledata = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private void tadjadjust()
        {
            if (tiledata[0] == 1 && tiledata[3] == 0 || tiledata[1] == 0) { tiledata[0] = 0; }
            if (tiledata[6] == 1 && tiledata[3] == 0 || tiledata[7] == 0) { tiledata[6] = 0; }
            if (tiledata[2] == 1 && tiledata[1] == 0 || tiledata[5] == 0) { tiledata[2] = 0; }
            if (tiledata[8] == 1 && tiledata[5] == 0 || tiledata[7] == 0) { tiledata[8] = 0; }
        }

        private int BitToFloor(int bit)
        {
            switch (bit)
            {
                //1st row
                case 255:
                    return 0;
                case 254:
                    return 1;
                case 223:
                    return 2;
                case 222:
                    return 3;
                case 127:
                    return 4;
                case 126:
                    return 5;
                case 95:
                    return 6;
                case 94:
                    return 7;
                case 251:
                    return 8;
                case 250:
                    return 9;
                case 219:
                    return 10;
                case 218:
                    return 11;
                case 123:
                    return 12;
                case 122:
                    return 13;
                case 91:
                    return 14;
                case 90:
                    return 15;
                //2nd row
                case 248:
                    return 16;
                case 216:
                    return 17;
                case 120:
                    return 18;
                case 88:
                    return 19;
                case 214:
                    return 20;
                case 86:
                    return 21;
                case 210:
                    return 22;
                case 82:
                    return 23;
                case 31:
                    return 24;
                case 27:
                    return 25;
                case 30:
                    return 26;
                case 26:
                    return 27;
                case 107:
                    return 28;
                case 106:
                    return 29;
                case 75:
                    return 30;
                case 74:
                    return 31;
                //3rd row
                case 24:
                    return 32;
                case 66:
                    return 33;
                case 208:
                    return 34;
                case 80:
                    return 35;
                case 22:
                    return 36;
                case 18:
                    return 37;
                case 11:
                    return 38;
                case 10:
                    return 39;
                case 104:
                    return 40;
                case 72:
                    return 41;
                case 16:
                    return 42;
                case 64:
                    return 43;
                case 8:
                    return 44;
                case 2:
                    return 45;
                case 0:
                    return 46;
            }
            return 0;
        }

        private int BitToWaterfall(int bit)
        {
            switch (bit)
            {
                case 0:
                    return 3;
                case 2:
                    return 2;
                case 64:
                    return 1;
                case 66:
                    return 0;
            }
            return 0;
        }

        private int BitToWall(int bit)
        {
            switch (bit)
            {
                case 90: return 0;
                case 74: return 1;
                case 26: return 2;
                case 10: return 3;
                case 82: return 4;
                case 66: return 5;
                case 18: return 6;
                case 2: return 7;
                case 88: return 8;
                case 72: return 9;
                case 24: return 10;
                case 8: return 11;
                case 80: return 12;
                case 64: return 13;
                case 16: return 14;
                case 0: return 15;
            }
            return 0;
        }

        #endregion

        #region Autotile Mapping

        public void MapAutoTile(int x, int y, int t)
        {
            int type = (int)((t & 0xFF000000) >> 24);
            int index = (int)((t & 0x00FF0000) >> 16);
            int id = (int)(t & 0x0000FFFF);
            int kind = Core.div(id, 100);
            int shape = id % 100;
            if (type == 0 || type > 4) { return; }
            if (type == 1 && kind >= 10)
            {
                MapWaterfallTile(x, y, t);
            }
            else if (type == 3)
            {
                MapWallTile(x, y, t);
            }
            else if (type == 4 && (Core.div(kind, 8) % 2) == 1)
            {
                MapA4WallTile(x, y, t);
            }
            else
            {
                MapGroundTile(x, y, t);
            }
        }

        public void MapWaterfallTile(int x, int y, int t)
        {
            Core.Map.Layers[x, y, edit_mode] = t;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    ProcessWaterfallTiles(x + i, y + j, t);
                }
            }
            return;
        }

        public void ProcessWaterfallTiles(int x, int y, int tid)
        {
            if (!InMap(x, y)) { return; }
            if ((Core.Map.Layers[x, y, edit_mode] < tid ||
                Core.Map.Layers[x, y, edit_mode] > tid + 3))
            { return; }

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    tiledata[3 * (i + 1) + j + 1] = GetSideValue(x, y, i, j, tid, 2); //waterfall offset 0
                }
            }

            int bit = 0;
            for (int i = -1; i < 2; i++)
            {
                bit = bit | (tiledata[3 * (i + 1) + 1] * tilecode[3 * (i + 1) + 1]);
            }
            tid = tid + BitToWaterfall(bit);
            Core.Map.Layers[x, y, edit_mode] = tid;
        }

        private int GetSideValue(int x, int y, int i, int j, int tid, int type)
        {
            x = x + i;
            y = y + j;
            int a = 0;
            if (!InMap(x, y)) { return 1; }
            if (type == 0)
            {
                if (Core.Map.Layers[x, y, edit_mode] >= tid && Core.Map.Layers[x, y, edit_mode] < tid + 48) { return 1; }
                else { return 0; }
            }
            else if (type == 1) { }
            else
            {
                // return mdata1[y * mw + x];
                if (Core.Map.Layers[x, y, edit_mode] >= tid && Core.Map.Layers[x, y, edit_mode] < tid + 4) { return 1; }
                else { return 0; }

            }
            return a;
        }

        public void MapGroundTile(int x, int y, int t)
        {
            Core.Map.Layers[x, y, edit_mode] = t;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    ProcessGroundTiles(x + i, y + j, t);
                }
            }
            return;
        }

        public void ProcessGroundTiles(int x, int y, int tid)
        {
            if (!InMap(x, y)) { return; }
            if ((Core.Map.Layers[x, y, edit_mode] < tid ||
                Core.Map.Layers[x, y, edit_mode] > tid + 47)) { return; }
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    tiledata[3 * (i + 1) + j + 1] = GetSideValue(x, y, i, j, tid, 0); //floor = 0
                }
            }
            int bit = 0;
            tadjadjust();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    bit = bit | (tiledata[3 * (i + 1) + j + 1] * tilecode[(3 * (i + 1)) + j + 1]);
                }
            }
            tid = tid + BitToFloor(bit);
            Core.Map.Layers[x, y, edit_mode] = tid;
        }

        #endregion

        #region Wall Tiles

        public void MapWallTile(int x, int y, int t)
        {
            Core.Map.Layers[x, y, edit_mode] = t;
            List<int> coords = GetRelatedCoords(x, y, t);
            foreach (int i in coords)
            {
                int xx = (int)((i & 0xFFFF0000) >> 16);
                int yy = i & 0x0000FFFF;
                UpdateWallShape(xx, yy, t);
            }
        }

        public List<int> GetRelatedCoords(int x, int y, int t)
        {
            List<int> coords = new List<int>();
            GetExpandCoords(x, y, t, ref coords);
            return coords;
        }

        public void GetExpandCoords(int x, int y, int t, ref List<int> coords)
        {
            if (coords.Contains((x << 16) | y)) { return; }
            if (!SameTile(x, y, t)) { return; }
            coords.Add((x << 16) | y);
            GetExpandCoords(x - 1, y, t, ref coords);
            GetExpandCoords(x + 1, y, t, ref coords);
            GetExpandCoords(x, y - 1, t, ref coords);
            GetExpandCoords(x, y + 1, t, ref coords);
        }

        public bool SameTile(int x, int y, int t)
        {
            if (!InMap(x, y)) { return false; }
            int i = Core.Map.Layers[x, y, edit_mode];
            i = ((int)(i & 0xFFFF0000)) + (Core.div((i & 0x0000FFFF), 100) * 100);
            int j = ((int)(t & 0xFFFF0000)) + (Core.div((t & 0x0000FFFF), 100) * 100);
            return (i == j);
        }

        public bool IsNeighbour(int x, int y, int t)
        {
            if (!InMap(x, y)) { return true; }
            int i = Core.Map.Layers[x, y, edit_mode];
            i = ((int)(i & 0xFFFF0000)) + (Core.div((i & 0x0000FFFF), 100) * 100);
            int j = ((int)(t & 0xFFFF0000)) + (Core.div((t & 0x0000FFFF), 100) * 100);
            return (i == j);
        }

        public bool InMap(int x, int y)
        {
            if (x < 0 || x >= Core.Map.Width) { return false; }
            if (y < 0 || y >= Core.Map.Height) { return false; }
            return true;
        }

        public int BaseTile(int i)
        {
            return ((int)(i & 0xFFFF0000)) + (Core.div((i & 0x0000FFFF), 100) * 100);
        }

        public void UpdateWallShape(int x, int y, int t)
        {
            if (!InMap(x, y)) { return; }
            int tile = BaseTile(Core.Map.Layers[x, y, edit_mode]);
            int surround = GetSurround(x, y, t);
            surround &= 90;
            int shape = BitToWall(surround);
            Core.Map.Layers[x, y, edit_mode] = t + shape;
        }

        public int GetSurround(int x, int y, int t)
        {
            int result = 0;
            //Up
            if (SameTile(x, y - 1, t))
            {
                result |= 64;
            }
            //Left, Right
            int top_y = GetTopY(x, y, t);
            if (IsNeighbour(x - 1, top_y, t))
            {
                result |= 16;
            }
            if (IsNeighbour(x + 1, top_y, t))
            {
                result |= 8;
            }
            //Down
            if (InMap(x, y + 1))
            {
                if (SameTile(x, y + 1, t))
                {
                    result |= 2;
                }
                else
                {
                    int bottom_y = y;
                    int _x = x - 1;
                    while (SameTile(_x, y, t) && GetTopY(_x, y, t) == top_y)
                    {
                        int _bottom_y = GetBtmY(_x, y, t);
                        if (_bottom_y > bottom_y) { bottom_y = _bottom_y; }
                        _x -= 1;
                    }
                    _x = x + 1;
                    while (SameTile(_x, y, t) && GetTopY(_x, y, t) == top_y)
                    {
                        int _bottom_y = GetBtmY(_x, y, t);
                        if (_bottom_y > bottom_y) { bottom_y = _bottom_y; }
                        _x += 1;
                    }
                    if (bottom_y > y)
                    {
                        result |= 2;
                    }
                }
            }
            return result;
        }

        public int GetTopY(int x, int y, int t)
        {
            int top_y = y;
            while (SameTile(x, top_y - 1, t))
            {
                top_y -= 1;
            }
            return top_y;
        }

        public int GetBtmY(int x, int y, int t)
        {
            int btm_y = y;
            while (SameTile(x, btm_y + 1, t))
            {
                btm_y += 1;
            }
            return btm_y;
        }

        #endregion

        List<int> fixedauto = new List<int>();

        public void FixAutoTile(int x, int y, int t)
        {
            fixedauto.Clear(); ;
            FixAuto(x, y, t);
        }

        public void FixAuto(int x, int y, int t)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Math.Abs(i) + Math.Abs(j) != 1) { continue; }
                    int ax = x + i; int ay = y + j;
                    if (!InMap(ax, ay)) { continue; }
                    if (!SameTile(ax, ay, t)) { continue; }
                    if (fixedauto.Contains(ax * 1000 + ay)) { continue; }
                    MapAutoTile(ax, ay, t);
                    fixedauto.Add(ax * 1000 + ay);
                    FixAuto(ax, ay, t);
                }
            }
        }

        public void MapA4WallTile(int x, int y, int t)
        {
            Core.Map.Layers[x, y, edit_mode] = t;
            List<int> coords = GetRelatedCoords(x, y, t);
            foreach (int i in coords)
            {
                int xx = (int)((i & 0xFFFF0000) >> 16);
                int yy = i & 0x0000FFFF;
                UpdateWallShapeA4(xx, yy, t);
            }
        }

        public void UpdateWallShapeA4(int x, int y, int t)
        {
            if (!InMap(x, y)) { return; }
            int tile = BaseTile(Core.Map.Layers[x, y, edit_mode]);
            int surround = GetSurroundA4(x, y, t);
            surround &= 90;
            int shape = BitToWall(surround);
            Core.Map.Layers[x, y, edit_mode] = t + shape;
        }

        public int GetSurroundA4(int x, int y, int t)
        {
            int result = 0;
            //Up
            if (SameTile(x, y - 1, t))
            {
                //result |= 64;
                result |= 64;
            }
            //Left, Right
            int top_y = GetTopY(x, y, t);
            int btm_y = GetBtmY(x, y, t) + 1;
            int btm_left_x = GetLeftX(x, btm_y, t);
            int btm_right_x = GetRightX(x, btm_y, t);
            if (IsNeighbour(btm_right_x + 1, btm_y, t))
            {
                result |= 8;
            }
            if (IsNeighbour(btm_left_x - 1, btm_y, t))
            {
                result |= 16;
            }
            if (IsNeighbour(x - 1, btm_y - 1, t) && IsNeighbour(x + 1, btm_y - 1, t))
            {
                result |= 8;
                result |= 16;
            }
            if (!IsNeighbour(btm_right_x + 1, btm_y, t) && !IsNeighbour(btm_left_x - 1, btm_y, t))
            {
                if (!(!IsNeighbour(x + 1, y, t) && !IsNeighbour(x - 1, y, t)))
                {
                    if (!IsNeighbour(x - 1, btm_y - 1, t))
                    {
                        result |= 8;
                    }
                    if (!IsNeighbour(x + 1, btm_y - 1, t))
                    {
                        result |= 16;
                    }
                }
            }
            //Down
            if (InMap(x, y + 1))
            {
                if (SameTile(x, y + 1, t))
                {
                    result |= 2;
                }
                else
                {
                    int bottom_y = y;
                    int _x = x - 1;
                    while (SameTile(_x, y, t) && GetTopY(_x, y, t) == top_y)
                    {
                        int _bottom_y = GetBtmY(_x, y, t);
                        if (_bottom_y > bottom_y) { bottom_y = _bottom_y; }
                        _x -= 1;
                    }
                    _x = x + 1;
                    while (SameTile(_x, y, t) && GetTopY(_x, y, t) == top_y)
                    {
                        int _bottom_y = GetBtmY(_x, y, t);
                        if (_bottom_y > bottom_y) { bottom_y = _bottom_y; }
                        _x += 1;
                    }
                    if (bottom_y > y)
                    {
                        result |= 2;
                    }
                }
            }
            return result;
        }

        public bool IsA4Tile(int x, int y, int t)
        {
            if (!InMap(x, y)) { return true; }
            int i = Core.Map.Layers[x, y, edit_mode];
            return (GetTileType(i) == 3);
        }

        public bool TileZero(int x, int y, int t)
        {
            if (!InMap(x, y)) { return false; }
            int i = Core.Map.Layers[x, y, edit_mode];
            int j = Core.Map.Layers[x, y - 1, edit_mode];
            return i == 0 && IsNeighbour(x, y - 1, t);
        }

        public int GetLeftX(int x, int y, int t)
        {
            int left_x = x;
            while (TileZero(left_x - 1, y, t))
            {
                left_x -= 1;
            }
            return left_x;
        }

        public int GetRightX(int x, int y, int t)
        {
            int right_x = x;
            while (TileZero(right_x + 1, y, t))
            {
                right_x += 1;
            }
            return right_x;
        }

    }
}
