namespace Laboratorio
{
    partial class Login
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
            this.label3 = new System.Windows.Forms.Label();
            this.ServidoresCheck = new FontAwesome.Sharp.IconButton();
            this.ServidoresCBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.CerrarBtn = new FontAwesome.Sharp.IconButton();
            this.LoginBtn = new FontAwesome.Sharp.IconButton();
            this.label2 = new System.Windows.Forms.Label();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.iconButton3 = new FontAwesome.Sharp.IconButton();
            this.CaptahuellasBtn = new FontAwesome.Sharp.IconButton();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(77, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 20);
            this.label3.TabIndex = 28;
            this.label3.Text = "Servidor";
            this.label3.Visible = false;
            // 
            // ServidoresCheck
            // 
            this.ServidoresCheck.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.ServidoresCheck.FlatAppearance.BorderSize = 0;
            this.ServidoresCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ServidoresCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServidoresCheck.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(242)))));
            this.ServidoresCheck.IconChar = FontAwesome.Sharp.IconChar.Stop;
            this.ServidoresCheck.IconColor = System.Drawing.Color.White;
            this.ServidoresCheck.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.ServidoresCheck.IconSize = 25;
            this.ServidoresCheck.Location = new System.Drawing.Point(200, 169);
            this.ServidoresCheck.Name = "ServidoresCheck";
            this.ServidoresCheck.Size = new System.Drawing.Size(34, 24);
            this.ServidoresCheck.TabIndex = 27;
            this.ServidoresCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.ServidoresCheck.UseVisualStyleBackColor = true;
            // 
            // ServidoresCBox
            // 
            this.ServidoresCBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ServidoresCBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ServidoresCBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServidoresCBox.ForeColor = System.Drawing.Color.Black;
            this.ServidoresCBox.FormattingEnabled = true;
            this.ServidoresCBox.Items.AddRange(new object[] {
            "RIVANA",
            "ARCOS PARADA",
            "HOSPITALAB",
            "NARDO",
            "ARO",
            "ESPECIALES",
            "PARAVET REMOTO",
            "PARAVET",
            "ARCOS PARADA 2"});
            this.ServidoresCBox.Location = new System.Drawing.Point(33, 168);
            this.ServidoresCBox.Name = "ServidoresCBox";
            this.ServidoresCBox.Size = new System.Drawing.Size(162, 21);
            this.ServidoresCBox.TabIndex = 24;
            this.ServidoresCBox.Visible = false;
            this.ServidoresCBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(177, 301);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 18);
            this.label5.TabIndex = 25;
            this.label5.Text = "Vr. 2.75";
            // 
            // CerrarBtn
            // 
            this.CerrarBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.CerrarBtn.FlatAppearance.BorderSize = 0;
            this.CerrarBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CerrarBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CerrarBtn.ForeColor = System.Drawing.Color.White;
            this.CerrarBtn.IconChar = FontAwesome.Sharp.IconChar.Multiply;
            this.CerrarBtn.IconColor = System.Drawing.Color.White;
            this.CerrarBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.CerrarBtn.IconSize = 30;
            this.CerrarBtn.Location = new System.Drawing.Point(2, 2);
            this.CerrarBtn.Name = "CerrarBtn";
            this.CerrarBtn.Size = new System.Drawing.Size(42, 30);
            this.CerrarBtn.TabIndex = 26;
            this.CerrarBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CerrarBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.CerrarBtn.UseVisualStyleBackColor = true;
            this.CerrarBtn.Click += new System.EventHandler(this.iconButton2_Click_2);
            // 
            // LoginBtn
            // 
            this.LoginBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.LoginBtn.FlatAppearance.BorderSize = 0;
            this.LoginBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoginBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginBtn.ForeColor = System.Drawing.Color.White;
            this.LoginBtn.IconChar = FontAwesome.Sharp.IconChar.CircleArrowRight;
            this.LoginBtn.IconColor = System.Drawing.Color.White;
            this.LoginBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.LoginBtn.IconSize = 30;
            this.LoginBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LoginBtn.Location = new System.Drawing.Point(36, 240);
            this.LoginBtn.Margin = new System.Windows.Forms.Padding(0);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(159, 30);
            this.LoginBtn.TabIndex = 22;
            this.LoginBtn.Text = "Ingresar";
            this.LoginBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LoginBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.LoginBtn.UseVisualStyleBackColor = true;
            this.LoginBtn.Click += new System.EventHandler(this.iconButton1_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(29, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 20);
            this.label2.TabIndex = 23;
            this.label2.Text = "Contraseña";
            // 
            // PasswordBox
            // 
            this.PasswordBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PasswordBox.ForeColor = System.Drawing.Color.Black;
            this.PasswordBox.Location = new System.Drawing.Point(33, 215);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.PasswordChar = '*';
            this.PasswordBox.Size = new System.Drawing.Size(162, 20);
            this.PasswordBox.TabIndex = 21;
            this.PasswordBox.UseSystemPasswordChar = true;
            this.PasswordBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(44, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 29);
            this.label1.TabIndex = 20;
            this.label1.Text = "Bienvenido";
            // 
            // iconButton3
            // 
            this.iconButton3.Enabled = false;
            this.iconButton3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.iconButton3.FlatAppearance.BorderSize = 0;
            this.iconButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(242)))));
            this.iconButton3.IconChar = FontAwesome.Sharp.IconChar.Vcard;
            this.iconButton3.IconColor = System.Drawing.Color.White;
            this.iconButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton3.IconSize = 120;
            this.iconButton3.Location = new System.Drawing.Point(64, 34);
            this.iconButton3.Name = "iconButton3";
            this.iconButton3.Size = new System.Drawing.Size(117, 90);
            this.iconButton3.TabIndex = 19;
            this.iconButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.iconButton3.UseVisualStyleBackColor = true;
            // 
            // CaptahuellasBtn
            // 
            this.CaptahuellasBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.CaptahuellasBtn.FlatAppearance.BorderSize = 0;
            this.CaptahuellasBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CaptahuellasBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CaptahuellasBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(80)))), ((int)(((byte)(242)))));
            this.CaptahuellasBtn.IconChar = FontAwesome.Sharp.IconChar.Fingerprint;
            this.CaptahuellasBtn.IconColor = System.Drawing.Color.White;
            this.CaptahuellasBtn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.CaptahuellasBtn.IconSize = 25;
            this.CaptahuellasBtn.Location = new System.Drawing.Point(200, 215);
            this.CaptahuellasBtn.Name = "CaptahuellasBtn";
            this.CaptahuellasBtn.Size = new System.Drawing.Size(34, 24);
            this.CaptahuellasBtn.TabIndex = 29;
            this.CaptahuellasBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.CaptahuellasBtn.UseVisualStyleBackColor = true;
            this.CaptahuellasBtn.Click += new System.EventHandler(this.CaptahuellasBtn_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.ForeColor = System.Drawing.Color.Black;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "RIVANA",
            "ARCOS PARADA",
            "HOSPITALAB",
            "NARDO",
            "ARO",
            "ESPECIALES",
            "DIVETECH",
            "PARAVET",
            "ARCOS PARADA 2"});
            this.comboBox1.Location = new System.Drawing.Point(64, 7);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(162, 21);
            this.comboBox1.TabIndex = 30;
            this.comboBox1.Visible = false;
            // 
            // Login
            // 
            this.AcceptButton = this.LoginBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(247, 328);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.CaptahuellasBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ServidoresCheck);
            this.Controls.Add(this.ServidoresCBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.CerrarBtn);
            this.Controls.Add(this.LoginBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.iconButton3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private FontAwesome.Sharp.IconButton ServidoresCheck;
        private System.Windows.Forms.ComboBox ServidoresCBox;
        private System.Windows.Forms.Label label5;
        private FontAwesome.Sharp.IconButton CerrarBtn;
        private FontAwesome.Sharp.IconButton LoginBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PasswordBox;
        private System.Windows.Forms.Label label1;
        private FontAwesome.Sharp.IconButton iconButton3;
        private FontAwesome.Sharp.IconButton CaptahuellasBtn;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}