using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TheEditor
{

    public class ImageHandler
    {
        public string imageDataString;
        public Image imageData;
        public Image DefaultRef;
        public Control imageControllerPreview;
        public Histogram HistogramModule;

        private int offsetNoise = 1;

        public void increaseOffsetNoise(int value)
        {
            this.offsetNoise = value;
        }

        public ImageHandler(Image data, Control controllerRef)
        {
            this.imageControllerPreview = controllerRef;
            this.DefaultRef = data;
            this.updateImageRef(data);
        }

        public Color[][] GetBitmapMatrix()
        {
            Bitmap bitmapToConvert = (Bitmap)DefaultRef;

            int height = bitmapToConvert.Height;
            int width = bitmapToConvert.Width;

            Color[][] colorMatrix = new Color[width][];

            for(int i = 0; i < width; i++)
            {
                colorMatrix[i] = new Color[height];
                for (int j = 0; j < height; j++)
                {
                    colorMatrix[i][j] = bitmapToConvert.GetPixel(i, j);
                }
            }

            return colorMatrix;
        }

        public void updateImageRef(Image data)
        {
            this.imageDataString = data.ToString();
            this.imageData = data;
            this.imageControllerPreview.BackgroundImage = data;
        }

        public void convertImageToBlackAndWhite(double conversionValue)
        {
            // gets decoded bitmap
            Color[][] matrix = GetBitmapMatrix();

            // creates a new bitmap
            Bitmap convertedBitmap = new Bitmap(matrix.GetLength(0), matrix[0].GetLength(0));

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix[0].GetLength(0); j++)
                {
                    Color colorBlock = matrix[i][j];

                    int rgb = (int) Math.Round((
                        (0.299 * conversionValue) * colorBlock.R +
                        (0.587 * conversionValue) * colorBlock.G +
                        (0.114 * conversionValue) * colorBlock.B
                    ));

                    int blendR = (int) ((rgb * conversionValue) + (colorBlock.R * (1 - conversionValue)));
                    int blendG = (int) ((rgb * conversionValue) + (colorBlock.G * (1 - conversionValue)));
                    int blendB = (int) ((rgb * conversionValue) + (colorBlock.B * (1 - conversionValue)));

                    convertedBitmap.SetPixel(i, j, Color.FromArgb(blendR, blendG, blendB));
                }
            }

            this.updateImageRef(convertedBitmap);
            this.imageControllerPreview.Refresh();
            Console.WriteLine("Finished black/white transition...");
        }

        public void rotateImage90degrees()
        {
            Console.WriteLine("Starting image rotation...");

            imageData.RotateFlip(RotateFlipType.Rotate90FlipNone);

            this.imageControllerPreview.Refresh();
            Console.WriteLine("Finished image rotation...");
        }
        public void MirrorImageH()
        {
            Console.WriteLine("Starting image mirroring...");

            imageData.RotateFlip(RotateFlipType.RotateNoneFlipX);

            this.imageControllerPreview.Refresh();
            Console.WriteLine("Finished image rotation...");
        }
        public void MirrorImageV()
        {
            Console.WriteLine("Starting image mirroring...");

            imageData.RotateFlip(RotateFlipType.RotateNoneFlipY);

            this.imageControllerPreview.Refresh();
            Console.WriteLine("Finished image rotation...");
        }

        private int getMedianPoint(List<int> elementos)
        {
            if (elementos.Count % 2 == 0)
            {

                int minor =  (int) Math.Floor((double)(elementos.Count / 2));
                int bigger = (int) Math.Ceiling((double)elementos.Count / 2);

                int colorValueMinor = elementos.FindIndex(i => i == minor);
                int colorValueBigger = elementos.FindIndex(i => i == bigger);

                return (int)(colorValueMinor + colorValueBigger / 2);

            } else
            {
                int index = elementos.Count / 2;
                return elementos[index];
            }

        }

        public Color CalculateMedianPoint(List<Color> Values)
        {
            // ALPHA, RED, GREEN, BLUE

            if (Values.Count == 0)
                return default(Color);

            List<int> red = new List<int>();
            List<int> green = new List<int>();
            List<int> blue = new List<int>();

            foreach (Color element in Values)
            {
                red.Add(element.R);
                green.Add(element.G);
                blue.Add(element.B);
            }

            red.Sort();
            green.Sort();
            blue.Sort();

            int medianRed = this.getMedianPoint(red);
            int medianGreen = this.getMedianPoint(green);
            int medianBlue = this.getMedianPoint(blue);

            return Color.FromArgb(medianRed, medianGreen, medianBlue);
        }

        public void CauseNoiseReduction()
        {
            Console.WriteLine("Noise reduction running...");

            // gets decoded bitmap
            Color[][] matrix = GetBitmapMatrix();

            // creates a new bitmap
            Bitmap convertedBitmap = new Bitmap(matrix.GetLength(0), matrix[0].GetLength(0));

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix[0].GetLength(0); j++)
                {
                    Color currentElement = matrix[i][j];
                    Color left = new Color();
                    Color right = new Color(); 
                    Color top = new Color(); 
                    Color bottom = new Color(); 
                    Color topLeft = new Color(); 
                    Color topRight = new Color(); 
                    Color bottomLeft = new Color(); 
                    Color bottomRight = new Color();

                    // PEGA PIXELS PARENTES EM LINHA RETA
                    if ( !(i - this.offsetNoise < 0) ) {
                        left = matrix[i - this.offsetNoise][j];
                    }
                        
                    if ( !(i + this.offsetNoise > this.imageData.Width - this.offsetNoise) ) {
                        right = matrix[i + this.offsetNoise][j];
                    }
                                
                    if ( !(j - this.offsetNoise < 0) ) {
                        top = matrix[i][j - this.offsetNoise];
                    }
                                
                    if ( !(j + this.offsetNoise > this.imageData.Height - 1) ) {
                        bottom = matrix[i][j + this.offsetNoise];
                    }

                    // PEGA PIXELS NA DIAGONAL
                    if ( !( i - this.offsetNoise < 0) && !(j - this.offsetNoise < 0) ) {
                        topLeft = matrix[i - this.offsetNoise][j - this.offsetNoise];
                    }
                    
                    if ( !(j - this.offsetNoise < 0) && !(i + this.offsetNoise > this.imageData.Width - 1) ) {
                        topRight = matrix[i + this.offsetNoise][j - this.offsetNoise];
                    }
                    
                    if ( !(j + this.offsetNoise > this.imageData.Height - 1) && !(i - this.offsetNoise < 0) ) {
                        bottomLeft = matrix[i - this.offsetNoise][j + this.offsetNoise];
                    }

                    if ( !(j + this.offsetNoise > this.imageData.Height - 1) && !(i + this.offsetNoise > this.imageData.Width - 1) ) {
                        bottomRight = matrix[i + this.offsetNoise][j + this.offsetNoise];
                    }

                    List<Color> items = new List<Color>
                    {
                        currentElement,
                        left,
                        right,
                        top,
                        bottom,
                        topLeft,
                        topRight,
                        bottomLeft,
                        bottomRight
                    };

                    // PEGA O PONTO MÉDIO ENTRE PIXELS
                    Color medianPoint = this.CalculateMedianPoint(items);

                    convertedBitmap.SetPixel(i, j, medianPoint);
                }
            }

            this.updateImageRef(convertedBitmap);
            this.imageControllerPreview.Refresh();
            Console.WriteLine("Noise reduction finished.");
        }

        public String ApplyRobertsCrossGradient()
        {
            // gets decoded bitmap
            Color[][] matrix = GetBitmapMatrix();

            // creates a new bitmap
            Bitmap convertedBitmap = new Bitmap(matrix.GetLength(0), matrix[0].GetLength(0));

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix[0].GetLength(0); j++)
                {
                    Color currentElement = matrix[i][j];
                    Color right = new Color();
                    Color bottom = new Color();
                    Color bottomRight = new Color();

                    if (!(i + 1 > this.imageData.Width - 1))
                    {
                        right = matrix[i + 1][j];
                    }

                    if (!(j + 1 > this.imageData.Height - 1))
                    {
                        bottom = matrix[i][j + 1];
                    }

                    if (!(j + 1 > this.imageData.Height - 1) && !(i + 1 > this.imageData.Width - 1))
                    {
                        bottomRight = matrix[i + 1][j + 1];
                    }

                    // left to right
                    int redX = Math.Abs(currentElement.R - bottomRight.R);
                    int greenX = Math.Abs(currentElement.G - bottomRight.G);
                    int blueX = Math.Abs(currentElement.B - bottomRight.B);

                    // right to left
                    int redY = Math.Abs(right.R - bottom.R);
                    int greenY = Math.Abs(right.G - bottom.G);
                    int blueY = Math.Abs(right.B - bottom.B);

                    // get max
                    int red = Math.Max(redX, redY);
                    int green = Math.Max(greenX, greenY);
                    int blue = Math.Max(blueX, blueY);

                    Color edgeColor = Color.FromArgb(red, green, blue);
                    convertedBitmap.SetPixel(i, j, edgeColor);
                }
            }

            this.updateImageRef(convertedBitmap);
            this.imageControllerPreview.Refresh();
            return "Roberts Border detector applied, results on screen.";
        }

    }
}
