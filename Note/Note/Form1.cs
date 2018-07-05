using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication6 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            decimal d = 0.22222m;
            string str = d.ToString("P");

            textBox1.Text = str;

            int maxWidth = 100;
            foreach (var layouts in this.toolStripComboBox1.Items) {
                int measureTextWidth = TextRenderer.MeasureText(layouts.ToString(), this.Font).Width;
                maxWidth = maxWidth < measureTextWidth ? measureTextWidth : maxWidth;
            }

            this.toolStripComboBox1.DropDownWidth = maxWidth;
        }

        private void button1_Click(object sender, EventArgs e) {
            PopuForm form = new PopuForm("shenbao", DateTime.Now, "股票大涨了！！！");
            form.Show();
        }
    }
}
