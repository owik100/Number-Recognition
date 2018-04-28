using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Recognition_of_numbers
{
    public partial class Form1 : Form
    {
     
        Graphics graphic;
        Pen penBlack;
        Color penColor = new Color();

        int[][] tabFromPanel;

        public Form1()
        {
            InitializeComponent();

            penColor = Color.FromArgb(0, 150, 50);

            graphic = panelDrawing.CreateGraphics();

            penBlack = new Pen(Color.Black, 1);


            tabFromPanel = new int[10][];
            for (int i = 0; i < 10; i++)
            {
                tabFromPanel[i] = new int[10];
            }
        }


        private void panelDrawing_Paint(object sender, PaintEventArgs e)
        {
            for (float i = panelDrawing.Height / 10; i < panelDrawing.Height; i += panelDrawing.Height / 10)
            {
                graphic.DrawLine(penBlack, 0, i, panelDrawing.Width, i);

            }
            for (float i = panelDrawing.Width / 10; i < panelDrawing.Width; i += panelDrawing.Width / 10)
            {
                graphic.DrawLine(penBlack, i, 0, i, panelDrawing.Height);

            }
        }


        private void SaveToTable()
        {
            Bitmap bit;
            Graphics graph;
            Rectangle rect;


            if (textBox1.Text != null)
            {
                textBox1.Text = "";
            }


            bit = new Bitmap(panelDrawing.Width, panelDrawing.Height);
            graph = Graphics.FromImage(bit);
            rect = panelDrawing.RectangleToScreen(panelDrawing.ClientRectangle);
            graph.CopyFromScreen(rect.Location, Point.Empty, panelDrawing.Size);

            Color a = new Color();

            int H = 15;

            for (int i = 0; i < 10; i++)
            {
                int W = 15;
                for (int j = 0; j < 10; j++)
                {
                    a = bit.GetPixel(W, H);
                    if (a == penColor)
                    {
                        tabFromPanel[i][j] = 1;
                    }
                    else
                    {
                        tabFromPanel[i][j] = 0;
                    }

                    
                    int temp;
                    temp = tabFromPanel[i][j];
                    W += 30;
                }
                H += 30;
            }

           
        }

        private bool AllEmpty()
        {
            int emptyCounter = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                   if(tabFromPanel[i][j]==0)
                    {
                        emptyCounter++;
                    }
                }
            }

            if (emptyCounter == 100)
            {
                return true;
            }
            else return false;
        }

        private void panelDrawing_MouseMove(object sender, MouseEventArgs e)
        {
            Brush b = null;

            if (e.Button == MouseButtons.Left) b = new SolidBrush(penColor);
            else if (e.Button == MouseButtons.Right) b = b = new SolidBrush(Color.WhiteSmoke);
            if (b != null)
            {
                int w = 0, h = 0;


                for (int i = 1; i < 11; i++)
                {
                    if (e.Y < (panelDrawing.Height / 10)) h = 1;
                    else if (e.Y > (panelDrawing.Height / 10) * (i - 1) && e.Y < (panelDrawing.Height / 10) * i) h = i;
                    if (e.X < (panelDrawing.Width / 10)) w = 1;
                    else if (e.X > (panelDrawing.Width / 10) * (i - 1) && e.X < (panelDrawing.Width / 10) * i) w = i;
                }

                float w1, w2, h1, h2;
                w1 = (panelDrawing.Width * (w - 1)) / 10;
                w2 = panelDrawing.Width / 10;
                h1 = (panelDrawing.Height * (h - 1)) / 10;
                h2 = panelDrawing.Height / 10;

                graphic.FillRectangle(b, w1 + 1, h1 + 1, w2 - 1, h2 - 1);
                b = null;
            }
        }

        private void buttonTeachClick(object sender, EventArgs e)
        {
  
            int? whatNumber = null;

            foreach (Control control in groupBoxTeaching.Controls)
            {

                if (control is RadioButton)
                {
                    RadioButton radio = control as RadioButton;
                    if (radio.Checked)
                    {
                        whatNumber = Convert.ToInt32(control.Text);
                    }
                }
            }

            if(whatNumber ==null)
            {
                textBox1.Text = "Selct number!";
                return;
            }

            string desktopPatch = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            desktopPatch += @"\Data number\";
            if (!Directory.Exists(desktopPatch))
            {
                Directory.CreateDirectory(desktopPatch);
            }

            SaveToTable();

            textBox1.Text = "Teaching complete!";

            using (StreamWriter sw = new StreamWriter(desktopPatch + whatNumber + ".txt", true))
            {

                for (int i = 0; i < tabFromPanel.GetLength(0); i++)
                {
                    for (int j = 0; j < tabFromPanel.GetLength(0); j++)
                    {
                        sw.Write(tabFromPanel[i][j]);
                    }

                    sw.WriteLine();
                }
            }

        }

        private void buttonClean_Click(object sender, EventArgs e)
        {
            panelDrawing.BackgroundImage = null;
            textBox1.Text = "";

            panelDrawing.Refresh();

            panelDrawing_Paint(null, null);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    tabFromPanel[i][j] = 0;
                }
            } 
       }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            SaveToTable();

            if (AllEmpty())
            {
                textBox1.Text = "Empty Box!";
                goto StopChecking;
            }


            string desktopPatch = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            desktopPatch += @"\Data number\";

            if (!Directory.Exists(desktopPatch))
            {
                Directory.CreateDirectory(desktopPatch);
            }


            DirectoryInfo di = new DirectoryInfo(desktopPatch);
            FileInfo[] fi = di.GetFiles("*.txt", SearchOption.AllDirectories);

            int[] chanceForNumber = new int[10];
            int howManyNumbersInFile = 0;
            int whichNumberNow = 0;

            foreach (var file in fi)
            {
                int correctness = 0;
                
             

                using (StreamReader sr = new StreamReader(file.FullName))
                {


                    int lineCount = File.ReadLines(file.FullName).Count();

                    howManyNumbersInFile = lineCount / 10;


                    int[][] localtab = new int[lineCount][];

                    for (int i = 0; i < lineCount; i++)
                    {
                        localtab[i] = new int[10];
                    }


                    for (int i = 0; i < localtab.GetLength(0); i++)
                    {
                        string line = sr.ReadLine();

                        for (int j = 0; j < 10; j++)
                        {
                            int symbol = line[j] == 48 ? 0 : 1;
                            localtab[i][j] = symbol;
                        }

                    }

                   
                    

                    int[] chance = new int[howManyNumbersInFile];

                    int lineCounter = 0;

                    for (int i = 0; i <howManyNumbersInFile; i++)
                    {
                       

                        for (int j=0; j < tabFromPanel.GetLength(0); j++)
                        {
             
                            for (int k = 0; k < tabFromPanel.GetLength(0); k++)
                            {
                                if (tabFromPanel[j][k] == localtab[lineCounter][k])
                                {
                                    correctness++;
                                }
                            }
                            lineCounter++;
                        }

                        if(correctness>=95)
                        {
                            ShowResult(whichNumberNow, correctness);
                            goto StopChecking;
                        }

                        chance[i] = correctness;
                        correctness = 0;
                    }


                    chanceForNumber[whichNumberNow] = Convert.ToInt32(chance.Average());
                    whichNumberNow++;
                   
                }

            }

           

            int chanceForCorrect = chanceForNumber.Max();
            int whatNumber = Array.IndexOf(chanceForNumber, chanceForCorrect);



            ShowResult(whatNumber, chanceForCorrect);
      

            StopChecking:
            {

            }


        }

        void ShowResult(int whatNumber,int chanceForCorrect)
        {
            textBox1.Text = "Detected number " + whatNumber + " Chance " + chanceForCorrect + "%";
        }
    }
}
