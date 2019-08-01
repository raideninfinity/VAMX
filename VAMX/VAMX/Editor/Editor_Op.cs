using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace VAMX
{
    public partial class Editor
    {
        public void PerformSettings()
        {
            SettingsForm f_edit = new SettingsForm(Core.Map);
            f_edit.ShowDialog();
            if (f_edit.success)
            {
                SetUndo(1);
                Core.SetMap(f_edit.map);
            }
            ReloadEditor();
        }

        public void PerformMapShift()
        {
            MapShiftForm f_e = new MapShiftForm();
            f_e.ShowDialog();
            if (f_e.success)
            {
                int type = f_e.type;
                int amount = f_e.amount;
                SetUndo();
                if (type == 0) { type = 4; }
                else if (type == 1) { type = 6; }
                else if (type == 2) { type = 8; }
                else { type = 2; }
                Core.Map.MapShift(type, amount);
                ReloadEditor();
            }
        }

        public void PerformOutput()
        {
            OutputForm f_out = new OutputForm();
            f_out.ShowDialog();
            if (f_out.success)
            {
                OutputPNG(f_out.mode, f_out.out_layer, f_out.out_type, f_out.zoom, f_out.frame);
            }
            f_out.Dispose();
        }

        public void OutputPNG(int mode, bool[] out_layers, bool[] out_type, double zoom, int frame)
        {

            string out_folder = RubyCore.SelfPath + @"\Output\";

            if (mode >= 2) { out_layers = new bool[] { true, true, true, true, true, true }; }

            if (!Directory.Exists(out_folder)) { Directory.CreateDirectory(out_folder); }
            if (mode % 2 == 0) //Merge
            {
                OutputPNGMerge(out_folder, out_layers, out_type, zoom, frame);
            }
            else //Separate
            {
                OutputPNGSp(out_folder, out_layers, out_type, zoom, frame);
            }
            Process.Start("explorer.exe", out_folder);
        }

        public int XZoom(int a, double b)
        {
            return (Convert.ToInt32(a * b));
        }

        public void OutputPNGMerge(string out_folder, bool[] out_layers, bool[] out_type, double z, int frame)
        {
            using (Bitmap bmp = new Bitmap(XZoom(Core.Map.Width * 32, z), XZoom(Core.Map.Height * 32, z)))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    for (int k = 0; k < 6; k++)
                    {
                        if (!out_layers[k]) { continue; }
                        if (out_type[0])
                        {
                            for (int i = 0; i < Core.Map.Width; i++)
                            {
                                for (int j = 0; j < Core.Map.Height; j++)
                                {
                                    DrawTileZ(g, Core.Map.Layers[i, j, k], XZoom(i * 32, z), XZoom(j * 32, z), z, frame);
                                }
                            }
                        }
                        if (out_type[1])
                        {
                            //Output Objects

                            OutputObjects(g, z, k);
                        }
                    }
                }
                bmp.Save(out_folder + DateTime.Now.Ticks.ToString() + "_Merge.png");
            }
        }

        public void OutputPNGSp(string out_folder, bool[] out_layers, bool[] out_type, double z, int frame)
        {
            for (int k = 0; k < 6; k++)
            {
                if (!out_layers[k]) { continue; }
                using (Bitmap bmp = new Bitmap(XZoom(Core.Map.Width * 32, z), XZoom(Core.Map.Height * 32, z)))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        if (out_type[0])
                        {
                            for (int i = 0; i < Core.Map.Width; i++)
                            {
                                for (int j = 0; j < Core.Map.Height; j++)
                                {
                                    DrawTileZ(g, Core.Map.Layers[i, j, k], XZoom(i * 32, z), XZoom(j * 32, z), z, frame);
                                }
                            }
                        }
                        if (out_type[1])
                        {
                            //Output Objects
                            OutputObjects(g, z, k);
                        }
                    }
                    bmp.Save(out_folder + DateTime.Now.Ticks.ToString() + "_L" + (k + 1).ToString() + ".png");
                }
            }
        }

        public void DrawTileZ(Graphics g, int id, int x, int y, double zoom, int frame)
        {
            if (id == 0) { return; }
            int aid = id;
            int type = (int)((id & 0xFF000000) >> 24);
            if (type == 1) { aid += frame * 0x10000000; }
            Bitmap b;
            if (!tilecache.ContainsKey(aid))
            {
                anim = frame;
                tilecache[aid] = GetTile(id, true);
                anim = 0;
            }
            b = tilecache[aid];
            g.DrawImage(b, new Rectangle(x, y, XZoom(32, zoom), XZoom(32, zoom)), new Rectangle(0, 0, 32, 32), GraphicsUnit.Pixel);
        }


        public void OutputObjects(Graphics g, double zoom, int layer)
        {
            List<UObject> l_objects = Core.Map.GetCurrentLayerObjects(layer);
            foreach (UObject obj in l_objects)
            {
                int ow = obj.SpanX * 32 - obj.CropLeft - obj.CropRight;
                int oh = obj.SpanY * 32 - obj.CropTop - obj.CropBottom;
                for (int i = 0; i < obj.SpanX; i++)
                {
                    for (int j = 0; j < obj.SpanY; j++)
                    {
                        if ((i + 1) * 32 <= obj.CropLeft ||
                            (j + 1) * 32 <= obj.CropTop ||
                            i * 32 >= obj.SpanX * 32 - obj.CropRight ||
                            j * 32 >= obj.SpanY * 32 - obj.CropBottom) { continue; }

                        int tile = GetObjTile(obj.TileIndex, obj.TileID, i, j);
                        int x = obj.X + i * 32 - obj.CropLeft;
                        int y = obj.Y + j * 32 - obj.CropTop;

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
                        dest.X = XZoom(dest.X, zoom);
                        dest.Y = XZoom(dest.Y, zoom);
                        dest.Width = XZoom(dest.Width, zoom);
                        dest.Height = XZoom(dest.Height, zoom);
                        Core.Editor.DrawTileEX(g, tile, dest, src, true);
                    }
                }
            }
        }

        const int B_TILE = 0x06000000;

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

    }
}
