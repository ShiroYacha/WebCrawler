namespace WebCrawler
{
    partial class MainForm
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
            this.startButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.scanPagesSlider = new System.Windows.Forms.NumericUpDown();
            this.exportButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ctripCounterLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.qunarCounterLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.matchedCountLabel = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ctripBrowser = new System.Windows.Forms.WebBrowser();
            this.qunarBrowser = new System.Windows.Forms.WebBrowser();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scanPagesSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(190, 5);
            this.startButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(112, 35);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.scanPagesSlider);
            this.flowLayoutPanel1.Controls.Add(this.startButton);
            this.flowLayoutPanel1.Controls.Add(this.exportButton);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.ctripCounterLabel);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.qunarCounterLabel);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.matchedCountLabel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(944, 46);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 12);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Scan pages";
            // 
            // scanPagesSlider
            // 
            this.scanPagesSlider.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.scanPagesSlider.Location = new System.Drawing.Point(106, 9);
            this.scanPagesSlider.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.scanPagesSlider.Name = "scanPagesSlider";
            this.scanPagesSlider.Size = new System.Drawing.Size(76, 26);
            this.scanPagesSlider.TabIndex = 8;
            this.scanPagesSlider.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(310, 5);
            this.exportButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(112, 35);
            this.exportButton.TabIndex = 3;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(430, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "CTrip";
            // 
            // ctripCounterLabel
            // 
            this.ctripCounterLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ctripCounterLabel.AutoSize = true;
            this.ctripCounterLabel.Location = new System.Drawing.Point(484, 12);
            this.ctripCounterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ctripCounterLabel.Name = "ctripCounterLabel";
            this.ctripCounterLabel.Size = new System.Drawing.Size(18, 20);
            this.ctripCounterLabel.TabIndex = 4;
            this.ctripCounterLabel.Text = "0";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(510, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Qunar";
            // 
            // qunarCounterLabel
            // 
            this.qunarCounterLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.qunarCounterLabel.AutoSize = true;
            this.qunarCounterLabel.Location = new System.Drawing.Point(571, 12);
            this.qunarCounterLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.qunarCounterLabel.Name = "qunarCounterLabel";
            this.qunarCounterLabel.Size = new System.Drawing.Size(18, 20);
            this.qunarCounterLabel.TabIndex = 7;
            this.qunarCounterLabel.Text = "0";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(597, 12);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Matched";
            // 
            // matchedCountLabel
            // 
            this.matchedCountLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.matchedCountLabel.AutoSize = true;
            this.matchedCountLabel.Location = new System.Drawing.Point(676, 12);
            this.matchedCountLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.matchedCountLabel.Name = "matchedCountLabel";
            this.matchedCountLabel.Size = new System.Drawing.Size(18, 20);
            this.matchedCountLabel.TabIndex = 11;
            this.matchedCountLabel.Text = "0";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 46);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ctripBrowser);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.qunarBrowser);
            this.splitContainer1.Size = new System.Drawing.Size(944, 600);
            this.splitContainer1.SplitterDistance = 451;
            this.splitContainer1.TabIndex = 4;
            // 
            // ctripBrowser
            // 
            this.ctripBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctripBrowser.Location = new System.Drawing.Point(0, 0);
            this.ctripBrowser.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ctripBrowser.MinimumSize = new System.Drawing.Size(30, 31);
            this.ctripBrowser.Name = "ctripBrowser";
            this.ctripBrowser.Size = new System.Drawing.Size(451, 600);
            this.ctripBrowser.TabIndex = 2;
            // 
            // qunarBrowser
            // 
            this.qunarBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.qunarBrowser.Location = new System.Drawing.Point(0, 0);
            this.qunarBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.qunarBrowser.Name = "qunarBrowser";
            this.qunarBrowser.Size = new System.Drawing.Size(489, 600);
            this.qunarBrowser.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 646);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "WebCrawler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scanPagesSlider)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Label ctripCounterLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label qunarCounterLabel;
        private System.Windows.Forms.NumericUpDown scanPagesSlider;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label matchedCountLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.WebBrowser ctripBrowser;
        private System.Windows.Forms.WebBrowser qunarBrowser;
    }
}

