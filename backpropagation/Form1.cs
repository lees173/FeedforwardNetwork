using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace backpropagation
{

    /* Handwritten digits
     * Download input data from (MNIST) http://yann.lecun.com/exdb/mnist/
     * 
     * */

    public partial class Form1 : Form
    {
        int[,] wMatrix = new int[28 * 28, 28 * 28];
        string path = Application.StartupPath + @"\..\train-images\";
        double[][] imageInputData;
        NNetworks network;
        public Form1()
        {
            InitializeComponent();
           
        }



        public void button8_Click(object sender, EventArgs e)
        {
            

            String imageFilePath = path + "train-images.idx3-ubyte";
            String labelFilePath = path + "train-labels.idx1-ubyte";

            FileStream fs = new FileStream(@imageFilePath, FileMode.OpenOrCreate);
            FileStream fs1 = new FileStream(@labelFilePath, FileMode.OpenOrCreate);

            BinaryReader r = new BinaryReader(fs);
            BinaryReader r1 = new BinaryReader(fs1);

            for (Int32 i = 0; i < 4; i++)
            {

                BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
            }

            for (Int32 i = 0; i < 2; i++)
            {

                BitConverter.ToInt32(r1.ReadBytes(4).Reverse().ToArray(), 0);
            }
            int numImage = 60000;
            if (textBox2.Text != null)
            {
                numImage = Int32.Parse(textBox2.Text);
            }

            double[][] target = new double[numImage][];
            imageInputData = new double[numImage][];


            for (int count = 0; count < numImage; count++)
            {
                byte[] data = new byte[28 * 28];
                for (int i = 0; i < 28 * 28; i++)
                {
                    data[i] = r.ReadByte();
                }

                Bitmap bitmap = new Bitmap(28, 28);
                int[] tempData = new int[28 * 28];
                imageInputData[count] = new double[28*28];

                ///get bit map from input data
                for (int row = 0; row < 28; row++)
                {
                    for (int col = 0; col < 28; col++)
                    {
                        bitmap.SetPixel(col, row, Color.FromArgb(255 - data[row * 28 + col], 255 - data[row * 28 + col], 255 - data[row * 28 + col]));

                        if (data[row * 28 + col] == 0)
                        {
                            imageInputData[count][row * 28 + col] = 0;
                        }
                        else
                        {
                            imageInputData[count][row * 28 + col] = 1;
                        }

                    }
                }
                target[count] = new double[10];
                int getlabelNum = Convert.ToInt32(r1.ReadByte());
                for(int i=0; i<10;i++){
                    if (getlabelNum == i)
                    {
                        target[count][i]=1;
                    }
                    else
                    {
                        target[count][i] = 0;
                    }


                }
                

                


                textBox3.Text = "Making Matrix: " + (count + 1).ToString() + " / " + numImage.ToString();
                textBox3.Refresh();
                pictureBox1.Image = bitmap;
                pictureBox1.Refresh();
            }
            r.Close();
            fs.Close();
            r1.Close();
            fs1.Close();


            int hNode = Convert.ToInt32(textBox9.Text);
            network = new NNetworks();

            double sensitive = Convert.ToDouble(textBox7.Text);
            int epoch = Convert.ToInt32(textBox8.Text);

            network.Neuron(hNode, sensitive);

            for (int ep = 0; ep < epoch;ep++ )
            {
                textBox3.Text = "epoch:" + (ep+1).ToString(); ;
                textBox3.Refresh();
                    
                for (int i = 0; i < numImage; i++)
                {
                    network.sigmoid(imageInputData[i]);
                    network.learn(imageInputData[i], target[i]);

                }
                   
                               

            }
            textBox3.Text = "Training Done" ;
            textBox3.Refresh();
            

/////////////////////////
            
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            String testImgFile  = path + "t10k-images.idx3-ubyte";
            String testlabelPath = "t10k-labels.idx1-ubyte";

           // String testImgFile = path + "train-images.idx3-ubyte";
           // String testlabelPath = path + "train-labels.idx1-ubyte";

            FileStream fs = new FileStream(testImgFile, FileMode.OpenOrCreate);

            BinaryReader r = new BinaryReader(fs);


            FileStream fs1 = new FileStream(testlabelPath, FileMode.Open);
            BinaryReader r1 = new BinaryReader(fs1);


            for (Int32 i = 0; i < 2; i++)
            {

                BitConverter.ToInt32(r1.ReadBytes(4).Reverse().ToArray(), 0);
            }


            for (Int32 i = 0; i < 4; i++)
            {

                BitConverter.ToInt32(r.ReadBytes(4).Reverse().ToArray(), 0);
            }

            int numImage = Convert.ToInt32(textBox5.Text);
            int wCount=0;

            for (int count = 0; count < numImage; count++)
            {
                Bitmap bitmap = new Bitmap(28, 28);
                byte[] data = new byte[28 * 28];



                double[] tempData = new double[28 * 28];
                for (int i = 0; i < 28 * 28; i++)
                {
                    data[i] = r.ReadByte();
                }

                for (int col = 0; col < 28; col++)
                {

                    for (int row = 0; row < 28; row++)
                    {
                        bitmap.SetPixel(col, row, Color.FromArgb(255 - data[(row) * 28 + col], 255 - data[(row) * 28 + col], 255 - data[(row) * 28 + col]));
                        if (data[(row) * 28 + col] == 0)
                        {
                            tempData[(row) * 28 + col] = 0;
                        }
                        else
                        {
                            tempData[(row) * 28 + col] = 1;
                        }
                    }


                }

                pictureBox3.Image = bitmap;
                pictureBox3.Refresh();

                int testNumber = Convert.ToInt32(r1.ReadByte());
                int hNodes = Convert.ToInt32(textBox9.Text);
               
                 double[] test = network.sigmoid(tempData);

                   

                
                int getAnswer = 0;
                double max = 0;
                for (int i = 1; i < 10+1; i++)
                {

                    if (max < test[i])
                    {
                        max = test[i];
                        getAnswer = i;
                    }

                }
                double rating = test[getAnswer];


                textBoxAnswer.Text = (getAnswer-1).ToString() + " (rate: " + rating.ToString("F3") + ")";
                textBoxAnswer.Refresh();


                if((getAnswer-1)!=testNumber)
                {
                    wCount++;
                }

                textBox4.Text = "Error rate: " + (((double)wCount / (double)(count+1))*100).ToString("F2") + " %";
                textBox4.Refresh();


                int timer = 0;

                if (!textBox6.Text.Equals(""))
                {
                    timer = Convert.ToInt32(textBox6.Text);
                }
                System.Threading.Thread.Sleep(timer);



            }
            r.Close();
            r1.Close();
            fs1.Close();
            fs.Close();


        }


    }


    
    class NNetworks
    {
        public NNetworks()
        {

        }
        int hNode;
        double[][] weight1;//=new double[10][];
        double[][] weight;//=new double[10][];
        double[] hidden;
        double[] input;
        double[] output;
        double senstive;

        public void Neuron(int hNode, double senstive)
        {
            this.senstive = senstive;
            weight = new double[hNode+1][];
            weight1 = new double[10 + 1][];
            Random rnd = new Random();
            this.hNode = hNode;

            hidden = new double[hNode+1];
            input = new double[28 * 28 + 1];
            output = new double[10 + 1];


            for (int j = 0; j < hNode + 1; j++)
            {
                weight[j] = new double[28*28+1];
                for (int i = 0; i < 28*28+1; i++)
                {
                    weight[j][i] = rnd.NextDouble() * 2 - 1;
                   
                }
            }
            for (int j = 0; j < 10 + 1; j++)
            {
                weight1[j] = new double[hNode + 1];
                for (int i = 0; i < hNode + 1; i++)
                {
                    weight1[j][i] = rnd.NextDouble() * 2 - 1;

                }
            }
            
        }
        public double[] sigmoid(double[] inputData)
        {
          

            input[0] = 1.0;
            hidden[0] = 1.0;

            for (int i = 0; i < 28*28; i++)
            {
                input[i + 1] = inputData[i];
            }

            for (int j = 1; j < hNode+1; j++)
            {
                hidden[j] = 0.0;
                for (int i = 0; i < 28*28+1; i++)
                {
                    hidden[j] += weight[j][i] * input[i];
                }
                hidden[j] = 1.0 / (1.0 + Math.Exp(-hidden[j]));
            }

            // Passing through output layer
            for (int j = 1; j < 10 +1; j++)
            {
                output[j] = 0.0;
                for (int i = 0; i <= hNode; i++)
                {
                    output[j] += weight1[j][i] * hidden[i];
                }
                output[j] = 1.0 / (1 + Math.Exp(-output[j]));
            }

            return output;
            


        }

        public void learn(double[] inputData, double[] target)
        {
            
            double[] error2 = new double[10 + 1];
            double[] error1 = new double[hNode + 1];
            double sum = 0.0;

            for (int i = 1; i < 10+1; i++)  
                error2[i] = output[i] * (1.0 - output[i]) * ( output[i]-target[i - 1]);


            for (int i = 0; i <= hNode; i++)
            {  
                for (int j = 1; j < 10+1; j++)
                    sum += weight1[j][i] * error2[j];

                error1[i] = hidden[i] * (1.0 - hidden[i]) * sum;
                sum = 0.0;
            }

            //the gradient descent
            for (int j = 1; j < 10+1; j++)
                for (int i = 0; i < hNode+1; i++)
                    weight1[j][i] -= senstive * error2[j] * hidden[i];

            for (int j = 1; j < hNode+1; j++)
                for (int i = 0; i < 28*28 +1; i++)
                    weight[j][i] -= senstive * error1[j] * input[i];

        }



    }


}