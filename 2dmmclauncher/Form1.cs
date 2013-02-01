using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Net;
using System.Threading;
using System.Management;


namespace _2dmmclauncher
{
    public partial class Form1 : Form
    {
        public Process launcher = new Process();
        bool normalExit = true;
        public string playername;
        public string javaxmx;
        public string javaw;
        public Form1()
        {
            InitializeComponent();
        }
       
        private void GameExit(object sender, EventArgs e)
        {
            //MessageBox.Show("w");

            try
            {
                File.Delete("ver.txt");
                File.Delete(".minecraft\\bin\\ver.txt");
                File.Delete(".minecraft\\RMCAC");
                File.Delete(".minecraft\\RMCAL");
                File.Delete(".minecraft\\RMCAOK");
                foreach(string f in Directory.GetFiles(".minecraft","ForgeModLoader*",SearchOption.TopDirectoryOnly))
                {
                    File.Delete(f);
                }
            }
            catch { }
            if (normalExit==true)
            {
                notifyIcon1.Visible = false;
                Environment.Exit(0);
            }
        }

        private void erdmmc(string PlayerName,string  JavaXmx,string javaw)
        {
            
            launcher.StartInfo.UseShellExecute = false;
            launcher.StartInfo.FileName = javaw;
            launcher.StartInfo.WorkingDirectory = Environment.CurrentDirectory+"\\.minecraft\\bin";
            Environment.SetEnvironmentVariable("APPDATA", Environment.CurrentDirectory);
            launcher.StartInfo.Arguments = "-Xincgc -Xmx" + JavaXmx + "M -XX:PermSize=64m -XX:MaxPermSize=128m " + "-Dsun.java2d.noddraw=true -Dsun.java2d.pmoffscreen=false -Dsun.java2d.d3d=false -Dsun.java2d.opengl=false -cp \"" + Environment.CurrentDirectory + "\\.minecraft\\bin\\minecraft.jar;" + Environment.CurrentDirectory + "\\.minecraft\\bin\\lwjgl.jar;" + Environment.CurrentDirectory + "\\.minecraft\\bin\\lwjgl_util.jar;" + Environment.CurrentDirectory + "\\.minecraft\\bin\\jinput.jar\" -Djava.library.path=\"" + Environment.CurrentDirectory + "\\.minecraft\\bin\\natives\" net.minecraft.client.Minecraft " + PlayerName;

            launcher.EnableRaisingEvents = true;
            launcher.Exited += new EventHandler(GameExit);
            launcher.Start();
            Thread.Sleep(5000);
            timer1.Enabled = true;
        }


        private void delrmcafile()
        {
            while (File.Exists(".minecraft\\RMCAC")==false)
            {
                Thread.Sleep(100);
            }
            File.Delete(".minecraft\\RMCAC");
            Application.ExitThread();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            Control.CheckForIllegalCrossThreadCalls = false;
            if(Directory.Exists(".minecraft"))
            {
                StreamWriter RMCALFile = new StreamWriter(".minecraft\\RMCAL");
                RMCALFile.Close();
                RMCALFile.Dispose();
                Thread tdelrmcacfile = new Thread(new ThreadStart(delrmcafile));
                tdelrmcacfile.Start();
                StreamWriter RMCAOKFile = new StreamWriter(".minecraft\\RMCAOK");
                RMCAOKFile.Close();
                RMCAOKFile.Dispose();
            }
        }

        private void loadconfig()
        {
            if (File.Exists("2dmmclauncher.cfg"))
            {
                StreamReader cfg = new StreamReader("2dmmclauncher.cfg");
                playername = cfg.ReadLine();
                javaxmx = cfg.ReadLine();
                javaw = cfg.ReadLine();
            }
            else
            {
                StreamWriter cfg = new StreamWriter("2dmmclauncher.cfg");
                playername = Interaction.InputBox("请输入用户名(仅影响单机模式，用于单机模式获取皮肤)", "用户名", "Player");
                cfg.WriteLine(playername);
                double capacity = 0.0;
                ManagementClass cimobject1 = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection moc1 = cimobject1.GetInstances();
                foreach (ManagementObject mo1 in moc1)
                {
                    capacity += ((Math.Round(Int64.Parse(mo1.Properties["Capacity"].Value.ToString()) / 1024 / 1024.0, 1)));
                }
                moc1.Dispose();
                cimobject1.Dispose();
                int qmem = Convert.ToUInt16(capacity.ToString()) / 4;
                if (qmem < 512)
                {
                    qmem = 512;
                }
                //MessageBox.Show((MemInfo.dwAvailPhys / 4096 / 1024).ToString());
                javaxmx = qmem.ToString ();
                cfg.WriteLine(javaxmx);
                {
                    RegistryKey lm = Registry.LocalMachine;
                    RegistryKey sf = lm.OpenSubKey("SOFTWARE");
                    RegistryKey js = sf.OpenSubKey("JavaSoft");
                    RegistryKey jre = js.OpenSubKey("Java Runtime Environment");
                    RegistryKey reg = Registry.LocalMachine;
                    reg = reg.OpenSubKey("SOFTWARE").OpenSubKey("JavaSoft").OpenSubKey("Java Runtime Environment");

                    bool flag = false;
                    foreach (string ver in jre.GetSubKeyNames())
                    {
                        try
                        {
                            RegistryKey command = jre.OpenSubKey(ver);
                            string str = command.GetValue("JavaHome").ToString();
                            if (str != "")
                            {
                                javaw=str + @"\bin\javaw.exe";
                                flag = true;
                                break;
                            }
                        }
                        catch { }

                    }
                    if (!flag)
                    {
                        MessageBox.Show("获取javaw.exe目录失败，请手动查找");
                        OpenFileDialog javawp = new OpenFileDialog();
                        javawp.Multiselect = false;
                        javawp.Title = "请选择javaw.exe";
                        javawp.Filter = "javaw.exe|javaw.exe";
                        if (javawp.ShowDialog() == DialogResult.OK)
                        {
                            javaw = javawp.FileName;
                        }
                    }
                    

                }
                cfg.WriteLine(javaw);
                cfg.Close();
            }

        }
        private void checkupdate()
        {
            WebClient verc = new WebClient();
            verc.DownloadFile("http://2dmmc.bangbang93.com/ver.txt", "ver.txt");
            Thread.Sleep(10000);
            try
            {
                StreamReader ver = new StreamReader("ver.txt");
                StreamReader cver = new StreamReader(".minecraft\\bin\\ver.txt");
                string vers = ver.ReadLine();
                string isMust = ver.ReadLine();
                string updateurl = ver.ReadLine();
                string cvers = cver.ReadLine();
                ver.Close();
                cver.Close();
                Application.DoEvents();
                if (vers != cvers)
                {
                    if (isMust == "1")
                    {
                        if (MessageBox.Show("检测到强制更新版本，点击确定更新", "检测更新", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            this.Hide();
                            normalExit = false;
                            launcher.Kill();
                            downloader.ShowDialog();
                            downloader.Close();
                            Application.Restart();
                        }
                        else
                        {
                            if (MessageBox.Show("不更新可能会导致无法进入服务器，确定不更新？", "确定不更新？", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                            {
                                this.Hide();
                                normalExit = false;
                                launcher.Kill();
                                downloader.ShowDialog();
                                downloader.Close();
                                Application.Restart();
                            }
                        }
                    }
                    else
                    {
                        if (MessageBox.Show("检测到版本更新，是否更新？当前版本：" + cvers + "更新版本" + vers, "检测到更新", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            //this.Hide();
                            normalExit = false;
                            launcher.Kill();
                            downloader.ShowDialog();
                            downloader.Close();
                            Application.Restart();
                        }
                    }
                }
                else
                {
                    notifyIcon1.ShowBalloonTip(10, "废话二次元启动器", "已是最新版本", System.Windows.Forms.ToolTipIcon.Info);
                }
            }
            catch (FileNotFoundException )
            {
                MessageBox.Show("获取本地版本号失败");
            }
            catch (WebException)
            {
                MessageBox.Show("网络错误，获取最新版本号失败");
            }
            Application.ExitThread();
        }
        downloadForm downloader = new downloadForm();
        private void checkgame()
        {
            if (File.Exists(".minecraft\\bin\\minecraft.jar") == false)
            {
                if (MessageBox.Show("游戏不存在，是否下载？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    this.Hide();
                    downloader.ShowDialog();
                    downloader.Close();
                }
                else
                {
                    Environment.Exit(0);
                }
                
            }
        }
        private void downloadcfg()
        {
            WebClient cfg = new WebClient();
            cfg.DownloadFile("http://2dmmc.bangbang93.com/RMCAClien1.0", ".minecraft\\RMCAuthServer.txt");
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            label1.Text = "检测游戏";
            checkgame();
            progressBar1.Value += 1;
            label1.Text = "下载游戏配置文件";
            downloadcfg();
            progressBar1.Value+=1;
            label1.Text = "正在加载配置文件";
            loadconfig();
            progressBar1.Value += 1;
            label1.Text = "正在启动游戏";
            erdmmc(playername, javaxmx, javaw);
            progressBar1.Value += 1;
            label1.Text = "正在检查更新";
            Thread tCheckUpdate = new Thread(new ThreadStart(checkupdate));
            tCheckUpdate.Start();
            progressBar1.Value += 1;
            while (tCheckUpdate.ThreadState == System.Threading.ThreadState.Running)
            {
                Thread.Sleep(1000);
            }
            this.Hide();
        }

        private void About_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        private void runState_Click(object sender, EventArgs e)
        {
            runState rs = new runState(this );
            rs.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string f = @".minecraft\ForgeModLoader-0.log";
            DateTime lastct = DateTime.MinValue;
            foreach (string F in Directory.GetFiles(".minecraft", "ForgeModLoader*.log", SearchOption.TopDirectoryOnly))
            {
                DateTime dt = File.GetLastWriteTime(F);
                if (dt.Day != DateTime.Now.Day)
                {
                    continue;
                }
                if (dt > lastct)
                {
                    f = F;
                    lastct = dt;
                }
            }
            System.IO.FileStream MClog = new System.IO.FileStream(f, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            StreamReader mclog = new StreamReader(MClog, Encoding.Default);
            string log = mclog.ReadToEnd();
            if (log.Contains("SEVERE"))
            {
                timer1.Enabled = false;
                int index = log.IndexOf("SEVERE");
                errorReport er = new errorReport(log.Substring(index-1));
                er.ShowDialog();
            }
        }
        public long getworkset()
        {
            launcher.Refresh();
            return launcher.WorkingSet64;
        }




    }
}
