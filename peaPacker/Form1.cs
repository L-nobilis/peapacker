namespace peaPacker
{
    using ImageMagick;
    using System.Diagnostics;

    public partial class Form1 : Form
    {
        private bool imageOpened = false;
        public MagickImage currentImage;

        public Form1()
        {
            InitializeComponent();
            Start();
        }
        private void Start()
        {
            MagickNET.Initialize();
            DisableButtons();
            openImage.AllowDrop = true;
            SetAllTooltips();
        }
        // ====================================================  Load and Recombine Channel(s) ====================================================  

        /// <summary>
        /// Opens an image from the disk and returns a MagickImage. Also updates the pathLabel text.
        /// </summary>
        /// <param name="fileName"></param>
        public MagickImage LoadRGBAImage(string fileName)
        {
            var newImage = new MagickImage(fileName);
            pathLabel.Text = $"Loaded image: \n{fileName}";
            return newImage;
        }

        /// <summary>
        /// Sets the passed in image to be our current working image and displays the split channels.
        /// </summary>
        /// <param name="image"></param>
        public void SetRGBAImage(MagickImage loadedImage)
        {
            if (!loadedImage.HasAlpha)
            {
                loadedImage.Alpha(AlphaOption.On);
            }
            currentImage = loadedImage;

            //Since we just loaded in an image and haven't modified it yet, our output is identical to what we loaded.
            outputSizeLabel.Text = $"Output size: {loadedImage.Width} x {loadedImage.Height}";

            //Split our channels into seperate images, so the user can see them:
            DisplaySplitChannels();

            EnableButtons();
            imageOpened = true;
            pictureBoxA.AllowDrop = true;
            pictureBoxR.AllowDrop = true;
            pictureBoxG.AllowDrop = true;
            pictureBoxB.AllowDrop = true;
        }

        /// <summary>
        /// Grabs the current working image and displays its seperated RGBA channels. 
        /// </summary>
        public void DisplaySplitChannels()
        {
            using (MagickImageCollection channels = new MagickImageCollection())
            {
                channels.AddRange(currentImage.Separate(Channels.All));
                //MessageBox.Show($"Image has {currentImage.Channels.Count()} channels. Colorspace: {currentImage.ColorSpace.ToString()}");

                pictureBoxOutput.Image?.Dispose();
                pictureBoxOutput.Image = currentImage.ToBitmap();

                pictureBoxR.Image?.Dispose();
                pictureBoxG.Image?.Dispose();
                pictureBoxB.Image?.Dispose();
                pictureBoxA.Image?.Dispose();

                if (currentImage.ColorSpace == ColorSpace.sRGB)
                {
                    //Display each channel:
                    pictureBoxR.Image = channels[0].ToBitmap();
                    pictureBoxG.Image = channels[1].ToBitmap();
                    pictureBoxB.Image = channels[2].ToBitmap();
                    if (channels.Count() > 3)
                    {
                        pictureBoxA.Image = channels[3].ToBitmap();
                    }
                    else
                    {
                        pictureBoxA.Image = new MagickImage(new MagickColor("#FFFFFF"), currentImage.Width, currentImage.Height).ToBitmap();
                    }

                }

                else if(currentImage.ColorSpace == ColorSpace.Gray){
                    pictureBoxR.Image = channels[0].ToBitmap();
                    pictureBoxG.Image = channels[0].ToBitmap();
                    pictureBoxB.Image = channels[0].ToBitmap();
                    pictureBoxA.Image = channels[1].ToBitmap();
                }
            }
        }

        ///<summary>
        ///Called when individual channel boxes are clicked or dragged into. Calls SetIndividualChannel
        ///</summary>
        public void LoadIndividualChannel(int channel)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (var channelImage = new MagickImage(openFileDialog1.FileName))
                {
                    if (imageOpened && (channelImage.Width != currentImage.Width || channelImage.Height != currentImage.Height))
                    {
                        MessageBox.Show($"Channel size must match original image size: {currentImage.Width} x {currentImage.Height}");
                    }
                    else
                    {
                        SetIndividualChannel(channelImage, channel);
                    }
                }
            }
        }

        /// <summary>
        /// Replaces the appropriate picture box's image with the passed in image.  Calls DisplaySplitChannels when finished.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="channel">0: R, 1: G, 2: B, 3: A</param>
        public void SetIndividualChannel(MagickImage image, int channel)
        {
            //Puts all our current channels in a collection so we can modify them
            MagickImageCollection currentChannels = new MagickImageCollection();
            currentChannels.AddRange(currentImage.Separate(Channels.All));

            //Does the same for our passed in image.
            MagickImageCollection newChannels = new MagickImageCollection();
            newChannels.AddRange(image.Separate(Channels.All));


            if (image.ColorSpace == ColorSpace.sRGB)
            {
                //Replace our working image's channel with the one passed in
                currentChannels[channel] = newChannels[channel];
            }
            else if (image.ColorSpace == ColorSpace.Gray)
            {
                currentChannels[channel] = newChannels[0];
            }

            //Set our working image to be a recombination of the channels
            currentImage = (MagickImage)currentChannels.Combine();

            DisplaySplitChannels();
        }

        /// <summary>
        /// Creates a new image based on the passed in values. An alternative to opening an image.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void CreateNewImage(int width, int height, Color bgColor)
        {
            //MagickColor wants the color in string hex format, so we use ColorTranslator here to do that
            MagickImage newImage = new MagickImage(new MagickColor(ColorTranslator.ToHtml(bgColor)), width, height);
            SetRGBAImage(newImage);
            pathLabel.Text = $" ";

        }

        // =======================================================  Channel Manipulation =======================================================  
        ///<summary>
        ///Inverts the indicated channel, 0 = red, 1= green, 2 = blue, 3 = alpha
        ///</summary>
        private void InvertChannel(int channel)
        {
            using (MagickImageCollection currentChannels = new MagickImageCollection())
            {
                currentChannels.AddRange(currentImage.Separate(Channels.RGB));
                currentChannels.AddRange(currentImage.Separate(Channels.Alpha));

                using (MagickImage thisChannel = (MagickImage)currentChannels[channel])
                {
                    thisChannel.Negate();
                    currentChannels[channel] = thisChannel;
                    currentImage = (MagickImage)currentChannels.Combine();
                }
            }
            DisplaySplitChannels();
        }

        ///<summary>
        ///Fills the indicated channel with solid white. 0 = red, 1= green, 2 = blue, 3 = alpha
        ///</summary>
        private void FillChannel(int channel)
        {
            MagickImageCollection currentChannels = new MagickImageCollection();
            currentChannels.AddRange(currentImage.Separate(Channels.RGB));
            currentChannels.AddRange(currentImage.Separate(Channels.Alpha));

            MagickImage thisChannel = new MagickImage(new MagickColor("#FFFFFF"), currentImage.Width, currentImage.Height);

            currentChannels[channel] = thisChannel;
            currentImage = (MagickImage)currentChannels.Combine();
            DisplaySplitChannels();
        }

        // =======================================================  Button and Drag/Drop Events =======================================================  

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
                    SetIndividualChannel(LoadRGBAImage(fileNames[0]), 0);
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
                    SetIndividualChannel(LoadRGBAImage(fileNames[0]), 1);
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
                    SetIndividualChannel(LoadRGBAImage(fileNames[0]), 2);
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
                    SetIndividualChannel(LoadRGBAImage(fileNames[0]), 3);
                }
            }
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

        private void aboutButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PeaPacker version 1.0\nhttps://github.com/L-nobilis/peapacker", "About PeaPacker");
        }
        private void helpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("How to use:\n1. Open an image by clicking the Open button or by dragging an image onto the open button.\n" +
                "2. Click each channel to replace it with a new image, or modify them with the controls below.\n" +
                "3. Save your new packed image.", "PeaPacker Help");
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

            tooltips.Add(newImageButton, "Create a new image");
            //tooltips.Add(saveAsButton, "Save new image.");

            foreach (KeyValuePair<Control, string> entry in tooltips)
            {
                ToolTip newToolTip = new ToolTip();
                newToolTip.SetToolTip(entry.Key, entry.Value);
            }

        }

        private void newImageButton_Click(object sender, EventArgs e)
        {
            NewImage newImgWindow = new NewImage();
            newImgWindow.ShowDialog();

        }


    }
}