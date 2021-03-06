﻿using System;
using System.Windows.Forms;

namespace opencv
{
    public partial class SobelCoefficientPopUp : Form
    {
        public int XOrder, YOrder, WindowSize;
        public double Alpha;

        public SobelCoefficientPopUp()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            button1.Select();
        }

        public SobelCoefficientPopUp(bool isEdgeDetection)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            if (isEdgeDetection)
            {
                label4.Visible = false;
                textBox4.Visible = false;
                textBox4.Text = "1";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out XOrder) || 
                !int.TryParse(textBox2.Text, out YOrder) ||
                !int.TryParse(textBox3.Text, out WindowSize) ||
                !double.TryParse(textBox4.Text, out Alpha))
            {
                MessageBox.Show(@"输入错误");
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                if (WindowSize % 2 == 0)
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SobelCoefficientPopUp_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
            }
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        button1_Click(this, EventArgs.Empty);
                        break;
                }
            }
        }
    }
}
