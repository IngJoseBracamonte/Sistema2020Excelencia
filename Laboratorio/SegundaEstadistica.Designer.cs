namespace Laboratorio
{
    partial class SegundaEstadistica
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel4 = new System.Windows.Forms.Panel();
            this.OrdenarConvenio = new FontAwesome.Sharp.IconButton();
            this.OrdenarPerfil = new FontAwesome.Sharp.IconButton();
            this.OrdenarEdad = new FontAwesome.Sharp.IconButton();
            this.OrdenarAño = new FontAwesome.Sharp.IconButton();
            this.OrdenarMes = new FontAwesome.Sharp.IconButton();
            this.OrdenarDia = new FontAwesome.Sharp.IconButton();
            this.OrdenarTelefono = new FontAwesome.Sharp.IconButton();
            this.OrdenarNombre = new FontAwesome.Sharp.IconButton();
            this.dataGridView1 = new Zuby.ADGV.AdvancedDataGridView();
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.iconButton3 = new FontAwesome.Sharp.IconButton();
            this.iconButton2 = new FontAwesome.Sharp.IconButton();
            this.label19 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.panel4.Controls.Add(this.checkBox1);
            this.panel4.Controls.Add(this.OrdenarConvenio);
            this.panel4.Controls.Add(this.OrdenarPerfil);
            this.panel4.Controls.Add(this.OrdenarEdad);
            this.panel4.Controls.Add(this.OrdenarAño);
            this.panel4.Controls.Add(this.OrdenarMes);
            this.panel4.Controls.Add(this.OrdenarDia);
            this.panel4.Controls.Add(this.OrdenarTelefono);
            this.panel4.Controls.Add(this.OrdenarNombre);
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Controls.Add(this.iconButton1);
            this.panel4.Controls.Add(this.dateTimePicker2);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.dateTimePicker1);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.iconButton3);
            this.panel4.Controls.Add(this.iconButton2);
            this.panel4.Controls.Add(this.label19);
            this.panel4.Location = new System.Drawing.Point(12, 12);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1072, 562);
            this.panel4.TabIndex = 108;
            this.panel4.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
            // 
            // OrdenarConvenio
            // 
            this.OrdenarConvenio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.OrdenarConvenio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OrdenarConvenio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrdenarConvenio.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            this.OrdenarConvenio.IconColor = System.Drawing.Color.Black;
            this.OrdenarConvenio.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.OrdenarConvenio.IconSize = 25;
            this.OrdenarConvenio.Location = new System.Drawing.Point(914, 129);
            this.OrdenarConvenio.Name = "OrdenarConvenio";
            this.OrdenarConvenio.Size = new System.Drawing.Size(147, 33);
            this.OrdenarConvenio.TabIndex = 161;
            this.OrdenarConvenio.Text = "Convenio";
            this.OrdenarConvenio.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.OrdenarConvenio.UseVisualStyleBackColor = false;
            this.OrdenarConvenio.Click += new System.EventHandler(this.OrdenarConvenio_Click);
            // 
            // OrdenarPerfil
            // 
            this.OrdenarPerfil.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.OrdenarPerfil.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OrdenarPerfil.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrdenarPerfil.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            this.OrdenarPerfil.IconColor = System.Drawing.Color.Black;
            this.OrdenarPerfil.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.OrdenarPerfil.IconSize = 25;
            this.OrdenarPerfil.Location = new System.Drawing.Point(629, 129);
            this.OrdenarPerfil.Name = "OrdenarPerfil";
            this.OrdenarPerfil.Size = new System.Drawing.Size(279, 33);
            this.OrdenarPerfil.TabIndex = 160;
            this.OrdenarPerfil.Text = "Nombre Analisis";
            this.OrdenarPerfil.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.OrdenarPerfil.UseVisualStyleBackColor = false;
            this.OrdenarPerfil.Click += new System.EventHandler(this.OrdenarPerfil_Click);
            // 
            // OrdenarEdad
            // 
            this.OrdenarEdad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.OrdenarEdad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OrdenarEdad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrdenarEdad.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            this.OrdenarEdad.IconColor = System.Drawing.Color.Black;
            this.OrdenarEdad.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.OrdenarEdad.IconSize = 25;
            this.OrdenarEdad.Location = new System.Drawing.Point(546, 129);
            this.OrdenarEdad.Name = "OrdenarEdad";
            this.OrdenarEdad.Size = new System.Drawing.Size(74, 33);
            this.OrdenarEdad.TabIndex = 159;
            this.OrdenarEdad.Text = "Edad";
            this.OrdenarEdad.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.OrdenarEdad.UseVisualStyleBackColor = false;
            this.OrdenarEdad.Click += new System.EventHandler(this.OrdenarEdad_Click);
            // 
            // OrdenarAño
            // 
            this.OrdenarAño.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.OrdenarAño.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OrdenarAño.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrdenarAño.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            this.OrdenarAño.IconColor = System.Drawing.Color.Black;
            this.OrdenarAño.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.OrdenarAño.IconSize = 25;
            this.OrdenarAño.Location = new System.Drawing.Point(470, 129);
            this.OrdenarAño.Name = "OrdenarAño";
            this.OrdenarAño.Size = new System.Drawing.Size(70, 33);
            this.OrdenarAño.TabIndex = 158;
            this.OrdenarAño.Text = "Año";
            this.OrdenarAño.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.OrdenarAño.UseVisualStyleBackColor = false;
            this.OrdenarAño.Click += new System.EventHandler(this.OrdenarAño_Click);
            // 
            // OrdenarMes
            // 
            this.OrdenarMes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.OrdenarMes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OrdenarMes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrdenarMes.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            this.OrdenarMes.IconColor = System.Drawing.Color.Black;
            this.OrdenarMes.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.OrdenarMes.IconSize = 25;
            this.OrdenarMes.Location = new System.Drawing.Point(399, 129);
            this.OrdenarMes.Name = "OrdenarMes";
            this.OrdenarMes.Size = new System.Drawing.Size(65, 33);
            this.OrdenarMes.TabIndex = 157;
            this.OrdenarMes.Text = "Mes";
            this.OrdenarMes.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.OrdenarMes.UseVisualStyleBackColor = false;
            this.OrdenarMes.Click += new System.EventHandler(this.OrdenarMes_Click);
            // 
            // OrdenarDia
            // 
            this.OrdenarDia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.OrdenarDia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OrdenarDia.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrdenarDia.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            this.OrdenarDia.IconColor = System.Drawing.Color.Black;
            this.OrdenarDia.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.OrdenarDia.IconSize = 25;
            this.OrdenarDia.Location = new System.Drawing.Point(332, 129);
            this.OrdenarDia.Name = "OrdenarDia";
            this.OrdenarDia.Size = new System.Drawing.Size(61, 33);
            this.OrdenarDia.TabIndex = 156;
            this.OrdenarDia.Text = "Dia";
            this.OrdenarDia.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.OrdenarDia.UseVisualStyleBackColor = false;
            this.OrdenarDia.Click += new System.EventHandler(this.OrdenarDia_Click);
            // 
            // OrdenarTelefono
            // 
            this.OrdenarTelefono.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.OrdenarTelefono.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OrdenarTelefono.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrdenarTelefono.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            this.OrdenarTelefono.IconColor = System.Drawing.Color.Black;
            this.OrdenarTelefono.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.OrdenarTelefono.IconSize = 25;
            this.OrdenarTelefono.Location = new System.Drawing.Point(216, 129);
            this.OrdenarTelefono.Name = "OrdenarTelefono";
            this.OrdenarTelefono.Size = new System.Drawing.Size(106, 33);
            this.OrdenarTelefono.TabIndex = 155;
            this.OrdenarTelefono.Text = "Telefono";
            this.OrdenarTelefono.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.OrdenarTelefono.UseVisualStyleBackColor = false;
            this.OrdenarTelefono.Click += new System.EventHandler(this.OrdenarTelefono_Click);
            // 
            // OrdenarNombre
            // 
            this.OrdenarNombre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.OrdenarNombre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OrdenarNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrdenarNombre.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            this.OrdenarNombre.IconColor = System.Drawing.Color.Black;
            this.OrdenarNombre.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.OrdenarNombre.IconSize = 25;
            this.OrdenarNombre.Location = new System.Drawing.Point(12, 129);
            this.OrdenarNombre.Name = "OrdenarNombre";
            this.OrdenarNombre.Size = new System.Drawing.Size(188, 33);
            this.OrdenarNombre.TabIndex = 154;
            this.OrdenarNombre.Text = "Nombre Completo";
            this.OrdenarNombre.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.OrdenarNombre.UseVisualStyleBackColor = false;
            this.OrdenarNombre.Click += new System.EventHandler(this.iconButton4_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.FilterAndSortEnabled = true;
            this.dataGridView1.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.dataGridView1.Location = new System.Drawing.Point(12, 168);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView1.Size = new System.Drawing.Size(1049, 337);
            this.dataGridView1.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.dataGridView1.TabIndex = 141;
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            // 
            // iconButton1
            // 
            this.iconButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton1.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltDown;
            this.iconButton1.IconColor = System.Drawing.Color.Black;
            this.iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton1.IconSize = 25;
            this.iconButton1.Location = new System.Drawing.Point(615, 23);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Size = new System.Drawing.Size(106, 33);
            this.iconButton1.TabIndex = 93;
            this.iconButton1.Text = "Buscar";
            this.iconButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton1.UseVisualStyleBackColor = false;
            this.iconButton1.Click += new System.EventHandler(this.iconButton1_Click);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(484, 30);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(103, 20);
            this.dateTimePicker2.TabIndex = 92;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(429, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 16);
            this.label2.TabIndex = 91;
            this.label2.Text = "Hasta:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(287, 31);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(103, 20);
            this.dateTimePicker1.TabIndex = 90;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(224, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 16);
            this.label1.TabIndex = 89;
            this.label1.Text = "Desde:";
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
            this.iconButton3.Location = new System.Drawing.Point(12, 511);
            this.iconButton3.Name = "iconButton3";
            this.iconButton3.Size = new System.Drawing.Size(100, 41);
            this.iconButton3.TabIndex = 88;
            this.iconButton3.Text = "Salir ";
            this.iconButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton3.UseVisualStyleBackColor = false;
            this.iconButton3.Click += new System.EventHandler(this.iconButton3_Click);
            // 
            // iconButton2
            // 
            this.iconButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton2.IconChar = FontAwesome.Sharp.IconChar.Print;
            this.iconButton2.IconColor = System.Drawing.Color.Black;
            this.iconButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton2.IconSize = 25;
            this.iconButton2.Location = new System.Drawing.Point(900, 511);
            this.iconButton2.Name = "iconButton2";
            this.iconButton2.Size = new System.Drawing.Size(161, 48);
            this.iconButton2.TabIndex = 84;
            this.iconButton2.Text = "Exportar a Excel";
            this.iconButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton2.UseVisualStyleBackColor = false;
            this.iconButton2.Click += new System.EventHandler(this.iconButton2_Click);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(4, 1);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(155, 16);
            this.label19.TabIndex = 38;
            this.label19.Text = "Personas Facturadas";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(7, 106);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(140, 17);
            this.checkBox1.TabIndex = 162;
            this.checkBox1.Text = "Agrupar Por Nombre";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // SegundaEstadistica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(1093, 585);
            this.Controls.Add(this.panel4);
            this.Name = "SegundaEstadistica";
            this.Text = "SegundaEstadistica";
            this.Load += new System.EventHandler(this.SegundaEstadistica_Load);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel4;
        private FontAwesome.Sharp.IconButton iconButton1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label1;
        private FontAwesome.Sharp.IconButton iconButton3;
        private FontAwesome.Sharp.IconButton iconButton2;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private Zuby.ADGV.AdvancedDataGridView dataGridView1;
        private FontAwesome.Sharp.IconButton OrdenarConvenio;
        private FontAwesome.Sharp.IconButton OrdenarPerfil;
        private FontAwesome.Sharp.IconButton OrdenarEdad;
        private FontAwesome.Sharp.IconButton OrdenarAño;
        private FontAwesome.Sharp.IconButton OrdenarMes;
        private FontAwesome.Sharp.IconButton OrdenarDia;
        private FontAwesome.Sharp.IconButton OrdenarTelefono;
        private FontAwesome.Sharp.IconButton OrdenarNombre;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}