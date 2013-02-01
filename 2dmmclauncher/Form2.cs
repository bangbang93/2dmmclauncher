﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace _2dmmclauncher
{
    public partial class downloadForm : Form
    {
        public downloadForm()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (File.Exists("2dmmc.dat"))
            {
                WebClient cmd5 = new WebClient();
                if (cmd5.DownloadString("http://2dmmc.bangbang93.com/cmd5.txt").Substring(0, 32) != GetMD5HashFromFile("2dmmc.dat").ToUpper())
                {
                    MessageBox.Show("找到下载文件，但是与服务器MD5效验不同，重新下载");
                    File.Delete("2dmmc.dat");
                }
                else
                {
                    MessageBox.Show("找到下载文件，校验通过，开始解压");
                    WebClient d7z = new WebClient();
                    d7z.DownloadFile("http://image.bangbang93.com/7za.exe", "unpakcer.exe");
                    if (File.Exists("2dmmc.dat") == true)
                    {
                        Process un7z = new Process();
                        un7z.StartInfo.FileName = "unpakcer.exe";
                        un7z.StartInfo.Arguments = "x -y 2dmmc.dat";
                        //un7z.Exited += new EventHandler(un7z_Exited);
                        un7z.Start();
                        un7z.WaitForExit();
                        File.Delete("unpakcer.exe");
                        this.Close();
                    }
                }
            }
            if (File.Exists("2dmmc.dat.cfg"))
            {
                File.Delete("2dmmc.dat.cfg");
            }
            if (File.Exists("2dmmc.dat.tfg"))
            {
                File.Delete("2dmmc.dat.tfg");
            }
        }
        downloader game = new downloader(@"http://image.bangbang93.com/2dmmc.7z");
        //downloader game = new downloader(@"file:\\E:\新建文件夹\2dmmc1.7z");
        public string stat = "unfinish";

        WebClient downl = new WebClient();
        private void downloadForm_Shown(object sender, EventArgs e)
        {
            //MessageBox.Show("w");
            game.ThreadCount = 5;
            game.Filename = "2dmmc.dat";
            game.DirectoryName = Environment.CurrentDirectory;
            game.Progress += new downloader.ProgressEventHandler(game_Progress);
            game.Finished += new downloader.FinishedEventHandler(game_Finished);
            game.Exception += new downloader.ExceptionEventHandler(game_Exception);
            game.Speed += new downloader.SpeedHandler(game_Speed);
            game.Start();
            game.Connected += new downloader.ConnectedEventHandler(game_Connected);
            
        }

        void game_Connected(downloader sender, string filename, string contentType)
        {
            size.Text = ((double)game.ContentLength/1024.0/1024.0).ToString("f")+"MB";
            //throw new NotImplementedException();
        }

        void game_Exception(downloader sender, Exception e)
        {
            
            MessageBox.Show("下载失败，请重试"+game.Url+"\n"+e.Message );
            Environment.Exit(0);
            throw new NotImplementedException();
        }
        
        void game_Speed(downloader sender)
        {
            speed.Text = sender.SpeedStr;
            //throw new NotImplementedException();
        }
        void game_Finished(downloader sender)
        {
            WebClient cmd5 = new WebClient();
            if (cmd5.DownloadString("http://2dmmc.bangbang93.com/cmd5.txt").Substring(0,32)!=GetMD5HashFromFile("2dmmc.dat").ToUpper())
            {
                MessageBox.Show("下载错误,请重试" + cmd5.DownloadString("http://2dmmc.bangbang93.com/cmd5.txt") + "\n" + GetMD5HashFromFile("2dmmc.dat").ToUpper());
                Environment.Exit(0);
            }
            WebClient d7z=new WebClient();
            d7z.DownloadFile("http://image.bangbang93.com/7za.exe", "unpakcer.exe");
            if (File.Exists("2dmmc.dat") == true)
            {
                Process un7z = new Process();
                un7z.StartInfo.FileName = "unpakcer.exe";
                un7z.StartInfo.Arguments = "x -y 2dmmc.dat";
                //un7z.Exited += new EventHandler(un7z_Exited);
                un7z.Start();
                un7z.WaitForExit();
                File.Delete("unpakcer.exe");
                this.Close();
             }
                else
                {
                    MessageBox.Show("找不到下载的文件");
                    Environment.Exit(0);
                 }
            stat = "finish";
        }

        void un7z_Exited(object sender, EventArgs e)
        {
            Thread.Sleep(2000);
            throw new NotImplementedException();
        }

        void game_Progress(downloader sender)
        {
            progressBar1.Value = sender.FinishedRate;
            prog.Text = sender.FinishedRate.ToString()+"%";
            //speed.Text = sender.SpeedStr;
            //throw new NotImplementedException();
        }

        private void buttonCancal_Click(object sender, EventArgs e)
        {
            game.Stop();
            Environment.Exit(0);
        }
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

    }
}
