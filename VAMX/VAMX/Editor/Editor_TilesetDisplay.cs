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
        List<Bitmap> tileset_display = new List<Bitmap>();
        List<Bitmap> tileset_sel = new List<Bitmap>();

        public List<Bitmap> TilesetDisplay { get { return tileset_display; } }
        public List<Bitmap> TilesetSelect { get { return tileset_sel; } }

        public void PrepareTileset()
        {
            tileset_display.Clear();
            tileset_sel.Clear();
            PrepareDisplayTileset();
            PrepareSelTileset();
        }

        public void PrepareDisplayTileset()
        {
            //A1~B
            for (int i = 0; i < 4; i++) { tileset_display.Add(PrepareTilesetImage(i)); }
            //Flags
            for (int i = 0; i < 3; i++) { tileset_display.Add(Core.FlagImages[i]); }
        }

        public void PrepareSelTileset()
        {
            foreach (Bitmap b in tileset_display)
            {
                tileset_sel.Add(new Bitmap(b.Width, b.Height));
            }
        }

        public Bitmap PrepareTilesetImage(int i)
        {
            switch (i)
            {
                case 0: return PrepareTilesetA12();
                case 1: return PrepareTilesetA34();
                case 2: return PrepareTilesetA5();
                case 3: return PrepareTilesetB();
                default: return new Bitmap(1, 1);
            }
        }

        int tw = 32; int hw = 16;

        public Bitmap PrepareTilesetA12()
        {
            int count = Core.Map.Tilesets[0].Count;
            int count1 = Core.Map.Tilesets[1].Count;
            Bitmap b = Core.CreateBack32(8, 2 * count + 4 * count1);
            //a1
            int index = 0;
            foreach (string str in Core.Map.Tilesets[0])
            {
                Bitmap a = ts_pic[str];
                a = Core.ResizeImage(a, 16 * tw, 12 * tw);
                using (Graphics graph = Graphics.FromImage(b))
                {
                    //row 1
                    for (int x = 0; x < 8; x++)
                    {
                        int k = x.div(2);
                        int i = (x < 4) ? 0 : 1;
                        int j = x % 4;
                        Rectangle r = new Rectangle(8 * i * tw, 3 * j * tw, tw, tw);
                        Bitmap cl = a.Clone(r, a.PixelFormat);
                        graph.DrawImage(cl, x * tw, 2 * tw * index);
                    }
                    //row 2
                    for (int x = 0; x < 8; x++)
                    {
                        int k = x.div(2);
                        int i = (x < 4) ? 0 : 1;
                        int j = x % 4;
                        //surface
                        if (x < 2 || x == 7)
                        {
                            Rectangle r = new Rectangle(6 * tw + (8 * i) * tw, 3 * j * tw, tw, tw);
                            Bitmap cl = a.Clone(r, a.PixelFormat);
                            graph.DrawImage(cl, x * tw, tw + 2 * tw * index);
                        }
                        //waterfall
                        else
                        {
                            Rectangle r = new Rectangle(6 * tw + (8 * i) * tw, 3 * j * tw, hw, tw);
                            Bitmap cl = a.Clone(r, a.PixelFormat);
                            graph.DrawImage(cl, x * tw, tw + 2 * tw * index);
                            r = new Rectangle(3 * hw + 6 * tw + (8 * i) * tw, 3 * j * tw, hw, tw);
                            cl = a.Clone(r, a.PixelFormat);
                            graph.DrawImage(cl, hw + x * tw, tw + 2 * tw * index);
                        }
                    }
                }
                index += 1;
            }
            int ah = index * 2 * tw;
            index = 0;
            //a2
            foreach (string str in Core.Map.Tilesets[1])
            {
                Bitmap a = ts_pic[str];
                a = Core.ResizeImage(a, 16 * tw, 12 * tw);
                using (Graphics graph = Graphics.FromImage(b))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            Rectangle r = new Rectangle(2 * i * tw, 3 * j * tw, tw, tw);
                            Bitmap cl = a.Clone(r, a.PixelFormat);
                            graph.DrawImage(cl, i * tw, j * tw + (4 * tw * index) + ah);
                        }
                    }
                }
                index += 1;
            }
            //finish
            return b;
        }

        public Bitmap PrepareTilesetA34()
        {
            int count = Core.Map.Tilesets[2].Count;
            int count1 = Core.Map.Tilesets[3].Count;
            Bitmap b = Core.CreateBack32(8, 4 * count + 6 * count1);
            int index = 0;
            //a3
            foreach (string str in Core.Map.Tilesets[2])
            {
                Bitmap a = ts_pic[str];
                a = Core.ResizeImage(a, 16 * tw, 8 * tw);
                using (Graphics g = Graphics.FromImage(b))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            //left top
                            Rectangle r = new Rectangle(2 * i * tw, 2 * j * tw, hw, hw);
                            Bitmap cl = a.Clone(r, a.PixelFormat);
                            g.DrawImage(cl, i * tw, j * tw + (4 * tw * index));
                            //right top
                            r = new Rectangle(3 * hw + 2 * i * tw, 2 * j * tw, hw, hw);
                            cl = a.Clone(r, a.PixelFormat);
                            g.DrawImage(cl, hw + i * tw, j * tw + (4 * tw * index));
                            //left bottom
                            r = new Rectangle(2 * i * tw, 3 * hw + 2 * j * tw, hw, hw);
                            cl = a.Clone(r, a.PixelFormat);
                            g.DrawImage(cl, i * tw, hw + j * tw + (4 * tw * index));
                            //right bottom
                            r = new Rectangle(3 * hw + 2 * i * tw, 3 * hw + 2 * j * tw, hw, hw);
                            cl = a.Clone(r, a.PixelFormat);
                            g.DrawImage(cl, hw + i * tw, hw + j * tw + (4 * tw * index));
                        }
                    }
                }
                index += 1;
            }
            int ah = index * 4 * tw;
            index = 0;
            //a4
            foreach (string str in Core.Map.Tilesets[3])
            {
                Bitmap a = ts_pic[str];
                a = Core.ResizeImage(a, 16 * tw, 15 * tw);
                using (Graphics g = Graphics.FromImage(b))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            int k = j.div(2);//VADB.div(j, 2);
                            if (j % 2 == 0)
                            {

                                //ground
                                Rectangle r = new Rectangle(2 * i * tw, 5 * k * tw, tw, tw);
                                Bitmap cl = a.Clone(r, a.PixelFormat);
                                g.DrawImage(cl, i * tw, j * tw + (6 * tw * index) + ah);
                            }
                            else
                            {
                                //wall
                                int pad = (6 * tw * index) + ah;
                                //left top
                                Rectangle r = new Rectangle(2 * i * tw, 3 * tw + 5 * k * tw, hw, hw);
                                Bitmap cl = a.Clone(r, a.PixelFormat);
                                g.DrawImage(cl, i * tw, j * tw + pad);
                                //right top
                                r = new Rectangle(3 * hw + 2 * i * tw, 3 * tw + 5 * k * tw, hw, hw);
                                cl = a.Clone(r, a.PixelFormat);
                                g.DrawImage(cl, hw + i * tw, j * tw + pad);
                                //left bottom
                                r = new Rectangle(2 * i * tw, 3 * hw + 3 * tw + 5 * k * tw, hw, hw);
                                cl = a.Clone(r, a.PixelFormat);
                                g.DrawImage(cl, i * tw, hw + j * tw + pad);
                                //right bottom
                                r = new Rectangle(3 * hw + 2 * i * tw, 3 * hw + 3 * tw + 5 * k * tw, hw, hw);
                                cl = a.Clone(r, a.PixelFormat);
                                g.DrawImage(cl, hw + i * tw, hw + j * tw + pad);
                            }
                        }
                    }
                }
                index += 1;
            }
            //finish
            return b;
        }

        public Bitmap PrepareTilesetA5()
        {
            int count = Core.Map.Tilesets[4].Count;
            Bitmap b = Core.CreateBack32(8, 16 * count);
            int index = 0;
            foreach (string str in Core.Map.Tilesets[4])
            {
                Bitmap a = ts_pic[str];
                a = Core.ResizeImage(a, 8 * tw, 16 * tw);
                using (Graphics graph = Graphics.FromImage(b))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            Rectangle r = new Rectangle(i * tw, j * tw, tw, tw);
                            Bitmap cl = a.Clone(r, a.PixelFormat);
                            graph.DrawImage(cl, i * tw, j * tw + (16 * tw * index));
                        }
                    }
                }
                index += 1;
            }
            return b;
        }

        public Bitmap PrepareTilesetB()
        {
            int count = Core.Map.Tilesets[5].Count;
            Bitmap b = Core.CreateBack32(8, 32 * count);
            int index = 0;
            foreach (string str in Core.Map.Tilesets[5])
            {
                Bitmap a = ts_pic[str];
                a = Core.ResizeImage(a, 16 * tw, 16 * tw);
                using (Graphics graph = Graphics.FromImage(b))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            Rectangle r = new Rectangle(i * tw, j * tw, tw, tw);
                            Bitmap cl = a.Clone(r, a.PixelFormat);
                            graph.DrawImage(cl, i * tw, j * tw + (32 * tw * index));
                        }
                        for (int j = 0; j < 16; j++)
                        {
                            Rectangle r = new Rectangle((8 + i) * tw, j * tw, tw, tw);
                            Bitmap cl = a.Clone(r, a.PixelFormat);
                            graph.DrawImage(cl, i * tw, (16 + j) * tw + (32 * tw * index));
                        }
                    }
                }
                index += 1;
            }
            return b;
        }

    }
}
