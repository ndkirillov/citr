using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace citr
{
    public partial class Form1 : Form
    {
        string path = "";
        string filter = "Image Files (*.bmp)|*.bmp";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = filter;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                label2.Text = ofd.FileName;
                path = ofd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (path == "")
            {
                return;
            }
            Bitmap img = (Bitmap)Image.FromFile(@path);
            bool[,] pik0 = new bool[img.Height, img.Width];
            Color pix = img.GetPixel(0, 0);
            int minx = img.Height, miny = img.Width, maxx = 0, maxy = 0;
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (img.GetPixel(j, i) != pix)
                    {
                        pik0[i, j] = true;
                        minx = Math.Min(minx, i);
                        miny = Math.Min(miny, j);
                        maxx = Math.Max(maxx, i);
                        maxy = Math.Max(maxy, j);
                    }
                }
            }
            // вот тут уменьшим картинку чтобы были точнее резы
            int width = maxy - miny + 1;
            int height = maxx - minx + 1;
            bool[,] pik = new bool[height, width];
            for (int i = minx; i <= maxx; i++)
            {
                for (int j = miny; j <= maxy; j++)
                {
                    pik[i - minx, j - miny] = pik0[i, j];
                }
            }
            // закончили уменьшать картинку

            int deep = 10;
            int[] points = new int[deep];
            int start = 5;
            int g = 0;
            for (int k = start; k < start + deep; k++) // кол-во шагов
            {
                g = 0;
                for (int i = 0; i <= height / k; i++) // квадраты большие
                {
                    for (int j = 0; j <= width / k; j++) // квадраты большие
                    {
                        for (int ii = 0; ii < k; ii++)  // квадраты малые
                        {
                            if (k * i + ii >= height)
                            {
                                continue;
                            }
                            for (int jj = 0; jj < k; jj++)  // квадраты малые
                            {
                                if (k * j + jj >= width)
                                {
                                    continue;
                                }
                                if (pik[k * i + ii, k * j + jj])
                                {
                                    g++;
                                    jj = k;
                                    ii = k;
                                }
                            }
                        }
                    }
                }
                points[k - start] = g;
                if (g == 1)
                {
                    deep = k - start;
                }
            }
            double[,] loge = new double[2, deep];
            for (int i = 0; i < deep; i++)
            {
                loge[0, i] = Math.Log(start + i);
                loge[1, i] = Math.Log(points[i]);
            }
            double sumx = 0, sumy = 0, sumxy = 0, sumxx = 0;
            for (int i = 0; i < deep; i++)
            {
                sumx += loge[0, i];
                sumy += loge[1, i];
                sumxx += loge[0, i] * loge[0, i];
                sumxy += loge[0, i] * loge[1, i];
            }
            label1.Text = (-(sumxy - sumy * sumx / deep) / (sumxx - sumx * sumx / deep)).ToString();
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
