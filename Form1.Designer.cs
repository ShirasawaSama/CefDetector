namespace CefDetector
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.apps = new System.Windows.Forms.FlowLayoutPanel();
            this.label = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // apps
            // 
            this.apps.BackColor = System.Drawing.Color.Transparent;
            this.apps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apps.Location = new System.Drawing.Point(150, 300);
            this.apps.Margin = new System.Windows.Forms.Padding(0);
            this.apps.Name = "apps";
            this.apps.Size = new System.Drawing.Size(834, 394);
            this.apps.TabIndex = 0;
            this.apps.HorizontalScroll.Maximum = 0;
            this.apps.AutoScroll = false;
            this.apps.VerticalScroll.Visible = false;
            this.apps.AutoScroll = true;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.BackColor = System.Drawing.Color.Transparent;
            this.label.Font = new System.Drawing.Font("Microsoft YaHei UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label.ForeColor = System.Drawing.Color.Black;
            this.label.Location = new System.Drawing.Point(90, 210);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(954, 58);
            this.label.TabIndex = 1;
            this.label.Text = "扫描中...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1160, 800);
            this.Controls.Add(this.label);
            this.Controls.Add(this.apps);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(130, 300, 130, 100);
            this.Text = "Cef Detector";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private FlowLayoutPanel apps;
        private Label label;
        private ToolTip toolTip1;
    }
}