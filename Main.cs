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
using System.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Video_Cutter_and_Subtitle_Burn_In
{
    public partial class Form1 : Form
    {
        public double Progress { get; set; }
        
        bool currentlyProcessing;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
            label6.Text = "Ready";
            System.IO.Directory.CreateDirectory(Application.StartupPath + "/ffmpeg");
            currentlyProcessing = false;
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
            if (File.Exists("./ffmpeg/ffmpeg.exe")) { }
            else
            {
                MessageBox.Show("FFmpeg not detected");
                return;
            }

            if (currentlyProcessing == true)
            {
                MessageBox.Show("Please wait for the current one to finish.");
                return;
            }

            backgroundWorker1.RunWorkerAsync();
            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            StartConvert();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
        }

        public async void StartConvert()
        {
            currentlyProcessing = true; 
            
            string inputFile = "\"" + textBox1.Text + "\"";
            string outputFile = "\"" + textBox2.Text + "\"";
            string inputSubtitleFile = "\"" + textBox3.Text + "\"";
            string quality = textBox6.Text;
            string gain = textBox7.Text;
            string startTime = textBox4.Text;
            string endTime = textBox5.Text;

            string argExtractSub = null;
            if (checkBox1.Checked == true)
            {
                argExtractSub = "-y " + "-i " + inputSubtitleFile + " -ss " + startTime + " -to " + endTime + " -map 0:s:0 subtitle.srt";
            }
            else
            {
                argExtractSub = "-y " + "-i " + inputFile + " -ss " + startTime + " -to " + endTime + " -map 0:s:0 subtitle.srt";
            }

            string argVideoProc = "-y " + "-ss " + startTime + " -to " + endTime + " -i " + inputFile + " -map 0:v -map 0:a -c:v libx264 -b:a 320k -ac 2 -qp " + quality + " -filter:a \"volume=" + gain + "dB\" -vf \"subtitles=subtitle.srt:force_style='Fontsize=20,BorderStyle=4,BackColour=&H80000000&,Outline=0,FontName=Bahnschrift Light'\" " + outputFile;

            Process process = new Process();
            Control.CheckForIllegalCrossThreadCalls = false;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            progressBar1.Value = 0;
            process.StandardInput.WriteLine("cd ffmpeg");
            label6.Text = "Processing... Please Wait.";
            
            progressBar1.Value = 25;
            process.StandardInput.WriteLine("ffmpeg.exe " + argExtractSub);

            process.StandardInput.Flush();
            process.StandardInput.Close();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            Console.WriteLine(output);
            Console.WriteLine(error);
            process.WaitForExit();

            Process process2 = new Process();
            process2.StartInfo.FileName = "cmd.exe";
            process2.StartInfo.UseShellExecute = false;
            process2.StartInfo.RedirectStandardError = false;
            process2.StartInfo.RedirectStandardOutput = false;
            process2.StartInfo.RedirectStandardInput = true;
            process2.StartInfo.CreateNoWindow = true;


            progressBar1.Value = 50;

            process2.Start();
            process2.StandardInput.WriteLine("cd ffmpeg");
            process2.StandardInput.WriteLine("ffmpeg.exe " + argVideoProc);

            process2.StandardInput.Flush();
            process2.StandardInput.Close();

            process2.WaitForExit();

            progressBar1.Value = 100;
            label6.Text = "Done!";
            currentlyProcessing = false;
            /*StreamReader streamReader = process.StandardError;
            while (!streamReader.EndOfStream)
            {
                getTotalSecondProcessed(streamReader.ReadLine());
            }*/

        }

        /*private void getTotalSecondProcessed(string line)
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
        }*/

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (File.Exists("./ffmpeg/ffmpeg.exe"))
            {
                MessageBox.Show("FFmpeg already downloaded.");
            }
            else
            {
                /*using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/ilman01/Video-Cutter-and-Subtitle-Burn-In/raw/main/ffmpeg.exe", "./ffmpeg/ffmpeg.exe");
                }*/
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileAsync(
                        // Param1 = Link of file
                        new System.Uri("https://github.com/ilman01/Video-Cutter-and-Subtitle-Burn-In/raw/main/ffmpeg.exe"),
                        // Param2 = Path to save
                        "./ffmpeg/ffmpeg.exe"
                    );
                }
            }
        }
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            label6.Text = "Downloading FFmpeg: " + e.ProgressPercentage.ToString() + "%";

            if (e.ProgressPercentage == 100)
            {
                label6.Text = "Ready";
            }
        }

    }
}
