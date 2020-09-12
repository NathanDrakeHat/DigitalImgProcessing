﻿using System;
using System.Windows.Forms;

namespace opencv
{
    public partial class UniNoisePopUp : Form
    {
        public double Low;
        public double High;
        public UniNoisePopUp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text, out Low) || !double.TryParse(textBox2.Text, out High))
            {
                MessageBox.Show(@"输入错误");
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}