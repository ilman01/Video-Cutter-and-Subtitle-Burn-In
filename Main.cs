using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace Video_Cutter_and_Subtitle_Burn_In
{
    public partial class Form1 : Form
    {
        public double Progress { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label3.Enabled = true;
                textBox3.Enabled = true;
                button4.Enabled = true;
            }
            else
            {
                label3.Enabled = false;
                textBox3.Enabled = false;
                button4.Enabled = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Select Input Video";
            openFile.Filter = "All Files|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string input = openFile.FileName;
                textBox1.Text = input;
                axWindowsMediaPlayer1.URL = textBox1.Text;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = textBox1.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Select Output File";
            saveFile.Filter = "All Files|*.*";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                string output = saveFile.FileName;
                textBox2.Text = output;

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Select Input Video";
            openFile.Filter = "All Files|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string input = openFile.FileName;
                textBox3.Text = input;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox4.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox5.Text = axWindowsMediaPlayer1.Ctlcontrols.currentPositionString;
        }

        private void textBox1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox2_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox3_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) & checkBox1.Checked == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }     
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                textBox1.Lines = fileNames;
                axWindowsMediaPlayer1.URL = textBox1.Text;
            }
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                textBox2.Lines = fileNames;
            }
        }

        private void textBox3_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                textBox3.Lines = fileNames;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            StartConvert();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            for (var i = 0; i <= progressBar1.Maximum; i++)
            {
                backgroundWorker1.ReportProgress(i);
            }
        }

        public void StartConvert()
        {
            Process process = new Process();
            Control.CheckForIllegalCrossThreadCalls = false;
            string arguements = "-y " + "-i " + "\"" + textBox1.Text + "\"" + " " + "\"" + textBox2.Text + "\"";
            process.StartInfo.FileName = Application.StartupPath + "/ffmpeg/ffmpeg.exe";
            process.StartInfo.Arguments = arguements;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            StreamReader streamReader = process.StandardError;
            while (!streamReader.EndOfStream)
            {
                getTotalSecondProcessed(streamReader.ReadLine());
            }

        }

        private void getTotalSecondProcessed(string line)
        {
            try
            {
                string[] split = line.Split(' ');
                foreach (var row in split)
                {
                    if (row.StartsWith("time="))
                    {
                        var time = row.Split('=');
                        Progress = TimeSpan.Parse(time[1]).TotalSeconds;
                    }
                }
            }
            catch
            {

            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
    }
}
