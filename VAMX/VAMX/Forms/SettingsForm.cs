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
    public partial class SettingsForm : Form
    {
        private ListViewAllColumnComparer lvwColumnSorter;

        public SettingsForm(MapX map = null)
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewAllColumnComparer(SortOrder.Ascending);
            listView1.ListViewItemSorter = lvwColumnSorter;
            SetForm(map);
        }


        //Members
        bool new_map = false;
        bool _success = false;
        MapX _map;

        public bool success { get { return _success; } }
        public MapX map { get { return _map; } }

        //Set Form
        private void SetForm(MapX map)
        {
            if (map == null) { new_map = true;  this.Text = "新建地图"; _map = new MapX(); return; }
            this.Text = "地图设置";
            _map = map;
            numericUpDown1.Value = map.Width;
            numericUpDown2.Value = map.Height;
            numericUpDown3.Value = map.ZIndex[0];
            numericUpDown4.Value = map.ZIndex[1];
            numericUpDown5.Value = map.ZIndex[2];
            numericUpDown6.Value = map.ZIndex[3];
            numericUpDown7.Value = map.ZIndex[4];
            numericUpDown8.Value = map.ZIndex[5];
            //Tilesets
            listView1.Items.Clear();
            for (int i = 0; i < 6; i++)
            {
                int count = 0;
                foreach (string str in map.Tilesets[i])
                {
                    ListViewItem lvi = new ListViewItem(GetTileType(i));
                    lvi.SubItems.Add(count.ToString());
                    lvi.SubItems.Add(str);
                    listView1.Items.Add(lvi);
                    count++;
                }
            }
        }

        private string GetTileType(int i)
        {
            switch (i)
            {
                case 0: return "A1";
                case 1: return "A2";
                case 2: return "A3";
                case 3: return "A4";
                case 4: return "A5";
                case 5: return "B";
                default: return "";
            }
        }

        private int GetTileTypeID(string str)
        {
            switch (str)
            {
                case "A1": return 0;
                case "A2": return 1;
                case "A3": return 2;
                case "A4": return 3;
                case "A5": return 4;
                case "B": return 5;
                default: return 0;
            }
        }

        private void ConfirmMap()
        {
            if (new_map)
            {
                _map.Initialize((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            }
            else
            {
                _map.ResizeMap((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            }
            _map.ZIndex[0] = (int)numericUpDown3.Value;
            _map.ZIndex[1] = (int)numericUpDown4.Value;
            _map.ZIndex[2] = (int)numericUpDown5.Value;
            _map.ZIndex[3] = (int)numericUpDown6.Value;
            _map.ZIndex[4] = (int)numericUpDown7.Value;
            _map.ZIndex[5] = (int)numericUpDown8.Value;
            //Tilesets
            _map.ClearAllTilesets();
            foreach (ListViewItem lvi in listView1.Items)
            {
                _map.Tilesets[GetTileTypeID(lvi.Text)].Add(lvi.SubItems[2].Text);
            }
        }

        //Z Reset
        private void button1_Click(object sender, EventArgs e)
        {
            numericUpDown3.Value = 0;
            numericUpDown4.Value = 50;
            numericUpDown5.Value = 75;
            numericUpDown6.Value = 100;
            numericUpDown7.Value = 150;
            numericUpDown8.Value = 250;
        }

        //Confirm
        private void button2_Click(object sender, EventArgs e)
        {
            ConfirmMap();
            _success = true;
            this.Close();
        }

        //Cancel
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region TilesetEdit

        //Add
        private void button4_Click(object sender, EventArgs e)
        {
            TilesetForm f = new TilesetForm();
            f.ShowDialog();
            if (f.success)
            {
                ListViewItem lvi = new ListViewItem(GetTileType(f.type));
                lvi.Name = lvi.Text;
                int count = 0;
                foreach (ListViewItem l in listView1.Items)
                {
                    if (l.Text == lvi.Text) { count += 1; }
                }
                lvi.SubItems.Add(count.ToString());
                lvi.SubItems.Add(f.name);
                listView1.Items.Add(lvi);
                listView1.Sort();
            }
            f.Dispose();
        }

        //Edit
        private void button5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) { return; }
            ListViewItem item = listView1.SelectedItems[0];
            int type = GetTileTypeID(item.Text);
            string name = item.SubItems[2].Text;
            TilesetForm f = new TilesetForm(type,name);
            f.ShowDialog();
            if (f.success)
            {
                string t = GetTileType(f.type);
                item.Text = t;
                item.Name = item.Text;
                item.SubItems[2].Text = f.name;
                if (type != f.type)
                {
                    //Fix Index
                    int count = 0;
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (lvi.Text != t) { continue; }
                        lvi.SubItems[1].Text = count.ToString();
                        count += 1;
                    }
                    count = 0;
                    foreach (ListViewItem lvi in listView1.Items)
                    {
                        if (lvi.Text != GetTileType(type)) { continue; }
                        lvi.SubItems[1].Text = count.ToString();
                        count += 1;
                    }
                    listView1.Sort();
                }
            }
            f.Dispose();
        }

        //Delete
        private void button6_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) { return; }
            string str = listView1.SelectedItems[0].Text;
            listView1.Items.Remove(listView1.SelectedItems[0]);
            //Fix Index
            int count = 0;
            foreach (ListViewItem lvi in listView1.Items)
            {
                if (lvi.Text != str) { continue; }
                lvi.SubItems[1].Text = count.ToString();
                count += 1;
            }
            listView1.Sort();
        }

        //Advanced
        private void button7_Click(object sender, EventArgs e)
        {
            TilesetAdvForm f = new TilesetAdvForm();
            f.ShowDialog();
            if (f.success)
            {
                int command = f.index;
                if (command == 0)
                {
                    listView1.Items.Clear();
                }
                else
                {
                    WipeTilesetType(command - 1);
                }
            }
        }

        private void WipeTilesetType(int i)
        {
            var lv = listView1.Items.Find(GetTileType(i),false);
            foreach (ListViewItem lvi in lv)
            {
                listView1.Items.Remove(lvi);
            }
            listView1.Refresh();
        }

        #endregion

    }
}
