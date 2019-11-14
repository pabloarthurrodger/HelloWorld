namespace PalmBeachScrapper
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
            this.tbMain = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.MainBrowser = new System.Windows.Forms.WebBrowser();
            this.pnlMainTop = new System.Windows.Forms.Panel();
            this.nupYear = new System.Windows.Forms.NumericUpDown();
            this.cbGetCases = new System.Windows.Forms.CheckBox();
            this.btnGO = new System.Windows.Forms.Button();
            this.tbURI = new System.Windows.Forms.TextBox();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.tbMain.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.pnlMainTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupYear)).BeginInit();
            this.SuspendLayout();
            // 
            // tbMain
            // 
            this.tbMain.Controls.Add(this.tabPage1);
            this.tbMain.Controls.Add(this.tabPage2);
            this.tbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMain.Location = new System.Drawing.Point(0, 0);
            this.tbMain.Name = "tbMain";
            this.tbMain.SelectedIndex = 0;
            this.tbMain.Size = new System.Drawing.Size(1438, 751);
            this.tbMain.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.MainBrowser);
            this.tabPage1.Controls.Add(this.pnlMainTop);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1430, 725);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // MainBrowser
            // 
            this.MainBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainBrowser.Location = new System.Drawing.Point(3, 90);
            this.MainBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.MainBrowser.Name = "MainBrowser";
            this.MainBrowser.Size = new System.Drawing.Size(1424, 632);
            this.MainBrowser.TabIndex = 1;
            this.MainBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.MainBrowser_DocumentCompleted);
            // 
            // pnlMainTop
            // 
            this.pnlMainTop.Controls.Add(this.button1);
            this.pnlMainTop.Controls.Add(this.nupYear);
            this.pnlMainTop.Controls.Add(this.cbGetCases);
            this.pnlMainTop.Controls.Add(this.btnGO);
            this.pnlMainTop.Controls.Add(this.tbURI);
            this.pnlMainTop.Controls.Add(this.btnForward);
            this.pnlMainTop.Controls.Add(this.btnBack);
            this.pnlMainTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMainTop.Location = new System.Drawing.Point(3, 3);
            this.pnlMainTop.Name = "pnlMainTop";
            this.pnlMainTop.Size = new System.Drawing.Size(1424, 87);
            this.pnlMainTop.TabIndex = 0;
            // 
            // nupYear
            // 
            this.nupYear.Location = new System.Drawing.Point(20, 52);
            this.nupYear.Maximum = new decimal(new int[] {
            2019,
            0,
            0,
            0});
            this.nupYear.Minimum = new decimal(new int[] {
            2008,
            0,
            0,
            0});
            this.nupYear.Name = "nupYear";
            this.nupYear.Size = new System.Drawing.Size(81, 20);
            this.nupYear.TabIndex = 6;
            this.nupYear.Value = new decimal(new int[] {
            2008,
            0,
            0,
            0});
            // 
            // cbGetCases
            // 
            this.cbGetCases.AutoSize = true;
            this.cbGetCases.Location = new System.Drawing.Point(1326, 18);
            this.cbGetCases.Name = "cbGetCases";
            this.cbGetCases.Size = new System.Drawing.Size(75, 17);
            this.cbGetCases.TabIndex = 4;
            this.cbGetCases.Text = "Get Cases";
            this.cbGetCases.UseVisualStyleBackColor = true;
            // 
            // btnGO
            // 
            this.btnGO.Location = new System.Drawing.Point(1268, 15);
            this.btnGO.Name = "btnGO";
            this.btnGO.Size = new System.Drawing.Size(37, 23);
            this.btnGO.TabIndex = 3;
            this.btnGO.Text = "GO";
            this.btnGO.UseVisualStyleBackColor = true;
            this.btnGO.Click += new System.EventHandler(this.btnGO_Click);
            // 
            // tbURI
            // 
            this.tbURI.Location = new System.Drawing.Point(116, 17);
            this.tbURI.Name = "tbURI";
            this.tbURI.Size = new System.Drawing.Size(1136, 20);
            this.tbURI.TabIndex = 2;
            this.tbURI.Text = "https://applications.mypalmbeachclerk.com/eCaseView/search.aspx";
            // 
            // btnForward
            // 
            this.btnForward.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnForward.Location = new System.Drawing.Point(68, 14);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(42, 23);
            this.btnForward.TabIndex = 1;
            this.btnForward.Text = "->";
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(20, 14);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(42, 23);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "<-";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1430, 725);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1268, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1438, 751);
            this.Controls.Add(this.tbMain);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.tbMain.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.pnlMainTop.ResumeLayout(false);
            this.pnlMainTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupYear)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbMain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.WebBrowser MainBrowser;
        private System.Windows.Forms.Panel pnlMainTop;
        private System.Windows.Forms.Button btnGO;
        private System.Windows.Forms.TextBox tbURI;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox cbGetCases;
        private System.Windows.Forms.NumericUpDown nupYear;
        private System.Windows.Forms.Button button1;
    }
}

