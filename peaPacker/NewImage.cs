using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace peaPacker
{
    public partial class NewImage : Form
    {
        
        public NewImage()
        {
            InitializeComponent();
            comboBoxSizes.Items.Clear();
            comboBoxSizes.Items.AddRange(new object[] { "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192", "16384" });

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            var form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            form1.CreateNewImage((int)numericUpDownHeight.Value, (int)numericUpDownWidth.Value);
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DropdownChanged(object sender, EventArgs e)
        {
            int value = Int32.Parse(comboBoxSizes.Items[comboBoxSizes.SelectedIndex].ToString());

            numericUpDownHeight.Value = value;
            numericUpDownWidth.Value = value;
        }

        private void buttonBgColor_Click(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            pictureBoxBgColor.BackColor = colorDialog.Color;
        }
    }
}
