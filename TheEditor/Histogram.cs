using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEditor
{
    public struct nivelInformation
    {
        public String levelName;
        public int nk;
        public float prRK;
        public float sk;
        public int faixaDeBits;
        public float rkR;
        public int novoSK;
    }


    public class Histogram
    {
        Color[][] imageRef;
        int levelsBits;
        int levels;
        int totalSize;
        int difference;
        int complement;
        float inv;

        List<nivelInformation> niveisInfo = new List<nivelInformation>();

        public Histogram(Color[][] Image, int levelsSize)
        {
            // Image Bitmap
            this.imageRef = Image;
            // 8 = 256 bits
            this.levelsBits = levelsSize;

            this.createAndDefine();
        }

        private void createAndDefine()
        {
            int height = imageRef.Length;
            int width = imageRef[0].Length;
            this.levels = (int)Math.Pow(2, this.levelsBits);
            this.totalSize = height * width;
            // COLOR PATTERN 256 BITS, NO ACCEPT 32 / 64
            this.difference = (int)(256 / this.levelsBits);
            // CALCULATES COMPLEMENT
            this.complement = (int)(this.totalSize / (this.levelsBits - 1));
            this.inv = (float)this.complement / this.totalSize;

            int levelCounter = 1;

            for (int i = 0; i < this.levelsBits; i++)
            {
                nivelInformation nivelModel = new nivelInformation();

                nivelModel.levelName = (i + "/" + (this.levelsBits - 1));

                nivelModel.faixaDeBits = i;

                nivelModel.rkR = (float)i * this.inv;

                this.niveisInfo.Add(nivelModel);

            }

            foreach (Color[] HeightColor in this.imageRef)
            {
                foreach (Color WidthColor in HeightColor)
                {
                    nivelInformation data = this.niveisInfo.Find((component) => {
                        return component.faixaDeBits == (int)(WidthColor.R + WidthColor.G + WidthColor.B) / (this.difference * 3);
                    });

                    data.nk += 1;
                    data.prRK = (float)data.nk / (float)this.totalSize;

                    float totalSK = 0f;
                    for (int i = 0; i < data.faixaDeBits; i++)
                    {
                        totalSK += this.niveisInfo[i].prRK;
                    }

                    data.sk = totalSK;

                    this.niveisInfo[data.faixaDeBits] = data;

                }
            }
        }

        private void calculateNovoSK()
        {
            int currentLevel = 1;
            for (int i = 0; i < this.niveisInfo.Count(); i++)
            {
                nivelInformation info = this.niveisInfo[i];

                if (this.niveisInfo[i].sk < this.niveisInfo[i].rkR || i == 0)
                {
                    info.novoSK = currentLevel;
                }
                else
                {
                    currentLevel++;
                    info.novoSK = currentLevel;
                }

                this.niveisInfo[i] = info;
            }
        }

        public String GetResults()
        {
            this.calculateNovoSK();


            return printInformation();
        }

        public String printInformation()
        {
            String returnOutput = "";

            returnOutput += String.Format("Histogram: \t| Niveis: {0} \t| Pixels: {1} \t| Diferença: {2} \t| Complemento: {3} \t| Inv: {4}\n", this.levelsBits, this.totalSize, this.difference, this.complement, this.inv);

            foreach (nivelInformation data in this.niveisInfo)
            {
                returnOutput += String.Format("\nNivel: {0} \t| Pixels: {1} \t| Porcentagem: {2} \t| Porcentagem atual: {3} \t| rk(R): {4} \t| NovoSK: {5}", data.levelName, data.nk, data.prRK.ToString("0.0000"), data.sk.ToString("0.0000"), data.rkR.ToString("0.0000"), data.novoSK);
            }

            return returnOutput;
        }
    }
}
