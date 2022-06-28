namespace peaPacker
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using System.Diagnostics;

    public partial class Form1 : Form
    {
        public int currentWidth;
        public int currentHeight;

        public Form1()
        {
            InitializeComponent();
        }


        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        // https://swharden.com/csdv/platforms/imagesharp/
        private void openImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image newImage = Image.Load(openFileDialog1.FileName);
                currentWidth = newImage.Width;
                currentHeight = newImage.Height;

                //Image[] channels = SplitChannels(newImage);

                PictureBox[] channelBoxes = new PictureBox[4];
                channelBoxes[0] = pictureBoxR;
                channelBoxes[1] = pictureBoxG;
                channelBoxes[2] = pictureBoxB;
                channelBoxes[3] = pictureBoxA;

                int i = 0;
                foreach (PictureBox box in channelBoxes)
                {
                    var stream = new System.IO.MemoryStream();
                    SplitOneChannel(newImage, i).SaveAsBmp(stream);
                    System.Drawing.Image channelImg = System.Drawing.Image.FromStream(stream);


                    box.Image?.Dispose();
                    box.Image = channelImg;

                    i++;
                }

                //var stream2 = new System.IO.MemoryStream();
                //SplitOneChannel(newImage, "r").SaveAsBmp(stream2);
                //pictureBoxR.Image = System.Drawing.Image.FromStream(stream2);

                //RecombineChannels();

                outputSizeLabel.Text = $"Output size: {currentWidth} x {currentHeight}";
            }

        }

        private void pictureBoxR_Click(object sender, EventArgs e)
        {
            LoadIndividualChannel("r");
        }

        private void pictureBoxG_Click(object sender, EventArgs e)
        {
            LoadIndividualChannel("g");
        }

        private void pictureBoxB_Click(object sender, EventArgs e)
        {
            LoadIndividualChannel("b");
        }

        private void pictureBoxA_Click(object sender, EventArgs e)
        {
            LoadIndividualChannel("a");
        }

        public void LoadIndividualChannel(string channel)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //    Image newImage = Image.FromFile(openFileDialog1.FileName);
                //    if (newImage.Width != currentWidth || newImage.Height != currentHeight)
                //    {
                //        MessageBox.Show($"Channel size must match original image size: {currentWidth} x {currentHeight}");
                //    }
                //    else
                //    {
                //        switch (channel)
                //        {
                //            case "r":
                //                pictureBoxR.Image = SplitOneChannel((Bitmap)newImage, channel);
                //                break;
                //            case "g":
                //                pictureBoxG.Image = SplitOneChannel((Bitmap)newImage, channel);
                //                break;
                //            case "b":
                //                pictureBoxB.Image = SplitOneChannel((Bitmap)newImage, channel);
                //                break;
                //            case "a":
                //                pictureBoxA.Image = SplitOneChannel((Bitmap)newImage, channel);
                //                break;
                //        }
                //        RecombineChannels();
                //    }
            }
        }

        // https://docs.sixlabors.com/articles/imagesharp/pixelbuffers.html
        public Image[] SplitChannels(Image sourceImage)
        {
            Debug.WriteLine("Splitting channels...");
            Image[] channelArray = new Image[4];
            Image redChannel = sourceImage;
            Image greenChannel = sourceImage;
            Image blueChannel = sourceImage;
            Image alphaChannel = sourceImage;

            redChannel.Mutate(r => r.ProcessPixelRowsAsVector4(row => {
                for (int x = 0; x < row.Length; x++)
                {
                    row[x] = new System.Numerics.Vector4(row[x].X, row[x].X, row[x].X, 1);
                }
            }));
            greenChannel.Mutate(g => g.ProcessPixelRowsAsVector4(row =>
            {
                for (int x = 0; x < row.Length; x++)
                {
                    row[x] = new System.Numerics.Vector4(row[x].Y, row[x].Y, row[x].Y, 1);
                }
            }));
            blueChannel.Mutate(b => b.ProcessPixelRowsAsVector4(row =>
            {
                for (int x = 0; x < row.Length; x++)
                {
                    row[x] = new System.Numerics.Vector4(row[x].Z, row[x].Z, row[x].Z, 1);
                }
            }));
            alphaChannel.Mutate(a => a.ProcessPixelRowsAsVector4(row =>
            {
                for (int x = 0; x < row.Length; x++)
                {
                    row[x] = new System.Numerics.Vector4(row[x].W, row[x].W, row[x].W, 1);
                }
            }));
            channelArray[0] = redChannel;
            channelArray[1] = greenChannel;
            channelArray[2] = blueChannel;
            channelArray[3] = alphaChannel;

            return channelArray;
        }

        public Image SplitOneChannel(Image sourceImage, int channel)
        {
            Image isolatedChannel = new Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(sourceImage.Width, sourceImage.Height);
           // isolatedChannel = sourceImage;

            switch (channel)
            {
                case 0:
                    isolatedChannel = sourceImage.Clone(r => r.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(row[x].X, row[x].X, row[x].X, 1);
                        }
                    }));
                    break;
                case 1:
                    isolatedChannel = sourceImage.Clone(g => g.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(row[x].Y, row[x].Y, row[x].Y, 1);
                        }
                    }));
                    break;
                case 2:
                    isolatedChannel = sourceImage.Clone(b => b.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(row[x].Z, row[x].Z, row[x].Z, 1);
                        }
                    }));
                    break;
                case 3:
                    isolatedChannel = sourceImage.Clone(a => a.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(row[x].W, row[x].W, row[x].W, 1);
                        }
                    }));
                    break;
            }

            return isolatedChannel;
        }

        public void RecombineChannels()
        {
            //Bitmap outputImage = new Bitmap(pictureBoxR.Image.Width, pictureBoxR.Image.Height);
            //Bitmap bitmapR = (Bitmap)pictureBoxR.Image;
            //Bitmap bitmapG = (Bitmap)pictureBoxG.Image;
            //Bitmap bitmapB = (Bitmap)pictureBoxB.Image;
            //Bitmap bitmapA = (Bitmap)pictureBoxA.Image;
            //for (int i=0; i<outputImage.Width; i++)
            //{
            //    for (int j=0; j<outputImage.Height; j++)
            //    {
            //        outputImage.SetPixel(i, j, Color.FromArgb(bitmapA.GetPixel(i, j).R, bitmapR.GetPixel(i,j).R, bitmapG.GetPixel(i, j).R, bitmapB.GetPixel(i, j).R));
            //    }
            //}
            //pictureBoxOutput.Image = outputImage;
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