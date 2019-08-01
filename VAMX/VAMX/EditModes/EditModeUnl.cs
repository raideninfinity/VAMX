using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace VAMX
{
    public class EditModeUnl : EditMode
    {
        public override int Mode { get { return 4; } }

        public bool selected;
        public bool selecting;
        public bool selector_on;
        public int sel_sx = 0;
        public int sel_sy = 0;
        public int sel_ex = 0;
        public int sel_ey = 0;
        public int current_group = 0;
        public int tile_index = 0;
        public int tile_id = 0;
        public int span_x = 0;
        public int span_y = 0;

        public int sel_w { get { return Math.Abs(sel_sx - sel_ex) + 1; } }
        public int sel_h { get { return Math.Abs(sel_sy - sel_ey) + 1; } }
        public int sel_rx { get { return Math.Min(sel_sx, sel_ex); } }
        public int sel_ry { get { return Math.Min(sel_sy, sel_ey); } }

        public override void Reset()
        {
            SelectedObjects.Clear();
            current_group = 0;
            selecting = false;
            sel_ex = 0;
            sel_ey = 0;
            sel_sx = 0;
            sel_sy = 0;
            tile_index = 0;
            tile_id = 0;
            span_x = 0;
            span_y = 0;
            selected = false;
            selector_on = false;
        }

        public override void PTopDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            TopDown(e.X, e.Y);
        }

        public override void PTopLeave()
        {
            if (Core.Editor.EditMode >= 9) { return; }
            TopUp();
        }

        public override void PTopMove(MouseEventArgs e)
        {
            if (Core.Editor.EditMode >= 9) { return; }
            TopMove(e.X, e.Y);
        }

        public override void PTopUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) { return; }
            if (Core.Editor.EditMode >= 9) { return; }
            TopUp();
        }

        public override void PLowDown(MouseEventArgs e)
        {

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
        bool MouseDownR = false;

        public override void MapDown(MouseEventArgs e)
        {
            Core.Editor.MapHover(e.X, e.Y);
            if (Core.Editor.EditMode < 6 || Core.Editor.EditMode == 9)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Core.Editor.SetUndo(2);
                    MouseDownL = true;
                    MapMouseDownL();
                }
                else
                {
                    Core.Editor.SetUndo(2);
                    MouseDownR = true;
                    MapMouseDownR();
                }
            }
            if (e.Clicks == 2) { MapUp(e); LVOMapDoubleClick(); return; }
            Core.Editor.RefreshMap();
        }

        public override void MapLeave()
        {
            Core.Editor.CancelHover();
            UnselectObject();
            Core.Editor.RefreshMap();
        }

        public override void MapMove(MouseEventArgs e)
        {
            Core.Editor.MapHover(e.X, e.Y);
            MoveSelectedObject();
            if (MouseDownL)
            {
                LVOMapMove();
            }
            else if (MouseDownR)
            {
                LVOMapRMove();
            }
            Core.Editor.RefreshMap();
        }
        public override void MapUp(MouseEventArgs e)
        {
            MouseDownL = false;
            MouseDownR = false;
            LVOMapUp();
            UnselectObject();
            Core.Editor.RefreshMap();
        }

        public override void MapMouseDownL()
        {
            if (Core.Editor.f.tabControlT.SelectedIndex == 0)
            {
                if (Core.Editor.CtrlDown)
                {
                    MapDeleteObject();
                }
                else
                {
                    MapDrawObject();
                }
            }
            else
            {
                LVOMapMouseDown();
            }
        }

        public override void MapMouseDownR()
        {
            if (Core.Editor.f.tabControlT.SelectedIndex == 0)
            {
                MapSelectObject();
            }
            else
            {
                LVOMapMouseRDown();
            }
        }

        //-------------------------------------------------------

        public void TopDown(int x, int y)
        {
            if (Core.Editor.EditMode >= 9) { return; }
            BDown(x, y);
        }

        public void TopMove(int x, int y)
        {
            if (Core.Editor.EditMode >= 9) { return; }
            BMove(x, y);
        }

        public void TopUp()
        {
            if (Core.Editor.EditMode >= 9) { return; }
            BUp();
        }

        public void DrawTopSelector(int i)
        {
            int x = Math.Min(sel_sx, sel_ex);
            int y = Math.Min(sel_sy, sel_ey);
            int w = sel_w; int h = sel_h;
            Core.Editor.ClearImg(Core.Editor.TilesetSelect[i]);
            using (Graphics g = Graphics.FromImage(Core.Editor.TilesetSelect[i]))
            {
                if (selector_on)
                {
                    Core.Editor.DrawPointerR(g, x, y, w, h);
                }
                else
                {
                    Core.Editor.DrawPointerG(g, x, y, w, h);
                }
            }
            Core.Editor.f.pictureBoxT.Refresh();
        }

        public void BDown(int mx, int my)
        {
            Core.Editor.ClearTop();
            Core.Editor.ClearLower();
            if (mx >= Core.Editor.TilesetSelect[3].Width || my >= Core.Editor.TilesetSelect[3].Height || mx < 0 || my < 0) { return; }
            int x = Core.div(mx, 32); int y = Core.div(my, 32);
            sel_sx = x;
            sel_sy = y;
            sel_ex = x;
            sel_ey = y;
            selector_on = true;
            DrawTopSelector(3);
        }

        public void BMove(int mx, int my)
        {
            if (Core.Editor.TilesetSelect[3] == null) { return; }
            if (selector_on)
            {
                if (mx >= Core.Editor.TilesetSelect[3].Width || my >= Core.Editor.TilesetSelect[3].Height || mx < 0 || my < 0) { return; }
                int x = Core.div(mx, 32); int y = Core.div(my, 32);
                sel_ex = x;
                sel_ey = y;
                DrawTopSelector(3);
            }
        }

        public void BUp()
        {
            if (Core.Editor.TilesetSelect[3] == null) { return; }
            if (selector_on)
            {
                selector_on = false;
                selected = true;
                DrawTopSelector(3);
                //Load Data
                int x = Math.Min(sel_sx, sel_ex);
                int y = Math.Min(sel_sy, sel_ey);
                int w = sel_w;
                int h = sel_h;
                int[] data = new int[w * h];
                int id = x + y * 8;
                tile_index = Core.div(id, 256);
                tile_id = id % 256;
                span_x = sel_w;
                span_y = sel_h;
            }
        }

        public void ResetSelector()
        {
            Reset();
            Core.Editor.ClearTop();
        }

        //-------------------------------------------------------

        UObject selected_object;
        int offset_x;
        int offset_y;

        public void MapDrawObject()
        {
            if (!selected) { return; }
            UObject obj = new UObject();
            obj.ID = Core.Map.GetNewObjectID();
            obj.X = Core.Editor.HoverMX;
            obj.Y = Core.Editor.HoverMY;
            obj.Layer = Core.Editor.EditMode;
            obj.SubLayer = GetSubLayer();
            obj.TileIndex = tile_index;
            obj.TileID = tile_id;
            obj.SpanX = span_x;
            obj.SpanY = span_y;
            Core.Map.Objects.Add(obj);
            Core.Map.SortObjectByID();
            selected_object = obj;
        }

        public int GetSubLayer()
        {
            return Core.Editor.f.tscU1.SelectedIndex;
        }

        /*
        public int GetSubLayerMode()
        {
            return Core.Editor.f.tscU2.SelectedIndex - 1;
        }
        */

        public void MapDeleteObject()
        {
            List<UObject> list = GetSelectedObject();
            if (list.Count > 0)
            {
                Core.Map.Objects.Remove(list[0]);
            }
        }

        public bool ObjSelected(UObject o, int mx, int my)
        {
            if ((Core.Editor.EditMode != 9 && o.Layer != Core.Editor.EditMode) || !CheckDisplaySubLayer(o.SubLayer)) { return false; }
            int w = o.SpanX * 32 - o.CropLeft - o.CropRight;
            int h = o.SpanY * 32 - o.CropTop - o.CropBottom;
            return !(o.X > mx || o.Y > my || (o.X + w) < mx || (o.Y + h) < my);
        }

        public bool ObjSelectedS(UObject o, int mx, int my)
        {
            int w = o.SpanX * 32 - o.CropLeft - o.CropRight;
            int h = o.SpanY * 32 - o.CropTop - o.CropBottom;
            return !(o.X > mx || o.Y > my || (o.X + w) < mx || (o.Y + h) < my);
        }

        public bool ObjSelectedSL(UObject o, int mx, int my)
        {
            if ((Core.Editor.EditMode != 9 && o.Layer != Core.Editor.EditMode) || !CheckDisplaySubLayer(o.SubLayer)) { return false; }
            if (o.SubLayer != GetSubLayer()) { return false; }
            int w = o.SpanX * 32 - o.CropLeft - o.CropRight;
            int h = o.SpanY * 32 - o.CropTop - o.CropBottom;
            return !(o.X > mx || o.Y > my || (o.X + w) < mx || (o.Y + h) < my);
        }

        public List<UObject> GetSelectedObject()
        {
            int sl = 0; //GetSubLayerMode();
            if (sl == 0)
            {
                var list = Core.Map.Objects.FindAll(o => ObjSelected(o, Core.Editor.HoverMX, Core.Editor.HoverMY)).ToList();
                return list.OrderBy(o => o.Z).OrderBy(o => o.ID).Reverse().ToList();
            }
            else
            {
                var list = Core.Map.Objects.FindAll(o => ObjSelectedSL(o, Core.Editor.HoverMX, Core.Editor.HoverMY)).ToList();
                return list.OrderBy(o => o.Z).OrderBy(o => o.ID).Reverse().ToList();
            }
        }

        public List<UObject> GetSelectedObjectS()
        {
            var list = Core.Map.Objects.FindAll(o => ObjSelectedS(o, Core.Editor.HoverMX, Core.Editor.HoverMY)).ToList();
            return list.OrderBy(o => o.Z).OrderBy(o => o.ID).Reverse().ToList();
        }

        public void UnselectObject()
        {
            selected_object = null;
            offset_x = 0;
            offset_y = 0;
        }

        public void MoveSelectedObject()
        {
            if (selected_object == null || Core.Editor.HoverX < 0) { return; }
            if (!undo_set) { Core.Editor.SetUndo(2); undo_set = true; }
            selected_object.X = Core.Editor.HoverMX - offset_x;
            selected_object.Y = Core.Editor.HoverMY - offset_y;
        }

        public void MapSelectObject()
        {
            List<UObject> list = GetSelectedObject();
            if (list.Count > 0)
            {
                selected_object = list[0];
                undo_set = false;
                offset_x = Core.Editor.HoverMX - selected_object.X;
                offset_y = Core.Editor.HoverMY - selected_object.Y;
            }
        }

        //-------------------------------------------------------

        List<UObject> lvo_selected_objects = new List<UObject>();
        public List<UObject> SelectedObjects { get { return lvo_selected_objects; } }

        public List<UObject> GetSortedObjects()
        {
            if (Core.Editor.f.tscLVOSortMode.SelectedIndex == 0)
            {
                return Core.Map.Objects.OrderBy(o => o.ID).ToList();
            }
            else if (Core.Editor.f.tscLVOSortMode.SelectedIndex == 1)
            {
                return Core.Map.Objects.OrderBy(o => o.Layer).ThenBy(o => o.SubLayer).ThenBy(o => o.Z).ToList();
            }
            else
            {
                return Core.Map.Objects.OrderBy(o => o.Layer).ThenBy(o => o.SubLayer).ThenBy(o => o.Z).Reverse().ToList();
            }
        }

        public void RedrawList()
        {
            Core.Editor.f.listViewO.Items.Clear();
            var objects = GetSortedObjects();
            foreach (UObject o in objects)
            {
                if (!DisplayInList(o)) { continue; }
                ListViewItem lvi = new ListViewItem(o.ID.ToString());
                lvi.SubItems.Add((o.Layer + 1).ToString() + " - " + (o.SubLayer + 1).ToString());
                lvi.SubItems.Add("(" + o.X.ToString() + ", " + o.Y.ToString() + ")");
                lvi.SubItems.Add(o.Z.ToString());
                lvi.SubItems.Add(o.TileIndex.ToString() + " - " + o.TileID.ToString());
                lvi.SubItems.Add("(" + o.SpanX.ToString() + ", " + o.SpanY.ToString() + ")");
                Core.Editor.f.listViewO.Items.Add(lvi);
            }
        }

        public bool DisplayInList(UObject o)
        {
            if (Core.Editor.f.tsbLVOLayer.Checked)
            {
                if (!(o.Layer == Core.Editor.EditMode || Core.Editor.EditMode == 9)) { return false; }
            }
            if (Core.Editor.f.tsbLVOSLayer.Checked)
            {
                if (!CheckDisplaySubLayer(o.SubLayer)) { return false; }
            }
            return true;
        }

        public void ChangeDisplayLayer()
        {
            if (Core.Editor.f.tsbLVOLayer.Checked || Core.Editor.f.tsbLVOSLayer.Checked)
            {
                RedrawList();
            }
        }

        public bool CheckDisplaySubLayer(int i)
        {
            switch (i)
            {
                case 0: return Core.Editor.f.tsbS1.Checked && !Core.Editor.f.tsbSH1.Checked;
                case 1: return Core.Editor.f.tsbS2.Checked && !Core.Editor.f.tsbSH2.Checked;
                case 2: return Core.Editor.f.tsbS3.Checked && !Core.Editor.f.tsbSH3.Checked;
                case 3: return Core.Editor.f.tsbS4.Checked && !Core.Editor.f.tsbSH4.Checked;
                case 4: return Core.Editor.f.tsbS5.Checked && !Core.Editor.f.tsbSH5.Checked;
                case 5: return Core.Editor.f.tsbS6.Checked && !Core.Editor.f.tsbSH6.Checked;
            }
            return false;
        }

        public void LVOSelChange()
        {
            if (_lock) { return; }
            lvo_selected_objects.Clear();
            foreach (ListViewItem lvi in Core.Editor.f.listViewO.SelectedItems)
            {
                int id = Convert.ToInt32(lvi.Text);
                lvo_selected_objects.Add(Core.Map.Objects.Find(o => o.ID == id));
            }
            Core.Editor.RefreshMap();
        }

        public void RenewObjects()
        {
            List<int> list = new List<int>();
            foreach (UObject o in SelectedObjects)
            {
                list.Add(o.ID);
            }
            SelectedObjects.Clear();
            foreach (int i in list)
            {
                SelectedObjects.Add(Core.Map.Objects.Find(o => o.ID == i));
            }
        }

        bool undo_set;
        List<int> offsets_x = new List<int>();
        List<int> offsets_y = new List<int>();
        bool moving;

        public void LVOMapMouseDown()
        {
            if (!SelectedObjectInRange())
            {
                selecting = true;
                sel_sx = Core.Editor.HoverMX;
                sel_sy = Core.Editor.HoverMY;
                sel_ex = Core.Editor.HoverMX;
                sel_ey = Core.Editor.HoverMY;
                return;
            }
            moving = true;
            undo_set = false;
            offsets_x.Clear();
            offsets_y.Clear();
            foreach (UObject o in SelectedObjects)
            {
                offsets_x.Add(Core.Editor.HoverMX - o.X);
                offsets_y.Add(Core.Editor.HoverMY - o.Y);
            }
            Core.Editor.RefreshMap();
        }

        public bool SelectedObjectInRange()
        {
            foreach (UObject obj in GetSelectedObjectS())
            {
                if (SelectedObjects.Exists(o => o.ID == obj.ID)) { return true; }
            }
            return false;
        }

        public void LVOMapMove()
        {
            if (selecting)
            {
                if (Core.Editor.HoverMX < 0) { return; }
                sel_ex = Core.Editor.HoverMX;
                sel_ey = Core.Editor.HoverMY;
            }
            else
            {
                if (moving && MouseDownL)
                {
                    if (SelectedObjects.Count == 0 || Core.Editor.HoverX < 0) { return; }
                    if (!undo_set) { Core.Editor.SetUndo(2); undo_set = true; }
                    for (int i = 0; i < SelectedObjects.Count; i++)
                    {
                        var o = SelectedObjects[i];
                        o.X = Core.Editor.HoverMX - offsets_x[i];
                        o.Y = Core.Editor.HoverMY - offsets_y[i];
                    }
                }
            }
        }

        bool _lock;

        public void LVOMapUp()
        {
            _lock = true;
            if (selecting)
            {
                selecting = false;
                //Perform Selection
                var list = GetObjectsInRange();
                if (Core.Editor.ShiftDown) { LVOAddMapUp(list); }
                else if (Core.Editor.CtrlDown) { LVOSubMapUp(list); }
                else { LVONormalMapUp(list); }
                _lock = false;
                return;
            }
            moving = false;
            RedrawListX();
        }

        public void LVONormalMapUp(List<UObject> list)
        {
            SelectedObjects.Clear();
            Core.Editor.DeselectLVO();
            if (list.Count > 0)
            {
                foreach (UObject obj in list)
                {
                    SelectedObjects.Add(obj);
                    foreach (ListViewItem lvi in Core.Editor.f.listViewO.Items)
                    {
                        if (lvi.Text == obj.ID.ToString())
                        {
                            lvi.Selected = true;
                            Core.Editor.f.listViewO.EnsureVisible(lvi.Index);
                            break;
                        }
                    }
                }
            }
        }

        public void LVOAddMapUp(List<UObject> list)
        {
            if (list.Count > 0)
            {
                foreach (UObject obj in list)
                {
                    if (SelectedObjects.Exists(o => o.ID == obj.ID)){ continue; }
                    SelectedObjects.Add(obj);
                    foreach (ListViewItem lvi in Core.Editor.f.listViewO.Items)
                    {
                        if (lvi.Text == obj.ID.ToString())
                        {
                            lvi.Selected = true;
                            Core.Editor.f.listViewO.EnsureVisible(lvi.Index);
                            break;
                        }
                    }
                }
            }
        }

        public void LVOSubMapUp(List<UObject> list)
        {
            if (list.Count > 0)
            {
                foreach (UObject obj in list)
                {
                    if (!SelectedObjects.Exists(o => o.ID == obj.ID)) { continue; }
                    SelectedObjects.Remove(obj);
                    foreach (ListViewItem lvi in Core.Editor.f.listViewO.Items)
                    {
                        if (lvi.Text == obj.ID.ToString())
                        {
                            lvi.Selected = false;
                            break;
                        }
                    }
                }
            }
        }

        public bool ObjSelectedInRange(UObject o)
        {
            if (!(o.Layer == Core.Editor.EditMode || Core.Editor.EditMode == 9) || !CheckDisplaySubLayer(o.SubLayer)) { return false; }
            int w = o.SpanX * 32 - o.CropLeft - o.CropRight;
            int h = o.SpanY * 32 - o.CropTop - o.CropBottom;
            return !(o.X + w < sel_rx || o.Y + h < sel_ry || o.X > sel_rx + sel_w || o.Y > sel_ry + sel_h);
        }

        public bool ObjSelectedInRangeSL(UObject o)
        {
            if (!(o.Layer == Core.Editor.EditMode || Core.Editor.EditMode == 9) || !CheckDisplaySubLayer(o.SubLayer)) { return false; }
            if (o.SubLayer != GetSubLayer()) { return false; }
            int w = o.SpanX * 32 - o.CropLeft - o.CropRight;
            int h = o.SpanY * 32 - o.CropTop - o.CropBottom;
            return !(o.X + w < sel_rx || o.Y + h < sel_ry || o.X > sel_rx + sel_w || o.Y > sel_ry + sel_h);
        }

        public List<UObject> GetObjectsInRange()
        {
            var list = Core.Map.Objects.FindAll(o => ObjSelectedInRange(o)).ToList();
            return list.OrderBy(o => o.Z).OrderBy(o => o.ID).Reverse().ToList();
        }

        public void RedrawListX()
        {
            List<int> list = new List<int>();
            foreach (int i in Core.Editor.f.listViewO.SelectedIndices) { list.Add(i); }
            _lock = true;
            RedrawList();
            foreach (int i in list) { Core.Editor.f.listViewO.SelectedIndices.Add(i); }
            _lock = false;
        }

        public void UpdateLVOSingle(int id)
        {
            foreach (UObject o in SelectedObjects)
            {
                foreach (ListViewItem lvi in Core.Editor.f.listViewO.Items)
                {
                    if (lvi.Text == o.ID.ToString())
                    {
                        lvi.SubItems[1].Text = ((o.Layer + 1).ToString() + " - " + (o.SubLayer + 1).ToString());
                        lvi.SubItems[2].Text = ("(" + o.X.ToString() + ", " + o.Y.ToString() + ")");
                        lvi.SubItems[3].Text = (o.Z.ToString());
                        lvi.SubItems[4].Text = (o.TileIndex.ToString() + " - " + o.TileID.ToString());
                        lvi.SubItems[5].Text = ("(" + o.SpanX.ToString() + ", " + o.SpanY.ToString() + ")");
                        int index = lvi.Index;
                        Core.Editor.f.listViewO.RedrawItems(index, index, false);
                        break;
                    }
                }
            }
        }

        public void LVODoubleClick()
        {
            if (Core.Editor.f.listViewO.SelectedItems.Count == 0) { return; }
            int id = Convert.ToInt32(Core.Editor.f.listViewO.SelectedItems[0].Text);
            var obj = Core.Map.Objects.Find(o => o.ID == id);
            ObjectForm f_edit = new ObjectForm(obj);
            f_edit.ShowDialog();
            if (f_edit.success)
            {
                Core.Editor.SetUndo(2);
                obj = f_edit.obj;
                UpdateLVOSingle(obj.ID);
                Core.Editor.RefreshMap();
            }
            f_edit.Dispose();
        }

        public void LVOMapDoubleClick()
        {
            var list = GetSelectedObject();
            if (list.Count == 0) { return; }
            var obj = list[0];
            ObjectForm f_edit = new ObjectForm(obj);
            f_edit.ShowDialog();
            if (f_edit.success)
            {
                Core.Editor.SetUndo(2);
                obj = f_edit.obj;
                UpdateLVOSingle(obj.ID);
                Core.Editor.RefreshMap();
            }
            f_edit.Dispose();
        }

        public bool moved;

        public void LVOControlKeys(int i)
        {
            if (SelectedObjects.Count == 0) { return; }
            else if (i > 5 && SelectedObjects.Count != 1) { return; }
            Core.Editor.SetUndo(2);
            if (i >= 1 && i < 5) { moved = true; }
            switch (i)
            {
                case 1: LVOMoveUp(); break;
                case 2: LVOMoveDown(); break;
                case 3: LVOMoveLeft(); break;
                case 4: LVOMoveRight(); break;
                case 5: LVODelete(); break;
                case 6: ShiftCropY(false, true); break;
                case 7: ShiftCropY(true, true); break;
                case 8: ShiftCropX(false, true); break;
                case 9: ShiftCropX(true, true); break;
                case 10: ShiftCropY(false, false); break;
                case 11: ShiftCropY(true, false); break;
                case 12: ShiftCropX(false, false); break;
                case 13: ShiftCropX(true, false); break;
            }
        }

        public void LVODelete()
        {
            _lock = true;
            foreach (UObject o in SelectedObjects)
            {
                ListViewItem item = null;
                foreach (ListViewItem lvi in Core.Editor.f.listViewO.Items)
                {
                    if (lvi.Text == o.ID.ToString())
                    {
                        item = lvi;
                        break;
                    }
                }
                if (item == null) { return; }
                Core.Map.DeleteObject(o);
                Core.Editor.f.listViewO.Items.Remove(item);
            }
            _lock = false;
            SelectedObjects.Clear();
        }

        public void LVOMoveUp()
        {
            foreach (UObject obj in lvo_selected_objects)
            {
                if (Core.Editor.SpaceDown)
                {
                    obj.Z += 1;
                }
                else
                {
                    obj.Y -= 1;
                }
            }
        }

        public void LVOMoveDown()
        {
            foreach (UObject obj in lvo_selected_objects)
            {
                if (Core.Editor.SpaceDown)
                {
                    obj.Z -= 1;
                }
                else
                {
                    obj.Y += 1;
                }
            }
        }

        public void LVOMoveLeft()
        {
            foreach (UObject obj in lvo_selected_objects)
            {
                obj.X -= 1;
            }
        }

        public void LVOMoveRight()
        {
            foreach (UObject obj in lvo_selected_objects)
            {
                obj.X += 1;
            }
        }

        //Move Crop

        public void ShiftCropX(bool dir, bool left)
        {
            var o = SelectedObjects[0];
            if (left)
            {
                if ((!dir && o.CropLeft == 0) || (dir && ((o.CropLeft + o.CropRight) == o.SpanX * 32 - 4))) { return; }
                o.CropLeft += dir ? 1 : -1;
                o.X += dir ? 1 : -1;
            }
            else
            {
                if ((dir && o.CropRight == 0) || (!dir && ((o.CropLeft + o.CropRight) == o.SpanX * 32 - 4))) { return; }
                o.CropRight += dir ? -1 : 1;
            }
        }

        public void ShiftCropY(bool dir, bool top)
        {
            var o = SelectedObjects[0];
            if (top)
            {
                if ((!dir && o.CropTop == 0) || (dir && ((o.CropTop + o.CropBottom) == o.SpanY * 32 - 4))) { return; }
                o.CropTop += dir ? 1 : -1;
                o.Y += dir ? 1 : -1;
            }
            else
            {
                if ((dir && o.CropBottom == 0) || (!dir && ((o.CropTop + o.CropBottom) == o.SpanY * 32 - 4))) { return; }
                o.CropBottom += dir ? -1 : 1;
            }
        }

        //End

        public void LVOMapMouseRDown()
        {
            if (!SelectedObjectInRange())
            {
                return;
            }
            moving = true;
            undo_set = false;
            offsets_y.Clear();
            foreach (UObject o in SelectedObjects)
            {
                offsets_y.Add(-(Core.Editor.HoverMY / 10 * 10) - o.Z);
            }
        }

        public void LVOMapRMove()
        {
            if (moving && MouseDownR)
            {
                if (SelectedObjects.Count == 0 || Core.Editor.HoverX < 0) { return; }
                if (!undo_set) { Core.Editor.SetUndo(2); undo_set = true; }
                for (int i = 0; i < SelectedObjects.Count; i++)
                {
                    var o = SelectedObjects[i];
                    o.Z = (-(Core.Editor.HoverMY / 10 * 10)) - offsets_y[i];
                }
            }
        }

        //-------------------------------------------------------

        List<UObject> copy_list = new List<UObject>();

        public void PerformCut()
        {
            if (SelectedObjects.Count == 0) { return; }
            Core.Editor.SetUndo(2);
            Copy();
            LVODelete();
            Core.Editor.RefreshMap();
        }

        public void PerformCopy()
        {
            if (SelectedObjects.Count == 0) { return; }
            Copy();
        }

        public void Copy()
        {
            copy_list.Clear();
            foreach (UObject o in SelectedObjects)
            {
                copy_list.Add(o);
            }
        }

        public void PerformPaste()
        {
            if (copy_list.Count == 0) { return; }
            Core.Editor.SetUndo(2);
            _lock = true;
            SelectedObjects.Clear();
            foreach (UObject o in copy_list)
            {
                var obj = new UObject();
                obj.ID = Core.Map.GetNewObjectID();
                obj.X = o.X;
                obj.Y = o.Y;
                obj.Layer = Core.Editor.EditMode;
                obj.SubLayer = GetSubLayer();
                obj.TileIndex = o.TileIndex;
                obj.TileID = o.TileID;
                obj.SpanX = o.SpanX;
                obj.SpanY = o.SpanY;
                obj.CropTop = o.CropTop;
                obj.CropBottom = o.CropBottom;
                obj.CropLeft = o.CropLeft;
                obj.CropRight = o.CropRight;
                Core.Map.Objects.Add(obj);
                SelectedObjects.Add(obj);
            }
            Core.Map.SortObjectByID();
            RedrawList();
            foreach (UObject obj in SelectedObjects)
            {
                foreach (ListViewItem lvi in Core.Editor.f.listViewO.Items)
                {
                    if (lvi.Text == obj.ID.ToString())
                    {
                        lvi.Selected = true;
                        lvi.Focused = true;
                        Core.Editor.f.listViewO.EnsureVisible(lvi.Index);
                        break;
                    }
                }
            }
            _lock = false;
        }

        //-------------------------------------------------------
    }
}
