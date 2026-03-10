namespace Laboratorio
{
    partial class PerfilSimple
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkActivo = new System.Windows.Forms.CheckBox();
            this.PrecioBs = new System.Windows.Forms.TextBox();
            this.TPrecioDolar = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TNombrePerfil = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.iconButton3 = new FontAwesome.Sharp.IconButton();
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SeccionCombo = new System.Windows.Forms.ComboBox();
            this.especiales = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Visible = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.TEtiquetas = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.TUnidad = new System.Windows.Forms.TextBox();
            this.TValores = new System.Windows.Forms.TextBox();
            this.cualitativoscheck = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.Tdesde = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cuantitativoscheck = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.Thasta = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.panel1.Controls.Add(this.checkActivo);
            this.panel1.Controls.Add(this.PrecioBs);
            this.panel1.Controls.Add(this.TPrecioDolar);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.TNombrePerfil);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(444, 162);
            this.panel1.TabIndex = 4;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // checkActivo
            // 
            this.checkActivo.AutoSize = true;
            this.checkActivo.Location = new System.Drawing.Point(146, 135);
            this.checkActivo.Name = "checkActivo";
            this.checkActivo.Size = new System.Drawing.Size(15, 14);
            this.checkActivo.TabIndex = 4;
            this.checkActivo.UseVisualStyleBackColor = true;
            // 
            // PrecioBs
            // 
            this.PrecioBs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PrecioBs.Location = new System.Drawing.Point(146, 98);
            this.PrecioBs.Name = "PrecioBs";
            this.PrecioBs.Size = new System.Drawing.Size(280, 20);
            this.PrecioBs.TabIndex = 3;
            this.PrecioBs.TextChanged += new System.EventHandler(this.PrecioBs_TextChanged);
            // 
            // TPrecioDolar
            // 
            this.TPrecioDolar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TPrecioDolar.Location = new System.Drawing.Point(146, 72);
            this.TPrecioDolar.Name = "TPrecioDolar";
            this.TPrecioDolar.Size = new System.Drawing.Size(280, 20);
            this.TPrecioDolar.TabIndex = 2;
            this.TPrecioDolar.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            this.TPrecioDolar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TPrecioDolar_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(39, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 16);
            this.label5.TabIndex = 118;
            this.label5.Text = "Activo";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(27, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 16);
            this.label4.TabIndex = 117;
            this.label4.Text = "Precio Bs";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(37, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 116;
            this.label3.Text = "Precio $";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 16);
            this.label2.TabIndex = 115;
            this.label2.Text = "Nombre";
            // 
            // TNombrePerfil
            // 
            this.TNombrePerfil.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TNombrePerfil.Location = new System.Drawing.Point(146, 44);
            this.TNombrePerfil.Name = "TNombrePerfil";
            this.TNombrePerfil.Size = new System.Drawing.Size(280, 20);
            this.TNombrePerfil.TabIndex = 1;
            this.TNombrePerfil.TextChanged += new System.EventHandler(this.TNombrePerfil_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 16);
            this.label1.TabIndex = 112;
            this.label1.Text = "Perfil";
            // 
            // iconButton3
            // 
            this.iconButton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton3.IconChar = FontAwesome.Sharp.IconChar.CircleArrowLeft;
            this.iconButton3.IconColor = System.Drawing.Color.Black;
            this.iconButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton3.IconSize = 25;
            this.iconButton3.Location = new System.Drawing.Point(13, 449);
            this.iconButton3.Name = "iconButton3";
            this.iconButton3.Size = new System.Drawing.Size(100, 33);
            this.iconButton3.TabIndex = 16;
            this.iconButton3.Text = "Salir ";
            this.iconButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton3.UseVisualStyleBackColor = false;
            this.iconButton3.Click += new System.EventHandler(this.iconButton3_Click);
            // 
            // iconButton1
            // 
            this.iconButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton1.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            this.iconButton1.IconColor = System.Drawing.Color.Black;
            this.iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton1.IconSize = 25;
            this.iconButton1.Location = new System.Drawing.Point(757, 449);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Size = new System.Drawing.Size(119, 33);
            this.iconButton1.TabIndex = 15;
            this.iconButton1.Text = "Guardar";
            this.iconButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.iconButton1.UseVisualStyleBackColor = false;
            this.iconButton1.Click += new System.EventHandler(this.iconButton1_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.panel2.Controls.Add(this.SeccionCombo);
            this.panel2.Controls.Add(this.especiales);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.Visible);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.TEtiquetas);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(12, 180);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(444, 263);
            this.panel2.TabIndex = 122;
            // 
            // SeccionCombo
            // 
            this.SeccionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SeccionCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SeccionCombo.FormattingEnabled = true;
            this.SeccionCombo.Items.AddRange(new object[] {
            "INDIVIDUAL",
            "COAGULACION",
            "QUIMICA",
            "ORINA",
            "HECES",
            "ESPECIALES",
            "CENTRALIZADOS"});
            this.SeccionCombo.Location = new System.Drawing.Point(146, 54);
            this.SeccionCombo.Name = "SeccionCombo";
            this.SeccionCombo.Size = new System.Drawing.Size(222, 21);
            this.SeccionCombo.TabIndex = 5;
            this.SeccionCombo.SelectedIndexChanged += new System.EventHandler(this.SeccionCombo_SelectedIndexChanged);
            // 
            // especiales
            // 
            this.especiales.AutoSize = true;
            this.especiales.Location = new System.Drawing.Point(146, 139);
            this.especiales.Name = "especiales";
            this.especiales.Size = new System.Drawing.Size(15, 14);
            this.especiales.TabIndex = 8;
            this.especiales.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(16, 137);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 16);
            this.label11.TabIndex = 162;
            this.label11.Text = "Especiales";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(20, 55);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(124, 16);
            this.label10.TabIndex = 161;
            this.label10.Text = "Seccion Etiqueta";
            // 
            // Visible
            // 
            this.Visible.AutoSize = true;
            this.Visible.Location = new System.Drawing.Point(146, 113);
            this.Visible.Name = "Visible";
            this.Visible.Size = new System.Drawing.Size(15, 14);
            this.Visible.TabIndex = 7;
            this.Visible.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(19, 111);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 16);
            this.label7.TabIndex = 159;
            this.label7.Text = "Estadistica";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(19, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 16);
            this.label9.TabIndex = 158;
            this.label9.Text = "Etiqueta";
            // 
            // TEtiquetas
            // 
            this.TEtiquetas.Enabled = false;
            this.TEtiquetas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TEtiquetas.Location = new System.Drawing.Point(146, 81);
            this.TEtiquetas.Name = "TEtiquetas";
            this.TEtiquetas.Size = new System.Drawing.Size(222, 20);
            this.TEtiquetas.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(5, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 16);
            this.label6.TabIndex = 156;
            this.label6.Text = "Analisis";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.TUnidad);
            this.panel3.Controls.Add(this.TValores);
            this.panel3.Controls.Add(this.cualitativoscheck);
            this.panel3.Controls.Add(this.label13);
            this.panel3.Controls.Add(this.checkBox4);
            this.panel3.Controls.Add(this.label16);
            this.panel3.Controls.Add(this.Tdesde);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.cuantitativoscheck);
            this.panel3.Controls.Add(this.label14);
            this.panel3.Controls.Add(this.Thasta);
            this.panel3.Controls.Add(this.label15);
            this.panel3.Location = new System.Drawing.Point(462, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(414, 431);
            this.panel3.TabIndex = 165;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(45, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 16);
            this.label8.TabIndex = 173;
            this.label8.Text = "Unidad";
            // 
            // TUnidad
            // 
            this.TUnidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TUnidad.Location = new System.Drawing.Point(108, 43);
            this.TUnidad.Name = "TUnidad";
            this.TUnidad.Size = new System.Drawing.Size(85, 20);
            this.TUnidad.TabIndex = 9;
            // 
            // TValores
            // 
            this.TValores.Enabled = false;
            this.TValores.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TValores.Location = new System.Drawing.Point(19, 157);
            this.TValores.Multiline = true;
            this.TValores.Name = "TValores";
            this.TValores.Size = new System.Drawing.Size(358, 260);
            this.TValores.TabIndex = 14;
            // 
            // cualitativoscheck
            // 
            this.cualitativoscheck.AutoSize = true;
            this.cualitativoscheck.Location = new System.Drawing.Point(151, 128);
            this.cualitativoscheck.Name = "cualitativoscheck";
            this.cualitativoscheck.Size = new System.Drawing.Size(15, 14);
            this.cualitativoscheck.TabIndex = 13;
            this.cualitativoscheck.UseVisualStyleBackColor = true;
            this.cualitativoscheck.CheckedChanged += new System.EventHandler(this.cualitativoscheck_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(16, 126);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(129, 16);
            this.label13.TabIndex = 169;
            this.label13.Text = "Cualicuantitativos";
            this.label13.Click += new System.EventHandler(this.label13_Click);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(110, 128);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(15, 14);
            this.checkBox4.TabIndex = 168;
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(49, 94);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 16);
            this.label16.TabIndex = 165;
            this.label16.Text = "Desde";
            // 
            // Tdesde
            // 
            this.Tdesde.Enabled = false;
            this.Tdesde.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tdesde.Location = new System.Drawing.Point(108, 94);
            this.Tdesde.Name = "Tdesde";
            this.Tdesde.Size = new System.Drawing.Size(85, 20);
            this.Tdesde.TabIndex = 11;
            this.Tdesde.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Tdesde_KeyPress);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(14, 72);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(96, 16);
            this.label12.TabIndex = 161;
            this.label12.Text = "Cuantitativos";
            // 
            // cuantitativoscheck
            // 
            this.cuantitativoscheck.AutoSize = true;
            this.cuantitativoscheck.Location = new System.Drawing.Point(149, 74);
            this.cuantitativoscheck.Name = "cuantitativoscheck";
            this.cuantitativoscheck.Size = new System.Drawing.Size(15, 14);
            this.cuantitativoscheck.TabIndex = 10;
            this.cuantitativoscheck.UseVisualStyleBackColor = true;
            this.cuantitativoscheck.CheckedChanged += new System.EventHandler(this.cuantitativoscheck_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(240, 94);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 16);
            this.label14.TabIndex = 158;
            this.label14.Text = "Hasta";
            // 
            // Thasta
            // 
            this.Thasta.Enabled = false;
            this.Thasta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Thasta.Location = new System.Drawing.Point(294, 94);
            this.Thasta.Name = "Thasta";
            this.Thasta.Size = new System.Drawing.Size(94, 20);
            this.Thasta.TabIndex = 12;
            this.Thasta.TextChanged += new System.EventHandler(this.Thasta_TextChanged);
            this.Thasta.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Thasta_KeyPress);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(5, 11);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(163, 16);
            this.label15.TabIndex = 156;
            this.label15.Text = "Valores de Referencia";
            // 
            // PerfilSimple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(888, 490);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.iconButton3);
            this.Controls.Add(this.iconButton1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PerfilSimple";
            this.Text = "PerfilSimple";
            this.Load += new System.EventHandler(this.PerfilSimple_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TPrecioDolar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TNombrePerfil;
        private System.Windows.Forms.CheckBox checkActivo;
        private System.Windows.Forms.TextBox PrecioBs;
        private FontAwesome.Sharp.IconButton iconButton3;
        private FontAwesome.Sharp.IconButton iconButton1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox SeccionCombo;
        private System.Windows.Forms.CheckBox especiales;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox Visible;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox TEtiquetas;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox TValores;
        private System.Windows.Forms.CheckBox cualitativoscheck;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox Tdesde;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cuantitativoscheck;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox Thasta;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TUnidad;
    }
}