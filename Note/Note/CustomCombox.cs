using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication6 {
    class CustomCombox : ComboBox {
        protected override void OnDropDown(EventArgs e) {
            base.OnDropDown(e);
            AdjustComboBoxDropDownListWidth();
        }

        private void AdjustComboBoxDropDownListWidth() {
            int vertScrollBarWidth = (this.Items.Count > this.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;

            int maxWidth = this.DropDownWidth;
            foreach (var layouts in this.Items) {
                int measureTextWidth = TextRenderer.MeasureText(layouts.ToString(), this.Font).Width;
                maxWidth = maxWidth < measureTextWidth ? measureTextWidth : maxWidth;
            }

            this.DropDownWidth = maxWidth + vertScrollBarWidth;
        }
    }
}
