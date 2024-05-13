using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheEditor
{
    public partial class MainWindow : Form
    {
        ImageHandler imageHandle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void importFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // importação de imagem para fazer a edição
        private void importarImagemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialogBox = new OpenFileDialog())
            {
                dialogBox.Filter = "Bitmap File | *.bmp | JPG file | *.jpg; *.jpeg | PNG file | *.png | Todos os arquivos | *.*";

                if (dialogBox.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    string filePath = dialogBox.FileName;

                    // Read the contents of the file into a stream
                    var fileStream = dialogBox.OpenFile();

                    Image data = Image.FromStream(fileStream);

                    Control[] controllers = Controls.Find("imageCanvas", true);

                    // refresh de imagem é uma situação interna (automática)
                    controllers[0].BackgroundImage = data;

                    this.imageHandle = new ImageHandler(data, controllers[0]);

                    this.increaseOffset3x3(this, null);

                    return;
                }

            }
        }

        private void imageCanvas_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (imageHandle != null)
            {
                this.imageHandle.rotateImage90degrees();
            } else
            {
                Console.WriteLine("skipped action, handle points to null");
            }
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        } 

        private void exportarImagemToolStripMenuItem_Click(object sender, EventArgs e)
        {

            using (SaveFileDialog dialogBox = new SaveFileDialog())
            {
                dialogBox.Filter = "Bitmap File | *.bmp | JPG file | *.jpeg | PNG file | *.png";

                if (dialogBox.ShowDialog() == DialogResult.OK)
                {
                    // Get the path of specified file
                    this.imageHandle.imageData.Save(dialogBox.FileName);

                    return;
                }
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            
            if (this.imageHandle == null)
            {
                return;
            }

            TrackBar track = (TrackBar) sender;

            Console.WriteLine($"value: {(track.Value / 10.0)}");
            this.imageHandle.convertImageToBlackAndWhite((track.Value / 10.0));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.imageHandle.updateImageRef(imageHandle.DefaultRef);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.imageHandle.MirrorImageH();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.imageHandle.MirrorImageV();
        }

        private void NoiseReductionButton(object sender, EventArgs e)
        {
            this.imageHandle.CauseNoiseReduction();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void increaseOffset3x3(object sender, EventArgs e)
        {
            if (imageHandle != null)
            {
                this.imageHandle.increaseOffsetNoise(1);
                Control[] controllers = Controls.Find("NoiseTextureValue", true);
                controllers[0].Text = "Current Rule: 3x3";
            }

        }

        private void increaseOffset5x5(object sender, EventArgs e)
        {
            if (imageHandle != null)
            {
                this.imageHandle.increaseOffsetNoise(3);
                Control[] controllers = Controls.Find("NoiseTextureValue", true);
                controllers[0].Text = "Current Rule: 5x5";
            }

        }

        private void increaseOffset7x7(object sender, EventArgs e)
        {
            if (imageHandle != null)
            {
                this.imageHandle.increaseOffsetNoise(5);
                Control[] controllers = Controls.Find("NoiseTextureValue", true);
                controllers[0].Text = "Current Rule: 7x7";
            }

        }

        private void increaseOffset15x15(object sender, EventArgs e)
        {
            if (imageHandle != null)
            {
                this.imageHandle.increaseOffsetNoise(13);
                Control[] controllers = Controls.Find("NoiseTextureValue", true);
                controllers[0].Text = "Current Rule: 15x15";
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checarEqualizacao(object sender, EventArgs e)
        {
            if (imageHandle != null)
            {
                this.imageHandle.HistogramModule = new Histogram(this.imageHandle.GetBitmapMatrix(), 16);

                Control[] controllers = Controls.Find("ConsoleOutput", true);

                controllers[0].ResetText();
                controllers[0].Text = this.imageHandle.HistogramModule.GetResults();
            }
        }

        private void ConsoleOutput_TextChanged(object sender, EventArgs e)
        {

        }

        private void ConsoleOutput_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void button5_Click(object sender, EventArgs e)
        {
            if (imageHandle != null)
            {
                Control[] controllers = Controls.Find("ConsoleOutput", true);

                controllers[0].ResetText();
                controllers[0].Text = "Applying Roberts Crossing Gradient, wait please...";
                controllers[0].Text += String.Format("\n" + this.imageHandle.ApplyRobertsCrossGradient());
            }
        }
    }
}
