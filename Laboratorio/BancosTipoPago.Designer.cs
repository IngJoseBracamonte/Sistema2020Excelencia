namespace Laboratorio
{
    partial class BancosTipoPago
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
            this.iconButton1 = new FontAwesome.Sharp.IconButton();
            this.label2 = new System.Windows.Forms.Label();
            this.iconButton2 = new FontAwesome.Sharp.IconButton();
            this.BancosDGV = new Zuby.ADGV.AdvancedDataGridView();
            this.idBancos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nombreBanco = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bancosBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.BancosAsignadosDGV = new Zuby.ADGV.AdvancedDataGridView();
            this.idBancosAgregado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nombreBancoAsignado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bancosBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tipoPagoDGV = new Zuby.ADGV.AdvancedDataGridView();
            this.idTipoPago = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tipoPagoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.iconButton5 = new FontAwesome.Sharp.IconButton();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BancosDGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bancosBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BancosAsignadosDGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bancosBindingSource1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tipoPagoDGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipoPagoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.panel1.Controls.Add(this.iconButton1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.iconButton2);
            this.panel1.Controls.Add(this.BancosDGV);
            this.panel1.Controls.Add(this.BancosAsignadosDGV);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(492, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(482, 521);
            this.panel1.TabIndex = 6;
            // 
            // iconButton1
            // 
            this.iconButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton1.IconChar = FontAwesome.Sharp.IconChar.Trash;
            this.iconButton1.IconColor = System.Drawing.Color.Black;
            this.iconButton1.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton1.IconSize = 20;
            this.iconButton1.Location = new System.Drawing.Point(449, 366);
            this.iconButton1.Name = "iconButton1";
            this.iconButton1.Size = new System.Drawing.Size(27, 34);
            this.iconButton1.TabIndex = 92;
            this.iconButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton1.UseVisualStyleBackColor = false;
            this.iconButton1.Click += new System.EventHandler(this.iconButton1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 230);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 20);
            this.label2.TabIndex = 91;
            this.label2.Text = "Bancos Asignados";
            // 
            // iconButton2
            // 
            this.iconButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton2.IconChar = FontAwesome.Sharp.IconChar.ArrowDown;
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
            // BancosDGV
            // 
            this.BancosDGV.AllowUserToAddRows = false;
            this.BancosDGV.AllowUserToDeleteRows = false;
            this.BancosDGV.AutoGenerateColumns = false;
            this.BancosDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BancosDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idBancos,
            this.nombreBanco});
            this.BancosDGV.DataSource = this.bancosBindingSource;
            this.BancosDGV.FilterAndSortEnabled = true;
            this.BancosDGV.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.BancosDGV.Location = new System.Drawing.Point(14, 35);
            this.BancosDGV.Name = "BancosDGV";
            this.BancosDGV.ReadOnly = true;
            this.BancosDGV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.BancosDGV.RowHeadersVisible = false;
            this.BancosDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.BancosDGV.Size = new System.Drawing.Size(433, 179);
            this.BancosDGV.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.BancosDGV.TabIndex = 88;
            // 
            // idBancos
            // 
            this.idBancos.DataPropertyName = "IdBancos";
            this.idBancos.HeaderText = "IdBancos";
            this.idBancos.MinimumWidth = 22;
            this.idBancos.Name = "idBancos";
            this.idBancos.ReadOnly = true;
            this.idBancos.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.idBancos.Visible = false;
            // 
            // nombreBanco
            // 
            this.nombreBanco.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nombreBanco.DataPropertyName = "NombreBanco";
            this.nombreBanco.HeaderText = "NombreBanco";
            this.nombreBanco.MinimumWidth = 22;
            this.nombreBanco.Name = "nombreBanco";
            this.nombreBanco.ReadOnly = true;
            this.nombreBanco.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // bancosBindingSource
            // 
            this.bancosBindingSource.DataSource = typeof(Conexiones.Modelos.Bancos);
            // 
            // BancosAsignadosDGV
            // 
            this.BancosAsignadosDGV.AllowUserToAddRows = false;
            this.BancosAsignadosDGV.AllowUserToDeleteRows = false;
            this.BancosAsignadosDGV.AutoGenerateColumns = false;
            this.BancosAsignadosDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BancosAsignadosDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idBancosAgregado,
            this.nombreBancoAsignado});
            this.BancosAsignadosDGV.DataSource = this.bancosBindingSource1;
            this.BancosAsignadosDGV.FilterAndSortEnabled = true;
            this.BancosAsignadosDGV.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.BancosAsignadosDGV.Location = new System.Drawing.Point(14, 256);
            this.BancosAsignadosDGV.Name = "BancosAsignadosDGV";
            this.BancosAsignadosDGV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.BancosAsignadosDGV.RowHeadersVisible = false;
            this.BancosAsignadosDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.BancosAsignadosDGV.Size = new System.Drawing.Size(433, 221);
            this.BancosAsignadosDGV.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.BancosAsignadosDGV.TabIndex = 88;
            // 
            // idBancosAgregado
            // 
            this.idBancosAgregado.DataPropertyName = "IdBancos";
            this.idBancosAgregado.HeaderText = "IdBancos";
            this.idBancosAgregado.MinimumWidth = 22;
            this.idBancosAgregado.Name = "idBancosAgregado";
            this.idBancosAgregado.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.idBancosAgregado.Visible = false;
            // 
            // nombreBancoAsignado
            // 
            this.nombreBancoAsignado.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nombreBancoAsignado.DataPropertyName = "NombreBanco";
            this.nombreBancoAsignado.HeaderText = "NombreBanco";
            this.nombreBancoAsignado.MinimumWidth = 22;
            this.nombreBancoAsignado.Name = "nombreBancoAsignado";
            this.nombreBancoAsignado.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // bancosBindingSource1
            // 
            this.bancosBindingSource1.DataSource = typeof(Conexiones.Modelos.Bancos);
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
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(184)))));
            this.panel2.Controls.Add(this.tipoPagoDGV);
            this.panel2.Controls.Add(this.iconButton5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(6, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(482, 522);
            this.panel2.TabIndex = 7;
            // 
            // tipoPagoDGV
            // 
            this.tipoPagoDGV.AllowUserToAddRows = false;
            this.tipoPagoDGV.AllowUserToDeleteRows = false;
            this.tipoPagoDGV.AutoGenerateColumns = false;
            this.tipoPagoDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tipoPagoDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idTipoPago,
            this.descripcion});
            this.tipoPagoDGV.DataSource = this.tipoPagoBindingSource;
            this.tipoPagoDGV.FilterAndSortEnabled = true;
            this.tipoPagoDGV.FilterStringChangedInvokeBeforeDatasourceUpdate = true;
            this.tipoPagoDGV.Location = new System.Drawing.Point(14, 138);
            this.tipoPagoDGV.Name = "tipoPagoDGV";
            this.tipoPagoDGV.ReadOnly = true;
            this.tipoPagoDGV.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tipoPagoDGV.RowHeadersVisible = false;
            this.tipoPagoDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tipoPagoDGV.Size = new System.Drawing.Size(433, 179);
            this.tipoPagoDGV.SortStringChangedInvokeBeforeDatasourceUpdate = true;
            this.tipoPagoDGV.TabIndex = 88;
            this.tipoPagoDGV.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.advancedDataGridView3_CellContentClick);
            this.tipoPagoDGV.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tipoPagoDGV_CellDoubleClick);
            // 
            // idTipoPago
            // 
            this.idTipoPago.DataPropertyName = "IdTipoPago";
            this.idTipoPago.HeaderText = "IdTipoPago";
            this.idTipoPago.MinimumWidth = 22;
            this.idTipoPago.Name = "idTipoPago";
            this.idTipoPago.ReadOnly = true;
            this.idTipoPago.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.idTipoPago.Visible = false;
            // 
            // descripcion
            // 
            this.descripcion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.descripcion.DataPropertyName = "Descripcion";
            this.descripcion.HeaderText = "Descripcion";
            this.descripcion.MinimumWidth = 22;
            this.descripcion.Name = "descripcion";
            this.descripcion.ReadOnly = true;
            this.descripcion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // tipoPagoBindingSource
            // 
            this.tipoPagoBindingSource.DataSource = typeof(Conexiones.Modelos.TipoPago);
            // 
            // iconButton5
            // 
            this.iconButton5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(161)))), ((int)(((byte)(0)))));
            this.iconButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconButton5.IconChar = FontAwesome.Sharp.IconChar.CircleArrowLeft;
            this.iconButton5.IconColor = System.Drawing.Color.Black;
            this.iconButton5.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton5.IconSize = 25;
            this.iconButton5.Location = new System.Drawing.Point(14, 467);
            this.iconButton5.Name = "iconButton5";
            this.iconButton5.Size = new System.Drawing.Size(100, 41);
            this.iconButton5.TabIndex = 86;
            this.iconButton5.Text = "Salir ";
            this.iconButton5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.iconButton5.UseVisualStyleBackColor = false;
            this.iconButton5.Click += new System.EventHandler(this.iconButton5_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(11, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 16);
            this.label4.TabIndex = 87;
            this.label4.Text = "Tipos de pago";
            // 
            // BancosTipoPago
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(110)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(979, 540);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BancosTipoPago";
            this.Text = "BancosTipoPago";
            this.Load += new System.EventHandler(this.BancosTipoPago_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BancosDGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bancosBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BancosAsignadosDGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bancosBindingSource1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tipoPagoDGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tipoPagoBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private FontAwesome.Sharp.IconButton iconButton2;
        private Zuby.ADGV.AdvancedDataGridView BancosDGV;
        private System.Windows.Forms.Label label1;
        private Zuby.ADGV.AdvancedDataGridView BancosAsignadosDGV;
        private System.Windows.Forms.Label label2;
        private FontAwesome.Sharp.IconButton iconButton1;
        private System.Windows.Forms.BindingSource bancosBindingSource;
        private System.Windows.Forms.BindingSource bancosBindingSource1;
        private System.Windows.Forms.Panel panel2;
        private Zuby.ADGV.AdvancedDataGridView tipoPagoDGV;
        private System.Windows.Forms.BindingSource tipoPagoBindingSource;
        private FontAwesome.Sharp.IconButton iconButton5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewTextBoxColumn idTipoPago;
        private System.Windows.Forms.DataGridViewTextBoxColumn descripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn idBancos;
        private System.Windows.Forms.DataGridViewTextBoxColumn nombreBanco;
        private System.Windows.Forms.DataGridViewTextBoxColumn idBancosAgregado;
        private System.Windows.Forms.DataGridViewTextBoxColumn nombreBancoAsignado;
    }
}