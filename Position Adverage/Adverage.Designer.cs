namespace Position_Adverage
{
    partial class Adverage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Adverage));
            this.Observation = new System.Windows.Forms.Button();
            this.Process = new System.Windows.Forms.Button();
            this.FilePath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Observation
            // 
            this.Observation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Observation.Location = new System.Drawing.Point(516, 16);
            this.Observation.Name = "Observation";
            this.Observation.Size = new System.Drawing.Size(85, 23);
            this.Observation.TabIndex = 0;
            this.Observation.Text = "Obs File";
            this.Observation.UseVisualStyleBackColor = true;
            this.Observation.Click += new System.EventHandler(this.button1_Click);
            // 
            // Process
            // 
            this.Process.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Process.Location = new System.Drawing.Point(516, 45);
            this.Process.Name = "Process";
            this.Process.Size = new System.Drawing.Size(85, 23);
            this.Process.TabIndex = 1;
            this.Process.Text = "Process";
            this.Process.UseVisualStyleBackColor = true;
            this.Process.Visible = false;
            this.Process.Click += new System.EventHandler(this.Process_Click);
            // 
            // FilePath
            // 
            this.FilePath.AutoSize = true;
            this.FilePath.Dock = System.Windows.Forms.DockStyle.Right;
            this.FilePath.Location = new System.Drawing.Point(556, 0);
            this.FilePath.Name = "FilePath";
            this.FilePath.Size = new System.Drawing.Size(45, 13);
            this.FilePath.TabIndex = 2;
            this.FilePath.Text = "FilePath";
            this.FilePath.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.FilePath.Visible = false;
            this.FilePath.Click += new System.EventHandler(this.label1_Click);
            // 
            // Adverage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(601, 261);
            this.Controls.Add(this.Process);
            this.Controls.Add(this.Observation);
            this.Controls.Add(this.FilePath);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Adverage";
            this.Text = "Adverage Points";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Observation;
        private System.Windows.Forms.Button Process;
        private System.Windows.Forms.Label FilePath;
    }
}

