﻿using Catswords.DataType.Client.Helper;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Catswords.DataType.Client
{
    public partial class UserControl1 : UserControl
    {
        private ImageList imageList = new ImageList();

        public string FilePath;
        public string FileMagic;
        public string FileName;
        public string FileExtension;

        public UserControl1(Form parent)
        {
            InitializeComponent();

            // Set image size
            imageList.Images.Add(Properties.Resources.data_database_icon_177024);
            imageList.Images.Add(Properties.Resources.message_bubble_conversation_speech_communication_talk_chat_icon_219299);
            imageList.Images.Add(Properties.Resources._2333410_android_os_smartphone_85588);
            imageList.Images.Add(Properties.Resources.office_18907);
            imageList.Images.Add(Properties.Resources.link_symbol_icon_icons_com_56927);
            imageList.Images.Add(Properties.Resources.tags_icon_icons_com_73382);

            // set image list
            listView1.SmallImageList = imageList;

            // Store the file path.
            FilePath = OpenFileDialog();
            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("Failed to get a file name", "Catswords.DataType.Client");
                parent.Close();
                return;
            }

            // Get first 4 bytes from the file.
            var extractor = new FileMagicExtractor(FilePath);
            FileMagic = extractor.GetString();

            // Show file magic to the label
            label1.Text = "#0x" + FileMagic;
            if (extractor.GetError() != null)
            {
                ShowStatus(extractor.GetError());
            }

            // Get file name and file extension
            try
            {
                FileExtension = Path.GetExtension(FilePath);
                FileName = Path.GetFileName(FilePath);
                if (FileExtension.Length > 0 && FileExtension.Substring(0, 1) == ".")
                {
                    FileExtension = FileExtension.Substring(1);
                }
            }
            catch
            {
                FileExtension = "";
                FileName = "";
            }

            // Extract
            var worker = new Worker(this);
            worker.FromFileExtension();    // Get data from file extension database
            worker.FromAndroidManifest();    // Get data from Android manifest
            worker.FromTimeline();   // Get data from timeline
            worker.FromLinks();  // Get links from file
            worker.FromExif();  //  Get EXIF tags from file
        }

        public string OpenFileDialog()
        {
            string filePath = null;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                }
            }

            return filePath;
        }

        public void AddIndicator(DateTime dt, string Description, int ImageIndex)
        {
            listView1.Items.Add(new ListViewItem(new string[] { dt.ToString(), Description }, ImageIndex));
        }

        public void ShowStatus(string status)
        {
            textBox1.Text = status;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://catswords.social/auth/sign_up");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form newForm = new Form2(this);
            newForm.Show();
        }
    }
}
