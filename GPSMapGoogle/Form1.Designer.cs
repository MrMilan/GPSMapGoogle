﻿namespace GPSMapFile
{
    partial class Form1
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
            this.btnReadGPS = new System.Windows.Forms.Button();
            this.Export = new System.Windows.Forms.Button();
            this.ExportToArray = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnReadGPS
            // 
            this.btnReadGPS.Location = new System.Drawing.Point(22, 39);
            this.btnReadGPS.Name = "btnReadGPS";
            this.btnReadGPS.Size = new System.Drawing.Size(174, 65);
            this.btnReadGPS.TabIndex = 1;
            this.btnReadGPS.Text = "Nacti gps";
            this.btnReadGPS.UseVisualStyleBackColor = true;
            this.btnReadGPS.Click += new System.EventHandler(this.btnReadGPS_Click);
            // 
            // Export
            // 
            this.Export.Location = new System.Drawing.Point(233, 39);
            this.Export.Name = "Export";
            this.Export.Size = new System.Drawing.Size(166, 65);
            this.Export.TabIndex = 2;
            this.Export.Text = "Export";
            this.Export.UseVisualStyleBackColor = true;
            this.Export.Click += new System.EventHandler(this.Export_Click);
            // 
            // ExportToArray
            // 
            this.ExportToArray.Location = new System.Drawing.Point(424, 39);
            this.ExportToArray.Name = "ExportToArray";
            this.ExportToArray.Size = new System.Drawing.Size(166, 65);
            this.ExportToArray.TabIndex = 2;
            this.ExportToArray.Text = "Export Array";
            this.ExportToArray.UseVisualStyleBackColor = true;
            this.ExportToArray.Click += new System.EventHandler(this.ExportToArray_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(1024, 1024);
            this.ClientSize = new System.Drawing.Size(885, 352);
            this.Controls.Add(this.ExportToArray);
            this.Controls.Add(this.Export);
            this.Controls.Add(this.btnReadGPS);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnReadGPS;
        private System.Windows.Forms.Button Export;
        private System.Windows.Forms.Button ExportToArray;
    }
}

