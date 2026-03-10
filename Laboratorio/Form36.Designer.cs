namespace Laboratorio
{
    partial class Form36
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
            this.panel4 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.SeccionCombo = new System.Windows.Forms.ComboBox();
            this.especiales = new System.Windows.Forms.CheckBox();
            this.Visible = new System.Windows.Forms.CheckBox();
            this.TEtiquetas = new System.Windows.Forms.TextBox();
            this.Nombre = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.TUnidad = new System.Windows.Forms.TextBox();
            this.TValores = new System.Windows.Forms.TextBox();
            this.cualitativoscheck = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.Tdesde = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cuantitativoscheck = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.Thasta = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.iconButton2 = new FontAwesome.Sharp.IconButton();
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.panel4.Controls.Add(this.label11);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.label12);
            this.panel4.Controls.Add(this.SeccionCombo);
            this.panel4.Controls.Add(this.especiales);
            this.panel4.Controls.Add(this.Visible);
            this.panel4.Controls.Add(this.TEtiquetas);
            this.panel4.Controls.Add(this.Nombre);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Location = new System.Drawing.Point(12, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(545, 185);
            this.panel4.TabIndex = 106;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(23, 150);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 16);
            this.label11.TabIndex = 174;
            this.label11.Text = "Especiales";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(27, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 16);
            this.label2.TabIndex = 173;
            this.label2.Text = "Seccion Etiqueta";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(26, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 16);
            this.label3.TabIndex = 172;
            this.label3.Text = "Estadistica";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(26, 94);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 16);
            this.label12.TabIndex = 171;
            this.label12.Text = "Etiqueta";
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
            this.SeccionCombo.Location = new System.Drawing.Point(176, 67);
            this.SeccionCombo.Name = "SeccionCombo";
            this.SeccionCombo.Size = new System.Drawing.Size(222, 21);
            this.SeccionCombo.TabIndex = 167;
            this.SeccionCombo.SelectedIndexChanged += new System.EventHandler(this.SeccionCombo_SelectedIndexChanged);
            // 
            // especiales
            // 
            this.especiales.AutoSize = true;
            this.especiales.Location = new System.Drawing.Point(176, 152);
            this.especiales.Name = "especiales";
            this.especiales.Size = new System.Drawing.Size(15, 14);
            this.especiales.TabIndex = 170;
            this.especiales.UseVisualStyleBackColor = true;
            // 
            // Visible
            // 
            this.Visible.AutoSize = true;
            this.Visible.Location = new System.Drawing.Point(176, 126);
            this.Visible.Name = "Visible";
            this.Visible.Size = new System.Drawing.Size(15, 14);
            this.Visible.TabIndex = 169;
            this.Visible.UseVisualStyleBackColor = true;
            // 
            // TEtiquetas
            // 
            this.TEtiquetas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TEtiquetas.Location = new System.Drawing.Point(176, 94);
            this.TEtiquetas.Name = "TEtiquetas";
            this.TEtiquetas.Size = new System.Drawing.Size(222, 20);
            this.TEtiquetas.TabIndex = 168;
            // 
            // Nombre
            // 
            this.Nombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.Nombre.Location = new System.Drawing.Point(167, 32);
            this.Nombre.Name = "Nombre";
            this.Nombre.Size = new System.Drawing.Size(327, 20);
            this.Nombre.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(13, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(147, 16);
            this.label10.TabIndex = 155;
            this.label10.Text = "Nombre del Analisis";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Maroon;
            this.label9.Location = new System.Drawing.Point(14, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(122, 16);
            this.label9.TabIndex = 141;
            this.label9.Text = "Agregar Analisis";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.TUnidad);
            this.panel2.Controls.Add(this.TValores);
            this.panel2.Controls.Add(this.cualitativoscheck);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.checkBox4);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.Tdesde);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.cuantitativoscheck);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.Thasta);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Location = new System.Drawing.Point(12, 202);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(545, 274);
            this.panel2.TabIndex = 173;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(42, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 16);
            this.label8.TabIndex = 186;
            this.label8.Text = "Unidad";
            // 
            // TUnidad
            // 
            this.TUnidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TUnidad.Location = new System.Drawing.Point(105, 44);
            this.TUnidad.Name = "TUnidad";
            this.TUnidad.Size = new System.Drawing.Size(85, 20);
            this.TUnidad.TabIndex = 174;
            // 
            // TValores
            // 
            this.TValores.Enabled = false;
            this.TValores.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TValores.Location = new System.Drawing.Point(219, 45);
            this.TValores.Multiline = true;
            this.TValores.Name = "TValores";
            this.TValores.Size = new System.Drawing.Size(306, 196);
            this.TValores.TabIndex = 179;
            // 
            // cualitativoscheck
            // 
            this.cualitativoscheck.AutoSize = true;
            this.cualitativoscheck.Location = new System.Drawing.Point(383, 8);
            this.cualitativoscheck.Name = "cualitativoscheck";
            this.cualitativoscheck.Size = new System.Drawing.Size(15, 14);
            this.cualitativoscheck.TabIndex = 178;
            this.cualitativoscheck.UseVisualStyleBackColor = true;
            this.cualitativoscheck.CheckedChanged += new System.EventHandler(this.cualitativoscheck_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(248, 6);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(129, 16);
            this.label13.TabIndex = 185;
            this.label13.Text = "Cualicuantitativos";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(342, 8);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(15, 14);
            this.checkBox4.TabIndex = 184;
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(45, 99);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 16);
            this.label16.TabIndex = 183;
            this.label16.Text = "Desde";
            // 
            // Tdesde
            // 
            this.Tdesde.Enabled = false;
            this.Tdesde.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tdesde.Location = new System.Drawing.Point(105, 98);
            this.Tdesde.Name = "Tdesde";
            this.Tdesde.Size = new System.Drawing.Size(85, 20);
            this.Tdesde.TabIndex = 176;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(11, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 16);
            this.label5.TabIndex = 182;
            this.label5.Text = "Cuantitativos";
            // 
            // cuantitativoscheck
            // 
            this.cuantitativoscheck.AutoSize = true;
            this.cuantitativoscheck.Location = new System.Drawing.Point(146, 75);
            this.cuantitativoscheck.Name = "cuantitativoscheck";
            this.cuantitativoscheck.Size = new System.Drawing.Size(15, 14);
            this.cuantitativoscheck.TabIndex = 175;
            this.cuantitativoscheck.UseVisualStyleBackColor = true;
            this.cuantitativoscheck.CheckedChanged += new System.EventHandler(this.cuantitativoscheck_CheckedChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(42, 132);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 16);
            this.label14.TabIndex = 181;
            this.label14.Text = "Hasta";
            // 
            // Thasta
            // 
            this.Thasta.Enabled = false;
            this.Thasta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Thasta.Location = new System.Drawing.Point(105, 132);
            this.Thasta.Name = "Thasta";
            this.Thasta.Size = new System.Drawing.Size(85, 20);
            this.Thasta.TabIndex = 177;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(2, 12);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(163, 16);
            this.label15.TabIndex = 180;
            this.label15.Text = "Valores de Referencia";
            // 
            // iconButton2
            // 
            this.iconButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton2.IconChar = FontAwesome.Sharp.IconChar.FloppyDisk;
            this.iconButton2.IconColor = System.Drawing.Color.Black;
            this.iconButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton2.IconSize = 25;
            this.iconButton2.Location = new System.Drawing.Point(457, 482);
            this.iconButton2.Name = "iconButton2";
            this.iconButton2.Size = new System.Drawing.Size(100, 41);
            this.iconButton2.TabIndex = 175;
            this.iconButton2.Text = "Guardar y Salir";
            this.iconButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton2.UseVisualStyleBackColor = false;
            this.iconButton2.Click += new System.EventHandler(this.iconButton2_Click);
            // 
            // iconButton1
            // 
            this.iconButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton1.IconChar = FontAwesome.Sharp.IconChar.CircleArrowLeft;
            this.iconButton1.IconColor = System.Drawing.Color.Black;
            this.iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton1.IconSize = 25;
            this.iconButton1.Location = new System.Drawing.Point(12, 482);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Size = new System.Drawing.Size(100, 41);
            this.iconButton1.TabIndex = 174;
            this.iconButton1.Text = "Salir Sin Modificar";
            this.iconButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton1.UseVisualStyleBackColor = false;
            this.iconButton1.Click += new System.EventHandler(this.iconButton1_Click);
            // 
            // Form36
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(565, 535);
            this.Controls.Add(this.iconButton2);
            this.Controls.Add(this.iconButton1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form36";
            this.Text = "Agregar Analisis";
            this.Load += new System.EventHandler(this.Form36_Load);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox Nombre;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel2;
        private FontAwesome.Sharp.IconButton iconButton2;
        private FontAwesome.Sharp.IconButton iconButton1;
        private System.Windows.Forms.ComboBox SeccionCombo;
        private System.Windows.Forms.CheckBox especiales;
        private System.Windows.Forms.CheckBox Visible;
        private System.Windows.Forms.TextBox TEtiquetas;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TUnidad;
        private System.Windows.Forms.TextBox TValores;
        private System.Windows.Forms.CheckBox cualitativoscheck;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox Tdesde;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cuantitativoscheck;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox Thasta;
        private System.Windows.Forms.Label label15;
    }
}