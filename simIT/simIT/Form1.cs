﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace simIT
{
    public partial class Form1 : Form
    {
        private static ArrayList allFiles = new ArrayList();
        private static string userPicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        private static string simItFolder = userPicFolder + "\\simIT";
        private static string[] dirsInSimIt = Directory.GetDirectories(simItFolder);
        private static string[] acceptedfiles = { ".jpg", ".jpeg", ".tiff", ".gif", ".bmp", ".png" };
        private static Regex r = new Regex(":");

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            this.button1.Enabled = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            DirectoryInfo simItDir = Directory.CreateDirectory(simItFolder);

            getAllFiles(userPicFolder);

            this.progressBar1.Maximum = allFiles.Count;

            foreach (string file in allFiles)
            {
                DateTime fileDate = GetDateTakenFromImage(file);
                Console.WriteLine(file + " har dato " + fileDate.ToString("D"));
                Console.WriteLine(simItFolder + "\\" + fileDate.ToString("D"));
                addPictureToFolder(file, fileDate);
                this.progressBar1.Increment(1);
            }
            Console.WriteLine("maximum stuff ------------ " + progressBar1.Maximum);
            
            sw.Stop();
            TimeSpan timeUsed = sw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}", timeUsed.Minutes, timeUsed.Seconds);
            Console.WriteLine("Runtime: " + elapsedTime);
            
        }


        private static void getAllFiles(string parentFolder)
        {
            if (parentFolder != simItFolder)
            {
                try
                {
                    foreach (string dir in Directory.GetDirectories(parentFolder))
                    {
                        {
                            foreach (string file in Directory.GetFiles(dir))
                            {
                                if (checkIfAccepted(file))
                                {
                                    allFiles.Add(file);
                                }
                            }
                            getAllFiles(dir);
                        }

                    }
                }
                catch (Exception excpt)
                {
                    Console.WriteLine(excpt);
                }
            }
        }

        private static Boolean checkIfAccepted(string file)
        {
            string fileExtention = Path.GetExtension(file);
            if (acceptedfiles.Contains(fileExtention.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                try
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
                catch (Exception excpt)
                {
                    Console.WriteLine(excpt);
                }
                return DateTime.MinValue;
            }
        }

        private static void addPictureToFolder(string file, DateTime date)
        {
            bool foldExists = checkIfFolderExists(date);
            if (!foldExists)
            {
                DirectoryInfo dateDir = Directory.CreateDirectory(simItFolder + "\\" + date.ToString("D"));
            }
            string filename = Path.GetFileName(file);
            string destFile = Path.Combine(simItFolder + "\\" + date.ToString("D"), filename);

            if (!fileExists(destFile))
            {
                try
                {
                    File.Copy(file, destFile, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }



        }

        private static bool checkIfFolderExists(DateTime date)
        {
            if (dirsInSimIt.Contains(simItFolder + "\\" + date.ToString("D")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool fileExists(string destPath)
        {
            if (File.Exists(destPath))
            {
                Console.WriteLine("files does exists----------------------------------------------");
                return true;
            }
            else
            {
                Console.WriteLine("DO NOT EXIST----------------------------------------------");
                return false;
            }
        }
       
    }
}
