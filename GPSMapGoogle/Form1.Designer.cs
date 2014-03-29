namespace GPSMapFile
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
            this.ExportToArray = new System.Windows.Forms.Button();
            this.tBCol = new System.Windows.Forms.TextBox();
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
            // tBCol
            // 
            this.tBCol.Location = new System.Drawing.Point(56, 153);
            this.tBCol.Multiline = true;
            this.tBCol.Name = "tBCol";
            this.tBCol.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tBCol.Size = new System.Drawing.Size(358, 120);
            this.tBCol.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(879, 374);
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(871, 340);
            this.Controls.Add(this.tBCol);
            this.Controls.Add(this.ExportToArray);
            this.Controls.Add(this.btnReadGPS);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReadGPS;
        private System.Windows.Forms.Button ExportToArray;
        private System.Windows.Forms.TextBox tBCol;
    }
}

