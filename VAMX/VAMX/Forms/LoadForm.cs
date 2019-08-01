using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace VAMX
{
    public partial class LoadForm : Form
    {
        public LoadForm()
        {
            InitializeComponent();
        }

        bool loaded = false;
        string recent_file = "";

        //New file
        private void button1_Click(object sender, EventArgs e)
        {
            if (!check_graphics()) { MessageBox.Show("出错！请确保所选的Graphics文件夹存在！"); return; }
            saveFileDialog1.InitialDirectory = get_initial_dir();
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                set_initial_dir(Path.GetDirectoryName(saveFileDialog1.FileName));
                NewMap(saveFileDialog1.FileName);
            }
        }

        //Load file
        private void button2_Click(object sender, EventArgs e)
        {
            if (!check_graphics()) { MessageBox.Show("出错！请确保所选的Graphics文件夹存在！"); return; }
            string p = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            p += @"\RPGVXAce";
            openFileDialog1.InitialDirectory = get_initial_dir();
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                set_initial_dir(Path.GetDirectoryName(openFileDialog1.FileName));
                LoadMap(openFileDialog1.FileName);
            }
        }

        //Load recent file
        private void button3_Click(object sender, EventArgs e)
        {
            LoadMap(recent_file);
        }

        #region MapLoading

        private void LoadMap(string path)
        {
                      
            Core.MapPath = path;
            Core.GraphicsPath = textBox1.Text;
            Core.LoadMap();
            MainForm main_form = new MainForm(this);
            main_form.Show();
            this.Hide();
            //Recent File
            recent_file = path;
            button3.Enabled = true;
            var a = new Ini.IniFile(RubyCore.SelfPath + @"\settings.ini");
            a.IniWriteValue("Recent", "path", recent_file);
        }

        private void NewMap(string path)
        {
            SettingsForm f = new SettingsForm();
            f.ShowDialog();
            if (f.success)
            {
                Core.SetMap(f.map);
                Core.MapPath = path;
                Core.SaveMap();
                LoadMap(path);
            }
        }

        #endregion

        #region Misc

        //Change graphics path
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        //Form Load
        private void LoadForm_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(RubyCore.SelfPath + @"\settings.ini"))
            {
                var a = new Ini.IniFile(RubyCore.SelfPath + @"\settings.ini");
                textBox1.Text = a.IniReadValue("Graphics", "path");
                recent_file = a.IniReadValue("Recent", "path");
                if (File.Exists(recent_file))
                {
                    button3.Enabled = true;
                }
            }
            loaded = true;
        }

        //Textbox 1 Change
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!loaded) { return; }
            var a = new Ini.IniFile(RubyCore.SelfPath + @"\settings.ini");
            a.IniWriteValue("Graphics", "path", textBox1.Text);
        }

        //Private Methods

        private string get_initial_dir()
        {
            string str = "";
            if (System.IO.File.Exists(RubyCore.SelfPath + @"\settings.ini"))
            {
                var a = new Ini.IniFile(RubyCore.SelfPath + "/settings.ini");
                str = a.IniReadValue("InitFolder", "path");
            }
            if (str == "")
            {
                return System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                return str;
            }
        }

        private void set_initial_dir(string f)
        {
            var a = new Ini.IniFile(RubyCore.SelfPath + @"\settings.ini");
            a.IniWriteValue("InitFolder", "path", f);
        }

        private bool check_graphics()
        {
            string str = textBox1.Text;
            if (System.IO.Path.GetFileName(str) != "Graphics") { return false; }
            if (!System.IO.Directory.Exists(str)) { return false; }
            return true;
        }

        #endregion

    }
}
