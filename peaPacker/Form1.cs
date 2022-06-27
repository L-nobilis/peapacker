namespace peaPacker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void openImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image newImage = Image.FromFile(openFileDialog1.FileName);
                Bitmap[] channels = SplitChannels((Bitmap)newImage);

                pictureBoxR.Image = channels[0];
                pictureBoxG.Image = channels[1];
                pictureBoxB.Image = channels[2];
                pictureBoxA.Image = channels[3];

                RecombineChannels();
            }

        }

        private void pictureBoxR_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image newImage = Image.FromFile(openFileDialog1.FileName);
                pictureBoxR.Image = SplitOneChannel((Bitmap)newImage, "r");
                RecombineChannels();
            }
        }

        private void pictureBoxG_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image newImage = Image.FromFile(openFileDialog1.FileName);
                pictureBoxG.Image = SplitOneChannel((Bitmap)newImage, "g");
                RecombineChannels();
            }
        }

        private void pictureBoxB_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image newImage = Image.FromFile(openFileDialog1.FileName);
                pictureBoxB.Image = SplitOneChannel((Bitmap)newImage, "b");
                RecombineChannels();
            }
        }

        private void pictureBoxA_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image newImage = Image.FromFile(openFileDialog1.FileName);
                pictureBoxA.Image = SplitOneChannel((Bitmap)newImage, "a");
                RecombineChannels();
            }
        }

        public Bitmap[] SplitChannels(Bitmap image)
        {
            Bitmap[] channelArray = new Bitmap[4];

            Bitmap redChannel = new Bitmap(image.Width, image.Height);
            Bitmap greenChannel = new Bitmap(image.Width, image.Height);
            Bitmap blueChannel = new Bitmap(image.Width, image.Height);
            Bitmap alphaChannel = new Bitmap(image.Width, image.Height);

            // Loop over all the pixels in the passed-in image.
            for (int i=0; i<image.Width; i++)
            {
                for (int j=0; j<image.Height; j++)
                {
                    Color pixelColor = image.GetPixel(i, j);

                    redChannel.SetPixel(i, j, Color.FromArgb(pixelColor.R, pixelColor.R, pixelColor.R));
                    greenChannel.SetPixel(i, j, Color.FromArgb(pixelColor.G, pixelColor.G, pixelColor.G));
                    blueChannel.SetPixel(i, j, Color.FromArgb(pixelColor.B, pixelColor.B, pixelColor.B));
                    alphaChannel.SetPixel(i, j, Color.FromArgb(pixelColor.A, pixelColor.A, pixelColor.A));
                }
            }

            channelArray[0] = redChannel;
            channelArray[1] = greenChannel;
            channelArray[2] = blueChannel;
            channelArray[3] = alphaChannel;

            return channelArray;
        }

        public Bitmap SplitOneChannel(Bitmap image, string channel)
        {
            Bitmap isolatedChannel = new Bitmap(image.Width, image.Height);

            for (int i=0; i<isolatedChannel.Width; i++)
            {
                for (int j=0; j<isolatedChannel.Height; j++)
                {
                    Color pixelColor = image.GetPixel(i, j);
                    switch (channel)
                    {
                        case "r":
                            isolatedChannel.SetPixel(i, j, Color.FromArgb(pixelColor.R, pixelColor.R, pixelColor.R));
                            break;
                        case "g":
                            isolatedChannel.SetPixel(i, j, Color.FromArgb(pixelColor.G, pixelColor.G, pixelColor.G));
                            break;
                        case "b":
                            isolatedChannel.SetPixel(i, j, Color.FromArgb(pixelColor.B, pixelColor.B, pixelColor.B));
                            break;
                        case "a":
                            isolatedChannel.SetPixel(i, j, Color.FromArgb(pixelColor.A, pixelColor.A, pixelColor.A));
                            break;
                    }
                }
            }
            return isolatedChannel;
        }

        public void RecombineChannels()
        {
            Bitmap outputImage = new Bitmap(pictureBoxR.Image.Width, pictureBoxR.Image.Height);
            Bitmap bitmapR = (Bitmap)pictureBoxR.Image;
            Bitmap bitmapG = (Bitmap)pictureBoxG.Image;
            Bitmap bitmapB = (Bitmap)pictureBoxB.Image;
            Bitmap bitmapA = (Bitmap)pictureBoxA.Image;
            for (int i=0; i<outputImage.Width; i++)
            {
                for (int j=0; j<outputImage.Height; j++)
                {
                    outputImage.SetPixel(i, j, Color.FromArgb(bitmapA.GetPixel(i, j).R, bitmapR.GetPixel(i,j).R, bitmapG.GetPixel(i, j).R, bitmapB.GetPixel(i, j).R));
                }
            }
            pictureBoxOutput.Image = outputImage;
        }

        private void splitContainerG_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void pictureBoxOutput_Click(object sender, EventArgs e)
        {
            saveAsButton_Click(sender, e);
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            //Displays a Save File Dialog so the user can save the outputImage. 
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp";
            saveFileDialog.FilterIndex = 2; //One-indexed! Sets the default image type to PNG.  
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Title = "Save output";

            if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName != "") //Filename cannot be an empty string
            {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();

                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        pictureBoxOutput.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 2:
                        pictureBoxOutput.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case 3:
                        pictureBoxOutput.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }
                fs.Close();
            }
        }
    }
}