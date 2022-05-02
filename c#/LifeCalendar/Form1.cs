using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LifeCalendar
{
    public partial class Form1 : Form
    {
        private const string phpURL= "http://shitao.tech/index.php";
        private int borderSize = 2;
        private Size formSize;
        private static DateTime birth = new DateTime();
        private static DateTime nowDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        private static DateTime firstSunday = new DateTime();
        private int yearPast;
        private int weekPast;
        private int lifeExpectancy = 0;
        private int initializedBig = 0;
        private int drawnBig = 0;
        private int lastW;
        private int lastH;
        private int loginStatus = 0;//1: logged in
        private int id=0;
        private int lastT = 0;
        private int[] widX = new int[52];
        private int[] wid = new int[52];
        private bool drawing = false;
        private int border = 0;
        private int wids = 0;
        private int selectYear = -1;
        private int selectWeek = -1;//no selection
        private int[] blinkColorSet = new int[712];
        private int blinkColorIndex = 0;
        private Color blinkColor = new Color();
        private int todayX=0, todayY=0;
        private int lastX = -1;
        private int lastY = -1;
        private Bitmap bmpWeekly;
        private string[] months = { "", "January", "Feburary", "March", "April", "May", "June","July","August","September","October","November","December" };
        public class AlterWeekly
        {
            public int count =new int();
            public int[] id=new int[5252];
            public int[] X=new int[5252];
            public int[] Y=new int[5252];
            public Color[] color=new Color[5252];
        }
        public class AlterWeekly100
        {
            public int count = new int();
            public int[] id = new int[100];
            public int[] X = new int[100];
            public int[] Y = new int[100];
            public Color[] color = new Color[100];
        }
        private AlterWeekly alterWeekly=new AlterWeekly();
        private AlterWeekly100 alterWeekly100 = new AlterWeekly100();
        private int alterCount = 0;
        private Color pickIdeal = Color.FromArgb(255, 59, 149, 27);
        private Color pickGood1 = Color.FromArgb(255, 254, 242, 88);
        private Color pickGood2 = Color.FromArgb(255, 74, 74, 255);
        private Color pickNotGreat1 = Color.FromArgb(255, 255, 128, 0);
        private Color pickNotGreat2 = Color.FromArgb(255, 64, 0, 128);
        private Color pickFailure = Color.FromArgb(255, 153, 153, 153);
        public class ModuleSelect
        {
            public int id;
            public Color color;
        }
        private ModuleSelect moduleSelect = new ModuleSelect();

        public class PullLetters
        {
            public int count;
            public int current;
            public int lastK;
            public string[] id = new string[1024]; 
            public string[] title= new string[1024];
            public string[] content=new string[1024];
            public string[] time= new string[1024];
        }
        private PullLetters pullLetters = new PullLetters();
        


        private Panel[] panelPulls;
        private Label[] labelPullTitles;
        private Label[] labelPullContents;
        private Label[] labelPullTimes;
        public Form1()
        {
            InitializeComponent();
            this.Padding = new System.Windows.Forms.Padding(borderSize);
            this.BackColor = panelMenu.BackColor;
            this.DoubleBuffered = true;
            panelPulls = new Panel[] {panelPull1,panelPull2,panelPull3,panelPull4, panelPull5, panelPull6, panelPull7, panelPull8, panelPull9, panelPull10, panelPull11, panelPull12, panelPull13, panelPull14, panelPull15, panelPull16, panelPull17, panelPull18, panelPull19, panelPull20};
            labelPullTitles = new Label[] {labelPullHeading1 , labelPullHeading2, labelPullHeading3, labelPullHeading4, labelPullHeading5, labelPullHeading6, labelPullHeading7, labelPullHeading8, labelPullHeading9, labelPullHeading10, labelPullHeading11, labelPullHeading12, labelPullHeading13, labelPullHeading14, labelPullHeading15, labelPullHeading16, labelPullHeading17, labelPullHeading18, labelPullHeading19, labelPullHeading20 };
            labelPullContents = new Label[] {labelPullContent1, labelPullContent2, labelPullContent3, labelPullContent4, labelPullContent5, labelPullContent6, labelPullContent7, labelPullContent8, labelPullContent9, labelPullContent10, labelPullContent11, labelPullContent12, labelPullContent13, labelPullContent14, labelPullContent15, labelPullContent16, labelPullContent17, labelPullContent18, labelPullContent19, labelPullContent20 };
            labelPullTimes = new Label[] { labelPullTime1, labelPullTime2, labelPullTime3, labelPullTime4, labelPullTime5, labelPullTime6, labelPullTime7, labelPullTime8, labelPullTime9, labelPullTime10, labelPullTime11, labelPullTime12, labelPullTime13, labelPullTime14, labelPullTime15, labelPullTime16, labelPullTime17, labelPullTime18, labelPullTime19, labelPullTime20 };
            pullLetters.count = 2;
            for(int i = 0; i < 20; i++)
            {
                panelPulls[i].Visible = false;
                labelPullTitles[i].Text = "";
                labelPullTitles[i].Visible = true;
                labelPullTimes[i].Visible = true;
                labelPullContents[i].Visible = true;

                labelPullContents[i].Text = "";
                labelPullTimes[i].Text = "";
                labelPullTitles[i].Parent = panelPulls[i];
                labelPullTimes[i].Parent = panelPulls[i];
                labelPullContents[i].Parent = panelPulls[i];
                labelPullTitles[i].Location = new Point(41,-6);
                labelPullContents[i].Location = new Point(11,43);
                labelPullTimes[i].Location = new Point(408,-5);

            }

        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL",EntryPoint ="SendMessage")]
        private static extern void SendMessage(System.IntPtr hWnd,int wM ,int wParam, int lParam);
        private void panelDesktop_Paint(object sender, PaintEventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
            AdjustColor();
            lastH = this.Height;
            lastW = this.Width;
            //initializing labels
            labelColumn5.AutoSize = false;
            labelColumn5.Text = "";
            labelColumn5.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn5.ForeColor = Color.White;
            labelColumn5.BackColor = Color.Transparent;
            labelColumn10.AutoSize = false;
            labelColumn10.Text = "";
            labelColumn10.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn10.ForeColor = Color.White;
            labelColumn10.BackColor = Color.Transparent;
            labelColumn15.AutoSize = false;
            labelColumn15.Text = "";
            labelColumn15.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn15.ForeColor = Color.White;
            labelColumn15.BackColor = Color.Transparent;
            labelColumn20.AutoSize = false;
            labelColumn20.Text = "";
            labelColumn20.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn20.ForeColor = Color.White;
            labelColumn20.BackColor = Color.Transparent;
            labelColumn25.AutoSize = false;
            labelColumn25.Text = "";
            labelColumn25.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn25.ForeColor = Color.White;
            labelColumn25.BackColor = Color.Transparent;
            labelColumn30.AutoSize = false;
            labelColumn30.Text = "";
            labelColumn30.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn30.ForeColor = Color.White;
            labelColumn30.BackColor = Color.Transparent;
            labelColumn35.AutoSize = false;
            labelColumn35.Text = "";
            labelColumn35.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn35.ForeColor = Color.White;
            labelColumn35.BackColor = Color.Transparent;
            labelColumn40.AutoSize = false;
            labelColumn40.Text = "";
            labelColumn40.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn40.ForeColor = Color.White;
            labelColumn40.BackColor = Color.Transparent;
            labelColumn45.AutoSize = false;
            labelColumn45.Text = "";
            labelColumn45.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn45.ForeColor = Color.White;
            labelColumn45.BackColor = Color.Transparent;
            labelColumn50.AutoSize = false;
            labelColumn50.Text = "";
            labelColumn50.TextAlign = ContentAlignment.MiddleCenter;
            labelColumn50.ForeColor = Color.White;
            labelColumn50.BackColor = Color.Transparent;

            labelRow0.Text = "";
            labelRow0.TextAlign = ContentAlignment.MiddleCenter;
            labelRow0.ForeColor = Color.White;
            labelRow0.BackColor = Color.Transparent;
            labelRow0.AutoSize = false;
            labelRow5.Text = "";
            labelRow5.TextAlign = ContentAlignment.MiddleCenter;
            labelRow5.ForeColor = Color.White;
            labelRow5.BackColor = Color.Transparent;
            labelRow5.AutoSize = false;
            labelRow10.Text = "";
            labelRow10.TextAlign = ContentAlignment.MiddleCenter;
            labelRow10.ForeColor = Color.White;
            labelRow10.BackColor = Color.Transparent;
            labelRow10.AutoSize = false;
            labelRow15.Text = "";
            labelRow15.TextAlign = ContentAlignment.MiddleCenter;
            labelRow15.ForeColor = Color.White;
            labelRow15.BackColor = Color.Transparent;
            labelRow15.AutoSize = false;
            labelRow20.Text = "";
            labelRow20.TextAlign = ContentAlignment.MiddleCenter;
            labelRow20.ForeColor = Color.White;
            labelRow20.BackColor = Color.Transparent;
            labelRow20.AutoSize = false;
            labelRow25.Text = "";
            labelRow25.TextAlign = ContentAlignment.MiddleCenter;
            labelRow25.ForeColor = Color.White;
            labelRow25.BackColor = Color.Transparent;
            labelRow25.AutoSize = false;
            labelRow30.Text = "";
            labelRow30.TextAlign = ContentAlignment.MiddleCenter;
            labelRow30.ForeColor = Color.White;
            labelRow30.BackColor = Color.Transparent;
            labelRow30.AutoSize = false;
            labelRow35.Text = "";
            labelRow35.TextAlign = ContentAlignment.MiddleCenter;
            labelRow35.ForeColor = Color.White;
            labelRow35.BackColor = Color.Transparent;
            labelRow35.AutoSize = false;
            labelRow40.Text = "";
            labelRow40.TextAlign = ContentAlignment.MiddleCenter;
            labelRow40.ForeColor = Color.White;
            labelRow40.BackColor = Color.Transparent;
            labelRow40.AutoSize = false;
            labelRow45.Text = "";
            labelRow45.TextAlign = ContentAlignment.MiddleCenter;
            labelRow45.ForeColor = Color.White;
            labelRow45.BackColor = Color.Transparent;
            labelRow45.AutoSize = false;
            labelRow50.Text = "";
            labelRow50.TextAlign = ContentAlignment.MiddleCenter;
            labelRow50.ForeColor = Color.White;
            labelRow50.BackColor = Color.Transparent;
            labelRow50.AutoSize = false;
            labelRow55.Text = "";
            labelRow55.TextAlign = ContentAlignment.MiddleCenter;
            labelRow55.ForeColor = Color.White;
            labelRow55.BackColor = Color.Transparent;
            labelRow55.AutoSize = false;
            labelRow60.Text = "";
            labelRow60.TextAlign = ContentAlignment.MiddleCenter;
            labelRow60.ForeColor = Color.White;
            labelRow60.BackColor = Color.Transparent;
            labelRow60.AutoSize = false;
            labelRow65.Text = "";
            labelRow65.TextAlign = ContentAlignment.MiddleCenter;
            labelRow65.ForeColor = Color.White;
            labelRow65.BackColor = Color.Transparent;
            labelRow65.AutoSize = false;
            labelRow70.Text = "";
            labelRow70.TextAlign = ContentAlignment.MiddleCenter;
            labelRow70.ForeColor = Color.White;
            labelRow70.BackColor = Color.Transparent;
            labelRow70.AutoSize = false;
            labelRow75.Text = "";
            labelRow75.TextAlign = ContentAlignment.MiddleCenter;
            labelRow75.ForeColor = Color.White;
            labelRow75.BackColor = Color.Transparent;
            labelRow75.AutoSize = false;
            labelRow80.Text = "";
            labelRow80.TextAlign = ContentAlignment.MiddleCenter;
            labelRow80.ForeColor = Color.White;
            labelRow80.BackColor = Color.Transparent;
            labelRow80.AutoSize = false;
            labelRow85.Text = "";
            labelRow85.TextAlign = ContentAlignment.MiddleCenter;
            labelRow85.ForeColor = Color.White;
            labelRow85.BackColor = Color.Transparent;
            labelRow85.AutoSize = false;
            labelRow90.Text = "";
            labelRow90.TextAlign = ContentAlignment.MiddleCenter;
            labelRow90.ForeColor = Color.White;
            labelRow90.BackColor = Color.Transparent;
            labelRow90.AutoSize = false;
            labelRow95.Text = "";
            labelRow95.TextAlign = ContentAlignment.MiddleCenter;
            labelRow95.ForeColor = Color.White;
            labelRow95.BackColor = Color.Transparent;
            labelRow95.AutoSize = false;
            labelRow100.Text = "";
            labelRow100.TextAlign = ContentAlignment.MiddleCenter;
            labelRow100.ForeColor = Color.White;
            labelRow100.BackColor = Color.Transparent;
            labelRow100.AutoSize = false;

            string str = Properties.Settings.Default.json;
            alterCount = Properties.Settings.Default.alterCount;
            if (str != "")
            {
                alterWeekly=JsonConvert.DeserializeObject<AlterWeekly>(str);
                alterCount = alterWeekly.count;
            }

            

        }
        private bool AutoLogin()
        {
            string mail = Properties.Settings.Default.mail;
            string password = Base64Decode(Base64Decode(Base64Decode(Base64Decode(Properties.Settings.Default.password))));
            //MessageBox.Show(mail + " " + password);
            if (mail=="")
            {
                return false;
            }
            if (password=="")
            {
                textBoxMail.Text = mail;
                return false;
            }
            string con = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(phpURL + "?method=login&mail=" + mail + "&pass=" + password);
                request.Timeout = 3000;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    con = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                textBoxMail.Text = mail;
                textBoxPassword.Text = password;
                
                return false;
            }
            
            if (con == "Error: User No Entry")
            {
                return false;
                
            }
            if (con == "Error: Password Wrong")
            {
                
                return false;
            }
            if (con == "")
            {
                textBoxMail.Text = mail;
                textBoxPassword.Text = password;
                return false;
            }
            if (con[0] != '[' || con[1] != 'T' || con[2] != 'r' || con[3] != 'u' || con[4] != 'e' || con[5] != ']')
            {
                textBoxMail.Text = mail;
                textBoxPassword.Text = password;
                //MessageBox.Show(con);
                return false;
            }
            
            string[] data = con.Split('&');
            id = Int32.Parse(data[1].Split('=')[1]);
            birth = new DateTime(Int32.Parse(data[2].Split('=')[1]), Int32.Parse(data[3].Split('=')[1]), Int32.Parse(data[4].Split('=')[1]));
            lifeExpectancy = int.Parse(data[5].Split('=')[1]);

            textBoxMail.Text = "";
            textBoxPassword.Text = "";

            loginStatus = 1;
            for (int i = 0; i < 7; i++)
            {
                TimeSpan timeSpan = new TimeSpan(i, 0, 0, 0);
                firstSunday = birth.Add(timeSpan);
                if (firstSunday.DayOfWeek == DayOfWeek.Sunday)
                {
                    //MessageBox.Show(firstSunday.Day.ToString());
                    break;
                }
            }
            SyncLogin();
            return true;
            //logging end
        }
        private void AdjustColor()
        {
            panelLogo.BackColor = panelMenu.BackColor;
            panelDesktop.BackColor = Color.FromArgb(255, 46, 51, 73);
            int r1,r2,g1,g2,b1,b2;
            int r, g, b;
            r1 = panelMenu.BackColor.R;
            g1= panelMenu.BackColor.G;
            b1= panelMenu.BackColor.B;
            r2=panelDesktop.BackColor.R;
            g2= panelDesktop.BackColor.G;
            b2= panelDesktop.BackColor.B;
            r = (int)Math.Sqrt((r1 * r1 + r2 * r2) / 2);
            g = (int)Math.Sqrt((g1 * g1 + g2 * g2) / 2);
            b = (int)Math.Sqrt((b1 * b1 + b2 * b2) / 2);
            panelBtnBack.BackColor = Color.FromArgb(255, r, g, b);
            blinkColor = Color.LightPink;
            for(int i = 0; i < 256; i++)
            {
                blinkColorSet[i] = i;
            }
            for(int i = 256; i < 456; i++)
            {
                blinkColorSet[i] = 255;
            }
            for(int i = 456; i < 712; i++)
            {
                blinkColorSet[i] = 711 - i;
            }

        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void buttonShut_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void panelTitle_Paint(object sender, PaintEventArgs e)
        {

        }
        private void panelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;//Standar Title Bar - Snap Window
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020; //Minimize form (Before)
            const int SC_RESTORE = 0xF120; //Restore form (Before)
            const int WM_NCHITTEST = 0x0084;//Win32, Mouse Input Notification: Determine what part of the window corresponds to a point, allows to resize the form.
            const int resizeAreaSize = 10;
            #region Form Resize
            // Resize/WM_NCHITTEST values
            const int HTCLIENT = 1; //Represents the client area of the window
            const int HTLEFT = 10;  //Left border of a window, allows resize horizontally to the left
            const int HTRIGHT = 11; //Right border of a window, allows resize horizontally to the right
            const int HTTOP = 12;   //Upper-horizontal border of a window, allows resize vertically up
            const int HTTOPLEFT = 13;//Upper-left corner of a window border, allows resize diagonally to the left
            const int HTTOPRIGHT = 14;//Upper-right corner of a window border, allows resize diagonally to the right
            const int HTBOTTOM = 15; //Lower-horizontal border of a window, allows resize vertically down
            const int HTBOTTOMLEFT = 16;//Lower-left corner of a window border, allows resize diagonally to the left
            const int HTBOTTOMRIGHT = 17;//Lower-right corner of a window border, allows resize diagonally to the right
            ///<Doc> More Information: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest </Doc>
            if (m.Msg == WM_NCHITTEST)
            { //If the windows m is WM_NCHITTEST
                base.WndProc(ref m);
                if (this.WindowState == FormWindowState.Normal)//Resize the form if it is in normal state
                {
                    if ((int)m.Result == HTCLIENT)//If the result of the m (mouse pointer) is in the client area of the window
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32()); //Gets screen point coordinates(X and Y coordinate of the pointer)                           
                        Point clientPoint = this.PointToClient(screenPoint); //Computes the location of the screen point into client coordinates                          
                        if (clientPoint.Y <= resizeAreaSize)//If the pointer is at the top of the form (within the resize area- X coordinate)
                        {
                            if (clientPoint.X <= resizeAreaSize) //If the pointer is at the coordinate X=0 or less than the resizing area(X=10) in 
                                m.Result = (IntPtr)HTTOPLEFT; //Resize diagonally to the left
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))//If the pointer is at the coordinate X=11 or less than the width of the form(X=Form.Width-resizeArea)
                                m.Result = (IntPtr)HTTOP; //Resize vertically up
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize)) //If the pointer is inside the form at the Y coordinate(discounting the resize area size)
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize horizontally to the left
                                m.Result = (IntPtr)HTLEFT;
                            else if (clientPoint.X > (this.Width - resizeAreaSize))//Resize horizontally to the right
                                m.Result = (IntPtr)HTRIGHT;
                        }
                        else
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize diagonally to the left
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize)) //Resize vertically down
                                m.Result = (IntPtr)HTBOTTOM;
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                    }
                }
                return;
            }
            #endregion
            //Remove border and keep snap window
            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                return;
            }
            //Keep form size when it is minimized and restored. Since the form is resized because it takes into account the size of the title bar and borders.
            if (m.Msg == WM_SYSCOMMAND)
            {
                /// <see cref="https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand"/>
                /// Quote:
                /// In WM_SYSCOMMAND messages, the four low - order bits of the wParam parameter 
                /// are used internally by the system.To obtain the correct result when testing 
                /// the value of wParam, an application must combine the value 0xFFF0 with the 
                /// wParam value by using the bitwise AND operator.
                int wParam = (m.WParam.ToInt32() & 0xFFF0);
                if (wParam == SC_MINIMIZE)  //Before
                    formSize = this.ClientSize;
                if (wParam == SC_RESTORE)// Restored form(Before)
                    this.Size = formSize;
            }
            base.WndProc(ref m);
        }
        private void panelTitle_DoubleClick(object sender, EventArgs e)
        {
         
        }
        private void panelTitle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                formSize = this.ClientSize;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Size = formSize;
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            AdjustForm();
            if (this.Width < 683)
            {
                this.Width = 683;
                DrawBig();
            }
            if (this.Height < 384) 
            { 
                this.Height = 384;
                DrawBig();
            }
            cancelClick();
            panelLogin.Location = new Point(panelPersonal.Size.Width/2-panelLogin.Width/2,panelPersonal.Size.Height/2-panelLogin.Height/2);
            for(int i = 0; i < 20; i++)
            {
                panelPulls[i].Size=new Size(flowLayoutPanelPull.Size.Width-10,panelPulls[i].Size.Height);    
            }
            AdjustPull();
            panelSyncCheckCenter.Location = new Point(panelSyncCheck.Width/2-panelSyncCheckCenter.Width-2, panelSyncCheck.Height / 2 - panelSyncCheckCenter.Height - 2);
        }
        private void AdjustPull()
        {
            flowLayoutPanelPull.Controls.Clear();
            int k = 0;
            for(int i = 1; i <= 20; i++)
            {
                if (86 * i-6 <= flowLayoutPanelPull.Height)
                {
                    k = i;
                }
                else
                {
                    break;
                }
            }
            for(int i =0; i <= k - 1; i++)
            {
                flowLayoutPanelPull.Controls.Add(panelPulls[i]);
                panelPulls[i].BackColor = Color.FromArgb(46, 51, 73);
            }
            int j = 0;//index of leter
            int height = (flowLayoutPanelPull.Height - 6 * (k - 1)) / k;
            bool b = false;
            for (int i = 0; i < k; i++)
            {
                j = pullLetters.current + i;
                if (j < pullLetters.count)
                {
                    panelPulls[i].Visible = true;
                    b = true;
                }
                else
                {
                    panelPulls[i].Visible = false;
                    b = false;
                }
                
            }
            if (b)
            {
                for (int i = 0; i < k; i++)
                {
                    panelPulls[i].Height = height;
                }
            }
            else
            {
                for (int i = 0; i < k; i++)
                {
                    panelPulls[i].Height = 80;
                }
            }
            pullLetters.lastK = k;
            for(int i = 0; i < k; i++)
            {
                labelPullTitles[i].Text = pullLetters.title[pullLetters.current + i]; //(pullLetters.current + i).ToString();
                labelPullContents[i].Text = pullLetters.content[pullLetters.current + i];
                labelPullTimes[i].Text = pullLetters.time[pullLetters.current + i];
                bool titleChanged=false;
                while (labelPullContents[i].Width > panelPulls[i].Width - 50)
                {
                    titleChanged = true;
                    labelPullContents[i].Text = labelPullContents[i].Text.Remove(labelPullContents[i].Text.Length - 1);
                }
                if (titleChanged)
                {
                    labelPullContents[i].Text += " ...";
                }
            }
            

        }
        private void buttonPullRefresh_Click(object sender, EventArgs e)
        {
            int k = pullLetters.lastK;
            if (pullLetters.current + k >= pullLetters.count)
            {//to page 0
                pullLetters.current = 0;
            }
            else
            {
                pullLetters.current = pullLetters.current + k;
            }
            AdjustPull();
        }
        private void InitializeBig()
        {

            if (initializedBig != 1) 
            {
                DateTime birthS = birth;
                if ((int)birthS.DayOfWeek != 0)
                {
                    birthS.AddDays(7 - (int)birthS.DayOfWeek);
                }
                Console.WriteLine(birthS);
                TimeSpan dDate=nowDate.Subtract(birthS);
                int modYear = dDate.Days % 364;
                yearPast = (dDate.Days - modYear) / 364;
                int modWeek = modYear % 7;
                weekPast = (modYear - modWeek) / 7;
                if (modWeek != 0) weekPast += 1;
            }
            initializedBig = 1;
        }
        private void AdjustForm()
        {
            switch (this.WindowState)
            {
                case FormWindowState.Maximized:
                    this.Padding = new Padding(0, 0, 0, 0);
                    break;
                case FormWindowState.Normal:
                    if (this.Padding.Top!=borderSize)
                        this.Padding = new Padding(borderSize);
                    break;
            }
        }
        private void buttonMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void AdjustLogin()
        {
            if (loginStatus == 0)
            {
                panelLogin.Visible = true;
            }else if (loginStatus == 1)
            {
                panelLogin.Visible = false;
            }
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            panelFloat.Visible = false;
            button1.BackColor = panelBtnBack.BackColor;
            button2.BackColor = panelMenu.BackColor;
            button3.BackColor = panelMenu.BackColor;
            button4.BackColor = panelMenu.BackColor;
            panelLetters.Visible = false;
            panelWeekly.Visible= true;
            panelWeeklyColumn.Visible= true;
            panelWeeklyRow.Visible= true;
            panelPersonal.Visible = false;
            DrawBig();
            if (drawnBig==0)
            {
                DrawBig();
                drawnBig = 1;
            }
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (loginStatus == 0)
            {
                MessageBox.Show("Please Login or Register. ");
                button3.PerformClick();
                return;
            }
            button1.BackColor = panelMenu.BackColor;
            button2.BackColor = panelBtnBack.BackColor;
            button3.BackColor = panelMenu.BackColor;
            button4.BackColor = panelMenu.BackColor;
            panelWeekly.Visible = false;
            panelWeeklyColumn.Visible = false;
            panelWeeklyRow.Visible = false;
            panelPersonal.Visible = false;
            panelLetters.Visible = true;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            button1.BackColor = panelMenu.BackColor;
            button2.BackColor = panelMenu.BackColor;
            button3.BackColor = panelBtnBack.BackColor;
            button4.BackColor = panelMenu.BackColor;
            panelWeekly.Visible = false;
            panelWeeklyColumn.Visible = false;
            panelWeeklyRow.Visible = false;
            panelPersonal.Visible = true;
            panelLetters.Visible = false;
            AdjustLogin();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            button1.BackColor = panelMenu.BackColor;
            button2.BackColor = panelMenu.BackColor;
            button3.BackColor = panelMenu.BackColor;
            button4.BackColor = panelBtnBack.BackColor;
            panelWeekly.Visible = false;
            panelWeeklyColumn.Visible = false;
            panelWeeklyRow.Visible = false;
            panelPersonal.Visible = false;
            panelLetters.Visible = false;
        }
        private void labelHeading_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void DrawBig()
        {
            drawing = true;
            if (birth==new DateTime())
            {
                return;
            }
            InitializeBig();
            Graphics g = panelWeekly.CreateGraphics();
            g.Clear(panelWeekly.BackColor);
            panelWeekly.BackgroundImage = null;
            //g.DrawRectangle(new Pen(Color.Black),new Rectangle(new Point(0, 0), new Size(1, 1)));
            g.Clear(panelWeekly.BackColor);
            Pen past = new Pen(Color.FromArgb(255,200,200,200));
            Rectangle[] pastF=new Rectangle[52*yearPast+weekPast];
            int w= panelDesktop.Width;
            labelTest.Text = "100";
            if (this.WindowState == FormWindowState.Maximized) w -= 8;
            int rowW = 0;
            for (int i = 1; i < 100; i++)//border width
            {

                int mod = (w - 53 * i) % 52;
                wids = ((w - 53 * i) - mod) / 52;
                if (wids < 0)
                {
                    continue;
                }
                labelTest.Font = new Font(labelTest.Font.Name, wids);
                rowW = (int)labelTest.CreateGraphics().MeasureString(labelTest.Text, labelTest.Font).Width;
                mod = (w - rowW - 53 * i) % 52;
                wids = ((w - rowW - 53 * i) - mod) / 52;
                if (wids/i>4)//ratio
                {
                    continue;
                }
                for(int j = 0; j < 52; j++)
                {
                    if (j < mod)
                    {
                        wid[j] = wids + 1;
                    }
                    else
                    {
                        wid[j] = wids;
                    }
                }
                border = i;
                break;
            }
            int columnH = wids;
            int dH = 2;
            panelWeekly.Location= new Point(rowW, dH+columnH);
            panelWeeklyColumn.Location= new Point(rowW, dH);
            panelWeeklyRow.Location= new Point(0, dH+columnH);
            panelWeeklyRow.Size = new Size(rowW, panelDesktop.Size.Height - dH - columnH);
            panelWeeklyColumn.Size = new Size(panelDesktop.Size.Width - rowW, columnH);
            panelWeekly.Size=new Size(panelWeeklyColumn.Size.Width, panelWeeklyRow.Size.Height);
            //begin drawing
            int k = 0;
            int yLast = 0;
            for (int i=0;i< yearPast; i++)
            {
                int Xsum = 0;
                for(int j = 0; j < 52; j++)
                {
                    pastF[k].Size = new Size(wid[j], wids);
                    pastF[k].X = (j + 1) * border + Xsum;
                    pastF[k].Y = (i + 1) * border + i * wids;
                    yLast = (i + 1) * border + i * wids;
                    k++;
                    widX[j] = Xsum;
                    Xsum += wid[j];
                }
            }
            int Xsum_ = 0;
            int l = 0;
            for (int i=0;i< weekPast; i++)
            {

                pastF[k].Size = new Size(wid[i], wids);
                pastF[k].X=(i + 1) * border + Xsum_;
                Xsum_ += wid[i];
                pastF[k].Y = yLast + border + wids;
                k++;
                l = i + 1;
            }

            g.DrawRectangles(past, pastF);
            Brush pastB=new SolidBrush(Color.FromArgb(255,200,200,200));
            g.FillRectangles(pastB, pastF);
            //drawing today
            Rectangle today = new Rectangle();
            today.Width = wid[l];
            Xsum_+=today.Width;
            today.Height = wids;
            today.X = (l + 1) * border+Xsum_-wid[l-1];
            today.Y = yLast + border + wids;
            todayX = weekPast;
            todayY = yearPast;
            g.DrawRectangle(new Pen(Color.Red),today);

            RectangleF[] nextF=new RectangleF[52];
            RectangleF[] nextFs = new RectangleF[52];
            for(int i = l + 1; i < 52; i++)
            {
                nextF[i].Size=new Size(wid[i], wids);
                nextF[i].X=(i + 1) * border + Xsum_;
                nextF[i].Y = yLast + border + wids;
                nextFs[i].Size = new Size(wid[i] - 2, wids - 2);
                nextFs[i].X = (i + 1) * border + Xsum_+1;
                nextFs[i].Y = yLast + border + wids+1;
                Xsum_ += wid[i];
            }
            Pen future = new Pen(Color.FromArgb(255, 255, 255, 255));
            g.DrawRectangles(future, nextF);
            g.DrawRectangles(future, nextFs);

            RectangleF[] nextFR = new RectangleF[(lifeExpectancy-yearPast+1)*52];
            RectangleF[] nextFRs = new RectangleF[(lifeExpectancy - yearPast + 1) * 52];
            k = 0;
            for (int i = yearPast+1; i <= lifeExpectancy; i++)
            {
                int Xsum = 0;
                for(int j = 0; j < 52; j++)
                {
                    nextFR[k].Size= new Size(wid[j], wids);
                    nextFRs[k].Size = new Size(wid[j] - 2, wids - 2);
                    nextFR[k].X=(j+1) * border + Xsum;
                    nextFRs[k].X = (j + 1) * border + Xsum+1;
                    nextFR[k].Y = (i + 1) * border + i * wids;
                    nextFRs[k].Y = (i + 1) * border + i * wids+1;
                    Xsum += wid[j];
                    k++;
                }
            }
            g.DrawRectangles(future, nextFR);
            g.DrawRectangles(future, nextFRs);

            //drawing column index
            k = 0;
            int desktopW = panelDesktop.Location.X;
            int desktopH = panelDesktop.Location.Y;
            int[] widp = new int[52];
            for(int i = 0; i < 52; i++)
            {
                if(i>0)
                widp[i]=widp[i-1];
                widp[i] += wid[i];
            }
            int labelW = 0;

            labelColumn5.Text = "5";
            labelColumn5.Font = labelTest.Font;
            labelW = (int)labelColumn5.CreateGraphics().MeasureString(labelColumn5.Text, labelColumn5.Font).Width;
            labelColumn5.Size = new Size(labelW * 2, columnH * 2);
            labelColumn5.Location = new Point(widp[5 - 1] - wids / 2 + border * 5 - labelW, -columnH / 2);
            labelColumn10.Text = "10";
            labelColumn10.Font = labelTest.Font;
            labelW = (int)labelColumn10.CreateGraphics().MeasureString(labelColumn10.Text, labelColumn10.Font).Width;
            labelColumn10.Size = new Size(labelW * 2, columnH * 2);
            labelColumn10.Location = new Point(widp[10 - 1] - wids / 2 + border * 10 - labelW, -columnH / 2);
            labelColumn15.Text = "15";
            labelColumn15.Font = labelTest.Font;
            labelW = (int)labelColumn15.CreateGraphics().MeasureString(labelColumn15.Text, labelColumn15.Font).Width;
            labelColumn15.Size = new Size(labelW * 2, columnH * 2);
            labelColumn15.Location = new Point(widp[15 - 1] - wids / 2 + border * 15 - labelW, -columnH / 2);
            labelColumn20.Text = "20";
            labelColumn20.Font = labelTest.Font;
            labelW = (int)labelColumn20.CreateGraphics().MeasureString(labelColumn20.Text, labelColumn20.Font).Width;
            labelColumn20.Size = new Size(labelW * 2, columnH * 2);
            labelColumn20.Location = new Point(widp[20 - 1] - wids / 2 + border * 20 - labelW, -columnH / 2);
            labelColumn25.Text = "25";
            labelColumn25.Font = labelTest.Font;
            labelW = (int)labelColumn25.CreateGraphics().MeasureString(labelColumn25.Text, labelColumn25.Font).Width;
            labelColumn25.Size = new Size(labelW * 2, columnH * 2);
            labelColumn25.Location = new Point(widp[25 - 1] - wids / 2 + border * 25 - labelW, -columnH / 2);
            labelColumn30.Text = "30";
            labelColumn30.Font = labelTest.Font;
            labelW = (int)labelColumn30.CreateGraphics().MeasureString(labelColumn30.Text, labelColumn30.Font).Width;
            labelColumn30.Size = new Size(labelW * 2, columnH * 2);
            labelColumn30.Location = new Point(widp[30 - 1] - wids / 2 + border * 30 - labelW, -columnH / 2);
            labelColumn35.Text = "35";
            labelColumn35.Font = labelTest.Font;
            labelW = (int)labelColumn35.CreateGraphics().MeasureString(labelColumn35.Text, labelColumn35.Font).Width;
            labelColumn35.Size = new Size(labelW * 2, columnH * 2);
            labelColumn35.Location = new Point(widp[35 - 1] -  wids / 2 + border * 35 - labelW, -columnH / 2);
            labelColumn40.Text = "40";
            labelColumn40.Font = labelTest.Font;
            labelW = (int)labelColumn40.CreateGraphics().MeasureString(labelColumn40.Text, labelColumn40.Font).Width;
            labelColumn40.Size = new Size(labelW * 2, columnH * 2);
            labelColumn40.Location = new Point(widp[40 - 1] -  wids / 2 + border * 40 - labelW, -columnH / 2);
            labelColumn45.Text = "45";
            labelColumn45.Font = labelTest.Font;
            labelW = (int)labelColumn45.CreateGraphics().MeasureString(labelColumn45.Text, labelColumn45.Font).Width;
            labelColumn45.Size = new Size(labelW * 2, columnH * 2);
            labelColumn45.Location = new Point(widp[45 - 1] - wids / 2 + border * 45 - labelW, -columnH / 2);
            labelColumn50.Text = "50";
            labelColumn50.Font = labelTest.Font;
            labelW = (int)labelColumn50.CreateGraphics().MeasureString(labelColumn50.Text, labelColumn50.Font).Width;
            labelColumn50.Size = new Size(labelW * 2, columnH * 2);
            labelColumn50.Location = new Point(widp[50 - 1] -wids/2 + border * 50 - labelW, -columnH / 2);
            //offset the font
            panelWeeklyColumn.Location=new Point(panelWeeklyColumn.Location.X+border/2, panelWeeklyColumn.Location.Y);

            labelRow0.Text = "   0";
            labelRow0.Font = labelTest.Font;
            labelRow0.Size = new Size(rowW*2, wids*2);
            labelRow0.Location = new Point(-rowW/2,(0+1)*border+0*wids-wids/2);
            labelRow5.Text = "   5";
            labelRow5.Font = labelTest.Font;
            labelRow5.Size = new Size(rowW * 2, wids * 2);
            labelRow5.Location = new Point(-rowW / 2, (5 + 1) * border + 5 * wids - wids / 2);
            labelRow10.Text = "  10";
            labelRow10.Font = labelTest.Font;
            labelRow10.Size = new Size(rowW * 2, wids * 2);
            labelRow10.Location = new Point(-rowW / 2, (10 + 1) * border + 10 * wids - wids / 2);
            labelRow15.Text = "  15";
            labelRow15.Font = labelTest.Font;
            labelRow15.Size = new Size(rowW * 2, wids * 2);
            labelRow15.Location = new Point(-rowW / 2, (15 + 1) * border + 15 * wids - wids / 2);
            labelRow20.Text = "  20";
            labelRow20.Font = labelTest.Font;
            labelRow20.Size = new Size(rowW * 2, wids * 2);
            labelRow20.Location = new Point(-rowW / 2, (20 + 1) * border + 20 * wids - wids / 2);
            labelRow25.Text = "  25";
            labelRow25.Font = labelTest.Font;
            labelRow25.Size = new Size(rowW * 2, wids * 2);
            labelRow25.Location = new Point(-rowW / 2, (25 + 1) * border + 25 * wids - wids / 2);
            labelRow30.Text = "  30";
            labelRow30.Font = labelTest.Font;
            labelRow30.Size = new Size(rowW * 2, wids * 2);
            labelRow30.Location = new Point(-rowW / 2, (30 + 1) * border + 30 * wids - wids / 2);
            labelRow35.Text = "  35";
            labelRow35.Font = labelTest.Font;
            labelRow35.Size = new Size(rowW * 2, wids * 2);
            labelRow35.Location = new Point(-rowW / 2, (35 + 1) * border + 35 * wids - wids / 2);
            labelRow40.Text = "  40";
            labelRow40.Font = labelTest.Font;
            labelRow40.Size = new Size(rowW * 2, wids * 2);
            labelRow40.Location = new Point(-rowW / 2, (40 + 1) * border + 40 * wids - wids / 2);
            labelRow45.Text = "  45";
            labelRow45.Font = labelTest.Font;
            labelRow45.Size = new Size(rowW * 2, wids * 2);
            labelRow45.Location = new Point(-rowW / 2, (45 + 1) * border + 45 * wids - wids / 2);
            labelRow50.Text = "  50";
            labelRow50.Font = labelTest.Font;
            labelRow50.Size = new Size(rowW * 2, wids * 2);
            labelRow50.Location = new Point(-rowW / 2, (50 + 1) * border + 50 * wids - wids / 2);
            labelRow55.Text = "  55";
            labelRow55.Font = labelTest.Font;
            labelRow55.Size = new Size(rowW * 2, wids * 2);
            labelRow55.Location = new Point(-rowW / 2, (55 + 1) * border + 55 * wids - wids / 2);
            labelRow60.Text = "  60";
            labelRow60.Font = labelTest.Font;
            labelRow60.Size = new Size(rowW * 2, wids * 2);
            labelRow60.Location = new Point(-rowW / 2, (60 + 1) * border + 60 * wids - wids / 2);
            labelRow65.Text = "  65";
            labelRow65.Font = labelTest.Font;
            labelRow65.Size = new Size(rowW * 2, wids * 2);
            labelRow65.Location = new Point(-rowW / 2, (65 + 1) * border + 65 * wids - wids / 2);
            labelRow70.Text = "  70";
            labelRow70.Font = labelTest.Font;
            labelRow70.Size = new Size(rowW * 2, wids * 2);
            labelRow70.Location = new Point(-rowW / 2, (70 + 1) * border + 70 * wids - wids / 2);
            labelRow75.Text = "  75";
            labelRow75.Font = labelTest.Font;
            labelRow75.Size = new Size(rowW * 2, wids * 2);
            labelRow75.Location = new Point(-rowW / 2, (75 + 1) * border + 75 * wids - wids / 2);
            labelRow80.Text = "  80";
            labelRow80.Font = labelTest.Font;
            labelRow80.Size = new Size(rowW * 2, wids * 2);
            labelRow80.Location = new Point(-rowW / 2, (80 + 1) * border + 80 * wids - wids / 2);
            labelRow85.Text = "  85";
            labelRow85.Font = labelTest.Font;
            labelRow85.Size = new Size(rowW * 2, wids * 2);
            labelRow85.Location = new Point(-rowW / 2, (85 + 1) * border + 85 * wids - wids / 2);
            labelRow90.Text = "  90";
            labelRow90.Font = labelTest.Font;
            labelRow90.Size = new Size(rowW * 2, wids * 2);
            labelRow90.Location = new Point(-rowW / 2, (90 + 1) * border + 90 * wids - wids / 2);
            labelRow95.Text = "  95";
            labelRow95.Font = labelTest.Font;
            labelRow95.Size = new Size(rowW * 2, wids * 2);
            labelRow95.Location = new Point(-rowW / 2, (95 + 1) * border + 95 * wids - wids / 2);
            labelRow100.Text = " 100";
            labelRow100.Font = labelTest.Font;
            labelRow100.Size = new Size(rowW * 2, wids * 2);
            labelRow100.Location = new Point(-rowW / 2, (100 + 1) * border + 100 * wids - wids / 2);


            //MessageBox.Show("start");
            for (int i = 0; i < alterCount; i++)
            {
                drawColor(alterWeekly.X[i], alterWeekly.Y[i], alterWeekly.color[i]);
                //MessageBox.Show(i.ToString());
            }
            //MessageBox.Show("end");

            saveWeekly();
            

            drawing = false;
            selectYear = -1;
            selectWeek = -1;

        }
        private void labelVer_Click(object sender, EventArgs e)
        {

        }
        private void labelVer_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void panelLogin_Paint(object sender, PaintEventArgs e)
        {

        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (!buttonLoginLogin())
            {
                buttonLoginLogin();
            }
            //button1.PerformClick();
        }
        private bool buttonLoginLogin()
        {
            labelLoginErr.Text = "";
            string mail = textBoxMail.Text;
            string password = textBoxPassword.Text;

            if (textBoxMail.Text == "" || textBoxMail.Text == "Email")
            {
                labelLoginErr.Text = "Please Enter Mail";
                return false;
            }
            if (textBoxPassword.Text == "" || textBoxPassword.Text == "Password")
            {
                labelLoginErr.Text = "Please Enter Password";
                return false;
            }
            string con = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(phpURL + "?method=login&mail=" + textBoxMail.Text + "&pass=" + textBoxPassword.Text);
                request.Timeout = 3000;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    con = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                labelLoginErr.Text = "Network Error";
                string msg = ex.Message;
                return false;
            }
            if (con == "Error: User No Entry")
            {
                labelLoginErr.Text = "User No Entry";
                return false;
            }
            if (con == "Error: Password Wrong")
            {
                labelLoginErr.Text = "Password Wrong";
                return false;
            }
            if (con == "")
            {
                labelLoginErr.Text = "Unknown Error";
                return false;
            }
            if (con[0] != '[' || con[1] != 'T' || con[2] != 'r' || con[3] != 'u' || con[4] != 'e' || con[5] != ']')
            {
                labelLoginErr.Text = "Unknown Error";
                //MessageBox.Show(con);
                return false;
            }
            string[] data = con.Split('&');
            id = Int32.Parse(data[1].Split('=')[1]);
            birth = new DateTime(Int32.Parse(data[2].Split('=')[1]), Int32.Parse(data[3].Split('=')[1]), Int32.Parse(data[4].Split('=')[1]));
            lifeExpectancy = int.Parse(data[5].Split('=')[1]);
            Properties.Settings.Default.mail = mail;
            Properties.Settings.Default.password = Base64Encode(Base64Encode(Base64Encode(Base64Encode(password))));
            Properties.Settings.Default.Save();
            textBoxMail.Text = "";
            textBoxPassword.Text = "";
            loginStatus = 1;
            for (int i = 0; i < 7; i++)
            {
                TimeSpan timeSpan = new TimeSpan(i, 0, 0, 0);
                firstSunday = birth.Add(timeSpan);
                if (firstSunday.DayOfWeek == DayOfWeek.Sunday)
                {
                    //MessageBox.Show(firstSunday.Day.ToString());
                    break;
                }
            }
            SyncLogin();
            
            
            return true;
        }
        private void textBoxMail_TextChanged(object sender, EventArgs e)
        {
            
        }
        private void textBoxMail_Click(object sender, EventArgs e)
        {
            if(textBoxMail.Text=="Email")
            textBoxMail.Clear();
            panelMail.BackColor = Color.FromArgb(255,96, 110, 253);
            panelPassword.BackColor = Color.White;
            panelMail.Size = new Size((int)panelMail.Size.Width,2);
            panelPassword.Size = new Size((int)panelPassword.Size.Width, 1);
        }
        private void textBoxPassword_Click(object sender, EventArgs e)
        {
            if(textBoxPassword.Text=="Password")
            textBoxPassword.Clear();
            panelPassword.BackColor = Color.FromArgb(255,96, 110, 253);
            panelMail.BackColor = Color.White;
            panelMail.Size = new Size((int)panelMail.Size.Width, 1);
            panelPassword.Size = new Size((int)panelPassword.Size.Width, 2);
        }
        private void textBoxMail_Leave(object sender, EventArgs e)
        {
            panelMail.BackColor = Color.White;
            panelMail.Size = new Size((int)panelMail.Size.Width, 1);
            if (textBoxMail.Text == "")
            {
                textBoxMail.Text = "Email";
            }
        }
        private void textBoxPassword_Leave(object sender, EventArgs e)
        {
            panelPassword.BackColor = Color.White;
            panelPassword.Size = new Size((int)panelPassword.Size.Width, 1);
            if (textBoxPassword.Text == "")
            {
                textBoxPassword.Text = "Password";
                textBoxPassword.PasswordChar = '\0';
            }
        }
        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            textBoxPassword.PasswordChar = '*';
        }
        private void textBoxPassword_Enter(object sender, EventArgs e)
        {
            if (textBoxPassword.Text == "Password")
                textBoxPassword.Clear();
            panelPassword.BackColor = Color.FromArgb(255, 96, 110, 253);
            panelMail.BackColor = Color.White;
            panelMail.Size = new Size((int)panelMail.Size.Width, 1);
            panelPassword.Size = new Size((int)panelPassword.Size.Width, 2);
        }
        private void textBoxMail_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyValue == (int)Keys.Enter)
            {
                buttonLogin.PerformClick();
            }
        }
        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {
                buttonLogin.PerformClick();
            }
        }
        private void timerFormSize_Tick(object sender, EventArgs e)
        {

            if (this.Height != lastH || this.Width != lastW)
            {
                if (lastT == 1)
                {
                    if (panelWeekly.Visible)
                    {
                        DrawBig();
                        lastT = 2;
                        lastH = this.Height;
                        lastW = this.Width;
                    }
                }
                else
                {
                    lastT = 1;
                }
            }
            else
            {
                if(lastT==1)lastT = 0;
            }
            if (lastT == 2)
            {
                DrawBig();
                lastT = 0;
            }
        }
        private void Form1_Scroll(object sender, ScrollEventArgs e)
        {
        }
        private void saveWeekly()
        {
            Bitmap controlBitMap = new Bitmap(panelWeekly.Width, panelWeekly.Height);
            Graphics g = Graphics.FromImage(controlBitMap);
            g.CopyFromScreen(pointAdd(PointToScreen(panelWeekly.Parent.Location), panelWeekly.Location), new Point(0, 0), panelWeekly.Size);
            bmpWeekly = controlBitMap;
            Graphics graphics=panelWeekly.CreateGraphics();
            graphics.Clear(panelWeekly.BackColor);
            panelWeekly.BackgroundImage = bmpWeekly;
            //bmpWeekly.Save("a.png");
        }
        private Bitmap getControlImage(Control theControl)
        {

            Bitmap controlBitMap = new Bitmap(theControl.Width, theControl.Height);
            Graphics g = Graphics.FromImage(controlBitMap);
            g.CopyFromScreen(pointAdd(PointToScreen(theControl.Parent.Location),theControl.Location), new Point(0, 0), theControl.Size);
            return controlBitMap;
        }
        private Point pointAdd(Point A,Point B)
        {
            return new Point(A.X+B.X,A.Y+B.Y);
        }
        private void panelWeekly_MouseDown(object sender, MouseEventArgs e)
        {
            //Graphics g=this.CreateGraphics();   
            timerSelectedColor.Stop();
            Bitmap bmp=new Bitmap(panelWeekly.Width, panelWeekly.Height);
            bmp = getControlImage(panelWeekly);
          //  using (var graphics = Graphics.FromImage(bmp))
          //  {
          //      graphics.CopyFromScreen(PointToScreen(panelWeekly.Location), new Point(0, 0), new Size(1, 1));
          //  }
           // bmp.Save("a.png");
            for (int i = 0; i < yearPast; i++)
            {
                for(int j = 0; j < 52; j++)
                {
                    if (wids * i + border * (i + 1) + 1 < panelWeekly.Height)
                    {
                        if (bmp.GetPixel(widX[j] + (j + 1) * border + 1, wids * i + border * (i + 1) + 1) != Color.FromArgb(255, 200, 200, 200))
                        {
                            drawPast(j, i);
                            //MessageBox.Show(j.ToString()+" "+i.ToString());
                        }
                    }
                }
            }
            
            if(border * (todayY + 1) + 3 < panelWeekly.Height)
                {
                for (int i = 0; i < weekPast; i++)
                {
                    if (bmp.GetPixel(widX[i] + (i + 1) * border + 1, wids * todayY + border * (todayY + 1) + 3) != Color.FromArgb(255, 200, 200, 200))
                    {
                        drawPast(i, todayY);
                    }
                }

                if (bmp.GetPixel(widX[todayX] + (todayX + 1) * border + 3, wids * todayY + border * (todayY + 1) + 3) != panelWeekly.BackColor)
                {
                    drawToday();
                }
                for (int i = todayX+1; i < 52; i++)
                {
                    if (bmp.GetPixel(widX[i] + (i + 1) * border + 3, wids * todayY + border * (todayY + 1) + 3) != panelWeekly.BackColor)
                    {
                        drawFuture(i, todayY);
                    }
                }
            }
            for(int i = yearPast+1; i < lifeExpectancy+1; i++)
            {
                for (int j = 0; j < 52; j++)
                {
                    if (wids * i + border * (i + 3) + 3 < panelWeekly.Height)
                    {
                        if (bmp.GetPixel(widX[j] + (j + 3) * border + 3, wids * i + border * (i + 3) + 3) != panelWeekly.BackColor)
                        {
                            drawFuture(j, i);
                            //MessageBox.Show(j.ToString() + " " + i.ToString());
                        }
                    }
                }
            }
            timerSelectedColor.Start();
            if (drawing)
                return;
            int clickX = e.X;
            int clickY = e.Y;
            int clickYear = 0;
            int clickWeek = 0;
            bool clickYearED = false;
            bool clickWeekED = false;
            for (int i = 0; i < 52; i++)
            {
                if (clickX >= widX[i] + (i + 1) * border && clickX <= widX[i] + (i + 1) * border + wid[i])
                {
                    clickWeek = i;
                    clickWeekED= true;
                }
            }
            for (int i = 0; i <= lifeExpectancy ; i++)
            {
                if (clickY >= wids * i + border * (i + 1) && clickY <= wids * (i + 1) + border * (i + 1))
                {
                    clickYear = i;
                    clickYearED = true;
                }
            }
            //MessageBox.Show(clickWeek.ToString());
            /*
            Graphics g=panelWeekly.CreateGraphics();
            Brush selectedB = new SolidBrush(Color.LightPink);
            Rectangle selected = new Rectangle(widX[clickWeek] + (clickWeek + 1) * border, wids * clickYear + border * (clickYear + 1), wid[clickWeek]+1, wids+1);
            g.FillRectangle(selectedB, selected);
            */
            if (!clickYearED || !clickWeekED)
            {//cancel selection
                panelFloat.Visible = false;
               //if (selectWeek != -1&& selectYear != -1)
                    panelWeekly.Refresh();
                blinkColorIndex = 0;
                timerSelectedColor.Enabled = false;
                lastX = selectWeek;
                lastY = selectYear;
                selectWeek = -1;
                selectYear = -1;
                return;
            }
            lastX = selectWeek;
            lastY = selectYear;
            selectWeek = clickWeek;
            selectYear= clickYear;
            int x=selectWeek,y=selectYear;
            int left, top;
            if (lastX != selectWeek || lastY != selectYear)
            {
                blinkColorIndex = 128;
                timerSelectedColor.Enabled = true;
                //new click
                if (widX[x] + (x + 1) * border - panelFloat.Width >= 70)
                {
                    left = widX[x] + (x + 1) * border - panelFloat.Width-50;
                }
                else
                {
                    left = widX[x] + (x + 1) * border + wid[x] + 50;
                }
                if (wids * y + border * (y + 1) + wids /2 - panelFloat.Height /2 < 20)
                {
                    top = 20;
                }
                else if (panelWeekly.Height-(wids * y + border * (y + 1) + wids / 2 + panelFloat.Height / 2)<20)
                {
                    top=panelWeekly.Height-panelFloat.Height-20;
                }else
                {
                    top = wids * y + border * (y + 1) + wids / 2 - panelFloat.Height / 2;
                }
                panelFloat.Location= new Point(left, top);
                //set week
                DateTime thisSunday = firstSunday.AddDays(7*(selectYear*52+selectWeek));
                DateTime thisSaturday = thisSunday.AddDays(-1);
                DateTime thisFriday = thisSunday.AddDays(-2);
                DateTime thisThursday = thisSunday.AddDays(-3);
                DateTime thisWednesday = thisSunday.AddDays(-4);
                DateTime thisTuesday = thisSunday.AddDays(-5);
                DateTime thisMonday = thisSunday.AddDays(-6);
                labelDate7.Text=thisSunday.Day.ToString();
                labelDate6.Text=thisSaturday.Day.ToString();
                labelDate5.Text=thisFriday.Day.ToString();
                labelDate4.Text=thisThursday.Day.ToString();
                labelDate3.Text=thisWednesday.Day.ToString();
                labelDate2.Text=thisTuesday.Day.ToString();
                labelDate1.Text=thisMonday.Day.ToString();
                if (thisMonday == nowDate)
                {
                    labelDate1.ForeColor = Color.Red;
                    labelDate2.ForeColor = Color.White;
                    labelDate3.ForeColor = Color.White;
                    labelDate4.ForeColor = Color.White;
                    labelDate5.ForeColor = Color.White;
                    labelDate6.ForeColor = Color.White;
                    labelDate7.ForeColor = Color.White;
                }
                else if(thisTuesday == nowDate)
                {
                    labelDate1.ForeColor = Color.White;
                    labelDate2.ForeColor = Color.Red;
                    labelDate3.ForeColor = Color.White;
                    labelDate4.ForeColor = Color.White;
                    labelDate5.ForeColor = Color.White;
                    labelDate6.ForeColor = Color.White;
                    labelDate7.ForeColor = Color.White;
                }
                else if (thisWednesday == nowDate)
                {
                    labelDate1.ForeColor = Color.White;
                    labelDate2.ForeColor = Color.White;
                    labelDate3.ForeColor = Color.Red;
                    labelDate4.ForeColor = Color.White;
                    labelDate5.ForeColor = Color.White;
                    labelDate6.ForeColor = Color.White;
                    labelDate7.ForeColor = Color.White;
                }
                else if (thisThursday == nowDate)
                {
                    labelDate1.ForeColor = Color.White;
                    labelDate2.ForeColor = Color.White;
                    labelDate3.ForeColor = Color.White;
                    labelDate4.ForeColor = Color.Red;
                    labelDate5.ForeColor = Color.White;
                    labelDate6.ForeColor = Color.White;
                    labelDate7.ForeColor = Color.White;
                }
                else if (thisFriday == nowDate)
                {
                    labelDate1.ForeColor = Color.White;
                    labelDate2.ForeColor = Color.White;
                    labelDate3.ForeColor = Color.White;
                    labelDate4.ForeColor = Color.White;
                    labelDate5.ForeColor = Color.Red;
                    labelDate6.ForeColor = Color.White;
                    labelDate7.ForeColor = Color.White;
                }
                else if (thisSaturday == nowDate)
                {
                    labelDate1.ForeColor = Color.White;
                    labelDate2.ForeColor = Color.White;
                    labelDate3.ForeColor = Color.White;
                    labelDate4.ForeColor = Color.White;
                    labelDate5.ForeColor = Color.White;
                    labelDate6.ForeColor = Color.Red;
                    labelDate7.ForeColor = Color.White;
                }
                else if (thisSunday == nowDate)
                {
                    labelDate1.ForeColor = Color.White;
                    labelDate2.ForeColor = Color.White;
                    labelDate3.ForeColor = Color.White;
                    labelDate4.ForeColor = Color.White;
                    labelDate5.ForeColor = Color.White;
                    labelDate6.ForeColor = Color.White;
                    labelDate7.ForeColor = Color.Red;
                }
                else
                {
                    labelDate1.ForeColor = Color.White;
                    labelDate2.ForeColor = Color.White;
                    labelDate3.ForeColor = Color.White;
                    labelDate4.ForeColor = Color.White;
                    labelDate5.ForeColor = Color.White;
                    labelDate6.ForeColor = Color.White;
                    labelDate7.ForeColor = Color.White;
                }
                if (thisMonday.Month == thisSunday.Month)
                {
                    labelMonth1.Text = months[thisMonday.Month];
                    labelMonth2.Text = "";
                }
                else
                {
                    labelMonth1.Text = months[thisMonday.Month];
                    labelMonth2.Text = months[thisSunday.Month];
                }
                if(thisMonday.Year== thisSunday.Year)
                {
                    labelYear.Text = thisMonday.Year.ToString();
                }
                else
                {
                    labelYear.Text=thisMonday.Year.ToString()+"-"+thisSunday.Year.ToString();
                }

                int l = -1;
                for(int i = 0; i < alterCount; i++)
                {
                    if (alterWeekly.X[i] == selectWeek && alterWeekly.Y[i] == selectYear)
                    {
                        l = i;
                    }
                }
                if (l != -1)
                {
                    moduleSelect.id = 4;
                    moduleSelect.color = alterWeekly.color[l];
                }
                else if (selectYear < todayY || (selectYear == todayY && selectWeek < todayX))
                {
                    moduleSelect.id = 1;
                }else if(selectYear> todayY || (selectYear == todayY && selectWeek > todayX))
                {
                    moduleSelect.id = 2;
                }else if (selectYear == todayY && selectWeek == todayX)
                {
                    moduleSelect.id = 3;
                }


                

            }

            panelFloat.Visible = true;
            panelWeekly.Refresh();
        }
        private void cancelClick()
        {
            panelFloat.Visible = false;
            if (selectWeek != -1 && selectYear != -1)
                panelWeekly.Refresh();
            blinkColorIndex = 0;
            timerSelectedColor.Enabled = false;
            lastX = selectWeek;
            lastY = selectYear;
            selectWeek = -1;
            selectYear = -1;
            return;
        }
        private void buttonFill_Click(object sender, EventArgs e)
        {
            button3.PerformClick();
            textBoxMail.Text = "admin@shitao.tech";
            textBoxPassword.Text = "CZW8O3czw8o3";
            buttonLogin.PerformClick();
        }//auto login as admin
        private void timerSelectedColor_Tick(object sender, EventArgs e)
        {
            
            if (panelWeekly.Visible && selectWeek != -1 && selectYear != -1)
            {//then update color
                //labelTest.Text= selectWeek.ToString()+" "+selectYear.ToString()+" "+ blinkColorSet[blinkColorIndex].ToString();

                Graphics g = panelWeekly.CreateGraphics();
                Brush selectedB = new SolidBrush(Color.FromArgb(blinkColorSet[blinkColorIndex],Color.LightPink));
                //Brush back = new SolidBrush(Color.FromArgb(255, 200, 200, 200));
                //Brush back = new SolidBrush(Color.Transparent);
                Rectangle selected = new Rectangle(widX[selectWeek] + (selectWeek + 1) * border, wids * selectYear + border * (selectYear + 1), wid[selectWeek] + 1, wids + 1);
                //g.FillRectangle(back, selected);
                //g.Clear(Color.Transparent);
                if (moduleSelect.id==1)
                {
                    drawPast(selectWeek, selectYear);
                }else if (moduleSelect.id==2)
                {
                    drawFuture(selectWeek, selectYear);
                }
                else if(moduleSelect.id==3)
                {
                    drawToday();
                }
                else if(moduleSelect.id==4)
                {
                    drawColor(selectWeek, selectYear, moduleSelect.color);
                }
                g.FillRectangle(selectedB, selected);
                blinkColorIndex += 10;
                if (blinkColorIndex > 711)
                {
                    blinkColorIndex -= 711;
                }
            }
        }
        private void drawPast(int X,int Y)
        {
            Graphics g=panelWeekly.CreateGraphics();
            SolidBrush solidBrush=new SolidBrush(Color.FromArgb(255,200,200,200));
            Rectangle rectangle=new Rectangle(widX[X] + (X + 1) * border, wids * Y + border * (Y + 1), wid[X]+1, wids+1);
            g.FillRectangle(solidBrush, rectangle);
            //MessageBox.Show((widX[X] + (X + 1) * border).ToString());
        }
        private void drawFuture(int X, int Y)
        {
            if (X == todayX && Y == todayY)
            {
                drawToday();
                return;
            }
            Graphics g = panelWeekly.CreateGraphics();
            Pen pen = new Pen(Color.White);
            Rectangle rectangle = new Rectangle(widX[X] + (X + 1) * border, wids * Y + border * (Y + 1), wid[X], wids);
            SolidBrush solidBrush = new SolidBrush(panelDesktop.BackColor);
            g.FillRectangle(solidBrush, rectangle);
            g.DrawRectangle(pen, rectangle);
            rectangle = new Rectangle(widX[X] + (X + 1) * border+1, wids * Y + border * (Y + 1)+1, wid[X]-2, wids-2);
            g.DrawRectangle(pen, rectangle);
            
        }
        private void panelWeekly_Paint(object sender, PaintEventArgs e)
        {

        }
        private void panelWeeklyColumn_MouseDown(object sender, MouseEventArgs e)
        {
            cancelClick();
        }
        private void panelWeeklyRow_MouseDown(object sender, MouseEventArgs e)
        {
            cancelClick();
        }
        private void panelPersonal_Paint(object sender, PaintEventArgs e)
        {
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void panelFloatDesktop_MouseDown(object sender, MouseEventArgs e)
        {
            int k = 0;
            timerSelectedColor.Enabled = false;
            int x = e.X, y = e.Y;
            if (x >= 33 && x < 33 + 144 && y >= 116 && y < 116 + 23)
            {//failure
                //MessageBox.Show("Failure");
                int l = -1;
                for (int i = 0; i < alterCount; i++)
                {
                    if (selectWeek == alterWeekly.X[i] && selectYear == alterWeekly.Y[i])
                    {// change exist
                        l = i;
                        break;
                    }
                }
                if (l != -1)
                {
                    alterWeekly.color[l] = pickFailure;
                    k = l;
                }
                else
                {//add a change
                    alterWeekly.id[alterCount] = alterCount;
                    alterWeekly.X[alterCount] = selectWeek;
                    alterWeekly.Y[alterCount] = selectYear;
                    alterWeekly.color[alterCount] = pickFailure;
                    alterCount++;
                    k = alterCount - 1;
                }
            }
            else if(x>7&&x<=7+16&&y>120&&y<=120+16)
            {//x
                for (int i=0; i<alterCount; i++)
                {
                    if (alterWeekly.X[i] == selectWeek && alterWeekly.Y[i] == selectYear)
                    {
                       
                        for (int j = i; j < alterCount-1; j++)
                        {
                            alterWeekly.id[j] = alterWeekly.id[j + 1];
                            alterWeekly.X[j]=alterWeekly.X[j + 1];
                            alterWeekly.Y[j] = alterWeekly.Y[j + 1];
                            alterWeekly.color[j] = alterWeekly.color[j + 1];
                        }

                        alterWeekly.id[alterCount] = 0;
                        alterWeekly.X[alterCount] = 0;
                        alterWeekly.X[alterCount] = 0;
                        alterWeekly.color[alterCount] = new Color();
                        alterCount--;
                        if (selectYear < todayY || (selectYear == todayY && selectWeek < todayX))
                        {
                            moduleSelect.id = 1;
                            drawColorPixels(selectWeek, selectYear, Color.FromArgb(255, 200, 200, 200));
                        }else if (selectYear > todayY || (selectYear == todayY && selectWeek > todayX)){
                            moduleSelect.id = 2;
                            drawColorPixels(selectWeek, selectYear, panelWeekly.BackColor);
                            drawPixelMargin(selectWeek, selectYear, Color.White, 2);
                        }
                        else
                        {
                            moduleSelect.id = 3;
                            drawColorPixels(selectWeek, selectYear, panelWeekly.BackColor);
                            drawPixelMargin(selectWeek, selectYear, Color.Red, 1);
                            //MessageBox.Show("draw today");
                        }
                    }
                }
                timerSelectedColor.Enabled = true;
                return;
            }
            else if (x >= 187 && x < 187 + 15 && y >= 121 && y < 121 + 15)
            {//?
                //MessageBox.Show("?");


                timerSelectedColor.Enabled = true;
                return;
            }
            else if (Math.Sqrt((x - 49) * (x - 49) + (y - 56) * (y - 56)) <= 19)
            {//not great 1
                //MessageBox.Show("not great1");
                int l = -1;
                for (int i = 0; i < alterCount; i++)
                {
                    if (selectWeek == alterWeekly.X[i] && selectYear == alterWeekly.Y[i])
                    {// change exist
                        l = i;
                        break;
                    }
                }
                if (l != -1)
                {
                    alterWeekly.color[l] = pickNotGreat1;
                    k = l;
                }
                else
                {//add a change
                    alterWeekly.id[alterCount] = alterCount;
                    alterWeekly.X[alterCount] = selectWeek;
                    alterWeekly.Y[alterCount] = selectYear;
                    alterWeekly.color[alterCount] = pickNotGreat1;
                    alterCount++;
                    k = alterCount - 1;
                }
            }
            else if (Math.Sqrt((x - 160) * (x - 160) + (y - 56) * (y - 56)) <= 19)
            {//not great 2
                //MessageBox.Show("not great2");
                int l = -1;
                for (int i = 0; i < alterCount; i++)
                {
                    if (selectWeek == alterWeekly.X[i] && selectYear == alterWeekly.Y[i])
                    {// change exist
                        l = i;
                        break;
                    }
                }
                if (l != -1)
                {
                    alterWeekly.color[l] = pickNotGreat2;
                    k = l;
                }
                else
                {//add a change
                    alterWeekly.id[alterCount] = alterCount;
                    alterWeekly.X[alterCount] = selectWeek;
                    alterWeekly.Y[alterCount] = selectYear;
                    alterWeekly.color[alterCount] = pickNotGreat2;
                    alterCount++;
                    k = alterCount - 1;
                }
            }
            else if (Math.Sqrt((x - 77) * (x - 77) + (y - 58) * (y - 58)) <= 55)
            {
                if(Math.Sqrt((x - 135) * (x - 135) + (y - 58) * (y - 58)) <= 55)
                {//ideal
                    //MessageBox.Show("ideal");
                    int l = -1;
                    for(int i = 0; i < alterCount; i++)
                    {
                        if (selectWeek == alterWeekly.X[i] && selectYear == alterWeekly.Y[i])
                        {// change exist
                            l = i;
                            break;
                        }
                    }
                    if (l != -1)
                    {
                        alterWeekly.color[l] = pickIdeal;
                        k = l;
                    }
                    else{//add a change
                        alterWeekly.id[alterCount] = alterCount;
                        alterWeekly.X[alterCount] = selectWeek;
                        alterWeekly.Y[alterCount] = selectYear;
                        alterWeekly.color[alterCount] = pickIdeal;
                        alterCount++;
                        k = alterCount - 1;
                    }
                }
                else
                {//good 1
                    //MessageBox.Show("good1");
                    int l = -1;
                    for (int i = 0; i < alterCount; i++)
                    {
                        if (selectWeek == alterWeekly.X[i] && selectYear == alterWeekly.Y[i])
                        {// change exist
                            l = i;
                            break;
                        }
                    }
                    if (l != -1)
                    {
                        alterWeekly.color[l] = pickGood1;
                        k = l;
                    }
                    else
                    {//add a change
                        alterWeekly.id[alterCount] = alterCount;
                        alterWeekly.X[alterCount] = selectWeek;
                        alterWeekly.Y[alterCount] = selectYear;
                        alterWeekly.color[alterCount] = pickGood1;
                        alterCount++;
                        k = alterCount - 1;
                    }
                }
            }
            else if(Math.Sqrt((x - 135) * (x - 135) + (y - 58) * (y - 58)) <= 55)
            {//good 2
                //MessageBox.Show("good2");
                int l = -1;
                for (int i = 0; i < alterCount; i++)
                {
                    if (selectWeek == alterWeekly.X[i] && selectYear == alterWeekly.Y[i])
                    {// change exist
                        l = i;
                        break;
                    }
                }
                if (l != -1)
                {
                    alterWeekly.color[l] = pickGood2;
                    k = l;
                }
                else
                {//add a change
                    alterWeekly.id[alterCount] = alterCount;
                    alterWeekly.X[alterCount] = selectWeek;
                    alterWeekly.Y[alterCount] = selectYear;
                    alterWeekly.color[alterCount] = pickGood2;
                    alterCount++;
                    k = alterCount - 1;
                }
            }
            else
            {//no selection
                timerSelectedColor.Enabled = true;
                return;
            }

            moduleSelect.id = 4;
            moduleSelect.color = alterWeekly.color[k];
            drawColorPixels(selectWeek, selectYear, alterWeekly.color[k]);


            timerSelectedColor.Enabled = true;
        }
        private void drawToday()
        {
            Graphics g = panelWeekly.CreateGraphics();
            SolidBrush solidBrush = new SolidBrush(panelDesktop.BackColor);
            Pen pen = new Pen(Color.Red);
            Rectangle rectangle = new Rectangle(widX[todayX] + (todayX + 1) * border, wids * todayY + border * (todayY + 1), wid[todayX], wids);
            g.FillRectangle(solidBrush, rectangle);
            g.DrawRectangle(pen, rectangle);
        }
        private void buttonTest2_Click(object sender, EventArgs e)
        {
            saveJson();
        }
        private void drawColor(int X,int Y,Color color)
        {
            Graphics g = panelWeekly.CreateGraphics();
            SolidBrush solidBrush = new SolidBrush(color);
            Rectangle rectangle = new Rectangle(widX[X] + (X + 1) * border, wids * Y + border * (Y + 1), wid[X] + 1, wids + 1);
            g.FillRectangle(solidBrush, rectangle);
        }
        private void drawColorPixels(int X,int Y,Color color)
        {
            //Rectangle rectangle = new Rectangle(widX[X] + (X + 1) * border, wids * Y + border * (Y + 1), wid[X] + 1, wids + 1);
            for(int i= widX[X] + (X + 1) * border; i< widX[X] + (X + 1) * border+ wid[X] + 1;i++)
            {
                for(int j= wids * Y + border * (Y + 1);j< wids * Y + border * (Y + 1)+wids + 1; j++)
                {
                    bmpWeekly.SetPixel(i, j, color);
                }
            }
        }
        private void drawPixelMargin(int X,int Y,Color color, int margin)
        {
            if (margin > 0)
            {
                for (int i = widX[X] + (X + 1) * border; i < widX[X] + (X + 1) * border + wid[X] + 1; i++)
                {
                    bmpWeekly.SetPixel(i, wids * Y + border * (Y + 1), color);
                    bmpWeekly.SetPixel(i, wids * Y + border * (Y + 1) + wids, color);
                }
                for(int j= wids * Y + border * (Y + 1); j < wids * Y + border * (Y + 1) + wids + 1; j++)
                {
                    bmpWeekly.SetPixel(widX[X] + (X + 1) * border, j, color);
                    bmpWeekly.SetPixel(widX[X] + (X + 1) * border + wid[X], j, color);
                }
            }
            if (margin == 2)
            {
                for (int i = widX[X] + (X + 1) * border+1; i < widX[X] + (X + 1) * border + wid[X]; i++)
                {
                    bmpWeekly.SetPixel(i, wids * Y + border * (Y + 1)+1, color);
                    bmpWeekly.SetPixel(i, wids * Y + border * (Y + 1) + wids-1, color);
                }
                for (int j = wids * Y + border * (Y + 1)+1; j < wids * Y + border * (Y + 1) + wids ; j++)
                {
                    bmpWeekly.SetPixel(widX[X] + (X + 1) * border+1, j, color);
                    bmpWeekly.SetPixel(widX[X] + (X + 1) * border-1 + wid[X], j, color);
                }
            }
        }
        private void buttonTest3_Click(object sender, EventArgs e)
        {
            loadJson();
        }
        private bool loadJson()
        {
            string con = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(phpURL + "?method=load&mail=" + Properties.Settings.Default.mail + "&pass=" + Base64Decode(Base64Decode(Base64Decode(Base64Decode(Properties.Settings.Default.password)))));
                request.Timeout = 3000;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    con = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                labelLoginErr.Text = "Network Error";
                string msg = ex.Message;
                return false;
            }
            if (con == "Error: User No Entry")
            {
                MessageBox.Show("User No Entry");
                return false;
            }
            if (con == "Error: Password Wrong")
            {
                MessageBox.Show("Password Wrong");
                return false;
            }
            if (con == "Fail")
            {
                MessageBox.Show("Fail to read the file");
                return false;
            }
            if (con[0] != '!')
            {
                MessageBox.Show("Unknown error");
                return false;
            }
            conn = "";
            for (int i = 1; i < con.Length; i++)
            {
                conn += con[i];
            }

            //MessageBox.Show(conn);
            return true;
        }
        private string conn = "";
        private string jsonLocal = "";
        private string jsonServer = "";
        private void SyncLogin()
        {
            jsonLocal = Properties.Settings.Default.json;
            jsonServer = "";
            string str = "";
            if (loadJson())
            {
                jsonServer = conn;
            }
            if (jsonLocal == "" && jsonServer == "")
            {
                return;
            }
            else if (jsonLocal == "")
            {
                str = jsonServer;
            }
            else if (jsonServer == "")
            {
                str = jsonLocal;
            }
            else if (jsonLocal == jsonServer)
            {
            }
            else
            {
                panelSyncCheck.Visible = true;
                panelSyncCheck.BringToFront();
                

                return;
            }

            alterWeekly = JsonConvert.DeserializeObject<AlterWeekly>(str);
            alterCount = alterWeekly.count;

            button1.PerformClick();

        }
        private void buttonPullLetters_Click(object sender, EventArgs e)
        {
            buttonPullLetters.BackColor = Color.FromArgb(76, 81, 103);
            buttonCompose.BackColor = panelLetters.BackColor;
            buttonReceive.BackColor=panelLetters.BackColor;
            buttonSent.BackColor = panelLetters.BackColor;
            panelCompose.Visible = false;
            pullLetters.current = 0;
            if (UpdatePull())
            {
            }
            else
            {
                pullLetters.current = 0;
                pullLetters.lastK = 0;
                for (int i = 0; i < pullLetters.count; i++)
                {
                    pullLetters.id[i] = "";
                    pullLetters.title[i] = "";
                    pullLetters.content[i] = "";
                    pullLetters.time[i] = "";
                }
                pullLetters.count = 0;
            }
            AdjustPull();
            panelPull.Visible=true;
        }
        // pullLetter myDeserializedClass = JsonConvert.DeserializeObject<pullLetter>(myJsonResponse);
        public class LetterW
        {
            public string id { get; set; }
            public string title { get; set; }
            public string content { get; set; }
            public int time { get; set; }
        }
        public class PullLetter
        {
            public int count { get; set; }
            public List<LetterW> letters { get; set; }
        }
        private bool UpdatePull()
        {
            string con = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(phpURL + "?method=pull&mail=" + Properties.Settings.Default.mail + "&pass=" + Base64Decode(Base64Decode(Base64Decode(Base64Decode(Properties.Settings.Default.password)))));
                request.Timeout = 3000;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    con = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Network error. ");
                string msg = ex.Message;
                return false;
            }
            if (con == "Error: User No Entry")
            {
                MessageBox.Show("No user. ");
                return false;
            }
            if (con == "Error: Password Wrong")
            {
                MessageBox.Show("Password is wrong. ");
                return false;
            }
            if (con == "")
            {
                MessageBox.Show("Unknown error. ");
                return false;
            }
            if (con[0] != 'O' || con[1] != 'K')
            {
                MessageBox.Show("Unknown error. ");
                return false;
            }
            string con1 = "";
            for(int i = 2; i < con.Length; i++)
            {
                con1+=con[i];
            }
            PullLetter pullLetter = JsonConvert.DeserializeObject<PullLetter>(con1);
            pullLetters.count = pullLetter.count;
            pullLetters.current = 0;
            pullLetters.lastK = 0;
            for(int i = 0; i < pullLetter.count; i++)
            {
                pullLetters.id[i] = pullLetter.letters[i].id;
                pullLetters.title[i] = pullLetter.letters[i].title;
                using (var reader = new StringReader(pullLetter.letters[i].content))
                {
                    string first = reader.ReadLine();
                    pullLetters.content[i] = first;
                }
                pullLetters.time[i] = "";
                int total = pullLetter.letters[i].time;
                int m = total % 60;
                int h = (total - m)/60;
                if(h > 0)
                {
                    pullLetters.time[i] += h.ToString() + "h ";
                }
                if (m > 0)
                {
                    pullLetters.time[i] += m.ToString() + "m";
                }
                if (h == 0 && m == 0)
                {
                    pullLetters.time[i] += "Just now";
                }
            }
            return true;
        }
        private void buttonCompose_Click(object sender, EventArgs e)
        {
            buttonCompose.BackColor = Color.FromArgb(76, 81, 103);
            buttonPullLetters.BackColor = panelLetters.BackColor;
            buttonReceive.BackColor = panelLetters.BackColor;
            buttonSent.BackColor = panelLetters.BackColor;
            panelCompose.Visible = true;
            panelPull.Visible = false;
        }
        private void buttonReceive_Click(object sender, EventArgs e)
        {
            buttonReceive.BackColor = Color.FromArgb(76, 81, 103);
            buttonCompose.BackColor = panelLetters.BackColor;
            buttonPullLetters.BackColor = panelLetters.BackColor;
            buttonSent.BackColor = panelLetters.BackColor;
            panelPull.Visible = false;
            panelCompose.Visible = false;
        }
        private void buttonSent_Click(object sender, EventArgs e)
        {
            buttonSent.BackColor = Color.FromArgb(76, 81, 103);
            buttonCompose.BackColor = panelLetters.BackColor;
            buttonPullLetters.BackColor = panelLetters.BackColor;
            buttonReceive.BackColor = panelLetters.BackColor;
            panelPull.Visible = false;
            panelCompose.Visible = false;
        }
        private void textBoxComposeTitle_TextChanged(object sender, EventArgs e)
        {
            int lim = 64;
            if (textBoxComposeTitle.Text.Length > lim)
            {
                textBoxComposeTitle.Text=textBoxComposeTitle.Text.Remove(lim);
                textBoxComposeTitle.SelectionStart = textBoxComposeTitle.Text.Length;
                textBoxComposeTitle.SelectionLength = 0;
            }
            labelComposeTitleCount.Text=textBoxComposeTitle.Text.Length.ToString()+"/"+lim.ToString();
        }
        private void textBoxComposeContent_TextChanged(object sender, EventArgs e)
        {
            int lim = 1024;
            try { 
                char lastDigit = textBoxComposeContent.Text[textBoxComposeContent.Text.Length - 1];
                if (textBoxComposeContent.Text.Length > lim||lastDigit=='&')
                {
                    textBoxComposeContent.Text = textBoxComposeContent.Text.Remove(textBoxComposeContent.Text.Length-1);
                    textBoxComposeContent.SelectionStart = textBoxComposeContent.Text.Length;
                    textBoxComposeContent.SelectionLength = 0;
                }
            }
            catch
            {

            }
            labelComposeContentCount.Text = textBoxComposeContent.Text.Length.ToString() + "/" + lim.ToString();
        }
        private void labelComposeContentCount_Click(object sender, EventArgs e)
        {

        }
        private void button6_Click_1(object sender, EventArgs e)
        {
            for(int i = 0; i < textBoxComposeTitle.Text.Length; i++)
            {
                if (textBoxComposeTitle.Text[i] == '&')
                {
                    int j = 1;
                    for(int k=i+1;k< textBoxComposeTitle.Text.Length; k++)
                    {
                        if (textBoxComposeTitle.Text[k] == '&')
                        {
                            j++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    MessageBox.Show("Please do not include \'&\' in the title. ");
                    textBoxComposeTitle.Focus();
                    textBoxComposeTitle.SelectionStart = i;
                    textBoxComposeTitle.SelectionLength = j;
                    return;
                }
            }
            for (int i = 0; i < textBoxComposeContent.Text.Length; i++)
            {
                if (textBoxComposeContent.Text[i] == '&')
                {
                    int j = 1;
                    for (int k = i + 1; k < textBoxComposeContent.Text.Length; k++)
                    {
                        if (textBoxComposeContent.Text[k] == '&')
                        {
                            j++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    MessageBox.Show("Please do not include \'&\' in the content. ");
                    textBoxComposeContent.Focus();
                    textBoxComposeContent.SelectionStart = i;
                    textBoxComposeContent.SelectionLength = j;
                    return;
                }
            }
            if(textBoxComposeTitle.Text=="" && textBoxComposeContent.Text == "")
            {
                MessageBox.Show("Please at least write the title or the content. ");
                return;
            }
            string con = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(phpURL + "?method=send&mail=" + Properties.Settings.Default.mail + "&pass=" + Base64Decode(Base64Decode(Base64Decode(Base64Decode(Properties.Settings.Default.password))))+"&title="+textBoxComposeTitle.Text+"&content="+textBoxComposeContent.Text);
                request.Timeout = 3000;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    con = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Network error. ");
                return;
            }
            if (con == "Error: User No Entry")
            {
                MessageBox.Show("User info not found. ");
                return;
            }
            if (con == "Error: Password Wrong")
            {
                MessageBox.Show("Password is wrong. Did you change your password? Please re-login. ");
                return;
            }
            if (con == "")
            {
                MessageBox.Show("Unspecified error. ");
                return;
            }
            if (con != "OK")
            {
                MessageBox.Show("Unspecified error. ");
                return;
            }
            MessageBox.Show("Sent to the letter pool. The letter will expire in 1 day if no one replys. ");
            textBoxComposeTitle.Text = "";
            textBoxComposeContent.Text = "";


        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Opacity = 1;
            if (!AutoLogin())
            {
                if (!AutoLogin())
                {
                    //MessageBox.Show("Login fail after 2 tries");
                }
            }
            //initializing many controls for letter. 
            
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel19_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://shitao.tech");
        }

        private void labelPullContent4_Click(object sender, EventArgs e)
        {

        }

        private bool saveJson()
        {
            alterWeekly.count = alterCount;
            string output = "";
            if (alterCount >= 100)
            {
                output = JsonConvert.SerializeObject(alterWeekly);
            }
            else
            {
                for (int i = 0; i < alterCount; i++)
                {
                    alterWeekly100.count = alterWeekly.count;
                    alterWeekly100.id[i] = alterWeekly.id[i];
                    alterWeekly100.X[i] = alterWeekly.X[i];
                    alterWeekly100.Y[i] = alterWeekly.Y[i];
                    alterWeekly100.color[i] = alterWeekly.color[i];
                }
                output = JsonConvert.SerializeObject(alterWeekly100);
            }

            Properties.Settings.Default.json = output;
            Properties.Settings.Default.alterCount = alterCount;
            Properties.Settings.Default.Save();

            string con = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(phpURL + "?method=save&mail=" + Properties.Settings.Default.mail + "&pass=" + Base64Decode(Base64Decode(Base64Decode(Base64Decode(Properties.Settings.Default.password))))+"&json="+output);
                request.Timeout = 3000;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    con = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                labelLoginErr.Text = "Network Error";
                string msg = ex.Message;
                return false;
            }
            if (con == "Error: User No Entry")
            {
                MessageBox.Show("User No Entry");
                return false;
            }
            if (con == "Error: Password Wrong")
            {
                MessageBox.Show("Password Wrong");
                return false;
            }
            if (con == "Fail")
            {
                MessageBox.Show("Fail to save the file");
                return false;
            }
            MessageBox.Show(con);

            return true;
        }

    }
}