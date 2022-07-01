namespace peaPacker
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Advanced;
    using SixLabors.ImageSharp.Formats.Png;
    using SixLabors.ImageSharp.PixelFormats;
    using System.Diagnostics;

    public partial class Form1 : Form
    {
        public int currentWidth;
        public int currentHeight;

        public Image redChannel;
        public Image blueChannel;
        public Image greenChannel;
        public Image alphaChannel;

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
                Image newImage = Image.Load(openFileDialog1.FileName);
                currentWidth = newImage.Width;
                currentHeight = newImage.Height;

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

                RecombineChannels();

                outputSizeLabel.Text = $"Output size: {currentWidth} x {currentHeight}";
            }

        }

        ///<summary>
        ///Called when individual channel boxes are clicked.
        ///</summary>
        public void LoadIndividualChannel(int channel)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image newImage = Image.Load(openFileDialog1.FileName);
                if (newImage.Width != currentWidth || newImage.Height != currentHeight)
                {
                    MessageBox.Show($"Channel size must match original image size: {currentWidth} x {currentHeight}");
                }
                else
                {
                    switch (channel)
                    {
                        case 0:
                            pictureBoxR.Image = ToBitmap(SplitOneChannel(newImage, channel));
                            break;
                        case 1:
                            pictureBoxG.Image = ToBitmap(SplitOneChannel(newImage, channel));
                            break;
                        case 2:
                            pictureBoxB.Image = ToBitmap(SplitOneChannel(newImage, channel));
                            break;
                        case 3:
                            pictureBoxA.Image = ToBitmap(SplitOneChannel(newImage, channel));
                            break;
                    }
                    RecombineChannels();
                }
            }
        }

        ///<summary>
        ///Called to split image into channels, once for each channel.
        ///</summary>
        public Image SplitOneChannel(Image sourceImage, int channel)
        {
            Image isolatedChannel = new Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(sourceImage.Width, sourceImage.Height);

            switch (channel)
            {
                case 0:
                    isolatedChannel = sourceImage.Clone(r => r.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(row[x].X, row[x].X, row[x].X, 1);
                        }
                    }));
                    redChannel = isolatedChannel;
                    break;
                case 1:
                    isolatedChannel = sourceImage.Clone(g => g.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(row[x].Y, row[x].Y, row[x].Y, 1);
                        }
                    }));
                    greenChannel = isolatedChannel;
                    break;
                case 2:
                    isolatedChannel = sourceImage.Clone(b => b.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(row[x].Z, row[x].Z, row[x].Z, 1);
                        }
                    }));
                    blueChannel = isolatedChannel;
                    break;
                case 3:
                    isolatedChannel = sourceImage.Clone(a => a.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(row[x].W, row[x].W, row[x].W, 1);
                        }
                    }));
                    alphaChannel = isolatedChannel;
                    break;
            }
            return isolatedChannel;
        }

        ///<summary>
        ///Recombines channels and updates output preview.  Currently very sloooooow.
        ///</summary>
        public void RecombineChannels()
        {
            Image<Rgba32> recombinedImage = new Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(currentWidth, currentHeight);

            Bitmap bitmapOutput = new Bitmap(currentWidth, currentHeight);
            Bitmap bitmapRed = (Bitmap)pictureBoxR.Image;
            Bitmap bitmapGreen = (Bitmap)pictureBoxG.Image;
            Bitmap bitmapBlue = (Bitmap)pictureBoxB.Image;
            Bitmap bitmapAlpha = (Bitmap)pictureBoxA.Image;

            //i is always 0 here.  How can we access the index of a row?
            //redChannel.Clone(x => x.ProcessPixelRowsAsVector4(row => {
            //    int i = 0;
            //    Debug.WriteLine($"Processing row {i}");
            //    for (int x = 0; x < row.Length; x++)
            //    {
            //        redValues[i + x] = row[x].X;
            //    }
            //    i++;
            //}));


            // Recombining the output the lazy way, because I haven't yet determined how to get a row's index in the above code
            // This solution is really slow and will be improved in future builds
            for (int i = 0; i < currentWidth; i++)
            {
                for (int j = 0; j < currentHeight; j++)
                {
                    bitmapOutput.SetPixel(i, j, System.Drawing.Color.FromArgb(bitmapAlpha.GetPixel(i,j).R, bitmapRed.GetPixel(i, j).R, bitmapGreen.GetPixel(i, j).R, bitmapBlue.GetPixel(i, j).R));
                }
            } 

            //recombinedImage = recombinedImage.Clone(r => r.ProcessPixelRowsAsVector4(row => {
            //    int i = 0;
            //    for (int x = 0; x < row.Length; x++)
            //    {
            //        row[x] = new System.Numerics.Vector4(redValues[i + x], 0, 0, 0);
            //    }
            //    i++;
            //}));

            //var stream = new System.IO.MemoryStream();
            //recombinedImage.SaveAsBmp(stream);
            //System.Drawing.Image outputImg = System.Drawing.Image.FromStream(stream);

            pictureBoxOutput.Image?.Dispose();
            pictureBoxOutput.Image = bitmapOutput;
        }

        private void splitContainerG_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
        private void pictureBoxR_Click(object sender, EventArgs e)
        {
            LoadIndividualChannel(0);
        }

        private void pictureBoxG_Click(object sender, EventArgs e)
        {
            LoadIndividualChannel(1);
        }

        private void pictureBoxB_Click(object sender, EventArgs e)
        {
            LoadIndividualChannel(2);
        }

        private void pictureBoxA_Click(object sender, EventArgs e)
        {
            LoadIndividualChannel(3);
        }
        private void pictureBoxOutput_Click(object sender, EventArgs e)
        {
            saveAsButton_Click(sender, e);
        }
        private void invertButtonR_Click(object sender, EventArgs e)
        {
            InvertChannel(0);
        }
        private void invertButtonG_Click(object sender, EventArgs e)
        {
            InvertChannel(1);
        }

        private void invertButtonB_Click(object sender, EventArgs e)
        {
            InvertChannel(2);
        }

        private void invertButtonA_Click(object sender, EventArgs e)
        {
            InvertChannel(3);
        }

        private void fillButtonR_Click(object sender, EventArgs e)
        {
            FillChannel(0);
        }

        private void fillButtonG_Click(object sender, EventArgs e)
        {
            FillChannel(1);
        }

        private void fillButtonB_Click(object sender, EventArgs e)
        {
            FillChannel(2);
        }

        private void fillButtonA_Click(object sender, EventArgs e)
        {
            FillChannel(3);
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

        ///<summary>
        ///Inverts the indicated channel, 0 = red, 1= green, 2 = blue, 3 = alpha
        ///</summary>
        private void InvertChannel(int channel)
        {
            switch (channel)
            {
                case 0:
                    redChannel.Mutate(x => x.Invert());
                    pictureBoxR.Image = ToBitmap(redChannel);
                    break;
                case 1:
                    greenChannel.Mutate(x => x.Invert());
                    pictureBoxG.Image = ToBitmap(greenChannel);
                    break;
                case 2:
                    blueChannel.Mutate(x => x.Invert());
                    pictureBoxB.Image = ToBitmap(blueChannel);
                    break;
                case 3:
                    alphaChannel.Mutate(x => x.Invert());
                    pictureBoxA.Image = ToBitmap(alphaChannel);
                    break;
            }
            RecombineChannels();
        }

        private void FillChannel(int channel)
        {
            switch (channel)
            {
                case 0:
                    redChannel.Mutate(r => r.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(0, 0, 0, 1);
                        }
                    }));
                    pictureBoxR.Image = ToBitmap(redChannel);
                    break;
                case 1:
                    greenChannel.Mutate(r => r.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(0, 0, 0, 1);
                        }
                    }));
                    pictureBoxG.Image = ToBitmap(greenChannel);
                    break;
                case 2:
                    blueChannel.Mutate(r => r.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(0, 0, 0, 1);
                        }
                    }));
                    pictureBoxB.Image = ToBitmap(blueChannel);
                    break;
                case 3:
                    alphaChannel.Mutate(r => r.ProcessPixelRowsAsVector4(row => {
                        for (int x = 0; x < row.Length; x++)
                        {
                            row[x] = new System.Numerics.Vector4(0, 0, 0, 1);
                        }
                    }));
                    pictureBoxA.Image = ToBitmap(alphaChannel);
                    break;
            }
            RecombineChannels();
        }


        ///<summary>
        ///Helper function to convert bitmaps to ImageSharp images.
        ///</summary>
        public static Image ToImageSharpImage(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return Image.Load(memoryStream);
            }
        }

        ///<summary>
        ///Helper function to convert ImageSharp images to bitmaps.
        ///</summary>
        public static Bitmap ToBitmap(Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
                image.Save(memoryStream, imageEncoder);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new Bitmap(memoryStream);
            }
        }

    }

}