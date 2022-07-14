using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace peaPacker
{
    public partial class NewImage : Form
    {
        
        public NewImage()
        {
            InitializeComponent();
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
    }
}
