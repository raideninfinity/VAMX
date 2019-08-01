using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAMX
{
    [Serializable]
    public class Table
    {
        public Table(int x, int y, int z = 1)
        {
            x_size = x;
            y_size = y;
            z_size = z;
            data = new int[x_size * y_size * z_size];
        }

        private int x_size;
        private int y_size;
        private int z_size;
        private int[] data;

        public int XSize { get { return x_size; } }
        public int YSize { get { return y_size; } }
        public int ZSize { get { return z_size; } }
        public int[] Data { get { return data; } }

        public int this[int i, int j = 0, int k = 0]
        {
            get { return data[i + j * x_size + k * x_size * y_size]; }
            set { data[i + j * x_size + k * x_size * y_size] = value; }
        }

        public void ResizeTable(int x, int y, int z = 1)
        {
            int[] new_data = new int[x * y * z];

            for (int k = 0; k < z; k++)
            {
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        if (i >= x_size || j >= y_size || k > z_size) { continue; }
                        new_data[i + j * x + k * x * y] = data[i + j * x_size + k * x_size * y_size];
                    }
                }
            }

            data = new_data;
            x_size = x;
            y_size = y;
            z_size = z;
        }

        public int[] SetData { set { data = value; } }

    }

    [Serializable]
    public class MapX
    {

        private Table layers;
        private int width;
        private int height;
        private List<string>[] tilesets;
        private int[] z_index;
        private List<UObject> objects;

        public Table Layers { get { return layers; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public List<string>[] Tilesets { get { return tilesets; } }
        public int[] ZIndex { get { return z_index; } }
        public List<UObject> Objects { get { return objects; } }

        public void Initialize(int x, int y)
        {
            width = x;
            height = y;
            layers = new Table(x, y, 9);
            tilesets = new List<string>[]{new List<string>(), new List<string>() , new List<string>() ,
                new List<string>() , new List<string>() , new List<string>() };
            z_index = new int[] { 0, 50, 75, 100, 150, 250 };
            objects = new List<UObject>();
        }

        public void SetLayers(Table l)
        {
            layers = l;
        }

        public void SetObjects(List<UObject> l)
        {
            objects = l;
        }

        public List<string> GetAllTilesets()
        {
            var list = new List<string>();
            foreach (List<string> str in tilesets)
            {
                list.AddRange(str);
            }
            return list;
        }

        public void ClearAllTilesets()
        {
            for (int i = 0; i < tilesets.Length; i++)
            {
                tilesets[i].Clear();
            }
        }

        public void ClearLayer(int layer)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    layers[i, j, layer] = 0;
                }
            }
        }

        public void ClearAllLayers()
        {
            layers = new Table(width, height, 9);
        }

        public void ResetZIndex()
        {
            z_index = new int[] { 0, 50, 75, 100, 150, 250 };
        }

        public int[] SetZIndex { set { z_index = value; } }

        public void ResizeMap(int x, int y)
        {
            width = x;
            height = y;
            layers.ResizeTable(x, y, 9);
        }

        public void MapShift(int direction, int amount)
        {
            switch (direction)
            {
                case 2: MapShiftDown(amount); break;
                case 4: MapShiftLeft(amount); break;
                case 6: MapShiftRight(amount); break;
                case 8: MapShiftUp(amount); break;
                default: return;
            }
        }

        private void MapShiftLeft(int amount)
        {
            Table t = new Table(width, height, 9);
            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int x = width - amount + i;
                    for (int k = 0; k < 9; k++)
                    {
                        t[x, j, k] = layers[i, j, k];
                    }
                }
            }
            for (int i = 0; i < width - amount; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int x = amount + i;
                    for (int k = 0; k < 6; k++)
                    {
                        t[i, j, k] = layers[x, j, k];
                    }
                }
            }
            layers = t;
        }

        private void MapShiftRight(int amount)
        {
            Table t = new Table(width, height, 9);
            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int x = width - amount + i;
                    for (int k = 0; k < 9; k++)
                    {
                        t[i, j, k] = layers[x, j, k];
                    }
                }
            }
            for (int i = 0; i < width - amount; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int x = amount + i;
                    for (int k = 0; k < 6; k++)
                    {
                        t[x, j, k] = layers[i, j, k];
                    }
                }
            }
            layers = t;
        }

        public void MapShiftUp(int amount)
        {
            Table t = new Table(width, height, 9);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    int y = height - amount + j;
                    for (int k = 0; k < 9; k++)
                    {
                        t[i, y, k] = layers[i, j, k];
                    }
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height - amount; j++)
                {
                    int y = amount + j;
                    for (int k = 0; k < 9; k++)
                    {
                        t[i, j, k] = layers[i, y, k];
                    }
                }
            }
            layers = t;
        }

        public void MapShiftDown(int amount)
        {
            Table t = new Table(width, height, 9);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    int y = height - amount + j;
                    for (int k = 0; k < 9; k++)
                    {
                        t[i, j, k] = layers[i, y, k];
                    }
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height - amount; j++)
                {
                    int y = amount + j;
                    for (int k = 0; k < 9; k++)
                    {
                        t[i, y, k] = layers[i, j, k];
                    }
                }
            }
            layers = t;
        }

        public void DeleteObject(UObject o)
        {
           objects.Remove(o);
        }

        public int GetNewObjectID()
        {
            int id = 1;
            SortObjectByID();
            foreach (UObject obj in objects)
            {
                if (obj.ID == id) { id++; continue; }
            }
            return id;
        }

        public void SortObjectByID()
        {
            objects = objects.OrderBy(o => o.ID).ToList();
        }

        public List<UObject> GetCurrentLayerObjects(int layer)
        {
            return objects.FindAll(o => o.Layer == layer).OrderBy(o => o.SubLayer).ThenBy(o => o.Z).ThenBy(o => o.ID).ToList();
        }

    }

    [Serializable]
    public class UObject
    {
        int id = 0;
        int layer = 0;
        int sub_layer = 0;
        int x = 0;
        int y = 0;
        int z = 0;
        int tile_index = 0;
        int tile_id = 0;
        int span_x = 0;
        int span_y = 0;
        int crop_top = 0;
        int crop_left = 0;
        int crop_bottom = 0;
        int crop_right = 0;

        public int ID { get { return id; } set { id = value; } }
        public int Layer { get { return layer; } set { layer = value; } }
        public int SubLayer { get { return sub_layer; } set { sub_layer = value; } }
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
        public int Z { get { return z; } set { z = value; } }
        public int TileIndex { get { return tile_index; } set { tile_index = value; } }
        public int TileID { get { return tile_id; } set { tile_id = value; } }
        public int SpanX { get { return span_x; } set { span_x = value; } }
        public int SpanY { get { return span_y; } set { span_y = value; } }
        public int CropTop { get { return crop_top; } set { crop_top = value; } }
        public int CropBottom { get { return crop_bottom; } set { crop_bottom = value; } }
        public int CropLeft { get { return crop_left; } set { crop_left = value; } }
        public int CropRight { get { return crop_right; } set { crop_right = value; } }
    }
}