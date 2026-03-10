namespace OA99_PLUS_DEMO
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.OpenDeviceT = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.FindDeviceT = new System.Windows.Forms.Button();
            this.m_btnImage = new System.Windows.Forms.PictureBox();
            this.ShowImageT = new System.Windows.Forms.Button();
            this.ShowBinFeaT = new System.Windows.Forms.Button();
            this.StoreImageT = new System.Windows.Forms.Button();
            this.StoreBinT = new System.Windows.Forms.Button();
            this.StoreFeatureT = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BrowseFeaFile = new System.Windows.Forms.Button();
            this.ResultFea = new System.Windows.Forms.Button();
            this.FindOneToMany = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.button11 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_btnImage)).BeginInit();
            this.SuspendLayout();
            // 
            // OpenDeviceT
            // 
            this.OpenDeviceT.Location = new System.Drawing.Point(24, 382);
            this.OpenDeviceT.Name = "OpenDeviceT";
            this.OpenDeviceT.Size = new System.Drawing.Size(256, 25);
            this.OpenDeviceT.TabIndex = 0;
            this.OpenDeviceT.Text = "Open Device";
            this.OpenDeviceT.UseVisualStyleBackColor = true;
            this.OpenDeviceT.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(143, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(137, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // FindDeviceT
            // 
            this.FindDeviceT.Location = new System.Drawing.Point(24, 13);
            this.FindDeviceT.Name = "FindDeviceT";
            this.FindDeviceT.Size = new System.Drawing.Size(101, 25);
            this.FindDeviceT.TabIndex = 2;
            this.FindDeviceT.Text = "Enum Device";
            this.FindDeviceT.UseVisualStyleBackColor = true;
            this.FindDeviceT.Click += new System.EventHandler(this.button2_Click);
            // 
            // m_btnImage
            // 
            this.m_btnImage.ErrorImage = null;
            this.m_btnImage.Location = new System.Drawing.Point(24, 61);
            this.m_btnImage.Name = "m_btnImage";
            this.m_btnImage.Size = new System.Drawing.Size(256, 303);
            this.m_btnImage.TabIndex = 3;
            this.m_btnImage.TabStop = false;
            // 
            // ShowImageT
            // 
            this.ShowImageT.Location = new System.Drawing.Point(307, 12);
            this.ShowImageT.Name = "ShowImageT";
            this.ShowImageT.Size = new System.Drawing.Size(101, 25);
            this.ShowImageT.TabIndex = 4;
            this.ShowImageT.Text = "Show Image";
            this.ShowImageT.UseVisualStyleBackColor = true;
            this.ShowImageT.Click += new System.EventHandler(this.button3_Click);
            // 
            // ShowBinFeaT
            // 
            this.ShowBinFeaT.Location = new System.Drawing.Point(307, 61);
            this.ShowBinFeaT.Name = "ShowBinFeaT";
            this.ShowBinFeaT.Size = new System.Drawing.Size(101, 64);
            this.ShowBinFeaT.TabIndex = 5;
            this.ShowBinFeaT.Text = "Show Bin&Fea";
            this.ShowBinFeaT.UseVisualStyleBackColor = true;
            this.ShowBinFeaT.Click += new System.EventHandler(this.button4_Click);
            // 
            // StoreImageT
            // 
            this.StoreImageT.Location = new System.Drawing.Point(414, 12);
            this.StoreImageT.Name = "StoreImageT";
            this.StoreImageT.Size = new System.Drawing.Size(117, 25);
            this.StoreImageT.TabIndex = 6;
            this.StoreImageT.Text = "Store Image";
            this.StoreImageT.UseVisualStyleBackColor = true;
            this.StoreImageT.Click += new System.EventHandler(this.button5_Click);
            // 
            // StoreBinT
            // 
            this.StoreBinT.Location = new System.Drawing.Point(414, 61);
            this.StoreBinT.Name = "StoreBinT";
            this.StoreBinT.Size = new System.Drawing.Size(117, 25);
            this.StoreBinT.TabIndex = 7;
            this.StoreBinT.Text = "Store Bin";
            this.StoreBinT.UseVisualStyleBackColor = true;
            this.StoreBinT.Click += new System.EventHandler(this.button6_Click);
            // 
            // StoreFeatureT
            // 
            this.StoreFeatureT.Location = new System.Drawing.Point(414, 100);
            this.StoreFeatureT.Name = "StoreFeatureT";
            this.StoreFeatureT.Size = new System.Drawing.Size(117, 25);
            this.StoreFeatureT.TabIndex = 8;
            this.StoreFeatureT.Text = "Store Feature";
            this.StoreFeatureT.UseVisualStyleBackColor = true;
            this.StoreFeatureT.Click += new System.EventHandler(this.button7_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(307, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "1:1 Match With Feature File";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(307, 160);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Fea File";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(307, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Result";
            // 
            // BrowseFeaFile
            // 
            this.BrowseFeaFile.Location = new System.Drawing.Point(473, 155);
            this.BrowseFeaFile.Name = "BrowseFeaFile";
            this.BrowseFeaFile.Size = new System.Drawing.Size(58, 25);
            this.BrowseFeaFile.TabIndex = 12;
            this.BrowseFeaFile.Text = "Browse";
            this.BrowseFeaFile.UseVisualStyleBackColor = true;
            this.BrowseFeaFile.Click += new System.EventHandler(this.button8_Click);
            // 
            // ResultFea
            // 
            this.ResultFea.Location = new System.Drawing.Point(473, 187);
            this.ResultFea.Name = "ResultFea";
            this.ResultFea.Size = new System.Drawing.Size(58, 25);
            this.ResultFea.TabIndex = 13;
            this.ResultFea.Text = "Match";
            this.ResultFea.UseVisualStyleBackColor = true;
            this.ResultFea.Click += new System.EventHandler(this.button9_Click);
            // 
            // FindOneToMany
            // 
            this.FindOneToMany.Location = new System.Drawing.Point(473, 266);
            this.FindOneToMany.Name = "FindOneToMany";
            this.FindOneToMany.Size = new System.Drawing.Size(58, 88);
            this.FindOneToMany.TabIndex = 18;
            this.FindOneToMany.Text = "Match";
            this.FindOneToMany.UseVisualStyleBackColor = true;
            this.FindOneToMany.Click += new System.EventHandler(this.button10_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(307, 335);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Time";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(307, 270);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Match Cnt";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(307, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(171, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "1:N Match with Current Dir Feature";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(307, 303);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Result";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(366, 157);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(102, 20);
            this.textBox1.TabIndex = 20;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(366, 190);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(102, 20);
            this.textBox2.TabIndex = 21;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(366, 266);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 22;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(366, 299);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 20);
            this.textBox4.TabIndex = 23;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(366, 332);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 20);
            this.textBox5.TabIndex = 24;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(364, 375);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(102, 20);
            this.textBox6.TabIndex = 27;
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(471, 373);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(58, 25);
            this.button11.TabIndex = 26;
            this.button11.Text = "GetCard";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(305, 378);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "Card No.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 431);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.FindOneToMany);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ResultFea);
            this.Controls.Add(this.BrowseFeaFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StoreFeatureT);
            this.Controls.Add(this.StoreBinT);
            this.Controls.Add(this.StoreImageT);
            this.Controls.Add(this.ShowBinFeaT);
            this.Controls.Add(this.ShowImageT);
            this.Controls.Add(this.m_btnImage);
            this.Controls.Add(this.FindDeviceT);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.OpenDeviceT);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.m_btnImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OpenDeviceT;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button FindDeviceT;
        private System.Windows.Forms.PictureBox m_btnImage;
        private System.Windows.Forms.Button ShowImageT;
        private System.Windows.Forms.Button ShowBinFeaT;
        private System.Windows.Forms.Button StoreImageT;
        private System.Windows.Forms.Button StoreBinT;
        private System.Windows.Forms.Button StoreFeatureT;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BrowseFeaFile;
        private System.Windows.Forms.Button ResultFea;
        private System.Windows.Forms.Button FindOneToMany;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Label label8;
    }
}

