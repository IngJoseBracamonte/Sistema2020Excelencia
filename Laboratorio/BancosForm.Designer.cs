namespace Laboratorio
{
    partial class BancosForm
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Actualizar = new FontAwesome.Sharp.IconButton();
            this.TNombreBanco = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Agregar = new FontAwesome.Sharp.IconButton();
            this.iconButton2 = new FontAwesome.Sharp.IconButton();
            this.advancedDataGridView1 = new Zuby.ADGV.AdvancedDataGridView();
            this.bancosBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            this.label1 = new System.Windows.Forms.Label();
            this.idBancos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nombreBancoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.advancedDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bancosBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.panel1.Controls.Add(this.Actualizar);
            this.panel1.Controls.Add(this.TNombreBanco);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.Agregar);
            this.panel1.Controls.Add(this.iconButton2);
            this.panel1.Controls.Add(this.advancedDataGridView1);
            this.panel1.Controls.Add(this.iconButton1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(482, 449);
            this.panel1.TabIndex = 6;
            // 
            // Actualizar
            // 
            this.Actualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.Actualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Actualizar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Actualizar.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.Actualizar.IconColor = System.Drawing.Color.Black;
            this.Actualizar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.Actualizar.IconSize = 25;
            this.Actualizar.Location = new System.Drawing.Point(14, 303);
            this.Actualizar.Name = "Actualizar";
            this.Actualizar.Size = new System.Drawing.Size(118, 41);
            this.Actualizar.TabIndex = 93;
            this.Actualizar.Text = "Actualizar";
            this.Actualizar.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.Actualizar.UseVisualStyleBackColor = false;
            this.Actualizar.Visible = false;
            this.Actualizar.Click += new System.EventHandler(this.Actualizar_Click);
            // 
            // TNombreBanco
            // 
            this.TNombreBanco.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TNombreBanco.Location = new System.Drawing.Point(14, 254);
            this.TNombreBanco.Name = "TNombreBanco";
            this.TNombreBanco.Size = new System.Drawing.Size(433, 26);
            this.TNombreBanco.TabIndex = 92;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 231);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(159, 20);
            this.label2.TabIndex = 91;
            this.label2.Text = "Nombre Del Banco";
            // 
            // Agregar
            // 
            this.Agregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.Agregar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Agregar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Agregar.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.Agregar.IconColor = System.Drawing.Color.Black;
            this.Agregar.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.Agregar.IconSize = 25;
            this.Agregar.Location = new System.Drawing.Point(337, 303);
            this.Agregar.Name = "Agregar";
            this.Agregar.Size = new System.Drawing.Size(110, 41);
            this.Agregar.TabIndex = 90;
            this.Agregar.Text = "Agregar";
            this.Agregar.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.Agregar.UseVisualStyleBackColor = false;
            this.Agregar.Click += new System.EventHandler(this.Agregar_Click);
            // 
            // iconButton2
            // 
            this.iconButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton2.IconChar = FontAwesome.Sharp.IconChar.Trash;
            this.iconButton2.IconColor = System.Drawing.Color.Black;
            this.iconButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton2.IconSize = 20;
            this.iconButton2.Location = new System.Drawing.Point(449, 113);
            this.iconButton2.Name = "iconButton2";
            this.iconButton2.Size = new System.Drawing.Size(27, 34);
            this.iconButton2.TabIndex = 89;
            this.iconButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton2.UseVisualStyleBackColor = false;
            this.iconButton2.Click += new System.EventHandler(this.iconButton2_Click);
            // 
            // advancedDataGridView1
            // 
            this.advancedDataGridView1.AllowUserToAddRows = false;
            this.advancedDataGridView1.AllowUserToDeleteRows = false;
            this.advancedDataGridView1.AutoGenerateColumns = false;
            this.advancedDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.advancedDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idBancos,
            this.nombreBancoDataGridViewTextBoxColumn});
            this.advancedDataGridView1.DataSource = this.bancosBindingSource;
            this.advancedDataGridView1.FilterAndSortEnabled = true;
            this.advancedDataGridView1.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.advancedDataGridView1.Location = new System.Drawing.Point(14, 35);
            this.advancedDataGridView1.Name = "advancedDataGridView1";
            this.advancedDataGridView1.ReadOnly = true;
            this.advancedDataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.advancedDataGridView1.RowHeadersVisible = false;
            this.advancedDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.advancedDataGridView1.Size = new System.Drawing.Size(433, 179);
            this.advancedDataGridView1.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.advancedDataGridView1.TabIndex = 88;
            this.advancedDataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.advancedDataGridView1_CellContentClick);
            this.advancedDataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.advancedDataGridView1_CellDoubleClick);
            // 
            // bancosBindingSource
            // 
            this.bancosBindingSource.DataSource = typeof(Conexiones.Modelos.Bancos);
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
            this.iconButton1.Location = new System.Drawing.Point(7, 395);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Size = new System.Drawing.Size(100, 41);
            this.iconButton1.TabIndex = 86;
            this.iconButton1.Text = "Salir ";
            this.iconButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton1.UseVisualStyleBackColor = false;
            this.iconButton1.Click += new System.EventHandler(this.iconButton1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 16);
            this.label1.TabIndex = 87;
            this.label1.Text = "Bancos";
            // 
            // idBancos
            // 
            this.idBancos.DataPropertyName = "IdBancos";
            this.idBancos.HeaderText = "IdBancos";
            this.idBancos.MinimumWidth = 22;
            this.idBancos.Name = "idBancos";
            this.idBancos.ReadOnly = true;
            this.idBancos.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // nombreBancoDataGridViewTextBoxColumn
            // 
            this.nombreBancoDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nombreBancoDataGridViewTextBoxColumn.DataPropertyName = "NombreBanco";
            this.nombreBancoDataGridViewTextBoxColumn.HeaderText = "NombreBanco";
            this.nombreBancoDataGridViewTextBoxColumn.MinimumWidth = 22;
            this.nombreBancoDataGridViewTextBoxColumn.Name = "nombreBancoDataGridViewTextBoxColumn";
            this.nombreBancoDataGridViewTextBoxColumn.ReadOnly = true;
            this.nombreBancoDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // BancosForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(491, 456);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BancosForm";
            this.Text = "BancosForm";
            this.Load += new System.EventHandler(this.BancosForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.advancedDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bancosBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private FontAwesome.Sharp.IconButton Actualizar;
        private System.Windows.Forms.TextBox TNombreBanco;
        private System.Windows.Forms.Label label2;
        private FontAwesome.Sharp.IconButton Agregar;
        private FontAwesome.Sharp.IconButton iconButton2;
        private Zuby.ADGV.AdvancedDataGridView advancedDataGridView1;
        private FontAwesome.Sharp.IconButton iconButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource bancosBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn idBancos;
        private System.Windows.Forms.DataGridViewTextBoxColumn nombreBancoDataGridViewTextBoxColumn;
    }
}