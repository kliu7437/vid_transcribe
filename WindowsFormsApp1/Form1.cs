using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NReco.VideoConverter;
using NAudio.Wave;
using NLayer.NAudioSupport;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using CognitiveServicesAuthorization;
using Microsoft.Bing.Speech;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        string InputFile;
        string OutputFile;
        public string lang;
        public string length;

        public Form1()
        {
            InitializeComponent();
            lang = "en-US";
            length = "";
        }

        public static object GetItemValue(ListControl list, object item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrEmpty(list.ValueMember))
                return item;

            var property = TypeDescriptor.GetProperties(item)[list.ValueMember];
            if (property == null)
                throw new ArgumentException(
                    string.Format("item doesn't contain '{0}' property or column.",
                    list.ValueMember));
            return property.GetValue(item);
        }

        private void label1_Click(object sender, EventArgs e)
        { }

        //Browse for video file
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Video File Only|*.mp4";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    InputFile = dialog.FileName;
                    OutputFile = InputFile.Substring(0, InputFile.IndexOf("."));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An Error Occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error
                    );
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        //Generate Transcript Button
        private void button2_Click(object sender, EventArgs e)
        {
            string outputfileName = OutputFile + ".mp3";

            //extract mp3 from mp4 file
            var ConvertVideo = new NReco.VideoConverter.FFMpegConverter();
            ConvertVideo.ConvertMedia(InputFile, outputfileName, "mp3");

            //convert mp3 to wav file
            string InputAudioFilePath = outputfileName;
            string OutputAudioFilePath = @"C:\Users\kliu7\Documents\Bitcamp2018\output.wav";

            Console.WriteLine(InputAudioFilePath);


            using (Mp3FileReader reader = new Mp3FileReader(InputAudioFilePath, wf => new Mp3FrameDecompressor(wf)))
            {
                WaveFileWriter.CreateWaveFile(OutputAudioFilePath, reader);
            }

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        //language dropdown
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ("en-us".Equals(listBox1.SelectedItem.ToString()))
            {
                lang = "en-us";
            }
            else if ("en-uk".Equals(listBox1.SelectedItem.ToString()))
            {
                lang = "en-uk";
            }
            else if ("fr-fr".Equals(listBox1.SelectedItem.ToString()))
            {
                lang = "fr-fr";
            }
            else
            {
                MessageBox.Show("Item is not available.");
            }
        }

        public string getLang()
        {
            return lang;
        }

        public string getLength()
        {
            return length;
        }


        public string setLang(String lang)
        {
            this.lang = lang;
            return lang;
        }

        public string getLength(String length)
        {
            this.length = length;
            return length;
        }


        private void label3_Click(object sender, EventArgs e)
        {

        }

        //video length drop down
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ("short".Equals(listBox2.SelectedItem.ToString()))
            {
                length = "short";
            }
            else if ("long".Equals(listBox2.SelectedItem.ToString()))
            {
                length = "long";
            }
            else
            {
                MessageBox.Show("Item is not available.");
            }
        }


        }
}

