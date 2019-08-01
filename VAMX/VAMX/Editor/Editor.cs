using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace VAMX
{
    public partial class Editor
    {
        public Editor(MainForm form)
        {
            f = form;
        }

        public MainForm f;

        #region Main Methods

        public void PerformInitialize()
        {
            ResetUndoRedo();
            f.tabControlT.TabPages.Clear();
        }

        public void PerformLoad()
        {
            f.Text = "VA拓展地图编辑器 - [" + Path.GetFileNameWithoutExtension(Core.MapPath) + "]";
            ReloadEditor();
        }

        public void PerformSave()
        {
            Cursor.Current = Cursors.WaitCursor;
            Core.SaveMap();
            Cursor.Current = Cursors.Default;
            ResetUndoRedo();
        }

        public void PerformClose()
        {
            string str1 = "即将关闭地图返回上一个窗…储存进度吗?";
            string str2 = "VA拓展地图编辑器";
            string str3 = "真的吗？你的进度将会永久遗失！";
            DialogResult result1 = MessageBox.Show(str1, str2,
            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result1 == DialogResult.Yes)
            {
                PerformSave();
                CloseMap();
            }
            else if (result1 == DialogResult.No)
            {
                DialogResult result2 = MessageBox.Show(str3, str2,
            MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result2 == DialogResult.OK)
                {
                    CloseMap();
                }
                else if (result2 == DialogResult.Cancel)
                {
                    return;
                }
            }
            else if (result1 == DialogResult.Cancel)
            {
                return;
            }
        }

        public void CloseMap()
        {
            Core.UnloadMap();
            f.FormClose();
            f.Close();
            f.LoadForm.Show();
            f.LoadForm.BringToFront();
        }

        #endregion

        #region UndoRedo

        private Stack<object> undo_stack = new Stack<object>();
        private Stack<object> redo_stack = new Stack<object>();

        public bool UndoEnabled()
        {
            return (undo_stack.Count > 0);
        }

        public bool RedoEnabled()
        {
            return (redo_stack.Count > 0);
        }

        public void PerformUndo()
        {
            object a = undo_stack.Pop();
            if (a is Table) { redo_stack.Push(Core.Map.Layers.DeepClone()); Core.Map.SetLayers((Table)a); }
            else if (a is MapX) { redo_stack.Push(Core.Map.DeepClone()); Core.SetMap((MapX)a); ReloadEditor(); }
            else if (a is List<UObject>) {
                bool b = Core.Map.Objects.Count != ((List<UObject>)a).Count;
                redo_stack.Push(Core.Map.Objects.DeepClone()); Core.Map.SetObjects((List<UObject>)a);
                if (DrawMode == 4 && f.tabControlT.SelectedIndex == 1)
                {
                    ((EditModeUnl)CurrentMode).RenewObjects();
                    if (b) { f.tsbLVORefresh.PerformClick(); }
                }
            }
            RefreshMap();
            EnableUndoRedo();
        }

        public void PerformRedo()
        {
            object a = redo_stack.Pop();
            if (a is Table) { undo_stack.Push(Core.Map.Layers.DeepClone()); Core.Map.SetLayers((Table)a); }
            else if (a is MapX) { undo_stack.Push(Core.Map.DeepClone()); Core.SetMap((MapX)a); ReloadEditor(); }
            else if (a is List<UObject>) {
                bool b = Core.Map.Objects.Count != ((List<UObject>)a).Count;
                undo_stack.Push(Core.Map.Objects.DeepClone()); Core.Map.SetObjects((List<UObject>)a);
                if (DrawMode == 4 && f.tabControlT.SelectedIndex == 1)
                {
                    ((EditModeUnl)CurrentMode).RenewObjects();
                    if (b) { f.tsbLVORefresh.PerformClick(); }
                }
            }
            RefreshMap();
            EnableUndoRedo();
        }

        public void SetUndo(int type = 0)
        {
            redo_stack.Clear();
            switch (type)
            {
                case 0: undo_stack.Push(Core.Map.Layers.DeepClone()); break;
                case 1: undo_stack.Push(Core.Map.DeepClone()); break;
                case 2: undo_stack.Push(Core.Map.Objects.DeepClone()); break;
            }
            EnableUndoRedo();
        }

        public void EnableUndoRedo()
        {
            if (UndoEnabled()) { f.tsbUndo.Enabled = true; } else { f.tsbUndo.Enabled = false; }
            if (RedoEnabled()) { f.tsbRedo.Enabled = true; } else { f.tsbRedo.Enabled = false; }
        }

        public void ResetUndoRedo()
        {
            undo_stack.Clear();
            redo_stack.Clear();
            EnableUndoRedo();
        }

        #endregion

        #region Map Display

        public void RefreshMap()
        {
            f.panelM1.RefreshMap();
        }

        #endregion

        #region Editor Loading

        private Dictionary<string, Bitmap> ts_pic = new Dictionary<string, Bitmap>();
        private Dictionary<string, Bitmap> ts_pic_o = new Dictionary<string, Bitmap>();

        public void ReloadEditor()
        {
            ResetTileCache();
            ts_pic.Clear();
            ts_pic_o.Clear();
            for (int i = 0; i < 6; i++)
            {
                foreach (string str in Core.Map.Tilesets[i])
                {
                    string p = Core.GraphicsPath + @"\Tilesets\" + str + ".png";
                    if (!File.Exists(p)) { AddBlankTileset(i, str); }
                    if (ts_pic.ContainsKey(str)) { continue; }
                    AddTilesetResource(i, str, p);
                }
            }
            foreach (KeyValuePair<string, Bitmap> k in ts_pic)
            {
                ts_pic_o.Add(k.Key, k.Value.ChangeImageOpacity(0.4f));
            }
            PrepareTileset();
            f.panelM1.EnablePanel();
            f.tsbEM1.PerformClick();
            f.tsbLA.PerformClick();
            SetTileEditTop();
            RefreshMap();
            f.tLabelSize.Text = "地图规格: " + Core.Map.Width.ToString() + " x " + Core.Map.Height.ToString() + " ";
            f.tLabelZoom.Text = "显示大小: " + Convert.ToInt32(Core.Zoom * 100).ToString() +
                "% (" + Convert.ToInt32(32 * Core.Zoom).ToString() + "像素) ";
            f.tLabelMouse.Text = "-";
            f.tLabelTile.Text = "-";
        }

        public void AddBlankTileset(int i, string str)
        {
            switch (i)
            {
                case 0: ts_pic.Add(str, new Bitmap(512, 384)); break;
                case 1: ts_pic.Add(str, new Bitmap(512, 384)); break;
                case 2: ts_pic.Add(str, new Bitmap(512, 256)); break;
                case 3: ts_pic.Add(str, new Bitmap(512, 480)); break;
                case 4: ts_pic.Add(str, new Bitmap(256, 512)); break;
                case 5: ts_pic.Add(str, new Bitmap(512, 512)); break;
            }
        }

        public void AddTilesetResource(int i, string str, string path)
        {
            switch (i)
            {
                case 0: ts_pic.Add(str, new Bitmap(Image.FromFile(path), 512, 384)); break;
                case 1: ts_pic.Add(str, new Bitmap(Image.FromFile(path), 512, 384)); break;
                case 2: ts_pic.Add(str, new Bitmap(Image.FromFile(path), 512, 256)); break;
                case 3: ts_pic.Add(str, new Bitmap(Image.FromFile(path), 512, 480)); break;
                case 4: ts_pic.Add(str, new Bitmap(Image.FromFile(path), 256, 512)); break;
                case 5: ts_pic.Add(str, new Bitmap(Image.FromFile(path), 512, 512)); break;
            }
        }

        #endregion

        public void PerformShortcut(Keys key)
        {
            switch (key)
            {    
                //Ctrl + Keys
                case Keys.S: if (f.CtrlDown) { PerformSave(); } else { f.tsbEM3.PerformClick(); } break;
                case Keys.Z: if (f.CtrlDown && UndoEnabled()) { PerformUndo(); } break;
                case Keys.Y: if (f.CtrlDown && RedoEnabled()) { PerformRedo(); } break;
                case Keys.X: if (f.CtrlDown && f.tsbCut.Enabled) { f.tsbCut.PerformClick(); } break;
                case Keys.C: if (f.CtrlDown && f.tsbCopy.Enabled) { f.tsbCopy.PerformClick(); } break;
                case Keys.V: if (f.CtrlDown && f.tsbPaste.Enabled) { f.tsbPaste.PerformClick(); } break;
                //Single Keys
                case Keys.G: f.tsbGrid.PerformClick(); break;
                case Keys.I: f.tsbIndic.PerformClick(); break;
                case Keys.OemMinus: f.tsbZoomIn.PerformClick(); break;
                case Keys.Oemplus: f.tsbZoomOut.PerformClick(); break;
                //You Know...
                case Keys.A: AutoKey(); break;
                case Keys.E: EraseKey(); break;
                case Keys.P: f.tsbEM1.PerformClick(); break;
                case Keys.R: f.tsbEM2.PerformClick(); break;
                case Keys.U: f.tsbEM4.PerformClick(); break;
                case Keys.D1: f.tsbL1.PerformClick(); break;
                case Keys.D2: f.tsbL2.PerformClick(); break;
                case Keys.D3: f.tsbL3.PerformClick(); break;
                case Keys.D4: f.tsbL4.PerformClick(); break;
                case Keys.D5: f.tsbL5.PerformClick(); break;
                case Keys.D6: f.tsbL6.PerformClick(); break;
                case Keys.D7: f.tsbPass.PerformClick(); break;
                case Keys.D8: f.tsbFlags.PerformClick(); break;
                case Keys.D9: f.tsbRegions.PerformClick(); break;
                case Keys.Oemtilde:
                case Keys.D0:
                    f.tsbLA.PerformClick(); break;
                case Keys.F1: if (f.CtrlDown && DrawMode == 4) { if (f.tsbSH1.Enabled) { f.tsbSH1.PerformClick(); } } else { if (f.tsbS1.Enabled) { f.tsbS1.PerformClick(); } } break;
                case Keys.F2: if (f.CtrlDown && DrawMode == 4) { if (f.tsbSH2.Enabled) { f.tsbSH2.PerformClick(); } } else { if (f.tsbS2.Enabled) { f.tsbS2.PerformClick(); } } break;
                case Keys.F3: if (f.CtrlDown && DrawMode == 4) { if (f.tsbSH3.Enabled) { f.tsbSH3.PerformClick(); } } else { if (f.tsbS3.Enabled) { f.tsbS3.PerformClick(); } } break;
                case Keys.F4: if (f.CtrlDown && DrawMode == 4) { if (f.tsbSH4.Enabled) { f.tsbSH4.PerformClick(); } } else { if (f.tsbS4.Enabled) { f.tsbS4.PerformClick(); } } break;
                case Keys.F5: if (f.CtrlDown && DrawMode == 4) { if (f.tsbSH5.Enabled) { f.tsbSH5.PerformClick(); } } else { if (f.tsbS5.Enabled) { f.tsbS5.PerformClick(); } } break;
                case Keys.F6: if (f.CtrlDown && DrawMode == 4) { if (f.tsbSH6.Enabled) { f.tsbSH6.PerformClick(); } } else { if (f.tsbS6.Enabled) { f.tsbS6.PerformClick(); } } break;
                case Keys.Up: if (f.CtrlDown) { LVOControlKeys(10); } else if (f.ShiftDown) { LVOControlKeys(6); } else { LVOControlKeys(1); } break;
                case Keys.Down: if (f.CtrlDown) { LVOControlKeys(11); } else if (f.ShiftDown) { LVOControlKeys(7); } else { LVOControlKeys(2); } break;
                case Keys.Left: if (f.CtrlDown) { LVOControlKeys(12); } else if (f.ShiftDown) { LVOControlKeys(8); } else { LVOControlKeys(3); } break;
                case Keys.Right: if (f.CtrlDown) { LVOControlKeys(13); } else if (f.ShiftDown) { LVOControlKeys(9); } else { LVOControlKeys(4); } break;
                case Keys.Delete: LVOControlKeys(5); break;
            }
            RefreshMap();
        }

        public void AutoKey()
        {
            var e = new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 0);
            CurrentMode.PLowDown(e);
        }

        public void EraseKey()
        {
            var e = new MouseEventArgs(MouseButtons.Left, 1, 32, 1, 0);
            CurrentMode.PLowDown(e);
        }

        public void LVOControlKeys(int i)
        {
            if (DrawMode != 4) { return; }
            ((EditModeUnl)CurrentMode).LVOControlKeys(i);
        }

    }
}
