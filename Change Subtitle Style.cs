using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Video_Cutter_and_Subtitle_Burn_In
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = Form1.subtitleStyle;
            if (Form1.encoder == "h264_nvenc")
            {
                checkBox1.Checked = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = Form1.defaultSubtitleStyle;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1.subtitleStyle = textBox1.Text;
            if (checkBox1.Checked == true)
            {
                Form1.encoder = "h264_nvenc";
            }
            else
            {
                Form1.encoder = "libx264";
            }
            this.Close();
        }
    }
}
