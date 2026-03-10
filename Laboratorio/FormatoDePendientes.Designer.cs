
namespace Laboratorio
{
    partial class FormatoDePendientes
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
            this.pdfViewer1 = new PdfiumViewer.PdfViewer();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.pdfViewer2 = new PdfiumViewer.PdfViewer();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.imprimirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pdfViewer1
            // 
            this.pdfViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pdfViewer1.Location = new System.Drawing.Point(0, 0);
            this.pdfViewer1.Name = "pdfViewer1";
            this.pdfViewer1.ShowToolbar = false;
            this.pdfViewer1.Size = new System.Drawing.Size(800, 450);
            this.pdfViewer1.TabIndex = 2;
            this.pdfViewer1.ZoomMode = PdfiumViewer.PdfViewerZoomMode.FitBest;
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // pdfViewer2
            // 
            this.pdfViewer2.ContextMenuStrip = this.contextMenuStrip1;
            this.pdfViewer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pdfViewer2.Location = new System.Drawing.Point(0, 0);
            this.pdfViewer2.Name = "pdfViewer2";
            this.pdfViewer2.ShowToolbar = false;
            this.pdfViewer2.Size = new System.Drawing.Size(800, 450);
            this.pdfViewer2.TabIndex = 4;
            this.pdfViewer2.ZoomMode = PdfiumViewer.PdfViewerZoomMode.FitBest;
            this.pdfViewer2.Load += new System.EventHandler(this.pdfViewer2_Load);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.imprimirToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(121, 26);
            // 
            // imprimirToolStripMenuItem
            // 
            this.imprimirToolStripMenuItem.Name = "imprimirToolStripMenuItem";
            this.imprimirToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.imprimirToolStripMenuItem.Text = "Imprimir";
            this.imprimirToolStripMenuItem.Click += new System.EventHandler(this.imprimirToolStripMenuItem_Click);
            // 
            // FormatoDePendientes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pdfViewer2);
            this.Controls.Add(this.pdfViewer1);
            this.Name = "FormatoDePendientes";
            this.Text = "FormatoDePendientes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormatoDePendientes_FormClosing);
            this.Load += new System.EventHandler(this.FormatoDePendientes_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PdfiumViewer.PdfViewer pdfViewer1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private PdfiumViewer.PdfViewer pdfViewer2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem imprimirToolStripMenuItem;
    }
}