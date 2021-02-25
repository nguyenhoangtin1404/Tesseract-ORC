using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using ImageMagick;

namespace Tesseract_ORC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private string OCR(Pix b)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "vie", EngineMode.Default))
            {
                
                using (var page = engine.Process(b, PageSegMode.AutoOnly))
                    res = page.GetText();
            }
            return res;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                using (var img = new MagickImage(openFileDialog1.FileName))
                {

                    img.ColorFuzz = new Percentage(15);
                    // -transparent white
                    img.Transparent(MagickColors.White);


                    //img.Threshold((Percentage)60); // 60 is OK 
                    //img.Depth = 1;

                    //-bordercolor black
                    //img.BorderColor = MagickColors.Black;

                    //img.Border(1);

                    //img.FloodFill(MagickColors.White, 0, 0);
                    // -alpha off




                    img.Alpha(AlphaOption.Off);
                    // -shave 1x1
                    img.Shave(1, 1);


                    img.Write("img/image.png");
                    
                }

                pictureBox1.LoadAsync("img/image.png");
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string result = "";
            Task.Factory.StartNew(() => {
                picloading.BeginInvoke(new Action(() =>
                {
                    picloading.Visible = true;
                }));

                result = OCR(Pix.LoadFromFile("img/image.png"));
                richTextBox1.BeginInvoke(new Action(() => {

                    richTextBox1.Text = result;

                }));
                picloading.BeginInvoke(new Action(() =>
                {
                    picloading.Visible = false;
                }));

            });
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var settings = new MagickReadSettings();
                // Settings the density to 300 dpi will create an image with a better quality
                settings.Density = new Density(300);

                using (var images = new MagickImageCollection())
                {
                    // Add all the pages of the pdf file to the collection
                    images.Read(openFileDialog1.FileName, settings);

                    /*    // Create new image that appends all the pages horizontally
                        using (var horizontal = images.AppendHorizontally())
                        {
                            // Save result as a png
                            horizontal.Write("img/image.png");
                        }*/

                    // Create new image that appends all the pages vertically
                    using (var vertical = images.AppendVertically())
                    {
                        // Save result as a png
                        vertical.Write("img/image.png");
                    }
                }
            }
           
        }
    }
}
