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
        // Tracks if a user has opened an image yet or not.
        private bool imageOpened = false;

        public int currentWidth;
        public int currentHeight;

        public Image redChannel;
        public Image blueChannel;
        public Image greenChannel;
        public Image alphaChannel;

        public Form1()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            DisableButtons();
            openImage.AllowDrop = true;
            SetAllTooltips();
        }



        // ====================================================  Load and Recombine Channel(s) ====================================================  

        /// <summary>
        /// The main function used when loading in a whole image.  
        /// </summary>
        /// <param name="fileName"></param>
        public Image LoadRGBAImage(string fileName)
        {
            Image newImage = Image.Load(fileName);
            pathLabel.Text = $"Loaded image: {fileName}";
            return newImage;
        }

        /// <summary>
        /// Sets the passed in image to be our current working image and splits its channels. 
        /// </summary>
        /// <param name="image"></param>
        public void SetRGBAImage(Image image)
        {
            currentWidth = image.Width;
            currentHeight = image.Height;

            //Grab references to all four picture boxes used to display our channels
            PictureBox[] channelBoxes = new PictureBox[4];
            channelBoxes[0] = pictureBoxR;
            channelBoxes[1] = pictureBoxG;
            channelBoxes[2] = pictureBoxB;
            channelBoxes[3] = pictureBoxA;

            int i = 0;
            foreach (PictureBox box in channelBoxes)
            {
                var stream = new System.IO.MemoryStream();
                SplitOneChannel(image, i).SaveAsBmp(stream);
                System.Drawing.Image channelImg = System.Drawing.Image.FromStream(stream);

                box.Image?.Dispose();
                box.Image = channelImg;

                i++;
            }

            RecombineChannels();
            outputSizeLabel.Text = $"Output size: {currentWidth} x {currentHeight}";
            EnableButtons();
            imageOpened = true;
            pictureBoxA.AllowDrop = true;
            pictureBoxR.AllowDrop = true;
            pictureBoxG.AllowDrop = true;
            pictureBoxB.AllowDrop = true;
        }

        ///<summary>
        ///Called when individual channel boxes are clicked or dragged into.
        ///</summary>
        public void LoadIndividualChannel(int channel)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image newImage = Image.Load(openFileDialog1.FileName);
                if (imageOpened && (newImage.Width != currentWidth || newImage.Height != currentHeight))
                {
                    MessageBox.Show($"Channel size must match original image size: {currentWidth} x {currentHeight}");
                }
                else
                {
                    SetIndividualChannel(newImage, channel);
                }
            }
        }

        /// <summary>
        /// Replaces the appropriate picture box's image with the passed in image.  Calls RecombineChannels when finished.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="channel">0: R, 1: G, 2: B, 3: A</param>
        public void SetIndividualChannel(Image image, int channel)
        {
            switch (channel)
            {
                case 0:
                    pictureBoxR.Image = ToBitmap(SplitOneChannel(image, channel));
                    break;
                case 1:
                    pictureBoxG.Image = ToBitmap(SplitOneChannel(image, channel));
                    break;
                case 2:
                    pictureBoxB.Image = ToBitmap(SplitOneChannel(image, channel));
                    break;
                case 3:
                    pictureBoxA.Image = ToBitmap(SplitOneChannel(image, channel));
                    break;
            }
            RecombineChannels();
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
            Bitmap bitmapRed = ToBitmap(redChannel);
            Bitmap bitmapGreen = ToBitmap(greenChannel);
            Bitmap bitmapBlue = ToBitmap(blueChannel);
            Bitmap bitmapAlpha = ToBitmap(alphaChannel);

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

        // =======================================================  Channel Manipulation =======================================================  

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

        // =======================================================  Button Events =======================================================  

        /// <summary>
        /// Disables fill/invert buttons and save-as buttons. 
        /// </summary>
        private void DisableButtons()
        {
            pictureBoxA.Enabled = false;
            fillButtonA.Enabled = false;
            invertButtonA.Enabled = false;

            pictureBoxR.Enabled = false;
            fillButtonR.Enabled = false;
            invertButtonR.Enabled = false;

            pictureBoxG.Enabled = false;
            fillButtonG.Enabled = false;
            invertButtonG.Enabled = false;

            pictureBoxB.Enabled = false;
            fillButtonB.Enabled = false;
            invertButtonB.Enabled = false;

            pictureBoxOutput.Enabled = false;
            saveAsButton.Enabled = false;
        }

        /// <summary>
        /// Enables invert buttons and save-as buttons. 
        /// </summary>
        private void EnableButtons()
        {
            pictureBoxA.Enabled = true;
            fillButtonA.Enabled = true;
            invertButtonA.Enabled = true;

            pictureBoxR.Enabled = true;
            fillButtonR.Enabled = true;
            invertButtonR.Enabled = true;

            pictureBoxG.Enabled = true;
            fillButtonG.Enabled = true;
            invertButtonG.Enabled = true;

            pictureBoxB.Enabled = true;
            fillButtonB.Enabled = true;
            invertButtonB.Enabled = true;

            pictureBoxOutput.Enabled = true;
            saveAsButton.Enabled = true;
        }
        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void openImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SetRGBAImage(LoadRGBAImage(openFileDialog1.FileName));
            }

        }

        /// <summary>
        /// Event for drag/dropping over Open Image button or channel boxes
        /// </summary>
        private void openImage_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
        /// <summary>
        /// Event for dropping image into Open Image button
        /// </summary>
        private void openImage_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var fileNames = data as string[];
                if (fileNames.Length > 0)
                {
                    SetRGBAImage(LoadRGBAImage(fileNames[0]));
                }
            }
        }

        public void pictureBoxR_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var fileNames = data as string[];
                if (fileNames.Length > 0)
                {
                    SetIndividualChannel(SplitOneChannel(LoadRGBAImage(fileNames[0]), 0), 0);
                }
            }
        }
        public void pictureBoxG_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var fileNames = data as string[];
                if (fileNames.Length > 0)
                {
                    SetIndividualChannel(SplitOneChannel(LoadRGBAImage(fileNames[0]), 1), 1);
                }
            }
        }

        public void pictureBoxB_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var fileNames = data as string[];
                if (fileNames.Length > 0)
                {
                    SetIndividualChannel(SplitOneChannel(LoadRGBAImage(fileNames[0]), 2), 2);
                }
            }
        }
        public void pictureBoxA_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var fileNames = data as string[];
                if (fileNames.Length > 0)
                {
                    SetIndividualChannel(SplitOneChannel(LoadRGBAImage(fileNames[0]), 3), 3);
                }
            }
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

        // =================================================== Setup Stuff ============================================================

        /// <summary>
        /// Gives tooltips to all our controls.
        /// </summary>
        public void SetAllTooltips()
        {
            Dictionary<Control, string> tooltips = new Dictionary<Control, string>();

            //tooltips.Add(openImage, "Open an image.");
            tooltips.Add(invertButtonA, "Invert alpha channel");
            tooltips.Add(invertButtonB, "Invert blue channel");
            tooltips.Add(invertButtonR, "Invert red channel");
            tooltips.Add(invertButtonG, "Invert green channel");

            tooltips.Add(fillButtonA, "Fill alpha channel with black pixels");
            tooltips.Add(fillButtonR, "Fill red channel with black pixels");
            tooltips.Add(fillButtonG, "Fill green channel with black pixels");
            tooltips.Add(fillButtonB, "Fill blue channel with black pixels");

            tooltips.Add(pictureBoxA, "Load individual alpha channel");
            tooltips.Add(pictureBoxR, "Load individual red channel");
            tooltips.Add(pictureBoxG, "Load individual green channel");
            tooltips.Add(pictureBoxB, "Load individual blue channel");
            //tooltips.Add(saveAsButton, "Save new image.");

            foreach (KeyValuePair<Control, string> entry in tooltips)
            {
                ToolTip newToolTip = new ToolTip();
                newToolTip.SetToolTip(entry.Key, entry.Value);
            }

        }

        // ===================================================  Image Type Conversion Helpers ===================================================  
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

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}