using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VAMX
{
    public abstract class EditMode
    {
        public abstract int Mode { get; }

        public abstract void Reset();

        public abstract void PTopDown(MouseEventArgs e);
        public abstract void PTopLeave();
        public abstract void PTopMove(MouseEventArgs e);
        public abstract void PTopUp(MouseEventArgs e);

        public abstract void PLowDown(MouseEventArgs e);
        public abstract void PLowLeave();
        public abstract void PLowMove(MouseEventArgs e);
        public abstract void PLowUp(MouseEventArgs e);

        public abstract void MapDown(MouseEventArgs e);
        public abstract void MapLeave();
        public abstract void MapMove(MouseEventArgs e);
        public abstract void MapUp(MouseEventArgs e);

        public abstract void MapMouseDownL();
        public abstract void MapMouseDownR();

    }
}
