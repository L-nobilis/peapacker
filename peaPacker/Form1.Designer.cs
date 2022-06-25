namespace peaPacker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainerR1 = new System.Windows.Forms.SplitContainer();
            this.pictureBoxR = new System.Windows.Forms.PictureBox();
            this.splitContainerR2 = new System.Windows.Forms.SplitContainer();
            this.invertButtonR = new System.Windows.Forms.Button();
            this.fillButtonR = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openImage = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerR1)).BeginInit();
            this.splitContainerR1.Panel1.SuspendLayout();
            this.splitContainerR1.Panel2.SuspendLayout();
            this.splitContainerR1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerR2)).BeginInit();
            this.splitContainerR2.Panel1.SuspendLayout();
            this.splitContainerR2.Panel2.SuspendLayout();
            this.splitContainerR2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainerR1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.openImage, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(910, 458);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // splitContainerR1
            // 
            this.splitContainerR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerR1.Location = new System.Drawing.Point(3, 48);
            this.splitContainerR1.Name = "splitContainerR1";
            this.splitContainerR1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerR1.Panel1
            // 
            this.splitContainerR1.Panel1.Controls.Add(this.pictureBoxR);
            // 
            // splitContainerR1.Panel2
            // 
            this.splitContainerR1.Panel2.Controls.Add(this.splitContainerR2);
            this.splitContainerR1.Size = new System.Drawing.Size(221, 200);
            this.splitContainerR1.SplitterDistance = 147;
            this.splitContainerR1.TabIndex = 0;
            // 
            // pictureBoxR
            // 
            this.pictureBoxR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.pictureBoxR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxR.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxR.Name = "pictureBoxR";
            this.pictureBoxR.Size = new System.Drawing.Size(221, 147);
            this.pictureBoxR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxR.TabIndex = 0;
            this.pictureBoxR.TabStop = false;
            this.pictureBoxR.Click += new System.EventHandler(this.pictureBoxR_Click);
            // 
            // splitContainerR2
            // 
            this.splitContainerR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerR2.Location = new System.Drawing.Point(0, 0);
            this.splitContainerR2.Name = "splitContainerR2";
            // 
            // splitContainerR2.Panel1
            // 
            this.splitContainerR2.Panel1.Controls.Add(this.invertButtonR);
            // 
            // splitContainerR2.Panel2
            // 
            this.splitContainerR2.Panel2.Controls.Add(this.fillButtonR);
            this.splitContainerR2.Size = new System.Drawing.Size(221, 49);
            this.splitContainerR2.SplitterDistance = 106;
            this.splitContainerR2.TabIndex = 0;
            // 
            // invertButtonR
            // 
            this.invertButtonR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.invertButtonR.Location = new System.Drawing.Point(0, 0);
            this.invertButtonR.Name = "invertButtonR";
            this.invertButtonR.Size = new System.Drawing.Size(106, 49);
            this.invertButtonR.TabIndex = 0;
            this.invertButtonR.Text = "Invert";
            this.invertButtonR.UseVisualStyleBackColor = true;
            // 
            // fillButtonR
            // 
            this.fillButtonR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fillButtonR.Location = new System.Drawing.Point(0, 0);
            this.fillButtonR.Name = "fillButtonR";
            this.fillButtonR.Size = new System.Drawing.Size(111, 49);
            this.fillButtonR.TabIndex = 1;
            this.fillButtonR.Text = "Fill";
            this.fillButtonR.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // openImage
            // 
            this.openImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openImage.Location = new System.Drawing.Point(230, 3);
            this.openImage.Name = "openImage";
            this.openImage.Size = new System.Drawing.Size(221, 39);
            this.openImage.TabIndex = 1;
            this.openImage.Text = "Open image...";
            this.openImage.UseVisualStyleBackColor = true;
            this.openImage.Click += new System.EventHandler(this.openImage_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 458);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainerR1.Panel1.ResumeLayout(false);
            this.splitContainerR1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerR1)).EndInit();
            this.splitContainerR1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxR)).EndInit();
            this.splitContainerR2.Panel1.ResumeLayout(false);
            this.splitContainerR2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerR2)).EndInit();
            this.splitContainerR2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private SplitContainer splitContainerR1;
        private PictureBox pictureBoxR;
        private OpenFileDialog openFileDialog1;
        private SplitContainer splitContainerR2;
        private Button invertButtonR;
        private Button fillButtonR;
        private Button openImage;
    }
}