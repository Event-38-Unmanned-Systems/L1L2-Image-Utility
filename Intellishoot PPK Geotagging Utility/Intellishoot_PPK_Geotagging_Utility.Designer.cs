namespace Intellishoot_PPK_Geotagging_Utility
{
    partial class Intellishoot_PPK_Geotagging_Utility
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Intellishoot_PPK_Geotagging_Utility));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.LogFilePath = new System.Windows.Forms.Label();
            this.ImagePath = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.camera_height_offset = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.button1.Location = new System.Drawing.Point(523, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Event File";
            this.toolTip1.SetToolTip(this.button1, "Select the events.pos file obtained from RTKLIB. This will be located inside the " +
        "Rover directory.");
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.button2.Location = new System.Drawing.Point(523, 36);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Image Path";
            this.toolTip1.SetToolTip(this.button2, "Select the directory containing images from flight.");
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // LogFilePath
            // 
            this.LogFilePath.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.LogFilePath.Location = new System.Drawing.Point(91, 9);
            this.LogFilePath.Name = "LogFilePath";
            this.LogFilePath.Size = new System.Drawing.Size(164, 13);
            this.LogFilePath.TabIndex = 2;
            this.LogFilePath.Text = "label1";
            this.LogFilePath.Visible = false;
            // 
            // ImagePath
            // 
            this.ImagePath.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ImagePath.Location = new System.Drawing.Point(91, 36);
            this.ImagePath.Name = "ImagePath";
            this.ImagePath.Size = new System.Drawing.Size(164, 13);
            this.ImagePath.TabIndex = 3;
            this.ImagePath.Text = "label2";
            this.ImagePath.Visible = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.button3.Location = new System.Drawing.Point(514, 112);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Process";
            this.toolTip1.SetToolTip(this.button3, "Begin Processing");
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // camera_height_offset
            // 
            this.camera_height_offset.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.camera_height_offset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.camera_height_offset.Location = new System.Drawing.Point(460, 88);
            this.camera_height_offset.Name = "camera_height_offset";
            this.camera_height_offset.Size = new System.Drawing.Size(138, 20);
            this.camera_height_offset.TabIndex = 6;
            this.camera_height_offset.Text = "Vertical Antenna Offset M";
            this.toolTip1.SetToolTip(this.camera_height_offset, "Vertical Antenna Offset in M between the antenna and the imager of the camera.");
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Select/Add an Antenna"});
            this.comboBox1.Location = new System.Drawing.Point(460, 61);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(138, 21);
            this.comboBox1.TabIndex = 7;
            this.toolTip1.SetToolTip(this.comboBox1, "Add or Select an antenna profile. Add a profile by entering a name and an offset " +
        "then clicking process.");
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            this.comboBox1.Click += new System.EventHandler(this.comboBox1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label2.Location = new System.Drawing.Point(506, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "?";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label2, "Select the events.pos file obtained from RTKLIB. This will be located inside the " +
        "Rover directory.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label1.Location = new System.Drawing.Point(506, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label1, "Select the directory containing images from flight.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label3.Location = new System.Drawing.Point(445, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "?";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.label3, "Add or Select an antenna profile. Add a profile by entering a name and an offset " +
        "then clicking process.");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.label4.Location = new System.Drawing.Point(445, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "?";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.label4, "Vertical Antenna Offset in M between the antenna and the imager of the camera.");
            // 
            // Intellishoot_PPK_Geotagging_Utility
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(601, 261);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.camera_height_offset);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.ImagePath);
            this.Controls.Add(this.LogFilePath);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Intellishoot_PPK_Geotagging_Utility";
            this.Text = "Intellishoot PPK Geotagging Utility";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label LogFilePath;
        private System.Windows.Forms.Label ImagePath;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox camera_height_offset;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

